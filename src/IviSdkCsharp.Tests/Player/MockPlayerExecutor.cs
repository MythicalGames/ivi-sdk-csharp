using System.Threading.Tasks;
using Ivi.Proto.Common.Player;
using IviSdkCsharp.Client.Executor;

namespace IviSdkCsharp.Tests
{
    class MockPlayerExecutor : IVIPlayerExecutor
    {
        public UpdatePlayerCall LastCall;
        public Task UpdatePlayerAsync(string playerId, string trackingId, PlayerState playerState)
        {
            LastCall = new UpdatePlayerCall(playerId, trackingId, playerState);
            return Task.CompletedTask;
        }

        public record UpdatePlayerCall(string playerId, string trackingId, PlayerState playerState);
    }
}