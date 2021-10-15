using System.Threading.Tasks;
using Grpc.Core;
using Ivi.Proto.Api.Player;
using Ivi.Rpc.Api.Player;

namespace IviSdkCsharp.Tests.Host.Services
{
    public class PlayerServiceImplementation: PlayerService.PlayerServiceBase
    {
        public override Task<IVIPlayer> GetPlayer(GetPlayerRequest request, ServerCallContext context) =>
            Task.FromResult(new IVIPlayer
            {
                PlayerId = request.PlayerId,
                DisplayName = "Just making sure this works"
            });
    }
}