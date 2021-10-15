using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace IviSdkCsharp.Tests
{
    [Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
    public class PlayerClientTests
    {
        private readonly GrpcTestServerFixture _fixture;
        public PlayerClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task GetPlayerAsync_ValidRequest_ReturnsRequestedPlayer()
        {
            var playerClient = new IviPlayerClient(null, NullLogger<IviPlayerClient>.Instance, _fixture.Channel);
            const string playerId = "Ninja";
            
            var result = await playerClient.GetPlayerAsync(playerId);
            
            result!.PlayerId.ShouldBe(playerId);
            result.DisplayName.ShouldBe("Just making sure this works");
        }
    }
}