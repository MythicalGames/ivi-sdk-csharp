using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Api.Item;
using Ivi.Proto.Common.Item;
using Ivi.Proto.Common.Sort;
using Mapster;
using Microsoft.Extensions.Logging.Abstractions;
using Mythical.Game.IviSdkCSharp.Exception;
using Mythical.Game.IviSdkCSharp.Model;
using Shouldly;
using Xunit;
using static IviSdkCsharp.Tests.Item.Services.FakeItemService;

namespace IviSdkCsharp.Tests.Item
{
    [Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
    public class ItemClientTests
    {
        private readonly  GrpcTestServerFixture _fixture;
        public ItemClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task GetItemAsync_ValidRequest_ReturnsRequestedItem()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);

            var result = await itemClient.GetItemAsync(GameInventoryIdExisting);
            result.GameInventoryId.ShouldBe(GameInventoryIdExisting);
            result.PlayerId.ShouldBe("Mario");
        }

        [Fact]
        public async Task GetItemAsync_NotFound_ReturnsNull()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);
            var result = await itemClient.GetItemAsync(GameInventoryIdNotFound);
            
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetItemAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var itemClient = new IviItemClient(null, _fixture.Client);
            Should.Throw<IVIException>(async () => await itemClient.GetItemAsync(GameInventoryIdThrow));
        }

        [Fact]
        public async Task GetItemsAsync_ValidRequest_ReturnsRequestedItems()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);
            var (offset, pageSize, sortOrder) = GetItemsExpectedRequestData;
            
            var result = await itemClient.GetItemsAsync(offset, pageSize, sortOrder);
            
            result!.ShouldBe(DefaultItems.Adapt<List<IviItem>>());
        }
        
        [Fact]
        public void GetItemsAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);
            var (offset, pageSize, sortOrder) = GetItemsExpectedRequestData;

            Should.Throw<IVIException>(async () =>
                await itemClient.GetItemsAsync(offset.AddDays(1), pageSize + 1, sortOrder));
        }

        [Fact]
        private async Task UpdateItemMetadataAsync_ValidRequest_ReturnsUpdatedMetadata()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);
            var result = await itemClient.UpdateItemMetadataAsync(GameInventoryIdIssueId,
                new IviMetadata("thing", "description of thing", "someImageUrl", new Dictionary<string, object>()));
            // TODO: this test breaks over the response/request
            result.ShouldBe(IssuedMetaData.Adapt<IviMetadata>());
        }
    }
}