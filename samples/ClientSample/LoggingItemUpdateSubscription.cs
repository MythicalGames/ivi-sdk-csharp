using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Item;
using IviSdkCsharp.Client.Executor;
using Microsoft.Extensions.Logging;

namespace ClientSample
{
    public class LoggingItemUpdateSubscription : IVIItemExecutor
    {
        private readonly ILogger<IviItemClient> _logger;
        
        public LoggingItemUpdateSubscription(ILogger<IviItemClient> logger) => _logger = logger;
        public Task UpdateItemAsync(string gameInventoryId, string itemTypeId, string playerId, long dGoodsId, int serialNumber,
            string metadataUri, string trackingId, ItemState itemState)
        {
            _logger.LogInformation("Item Update: {@UpdateItemData}", new UpdateItemData(gameInventoryId, itemTypeId, playerId, dGoodsId, serialNumber, metadataUri, trackingId, itemState));
            return Task.CompletedTask;
        }

        public Task UpdateItemStateAsync(string gameInventoryId, string trackingId, ItemState itemState)
        {
            _logger.LogInformation("ItemTypeStatus Update: {@UpdateItemStateData}", new UpdateItemStateData(gameInventoryId, trackingId, itemState));
            return Task.CompletedTask;
        }

        record UpdateItemData(string gameInventoryId, string itemTypeId, string playerId, long dGoodsId,
            int serialNumber,
            string metadataUri, string trackingId, ItemState itemState);

        record UpdateItemStateData(string gameInventoryId, string trackingId, ItemState itemState);
    }
}