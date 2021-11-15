using Grpc.Core;
using Ivi.Proto.Api.Order;
using Ivi.Rpc.Api.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IviSdkCsharp.Tests.Order.Services
{
    public class FakeOrderService : OrderService.OrderServiceBase
    {
        public static string blah = "";

        public override Task<Ivi.Proto.Api.Order.Order> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            return base.CreateOrder(request, context);
        }
    }
}
