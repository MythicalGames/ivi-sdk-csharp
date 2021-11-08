using System.Threading.Tasks;
using Ivi.Proto.Common.Player;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIPlayerExecutor
    {
        Task UpdatePlayerAsync(string playerId, string trackingId, PlayerState playerState);
    }
}