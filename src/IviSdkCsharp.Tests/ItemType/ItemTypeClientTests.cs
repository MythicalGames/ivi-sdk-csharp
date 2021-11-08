using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Itemtype;
using Microsoft.Extensions.Logging.Abstractions;
using Mythical.Game.IviSdkCSharp.Exception;
using Mythical.Game.IviSdkCSharp.Model;
using Shouldly;
using Xunit;
using static IviSdkCsharp.Tests.ItemType.Services.FakeItemTypeService;


namespace IviSdkCsharp.Tests
{
    [Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
    public class ItemTypeClientTests
    {
        private readonly  GrpcTestServerFixture _fixture;
        public ItemTypeClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task GetItemTypeAsync_ValidRequest_ReturnsRequestedItemType()
        {
            var itemTypeClient = new IviItemTypeClient(NullLogger<IviItemTypeClient>.Instance, _fixture.Client);
            
            var result = await itemTypeClient.GetItemTypesAsync(new List<string> { GameItemTypeIdExisting });
            
            result[0]!.GameItemTypeId.ShouldBe(GameItemTypeIdExisting);
            result[0]!.Category.ShouldBe("some category");
        }
        
        [Fact]
        public async Task GetItemTypeAsync_NotFound_ReturnsNull()
        {
            var itemTypeClient = new IviItemTypeClient(NullLogger<IviItemTypeClient>.Instance, _fixture.Client);
            
            var result = await itemTypeClient.GetItemTypeAsync(GameItemTypeIdNotFound);
            
            result.ShouldBeNull();
        }
        
        [Fact]
        public async Task GetItemTypeAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var itemTypeClient = new IviItemTypeClient(NullLogger<IviItemTypeClient>.Instance, _fixture.Client);
            
            Should.Throw<IVIException>(async () => await itemTypeClient.GetItemTypeAsync(GameItemTypeIdThrow));
        }

        [Fact]
        public async Task GetItemTypesAsync_ValidRequest_ReturnsRequestedItemTypes()
        {
            var itemTypeClient = new IviItemTypeClient(NullLogger<IviItemTypeClient>.Instance, _fixture.Client);
            
            var result = await itemTypeClient.GetItemTypesAsync();

            result!.Count.ShouldBe(3);
        }
        
        [Fact]
        public void GetItemTypesAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var itemTypeClient = new IviItemTypeClient(null, _fixture.Client);

            Should.Throw<IVIException>(async () => await itemTypeClient.GetItemTypesAsync(new List<string>(){GameItemTypeIdThrow}));
        }

        [Fact]
        public async Task CreateItemTypeAsync_ValidInput_CreatesItemType()
        {
            var executor = new MockItemTypeExecutor();
            var itemTypeClient = new IviItemTypeClient(null, _fixture.Client)
            {
                UpdateSubscription = executor
            };
            var expectedCall = new MockItemTypeExecutor.UpdateItemTypeCall(GameItemTypeIdNew, $"Traking_{GameItemTypeIdNew}",  ItemTypeState.PendingCreate);

            await itemTypeClient.CreateItemTypeAsync(new IviItemType
            {
                GameItemTypeId = GameItemTypeIdNew,
                TypeState = ItemTypeState.Failed
            });

            executor.LastCall.ShouldBe(expectedCall);
        }
    }
}