using Ivi.Proto.Common.Player;
using IviSdkCsharp.Client.Executor;

namespace IviSdkCsharp.Tests
{
    class MockPlayerExecutor : IVIPlayerExecutor
    {
        public UpdatePlayerCall LastCall;
        public void UpdatePlayer(string playerId, string trackingId, PlayerState playerState)
        {
            LastCall = new UpdatePlayerCall(playerId, trackingId, playerState);
        }

        public record UpdatePlayerCall(string playerId, string trackingId, PlayerState playerState);
    }
}