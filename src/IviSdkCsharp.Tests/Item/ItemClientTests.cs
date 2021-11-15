﻿using System.Collections.Generic;
using System.Threading;
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
        private MockItemExecutor.UpdateItemStateCall expectedStateCall;
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
        public async Task BurnItemAsync_ValidRequest()
        {
            var executor = new MockItemExecutor();
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client)
            {
                UpdateSubscription = executor
            };

            expectedStateCall =
                new MockItemExecutor.UpdateItemStateCall(GameInventoryIdBurned, "1234", ItemState.Burned);
            await itemClient.BurnItemAsync(GameInventoryIdBurned);
            Assert.Equal(GameInventoryIdBurned, executor.LastStateCall.gameInventoryId);
            executor.LastStateCall.ShouldBe(expectedStateCall);
        }
        
        [Fact]
        public async Task BurnItemAsync_NotFound()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);
            
            Should.Throw<IVIException>(async () => await itemClient.BurnItemAsync("DoesntExists"));
        }

        [Fact]
        public void BurnItemAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);

            Should.Throw<IVIException>(async () => await itemClient.BurnItemAsync(GameInventoryIdThrow));
        }
        
        [Fact]
        public async Task TransferItemAsync_ValidRequest()
        {
            var executor = new MockItemExecutor();
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client)
            {
                UpdateSubscription = executor
            };

            expectedStateCall =
                new MockItemExecutor.UpdateItemStateCall(GameInventoryIdExisting, "1234", ItemState.Transferred);
            await itemClient.TransferItemAsync(GameInventoryIdExisting, "PLayerA", "PlayerB", "MythicalStore");
            Assert.Equal(GameInventoryIdExisting, executor.LastStateCall.gameInventoryId);
            executor.LastStateCall.ShouldBe(expectedStateCall);
        }
        
        [Fact]
        public async Task TransferItemAsync_NotFound()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);
            
            Should.Throw<IVIException>(async () => await itemClient.TransferItemAsync("DoesntExists", "PLayerA", "PlayerB", "MythicalStore"));
        }

        [Fact]
        public void TransferItemAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);

            Should.Throw<IVIException>(async () => await itemClient.TransferItemAsync(GameInventoryIdThrow, "PLayerA", "PlayerB", "MythicalStore"));
        }

        [Theory]
        [InlineData("1234", "1234")]
        [InlineData("", "")]
        [InlineData(null, "")]
        [InlineData(" ", "")]
        [InlineData("\t", "")]
        public async Task IssueItemAsync_ValidRequest(string stringValue, string expectedStringValue)
        {
            var executor = new MockItemExecutor();
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client)
            {
                UpdateSubscription = executor
            };

            expectedStateCall =
                new MockItemExecutor.UpdateItemStateCall(GameInventoryIdExisting, expectedStringValue,
                    ItemState.Issued);
            await itemClient.IssueItemAsync(GameInventoryIdExisting, "SomePlayer", "wizardStaff", "gameTypeIdThing", 010,
                "notUsed",
                new IviMetadata("someMetaData", "description of metadata", "someimguri",
                    new Dictionary<string, object>()), "mythicalStore", stringValue, stringValue,
                CancellationToken.None);

            executor.LastStateCall.ShouldBe(expectedStateCall);
        }
        
        [Fact]
        public void IssueItemAsync_gRPCServiceThrows_ThrowsIVIException()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);
        
            Should.Throw<IVIException>(async () => await itemClient.IssueItemAsync(GameInventoryIdThrow, "SomePlayer", "wizardStaff", "gameTypeIdThing", 010,
                "notUsed",
                new IviMetadata("someMetaData", "description of metadata", "someimguri",
                    new Dictionary<string, object>()), "mythicalStore", "1234", "failedTrackingId",
                CancellationToken.None));
        }


        [Fact]
        public async Task UpdateItemMetadataAsync_ValidRequest()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);

            // This is asserted inside the service to check if the request is passing the correct
            // params down to the gRPC service
            await itemClient.UpdateItemMetadataAsync(GameInventoryIdExisting, new IviMetadata("TestingMetaData", "Description of Test", "someImgUrl", new Dictionary<string, object>()));
        }
        
        [Fact]
        public void UpdateItemMetadataAsync_ShouldThrow()
        {
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);

            // This is asserted inside the service to check if the request is passing the correct
            // params down to the gRPC service
            Should.Throw<IVIException>(async () => await itemClient.UpdateItemMetadataAsync(GameInventoryIdThrow, new IviMetadata("TestingMetaData", "Description of Test", "someImgUrl", new Dictionary<string, object>())));        
        }
        
        [Fact]
        public async Task UpdateItemMetadataAsync_ValidListRequest()
        {
            var testMetadata = new IviMetadata("testingListMetadata", "description of update list", "justanotherimgurl",
                new Dictionary<string, object>());
            List<IviMetadataUpdate>? updateList = new() {new IviMetadataUpdate(GameInventoryIdListed, testMetadata)};
            updateList.Add(new IviMetadataUpdate(GameInventoryIdIssueId, testMetadata));
            var itemClient = new IviItemClient(NullLogger<IviItemClient>.Instance, _fixture.Client);

            // This is asserted inside the service to check if the request is passing the correct
            // params down to the gRPC service
            await itemClient.UpdateItemMetadataAsync(updateList!);
        }
    }
}