using System.Runtime.CompilerServices;
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
        protected readonly string host;
        protected readonly int port;
        protected readonly string environmentId;
        protected readonly string apiKey;
        // gRPC settings
        protected int KeepAlive { get; }
        protected GrpcChannel _channel;
        protected AbstractIVIClient()
        {
            if (string.IsNullOrEmpty(IviConfiguration.EnvironmentId))
            {
                throw new IVIException("Environment Id not set!", IVIErrorCode.ENVIRONMENT_ID_NOT_SET);
            }

            environmentId = IviConfiguration.EnvironmentId;
            if (string.IsNullOrEmpty(IviConfiguration.ApiKey))
            {
                throw new IVIException("API Key not set!", IVIErrorCode.APIKEY_NOT_SET);
            }

            apiKey = IviConfiguration.ApiKey;
            if (string.IsNullOrEmpty(IviConfiguration.Host))
            {
                throw new IVIException("Host not set!", IVIErrorCode.HOST_NOT_SET);
            }

            host = IviConfiguration.Host;
            port = IviConfiguration.Port;
            KeepAlive = IviConfiguration.KeepAlive;
        }

        //public abstract void InitStub();

        public virtual CallCredentials AddAuthentication()
        {
            
            return null;
        }
    }
}