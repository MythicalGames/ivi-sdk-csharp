using Ivi.Proto.Common.Order;
using System.Threading.Tasks;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIOrderExecutor
    {
        Task UpdateOrder(string orderId, OrderState orderState);
    }
}