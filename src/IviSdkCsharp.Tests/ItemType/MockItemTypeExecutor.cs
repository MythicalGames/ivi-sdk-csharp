using IviSdkCsharp.Client.Executor;
using Mythical.Game.IviSdkCSharp;

namespace IviSdkCsharp.Tests
{
    class MockItemTypeExecutor : IVIItemTypeExecutor
    {
        public UpdateItemTypeCall LastCall;

        public void UpdateItemType(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState)
        {
            LastCall = new UpdateItemTypeCall(gameItemTypeId, trackingId, itemTypeState, currentSupply, issuedSupply, baseUri, issueTimeSpan);
        }

        public void SavedItemTypeStatus(CreateItemResponse response)
        {
            LastCall = new UpdateItemTypeCall(response.GameItemTypeId, response.TrackingId, response.ItemTypeState);
        }

        internal record UpdateItemTypeCall(string GameItemTypeId, string TrackingId, ItemTypeState ItemTypeState, int CurrentSupply = 0, int IssuedSupply = 0, string BaseUri = "", int IssueTimeSpan = 0);
    }

}