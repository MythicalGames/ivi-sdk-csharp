using System.Threading.Tasks;
using Ivi.Proto.Common.Itemtype;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIItemTypeExecutor
    {
        Task UpdateItemTypeAsync(string gameItemTypeId, int currentSupply, int issuedSupply, string baseUri, int issueTimeSpan, string trackingId, ItemTypeState itemTypeState);
        Task UpdateItemTypeStatusAsync(string gameItemTypeId, string trakingId, ItemTypeState state);
    }
}