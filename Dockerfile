FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG APP_NAME

WORKDIR /app

COPY . .

RUN cd microservices/$APP_NAME && dotnet restore && dotnet publish -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG APP_NAME

ENV APP_NAME=$APP_NAME
ENV OTEL_TRACES_EXPORTER=otlp
ENV OTEL_METRICS_EXPORTER=otlp
ENV OTEL_LOGS_EXPORTER=otlp
ENV OTEL_DOTNET_AUTO_TRACES_CONSOLE_EXPORTER_ENABLED=true
ENV OTEL_DOTNET_AUTO_METRICS_CONSOLE_EXPORTER_ENABLED=true
ENV OTEL_DOTNET_AUTO_LOGS_CONSOLE_EXPORTER_ENABLED=true
ENV OTEL_SERVICE_NAME=$APP_NAME
ENV OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4318
ENV OTEL_DOTNET_AUTO_HOME=/root/otel-auto

WORKDIR /app

COPY --from=build /app/publish .

RUN apt update && apt install curl unzip tzdata -y \
    && curl -L -O https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/latest/download/otel-dotnet-auto-install.sh \
    && chmod +x otel-dotnet-auto-install.sh \
    && sh otel-dotnet-auto-install.sh \
    && chmod +x /root/otel-auto/instrument.sh \
    &&  ln -fs /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime \
    &&  dpkg-reconfigure --frontend noninteractive tzdata

ENTRYPOINT /root/otel-auto/instrument.sh dotnet ${APP_NAME}.dll
