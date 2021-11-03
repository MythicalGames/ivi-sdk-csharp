using Games.Mythical.Ivi.Sdk.Client;
using IviSdkCsharp.Client.Executor;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp;
namespace ClientSample
{
    public class LoggingItemTypeUpdateSubscription : IVIItemTypeExecutor
    {
        private readonly ILogger<IviItemTypeClient> _logger;

        public LoggingItemTypeUpdateSubscription(ILogger<IviItemTypeClient> logger) => _logger = logger;
        
        public void UpdateItemType(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan,
            string trackingId, ItemTypeState itemTypeState)
        {
            _logger.LogInformation("ItemType Update: {@UpdateItemTypeData}", new UpdateItemTypeData(gameItemTypeId, currentSupply, issuedSupply, baseUri, issueTimeSpan, trackingId, itemTypeState));
        }

        public void SavedItemTypeStatus(CreateItemResponse response)
        {
            _logger.LogInformation("ItemTypeStatus Update: {@UpdateItemTypeData}", response);
        }

        record UpdateItemTypeData(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState);
    }
}