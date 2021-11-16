using System.Threading.Tasks;
using IviSdkCsharp.Client.Executor;
using Ivi.Proto.Common.Item;

namespace IviSdkCsharp.Tests
{
    class MockItemExecutor : IVIItemExecutor
    {
        public UpdateItemCall LastCall;
        public UpdateItemStateCall LastStateCall;
        
        public Task UpdateItemAsync(string gameInventoryId, string itemTypeId, string playerId, long dGoodsId, int serialNumber,
            string metadataUri, string trackingId, ItemState itemState)
        {
            LastCall = new UpdateItemCall(gameInventoryId, itemTypeId, playerId, dGoodsId, serialNumber, metadataUri,trackingId,itemState);
            return Task.CompletedTask;
        }

        public Task UpdateItemStateAsync(string gameInventoryId, string trackingId, ItemState itemState)
        {
            LastStateCall = new UpdateItemStateCall(gameInventoryId, trackingId, itemState);
            return Task.CompletedTask;
        }
        
        public record UpdateItemCall(string gameInventoryId, string gameItemTypeId, string PlayerId,  long dGoodsId, int serialNumber, string metadataUri, string trackingId, ItemState itemState);

        public record UpdateItemStateCall(string gameInventoryId, string trackingId, ItemState itemState);
    }
}