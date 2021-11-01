using Ivi.Proto.Common.Itemtype;
using IviSdkCsharp.Client.Executor;

namespace IviSdkCsharp.Tests
{
    class MockItemTypeExecutor : IVIItemTypeExecutor
    {
        public UpdateItemTypeCall LastCall;

        public void UpdateItemType(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState)
        {
            LastCall = new UpdateItemTypeCall(gameItemTypeId, trackingId, itemTypeState, currentSupply, issuedSupply, baseUri, issueTimeSpan);
        }

        public void SavedItemTypeStatus(string gameItemTypeId, string trackingId, ItemTypeState itemTypeState)
        {
            LastCall = new UpdateItemTypeCall(gameItemTypeId, trackingId, itemTypeState);
        }

        internal record UpdateItemTypeCall(string GameItemTypeId, string TrackingId, ItemTypeState ItemTypeState, int CurrentSupply = 0, int IssuedSupply = 0, string BaseUri = "", int IssueTimeSpan = 0);
    }

}