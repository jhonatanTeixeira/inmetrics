FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG APP_NAME

WORKDIR /app

COPY . .

RUN cd microservices/$APP_NAME && dotnet restore && dotnet publish -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG APP_NAME
ENV APP_NAME=$APP_NAME

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT dotnet ${APP_NAME}.dll
