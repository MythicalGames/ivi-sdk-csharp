using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Exception;
using Mythical.Game.IviSdkCSharp.Mapper;

[assembly: InternalsVisibleTo("IviSdkCsharp.Tests")]

namespace Games.Mythical.Ivi.Sdk.Client;

public interface IIviSubcribable<T> : IDisposable
{
    Task SubscribeToStream(T executor);
}

public abstract class AbstractIVIClient : IDisposable
{
    // IVI settings
    protected readonly string Host;
    protected readonly int Port;
    protected readonly string EnvironmentId;
    protected readonly string ApiKey;
    // gRPC settings
    protected readonly ILogger _logger;
    protected int KeepAlive { get; }
    protected GrpcChannel Channel;
    private readonly CancellationTokenSource cancellationTokenSource;
    protected readonly CancellationToken cancellationToken;

    static AbstractIVIClient() => MappersConfig.RegisterMappings();

    protected AbstractIVIClient(IviConfiguration config, Uri? address = default, GrpcChannelOptions? options = default, ILogger? logger = null)
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        config.Validate();
        EnvironmentId = config.EnvironmentId!;
        ApiKey = config.ApiKey!;
        Host = config.Host;
        Port = config.Port;
        KeepAlive = config.KeepAlive;
        Channel = ConstructChannel(address ?? new Uri($"{Host}:{Port}"), options);
        _logger = logger ?? NullLogger.Instance;
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

    protected async Task<TReturn> TryCall<TReturn>(Func<Task<TReturn>> action, [CallerMemberName] string caller = "")
    {
        try
        {
            return await action();
        }
        catch (RpcException e)
        {
            throw IVIException.FromGrpcException(e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Exception calling {caller}. ");
            throw new IVIException(e, IVIErrorCode.LOCAL_EXCEPTION);
        }
    }

    protected (Func<Task> wait, Action reset) GetReconnectAwaiter(ILogger? logger)
    {
        var (wait, reset) = new ReconnectAwaiter(logger, cancellationToken);
        return (wait, reset);
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        Channel.Dispose();
    }

    private class ReconnectAwaiter
    {
        private readonly ILogger? _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly Random rnd = new((int)DateTime.Now.Ticks);
        private bool _skippedDelayingFirstRetry;
        private int _requestCount = 1;
        private const int MaxPower = 15; // 2^15 = 32768 milliseconds ~ 33 seconds

        public ReconnectAwaiter(ILogger? logger, CancellationToken cancellationToken)
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        private async Task WaitBeforeReconnect()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (!_skippedDelayingFirstRetry)
            {
                _logger?.LogInformation("Immediately reconnecting");
                _skippedDelayingFirstRetry = true;
                return;
            }

            var jitterMilliseconds = rnd.Next(1, 1000);
            var waitMilliseconds = (int)Math.Pow(2, _requestCount) + jitterMilliseconds;

            _logger?.LogInformation("Waiting {waitMilliseconds} milliseconds before reconnect", waitMilliseconds);
            await Task.Delay(waitMilliseconds);
            _requestCount = Math.Min(MaxPower, _requestCount + 1);
        }

        private void ResetConnectionRetry()
        {
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