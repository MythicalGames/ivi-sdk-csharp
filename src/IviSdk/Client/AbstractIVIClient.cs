using System;
using System.Linq;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;
using IviSdkCsharp.Config;
using IviSdkCsharp.Exception;

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
        protected GrpcChannel channel;
        protected AbstractIVIClient()
        {
            if (String.IsNullOrEmpty(IviConfiguration.GetEnvironmentId()))
            {
                throw new IVIException("Environment Id not set!", IVIErrorCode.ENVIRONMENT_ID_NOT_SET);
            }

            environmentId = IviConfiguration.GetEnvironmentId();
            if (String.IsNullOrEmpty(IviConfiguration.GetApiKey()))
            {
                throw new IVIException("API Key not set!", IVIErrorCode.APIKEY_NOT_SET);
            }

            apiKey = IviConfiguration.GetApiKey();
            if (String.IsNullOrEmpty(IviConfiguration.GetHost()))
            {
                throw new IVIException("Host not set!", IVIErrorCode.HOST_NOT_SET);
            }

            host = IviConfiguration.GetHost();
            if (IviConfiguration.GetPort() == null)
            {
                throw new IVIException("Port not set!", IVIErrorCode.PORT_NOT_SET);
            }

            port = IviConfiguration.GetPort();
            KeepAlive = IviConfiguration.GetKeepAlive();
        }

        //public abstract void InitStub();

        public virtual CallCredentials AddAuthentication()
        {
            
            return null;
        }
    }
}