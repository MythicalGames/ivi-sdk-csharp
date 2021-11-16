using System.Threading.Tasks;
using Ivi.Proto.Common.Item;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIItemExecutor
    {
        Task UpdateItemAsync(string gameInventoryId, string itemTypeId, string playerId, long dGoodsId, int serialNumber, string metadataUri, string trackingId, ItemState itemState);
        Task UpdateItemStateAsync(string gameInventoryId, string trackingId, ItemState itemState);
    }
}