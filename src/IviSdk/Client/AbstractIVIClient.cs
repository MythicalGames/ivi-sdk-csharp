using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Exception;
using Mythical.Game.IviSdkCSharp.Mapper;

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

        static AbstractIVIClient() => MappersConfig.RegisterMappings();

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

        protected static (Func<Task> wait, Action reset) GetReconnectAwaiter(ILogger? logger) 
        {
            var (wait, reset) = new ReconnectAwaiter(logger);
            return (wait, reset);
        }

        private class ReconnectAwaiter
        {
            private readonly ILogger? _logger;
            private readonly Random rnd = new((int) DateTime.Now.Ticks);
            private bool _skippedDelayingFirstRetry;
            private int _requestCount = 1;
            private const int MaxPower = 15; // 2^15 = 32768 milliseconds ~ 33 seconds

            public ReconnectAwaiter(ILogger? logger) => _logger = logger;

            private async Task WaitBeforeReconnect()
            {
                if(!_skippedDelayingFirstRetry) {
                    _logger?.LogInformation("Immediately reconnecting");
                    _skippedDelayingFirstRetry = true;
                    return;
                }

                var jitterMilliseconds = rnd.Next(1, 1000);
                var waitMilliseconds = (int) Math.Pow(2, _requestCount) +  jitterMilliseconds;
                
                _logger?.LogInformation("Waiting {waitMilliseconds} milliseconds before reconnect", waitMilliseconds);
                await Task.Delay(waitMilliseconds);
                _requestCount = Math.Min(MaxPower, _requestCount + 1);
            }

            private void ResetConnectionRetry() {
                _skippedDelayingFirstRetry = false;
                _requestCount = 1;
            }

            public void Deconstruct(out Func<Task> wait, out Action reset)
            {
                wait = WaitBeforeReconnect;
                reset = ResetConnectionRetry;
            }
        }
    }
}