FROM mcr.microsoft.com/dotnet/aspnet:6.0
RUN GRPC_HEALTH_PROBE_VERSION=v0.4.6 && \
    apt-get update && apt-get install -y wget && wget -qO/bin/grpc_health_probe https://github.com/grpc-ecosystem/grpc-health-probe/releases/download/${GRPC_HEALTH_PROBE_VERSION}/grpc_health_probe-linux-amd64 && \
    chmod +x /bin/grpc_health_probe
WORKDIR /app
COPY ./bin/Release/net6.0/ .
ENTRYPOINT ["dotnet", "Mythical.Game.Services.Backend.Grpc.Service.Player.dll"]
