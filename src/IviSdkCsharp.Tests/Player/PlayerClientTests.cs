using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Player;
using Ivi.Proto.Common.Sort;
using Microsoft.Extensions.Logging.Abstractions;
using Mythical.Game.IviSdkCSharp.Exception;
using Shouldly;
using Xunit;
using static IviSdkCsharp.Tests.Player.Services.FakePlayerService;


namespace IviSdkCsharp.Tests
{
    [Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
    public class PlayerClientTests
    {
        private readonly  GrpcTestServerFixture _fixture;
        private MockPlayerExecutor.UpdatePlayerCall expectedCall;
        public PlayerClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task GetPlayerAsync_ValidRequest_ReturnsRequestedPlayer()
        {
            var playerClient = new IviPlayerClient(NullLogger<IviPlayerClient>.Instance, _fixture.Client);
            
            var result = await playerClient.GetPlayerAsync(PlayerIdExisting);
            
            result!.PlayerId.ShouldBe(PlayerIdExisting);
            result.DisplayName.ShouldBe("Just making sure this works");
            result.CreatedTimestamp.ShouldBe(3_000_000_000);
        }
        
        [Fact]
        public async Task GetPlayerAsync_NotFound_ReturnsNull()
        {
            var playerClient = new IviPlayerClient(NullLogger<IviPlayerClient>.Instance, _fixture.Client);
            
            var result = await playerClient.GetPlayerAsync(PlayerIdNotFound);
            
            result.ShouldBeNull();
        }
        
        [Fact]
        public async Task GetPlayerAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var playerClient = new IviPlayerClient(NullLogger<IviPlayerClient>.Instance, _fixture.Client);
            
            Should.Throw<IVIException>(async () => await playerClient.GetPlayerAsync(PlayerIdThrow));
        }

        [Fact]
        public async Task GetPlayersAsync_ValidRequest_ReturnsRequestedPlayers()
        {
            var playerClient = new IviPlayerClient(NullLogger<IviPlayerClient>.Instance, _fixture.Client);
            var (offset, pageSize, sortOrder) = GetPlayersExpectedRequestData;
            
            var result = await playerClient.GetPlayersAsync(offset, pageSize, sortOrder);

            result!.ShouldBe(DefaultPlayers.IviPlayers);
        }
        
        [Fact]
        public void GetPlayersAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var playerClient = new IviPlayerClient(null, _fixture.Client);
            var (offset, pageSize, sortOrder) = GetPlayersExpectedRequestData;

            Should.Throw<IVIException>(async () =>
                await playerClient.GetPlayersAsync(offset.AddDays(1), pageSize + 1, SortOrder.Asc));
        }

        [Theory]
        [InlineData("192.168.1.1", "192.168.1.1")]
        [InlineData("", "")]
        [InlineData(null, "")]
        [InlineData(" ", "")]
        [InlineData("\t", "")]
        public async Task LinkPlayerAsync_ValidInput_LinksAndUpdatesPlayer(string passedIpAddress, string expectedIpAddress)
        {
            var executor = new MockPlayerExecutor();
            var playerClient = new IviPlayerClient(null, _fixture.Client)
            {
                UpdateSubscription = executor
            };
            expectedCall = new MockPlayerExecutor.UpdatePlayerCall(PlayerIdExisting, expectedIpAddress, PlayerState.PendingLinked);

            await playerClient.LinkPlayerAsync(PlayerIdExisting, "test@example.com", "Ninja", passedIpAddress);

            executor.LastCall.ShouldBe(expectedCall);
        }
        
        [Fact]
        public void LinkPlayerAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var playerClient = new IviPlayerClient(null, _fixture.Client);

            Should.Throw<IVIException>(async () =>
                await playerClient.LinkPlayerAsync(PlayerIdThrow, "test@example.com", "Ninja", "192.168.1.1"));
        }
    }
}