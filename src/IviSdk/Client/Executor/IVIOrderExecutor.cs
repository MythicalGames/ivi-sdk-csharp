using Ivi.Proto.Common.Order;

namespace IviSdkCsharp.Client.Executor
{
    public interface IVIOrderExecutor
    {
        void UpdateOrder(string orderId, OrderState orderState);
    }
}