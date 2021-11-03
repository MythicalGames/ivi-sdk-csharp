using System.Threading.Tasks;
using Grpc.Core;
using Mythical.Game.IviSdkCSharp;
using ProtoBuf.Grpc;

namespace IviSdkCsharp.Tests.ItemType.Services
{
    public partial class FakeItemTypeService: IItemTypeService
    {
        public ValueTask<CreateItemResponse> CreateItemTypeAsync(CreateItemTypeRequest request, CallContext context = default)
        {
            return ValueTask.FromResult(new CreateItemResponse
            {
                ItemTypeState = ItemTypeState.PendingCreate,
                GameItemTypeId = request.GameItemTypeId,
                TrackingId = request.TokenName
            });
        }

        public ValueTask<ItemTypes> GetItemTypesAsync(GetItemTypesRequest request, CallContext context = default)
        {
            if (request.GameItemTypeIds.Count == 0)
            {
                return ValueTask.FromResult(DefaultItemTypes);
            }
            
            if (request.GameItemTypeIds.Count == 1)
            {
                var itemTypes = new ItemTypes();
                var toAdd = request.GameItemTypeIds[0] switch
                {
                    GameItemTypeIdExisting => new Mythical.Game.IviSdkCSharp.ItemType
                    {
                        GameItemTypeId = GameItemTypeIdExisting,
                        Category = "some category"
                    },
                    GameItemTypeIdThrow => throw new System.Exception(),
                    _ => throw new RpcException(new Status(StatusCode.NotFound, string.Empty))
                };
                itemTypes.item_types.Add(toAdd);
                return ValueTask.FromResult(itemTypes);
            }
            
            throw new System.Exception("Only return data when get pre-configured request");
        }

        public ValueTask<FreezeItemTypeResponse> FreezeItemTypeAsync(FreezeItemTypeRequest value, CallContext context = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask UpdateItemTypeMetadataAsync(UpdateItemTypeMetadataPayload value, CallContext context = default)
        {
            throw new System.NotImplementedException();
        }
    }
}