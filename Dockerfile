FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.19 as builder
RUN apk update && apk add --no-cache gcompat
WORKDIR /build
COPY src/AccelByte.PluginArch.LootBox.Demo.Server/*.csproj .
RUN dotnet restore
COPY src/AccelByte.PluginArch.LootBox.Demo.Server/ .
RUN dotnet publish -c Release -o /output


FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.19
WORKDIR /app
COPY --from=builder /output/ .
# gRPC server port, Prometheus /metrics port
EXPOSE 6565 8080
ENTRYPOINT ["/app/AccelByte.PluginArch.LootBox.Demo.Server"]