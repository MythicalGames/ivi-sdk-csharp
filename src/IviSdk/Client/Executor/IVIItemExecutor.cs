using Ivi.Proto.Common.Item;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIItemExecutor
    {
        void UpdateItem(string gameInventoryId, string itemTypeId, string playerId, long dGoodsId, int serialNumber, string metadataUri, string trackingId, ItemState itemState);
        void UpdateItemState(string gameInventoryId, string trackingId, ItemState itemState);
    }
}