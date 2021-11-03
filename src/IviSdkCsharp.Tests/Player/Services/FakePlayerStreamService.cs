using System.Threading.Tasks;
using Grpc.Core;
using Ivi.Rpc.Streams;
using Ivi.Rpc.Streams.Player;

namespace IviSdkCsharp.Tests.Player.Services
{
    public class FakePlayerStreamService : PlayerStream.PlayerStreamBase
    {
        public override Task PlayerStatusStream(Subscribe request, IServerStreamWriter<PlayerStatusUpdate> responseStream, ServerCallContext context)
        {
            return base.PlayerStatusStream(request, responseStream, context);
        }
    }
}