using Ivi.Proto.Common.Itemtype;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIItemTypeExecutor
    {
        void UpdateItemType(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState);
        void SavedItemTypeStatus(string gameItemTypeId, string trackingId, ItemTypeState itemTypeState);
    }
}