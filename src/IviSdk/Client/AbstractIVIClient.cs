using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using IviSdkCsharp.Config;
using IviSdkCsharp.Exception;
[assembly:InternalsVisibleTo("IviSdkCsharp.Tests")]

namespace Games.Mythical.Ivi.Sdk.Client
{
    public abstract class AbstractIVIClient
    {
        // IVI settings
        protected readonly string host;
        protected readonly int port;
        protected readonly string environmentId;
        protected readonly string apiKey;
        // gRPC settings
        protected int KeepAlive { get; }
        protected GrpcChannel Channel;
        protected AbstractIVIClient()
        {
            EnsureStringValue(IviConfiguration.EnvironmentId, "Environment Id not set!", IVIErrorCode.ENVIRONMENT_ID_NOT_SET);
            EnsureStringValue(IviConfiguration.ApiKey, "API Key not set!", IVIErrorCode.APIKEY_NOT_SET);
            EnsureStringValue(IviConfiguration.Host, "Host not set!", IVIErrorCode.HOST_NOT_SET);
            
            environmentId = IviConfiguration.EnvironmentId!;
            apiKey = IviConfiguration.ApiKey!;
            host = IviConfiguration.Host;
            port = IviConfiguration.Port;
            KeepAlive = IviConfiguration.KeepAlive;

            static void EnsureStringValue(string? value, string errorMessage, IVIErrorCode errorCode)
            {
                if (string.IsNullOrWhiteSpace(value)) throw new IVIException(errorMessage, errorCode);
            }
        }

        protected GrpcChannel ConstructChannel(Uri address, GrpcChannelOptions? options = default)
        {
            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("API-KEY", apiKey);
                return Task.CompletedTask;
            });
            options ??= new();
            options.Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials);
            return GrpcChannel.ForAddress(address, options);
        }
    }
}