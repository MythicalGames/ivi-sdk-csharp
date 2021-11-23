using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ivi.Proto.Api.Itemtype;
using Ivi.Proto.Common.Itemtype;
using Ivi.Rpc.Api.Itemtype;
using Shouldly;

namespace IviSdkCsharp.Tests.ItemType.Services;

public partial class FakeItemTypeService : ItemTypeService.ItemTypeServiceBase
{
    public Task<Ivi.Proto.Api.Itemtype.ItemType> GetItemType(GetItemTypesRequest request, ServerCallContext context)
    {
        return request.GameItemTypeIds[0] switch
        {
            GameItemTypeIdExisting => Task.FromResult(new Ivi.Proto.Api.Itemtype.ItemType()
            {
                GameItemTypeId = GameItemTypeIdExisting,
                Category = "some category"
            }),
            GameItemTypeIdThrow => throw new System.Exception(),
            _ => throw new RpcException(new Status(StatusCode.NotFound, string.Empty))
        };
    }

    public override Task<ItemTypes> GetItemTypes(GetItemTypesRequest request, ServerCallContext context)
    {
        if (request.GameItemTypeIds.Count == 0)
        {
            return Task.FromResult(DefaultItemTypes);
        }

        if (request.GameItemTypeIds.Count == 1)
        {
            var itemTypes = new ItemTypes();
            var itemType = GetItemType(request, context).Result;
            itemTypes.ItemTypes_.Add(itemType);

            return Task.FromResult(itemTypes);
        }

        throw new System.Exception("Only return data when get pre-configured request");
    }

    public override Task<CreateItemAsyncResponse> CreateItemType(CreateItemTypeRequest request, ServerCallContext context)
    {
        request.EnvironmentId.ShouldBe(GrpcTestServerFixture.Config.EnvironmentId);

        return request.GameItemTypeId switch
        {
            GameItemTypeIdNew => Task.FromResult(new CreateItemAsyncResponse
            {
                ItemTypeState = ItemTypeState.PendingCreate,
                GameItemTypeId = request.GameItemTypeId,
                TrackingId = $"Traking_{request.GameItemTypeId}"
            }),
            _ => throw new Exception("Unexpected item type")
        };
    }

    public override Task<FreezeItemTypeAsyncResponse> FreezeItemType(FreezeItemTypeRequest request, ServerCallContext context)
    {
        request.EnvironmentId.ShouldBe(GrpcTestServerFixture.Config.EnvironmentId);
        return request.GameItemTypeId switch
        {
            GameItemTypeIdFreeze => Task.FromResult(new FreezeItemTypeAsyncResponse
            {
                ItemTypeState = ItemTypeState.Frozen,
                TrackingId = $"Tracking_{request.GameItemTypeId}",
            }),
            _ => throw new Exception("Unexpected item type")
        };
    }

    public override Task<Empty> UpdateItemTypeMetadata(UpdateItemTypeMetadataPayload request, ServerCallContext context)
    {
        request.EnvironmentId.ShouldBe(GrpcTestServerFixture.Config.EnvironmentId);
        request.GameItemTypeId.ShouldBe(GameItemTypeIdExisting);
        return Task.FromResult(new Empty());
    }
}