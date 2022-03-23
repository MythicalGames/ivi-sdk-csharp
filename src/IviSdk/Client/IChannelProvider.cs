using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Games.Mythical.Ivi.Sdk.Client;

public interface IChannelProvider : IDisposable
{
    GrpcChannel GetChannel(Uri address, string apiKey, string environment);
}

public class BasicChannelProvider : IChannelProvider
{
    private record ChannelKey(Uri address, string apiKey, string environment);

    private readonly ConcurrentDictionary<ChannelKey, GrpcChannel> channelLookup = new();
    private readonly ILogger<BasicChannelProvider> logger;

    public BasicChannelProvider(ILogger<BasicChannelProvider> logger)
    {
        this.logger = logger;
    }

    public void Dispose()
    {
        foreach (var channel in channelLookup.Values)
        {
            try
            {
                channel.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while disposing grpc channel");
            }
        }
    }

    public GrpcChannel GetChannel(Uri address, string apiKey, string environment)
    {
        var key = new ChannelKey(address, apiKey, environment);
        return channelLookup.GetOrAdd(key, _ => CreateGrpcChannel(address, apiKey));
    }

    internal static GrpcChannel CreateGrpcChannel(Uri address, string apiKey, GrpcChannelOptions? options = null)
    {
        var callCredentials = CallCredentials.FromInterceptor((_, metadata) =>
        {
            metadata.Add("API-KEY", apiKey);
            return Task.CompletedTask;
        });
        options ??= new ();
        options.Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials!);
        return GrpcChannel.ForAddress(address, options);
    }
}

public class InsecureChannelProvider : IChannelProvider, IDisposable
{
    public GrpcChannel GetChannel(Uri address, string apiKey, string environment)
    {
        var options = new GrpcChannelOptions()
        {
            Credentials = ChannelCredentials.Insecure
        };

        return GrpcChannel.ForAddress(address, options);
    }
    public void Dispose()
    {
    }
}