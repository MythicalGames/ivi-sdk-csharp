using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Ivi.Proto.Api.Item;
using Ivi.Proto.Common.Item;
using Ivi.Rpc.Api.Item;
using Mapster;
using Mythical.Game.IviSdkCSharp.Model;

namespace IviSdkCsharp.Tests.Item.Services
{
    public partial class FakeItemService: ItemService.ItemServiceBase
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

        public override Task<UpdateItemMetadataResponse> UpdateItemMetadata(UpdateItemMetadataRequest request,
            ServerCallContext context)
            => request.UpdateItems.Select(x => x.GameInventoryId).Single() switch
            {
                GameInventoryIdIssueId => Task.FromResult(IssuedMetaData),
                // GameInventoryIdPendingListed => Task.FromResult(PendingListedUpdateItems),
                // GameInventoryIdTransferred => Task.FromResult(TransferredUpdateItems),
                GameInventoryIdThrow => throw new System.Exception(),
                _ => throw new RpcException(new Status(StatusCode.NotFound, string.Empty))
            };

    }
}