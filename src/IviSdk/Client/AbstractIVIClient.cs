using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using IviSdkCsharp.Config;
using IviSdkCsharp.Exception;
[assembly:InternalsVisibleTo("IviSdkCsharp.Tests")]

namespace Games.Mythical.Ivi.Sdk.Client
{
    public abstract class AbstractIVIClient
    {
        // IVI settings
        protected readonly string Host;
        protected readonly int Port;
        protected readonly string EnvironmentId;
        protected readonly string ApiKey;
        // gRPC settings
        protected int KeepAlive { get; }
        protected GrpcChannel Channel;
        protected AbstractIVIClient(Uri? address = default, GrpcChannelOptions? options = default)
        {
            EnsureStringValue(IviConfiguration.EnvironmentId, "Environment Id not set!", IVIErrorCode.ENVIRONMENT_ID_NOT_SET);
            EnsureStringValue(IviConfiguration.ApiKey, "API Key not set!", IVIErrorCode.APIKEY_NOT_SET);
            EnsureStringValue(IviConfiguration.Host, "Host not set!", IVIErrorCode.HOST_NOT_SET);
            
            EnvironmentId = IviConfiguration.EnvironmentId!;
            ApiKey = IviConfiguration.ApiKey!;
            Host = IviConfiguration.Host;
            Port = IviConfiguration.Port;
            KeepAlive = IviConfiguration.KeepAlive;
            Channel = ConstructChannel(address ?? new Uri($"{Host}:{Port}"), options);

            static void EnsureStringValue(string? value, string errorMessage, IVIErrorCode errorCode)
            {
                if (string.IsNullOrWhiteSpace(value)) throw new IVIException(errorMessage, errorCode);
            }
        }

        private GrpcChannel ConstructChannel(Uri address, GrpcChannelOptions? options = default)
        {
            var callCredentials = CallCredentials.FromInterceptor((_, metadata) =>
            {
                metadata.Add("API-KEY", ApiKey);
                return Task.CompletedTask;
            });
            options ??= new();
            options.Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials!);
            return GrpcChannel.ForAddress(address, options);
        }
    }
}