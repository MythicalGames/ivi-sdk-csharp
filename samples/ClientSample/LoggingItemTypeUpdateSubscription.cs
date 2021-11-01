using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Itemtype;
using IviSdkCsharp.Client.Executor;
using Microsoft.Extensions.Logging;

namespace ClientSample
{
    public class LoggingItemTypeUpdateSubscription : IVIItemTypeExecutor
    {
        private readonly ILogger<IVIItemTypeClient> _logger;

        public LoggingItemTypeUpdateSubscription(ILogger<IVIItemTypeClient> logger) => _logger = logger;
        
        public void UpdateItemType(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan,
            string trackingId, ItemTypeState itemTypeState)
        {
            _logger.LogInformation("ItemType Update: {@UpdateItemTypeData}", new UpdateItemTypeData(gameItemTypeId, currentSupply, issuedSupply, baseUri, issueTimeSpan, trackingId, itemTypeState));
        }

        public void UpdateItemTypeStatus(string gameItemTypeId, string trackingId, ItemTypeState itemTypeState)
        {
            _logger.LogInformation("ItemTypeStatus Update: {@UpdateItemTypeData}", new UpdateItemTypeStatusData(gameItemTypeId, trackingId, itemTypeState));
        }

        record UpdateItemTypeData(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState);

        record UpdateItemTypeStatusData(string GameItemTypeId, string TrackingId, ItemTypeState ItemTypeState);
    }
}