using Ivi.Proto.Common.Player;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIPlayerExecutor
    {
        void UpdatePlayer(string playerId, string trackingId, PlayerState playerState);
    }
}