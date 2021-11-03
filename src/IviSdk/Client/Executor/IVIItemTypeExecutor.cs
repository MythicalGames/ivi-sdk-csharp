using Mythical.Game.IviSdkCSharp;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIItemTypeExecutor
    {
        void UpdateItemType(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState);
        void SavedItemTypeStatus(CreateItemResponse response);
    }
}