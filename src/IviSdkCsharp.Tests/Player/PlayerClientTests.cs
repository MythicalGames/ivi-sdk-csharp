using System.Linq;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Player;
using Mapster;
using Microsoft.Extensions.Logging.Abstractions;
using Mythical.Game.IviSdkCSharp.Exception;
using Mythical.Game.IviSdkCSharp.Model;
using Shouldly;
using Xunit;
using static IviSdkCsharp.Tests.Player.Services.FakePlayerService;


namespace IviSdkCsharp.Tests;

[Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
public class PlayerClientTests
{
    private readonly GrpcTestServerFixture _fixture;
    private MockPlayerExecutor.UpdatePlayerCall expectedCall;
    public PlayerClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetPlayerAsync_ValidRequest_ReturnsRequestedPlayer()
    {
        using var playerClient = new IviPlayerClient(GrpcTestServerFixture.Config, NullLogger<IviPlayerClient>.Instance, _fixture.Client);

        var result = await playerClient.GetPlayerAsync(PlayerIdExisting);

        result!.PlayerId.ShouldBe(PlayerIdExisting);
        result.DisplayName.ShouldBe("Just making sure this works");
        result.CreatedTimestamp.ShouldBe(3_000_000_000);
    }

    [Fact]
    public async Task GetPlayerAsync_NotFound_ReturnsNull()
    {
        using var playerClient = new IviPlayerClient(GrpcTestServerFixture.Config, NullLogger<IviPlayerClient>.Instance, _fixture.Client);

        var result = await playerClient.GetPlayerAsync(PlayerIdNotFound);

        result.ShouldBeNull();
    }

    [Fact]
    public void GetPlayerAsync_gRPCServiceThrows_ThrowsIVIException()
    {
        using var playerClient = new IviPlayerClient(GrpcTestServerFixture.Config, NullLogger<IviPlayerClient>.Instance, _fixture.Client);

        Should.Throw<IVIException>(async () => await playerClient.GetPlayerAsync(PlayerIdThrow));
    }

    [Fact]
    public async Task GetPlayersAsync_ValidRequest_ReturnsRequestedPlayers()
    {
        using var playerClient = new IviPlayerClient(GrpcTestServerFixture.Config, NullLogger<IviPlayerClient>.Instance, _fixture.Client);
        var (offset, pageSize, sortOrder) = GetPlayersExpectedRequestData;

        var result = await playerClient.GetPlayersAsync(offset, pageSize, sortOrder.Adapt<IviSortOrder>());

        var expected = DefaultPlayers.IviPlayers.Select(x => x.Adapt<IviPlayer>()).ToArray();
        result!.ShouldBe(expected);
    }

    [Fact]
    public void GetPlayersAsync_gRPCServiceThrows_ThrowsIVIException()
    {
        using var playerClient = new IviPlayerClient(GrpcTestServerFixture.Config, null, _fixture.Client);
        var (offset, pageSize, sortOrder) = GetPlayersExpectedRequestData;

        Should.Throw<IVIException>(async () =>
            await playerClient.GetPlayersAsync(offset.AddDays(1), pageSize + 1, IviSortOrder.Asc));
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
        using var playerClient = new IviPlayerClient(GrpcTestServerFixture.Config, null, _fixture.Client);
        playerClient.SubscribeToStream(executor);
        expectedCall = new MockPlayerExecutor.UpdatePlayerCall(PlayerIdExisting, expectedIpAddress, PlayerState.PendingLinked);

        await playerClient.LinkPlayerAsync(PlayerIdExisting, PlayerIdExisting, "test@example.com", "Ninja", passedIpAddress);

        executor.LastCall.ShouldBe(expectedCall);
    }

    [Fact]
    public void LinkPlayerAsync_gRPCServiceThrows_ThrowsIVIException()
    {
        using var playerClient = new IviPlayerClient(GrpcTestServerFixture.Config, null, _fixture.Client);

        Should.Throw<IVIException>(async () =>
            await playerClient.LinkPlayerAsync(PlayerIdThrow, PlayerIdThrow, "test@example.com", "Ninja", "192.168.1.1"));
    }
}