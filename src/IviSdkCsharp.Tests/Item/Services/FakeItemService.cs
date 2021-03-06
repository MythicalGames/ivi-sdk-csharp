using System;
using System.Threading.Tasks;
using Grpc.Core;
using Ivi.Proto.Api.Item;
using Ivi.Proto.Common.Item;
using Ivi.Rpc.Api.Item;
using Mapster;
using Mythical.Game.IviSdkCSharp.Model;
using Shouldly;

namespace IviSdkCsharp.Tests.Item.Services;

public partial class FakeItemService : ItemService.ItemServiceBase
{
    public override Task<Items> GetItems(GetItemsRequest request, ServerCallContext context) =>
        IsDefaultRequest(request)
            ? Task.FromResult(DefaultItems)
            : throw new System.Exception("Only return data when get pre-configured request");

    public override Task<Ivi.Proto.Api.Item.Item> GetItem(GetItemRequest request, ServerCallContext context)
        => request.GameInventoryId switch
        {
            GameInventoryIdExisting => Task.FromResult(new Ivi.Proto.Api.Item.Item
            {
                GameInventoryId = request.GameInventoryId,
                PlayerId = "Mario"
            }),
            GameInventoryIdThrow => throw new System.Exception(),
            _ => throw new RpcException(new Status(StatusCode.NotFound, string.Empty))
        };

    public override Task<BurnItemStartedResponse> BurnItem(BurnItemRequest request, ServerCallContext context)
        => request.GameItemInventoryId switch
        {
            GameInventoryIdBurned => Task.FromResult(new BurnItemStartedResponse()
            {
                ItemState = ItemState.Burned,
                TrackingId = "1234",
            }),
            GameInventoryIdThrow => throw new Exception(),
            _ => throw new RpcException(new Status(StatusCode.NotFound, string.Empty))
        };

    public override Task<TransferItemStartedResponse> TransferItem(TransferItemRequest request, ServerCallContext context)
        => request.GameItemInventoryId switch
        {
            GameInventoryIdExisting => Task.FromResult(new TransferItemStartedResponse()
            {
                ItemState = ItemState.Transferred,
                TrackingId = "1234",
            }),
            GameInventoryIdThrow => throw new Exception(),
            _ => throw new RpcException(new Status(StatusCode.NotFound, string.Empty))
        };

    public override Task<IssueItemStartedResponse> IssueItem(IssueItemRequest request, ServerCallContext context)
    {
        request.EnvironmentId.ShouldBe(GrpcTestServerFixture.Config.EnvironmentId);
        if (request.GameInventoryId == GameInventoryIdThrow) throw new System.Exception();
        return Task.FromResult(new IssueItemStartedResponse
        {
            ItemState = ItemState.Issued,
            TrackingId = request.RequestIp
        });
    }

    public override Task<UpdateItemMetadataResponse> UpdateItemMetadata(UpdateItemMetadataRequest request, ServerCallContext context)
    {
        var testMetadata = new IviMetadata
        {
            Name = "TestingMetaData",
            Description = "Description of Test",
            Image = "someImgUrl"
        };
        var testMetadataList = new IviMetadata
        {
            Name = "testingListMetadata",
            Description = "description of update list",
            Image = "justanotherimgurl"
        };
        request.EnvironmentId.ShouldBe(GrpcTestServerFixture.Config.EnvironmentId);

        foreach (var item in request.UpdateItems)
        {
            if (item.GameInventoryId == GameInventoryIdExisting)
            {
                item.GameInventoryId.ShouldBe(GameInventoryIdExisting);
                item.Metadata.ShouldBe(testMetadata.Adapt<Ivi.Proto.Common.Metadata>());
            }
            if (item.GameInventoryId is GameInventoryIdIssueId or GameInventoryIdListed)
            {
                item.Metadata.ShouldBe(testMetadataList.Adapt<Ivi.Proto.Common.Metadata>());
            }

            if (item.GameInventoryId == GameInventoryIdThrow)
            {
                throw new Exception();
            }
        }

        return Task.FromResult(new UpdateItemMetadataResponse());
    }
}