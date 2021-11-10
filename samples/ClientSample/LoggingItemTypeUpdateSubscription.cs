using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Itemtype;
using IviSdkCsharp.Client.Executor;
using Microsoft.Extensions.Logging;

namespace ClientSample
{
    public class LoggingItemTypeUpdateSubscription : IVIItemTypeExecutor
    {
        private readonly ILogger<IviItemTypeClient> _logger;

        public LoggingItemTypeUpdateSubscription(ILogger<IviItemTypeClient> logger) => _logger = logger;
        
        public Task UpdateItemTypeAsync(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan,
            string trackingId, ItemTypeState itemTypeState)
        {
            _logger.LogInformation("ItemType Update: {@UpdateItemTypeData}", new UpdateItemTypeData(gameItemTypeId, currentSupply, issuedSupply, baseUri, issueTimeSpan, trackingId, itemTypeState));
            return Task.CompletedTask;
        }

        public Task UpdateItemTypeStatusAsync(string gameItemTypeId, string trackingId, ItemTypeState state)
        {
            _logger.LogInformation("ItemTypeStatus Update: {@UpdateItemTypeData}", new UpdateItemTypeStatusData(gameItemTypeId, trackingId, state));
            return Task.CompletedTask;
        }

        record UpdateItemTypeData(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState);

        record UpdateItemTypeStatusData(string GameItemTypeId, string TrackingId, ItemTypeState ItemTypeState);
    }
}