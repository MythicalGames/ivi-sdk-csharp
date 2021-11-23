using Grpc.Core;
using Ivi.Proto.Api.Order;
using Ivi.Rpc.Api.Order;
using System.Threading.Tasks;

namespace IviSdkCsharp.Tests.Order.Services;

public class FakeOrderService : OrderService.OrderServiceBase
{
    private static FakeOrderService mock;

    public static void UseMock(FakeOrderService mock)
        => FakeOrderService.mock = mock;

    public override Task<Ivi.Proto.Api.Order.Order> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        => mock.CreateOrder(request, context);

    public override Task<FinalizeOrderAsyncResponse> FinalizeOrder(FinalizeOrderRequest request, ServerCallContext context)
        => mock.FinalizeOrder(request, context);

    public override Task<Ivi.Proto.Api.Order.Order> GetOrder(GetOrderRequest request, ServerCallContext context)
        => mock.GetOrder(request, context);
}