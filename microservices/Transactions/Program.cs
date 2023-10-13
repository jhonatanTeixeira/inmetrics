using Infrastructure.Extension;
using Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Logging.AddConsole();
// builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMongo(configuration.GetValue<string>("MongoConnectionString"), configuration.GetValue<string>("MongoDatabase"));
builder.Services.AddAuth(configuration.GetValue<string>("JwtSecretKey"));
builder.Services.AddTransactions(configuration.GetValue<string>("KafkaServers"));

var app = builder.Build();

app.UsePathBase(new PathString("/transactions"));
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserMiddleware>();

app.MapControllers();

app.Run();
