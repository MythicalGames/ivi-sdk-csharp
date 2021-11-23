using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Player;
using IviSdkCsharp.Client.Executor;
using Microsoft.Extensions.Logging;

namespace ClientSample;

public class LoggingPlayerUpdateSubscription : IVIPlayerExecutor
{
    private readonly ILogger<IviPlayerClient> _logger;

    public LoggingPlayerUpdateSubscription(ILogger<IviPlayerClient> logger) => _logger = logger;

    public Task UpdatePlayerAsync(string playerId, string trackingId, PlayerState playerState)
    {
        _logger.LogInformation("Player Update: {@playerUpdateData}", new UpdatePlayerData(playerId, trackingId, playerState));
        return Task.CompletedTask;
    }

    record UpdatePlayerData(string playerId, string trackingId, PlayerState playerState);
}