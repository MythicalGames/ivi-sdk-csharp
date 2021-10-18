using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Sort;
using IviSdkCsharp.Exception;
using IviSdkCsharp.Tests.Host.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace IviSdkCsharp.Tests
{
    [Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
    public class PlayerClientTests
    {
        private readonly  GrpcTestServerFixture _fixture;
        public PlayerClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task GetPlayerAsync_ValidRequest_ReturnsRequestedPlayer()
        {
            var playerClient = new IviPlayerClient(null, NullLogger<IviPlayerClient>.Instance, _fixture.Client);
            const string playerId = "Ninja";
            
            var result = await playerClient.GetPlayerAsync(playerId);
            
            result!.PlayerId.ShouldBe(playerId);
            result.DisplayName.ShouldBe("Just making sure this works");
        }

        [Fact]
        public async Task GetPlayersAsync_ValidRequest_ReturnsRequestedPlayers()
        {
            var playerClient = new IviPlayerClient(null, NullLogger<IviPlayerClient>.Instance, _fixture.Client);
            var (offset, pageSize, sortOrder) = PlayerServiceImplementation.GetPlayersExpectedRequestData;
            
            var result = await playerClient.GetPlayersAsync(offset, pageSize, sortOrder);

            result!.ShouldBe(PlayerServiceImplementation.DefaultPlayers.IviPlayers);
        }
        
        [Fact]
        public void GetPlayersAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var playerClient = new IviPlayerClient(null, null, _fixture.Client);
            var (offset, pageSize, sortOrder) = PlayerServiceImplementation.GetPlayersExpectedRequestData;

            Should.Throw<IVIException>(async () =>
                await playerClient.GetPlayersAsync(offset.AddDays(1), pageSize + 1, SortOrder.Asc));
        }
    }
}