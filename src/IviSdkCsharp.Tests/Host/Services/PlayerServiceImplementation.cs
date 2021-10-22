using System.Threading.Tasks;
using Grpc.Core;
using Ivi.Proto.Api.Player;
using Ivi.Proto.Common.Player;
using Ivi.Rpc.Api.Player;

namespace IviSdkCsharp.Tests.Host.Services
{
    public partial class PlayerServiceImplementation: PlayerService.PlayerServiceBase
    {
        public override Task<IVIPlayer> GetPlayer(GetPlayerRequest request, ServerCallContext context)
            => request.PlayerId switch
            {
                PlayerIdExisting => Task.FromResult(new IVIPlayer
                {
                    PlayerId = request.PlayerId,
                    DisplayName = "Just making sure this works",
                    CreatedTimestamp = 3_000_000_000
                }),
                PlayerIdThrow => throw new System.Exception(),
                _ => throw new RpcException(new Status(StatusCode.NotFound, string.Empty))
            };

        public override Task<IVIPlayers> GetPlayers(GetPlayersRequest request, ServerCallContext context) =>
          IsDefaultRequest(request) 
            ? Task.FromResult(DefaultPlayers)
            : throw new System.Exception("Only return data when get pre-configured request");

        public override Task<LinkPlayerAsyncResponse> LinkPlayer(LinkPlayerRequest request, ServerCallContext context)
        {
            if (request.PlayerId == PlayerIdThrow) throw new System.Exception();
            return Task.FromResult(new LinkPlayerAsyncResponse
            {
                PlayerState = PlayerState.PendingLinked,
                TrackingId = request.RequestIp
            });
        }
    }
}