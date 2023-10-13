using System.Text;
using Application.Service;
using Confluent.Kafka;
using Domain.Document;
using Domain.Repository;
using Infrastructure.Repository;
using Infrastructure.Serializer;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace Infrastructure.Extension
{
    public static class InfrastructureExtension
    {
        public static void AddMongo(this IServiceCollection services, string connectionString, string database)
        {
            var client = new MongoClient(connectionString);
            services.AddSingleton(client);

            services.AddSingleton(client.GetDatabase(database));
        }

        public static void AddUser(this IServiceCollection services, string collectionName = "users")
        {
            services.AddSingleton(p => p.GetRequiredService<IMongoDatabase>().GetCollection<User>(collectionName));
            services.AddSingleton<IRepository<User>, UserRepository>();
            services.AddSingleton<IUserService, UserService>();
        }

        public static void AddKafkaProducer<T>(this IServiceCollection services, string kafkaServers, Acks acks = Acks.All)
        {
            var producerBuilder = new ProducerBuilder<string, T>(
                new ProducerConfig {
                    BootstrapServers = kafkaServers,
                    Acks = acks
                }
            )
                .SetValueSerializer(new KafkaJsonSerializer<T>());

            services.AddSingleton(producerBuilder.Build());
        }

        public static void AddKafkaConsumer<TKey, TData>(this IServiceCollection services)
        {
            services.AddSingleton(p => {
                var config = p.GetRequiredService<IOptions<ConsumerConfig>>();

                var consumerBuilder = new ConsumerBuilder<TKey, TData>(config.Value);

                if (typeof(TKey).IsClass) {
                    consumerBuilder.SetKeyDeserializer(new KafkaJsonSerializer<TKey>());
                }

                if (typeof(TData).IsClass) {
                    consumerBuilder.SetValueDeserializer(new KafkaJsonSerializer<TData>());
                }

                return consumerBuilder.Build();
            });
        }

        public static void AddTransactions(
            this IServiceCollection services,
            string kafkaServers,
            string collectionName = "transactions",
            string transactionEventName = "transaction-created",
            Acks kafkaAcks = Acks.All
        ) {
            services.AddKafkaProducer<Transaction>(kafkaServers, kafkaAcks);

            services.AddSingleton(p => p.GetRequiredService<IMongoDatabase>().GetCollection<Transaction>(collectionName));
            services.AddSingleton<IRepository<Transaction>, TransactionRepository>();
            services.AddSingleton<IMessagingService<Transaction>, DocumentMessagingService<Transaction>>();

            services.AddSingleton<IUserResourceCrudService<Transaction>>(p => new TransactionService(
                p.GetRequiredService<IRepository<Transaction>>(),
                p.GetRequiredService<IMessagingService<Transaction>>(),
                transactionEventName
            ));
        }

        public static void AddDailyCashFlow(this IServiceCollection services, string collectionName = "dailycashflow")
        {
            services.AddSingleton(p => p.GetRequiredService<IMongoDatabase>().GetCollection<DailyCashFlow>(collectionName));
            services.AddSingleton<IRepository<DailyCashFlow>, DailyCashFlowRepsitory>();
            services.AddSingleton<IUserResourceCrudService<DailyCashFlow>, DailyCashFlowService>();
        }

        public static void AddAuth(this IServiceCollection services, string jwtSecretKey)
        {
            var key = Encoding.UTF8.GetBytes(jwtSecretKey);

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSingleton<IAuthentication>(p => new Authentication(jwtSecretKey, p.GetRequiredService<IUserService>()));
            services.AddScoped<UserContext>();
            
            services.AddSwaggerGen(c => {
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                        },
                        new string[] {}
                    }
                });
            });
        }
    }
}