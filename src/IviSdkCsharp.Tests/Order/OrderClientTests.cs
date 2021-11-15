using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Api.Order.Payment;
using IviSdkCsharp.Tests.Order.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Mythical.Game.IviSdkCSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IviSdkCsharp.Tests.Order
{
    [Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
    public class OrderClientTests
    {
        private readonly  GrpcTestServerFixture _fixture;
        public OrderClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task CreateOrder_ValidRequest_ReturnsOrder()
        {
            var client = new IviOrderClient(NullLogger<IviOrderClient>.Instance, _fixture.Client);
            FakeOrderService.blah = "foo";
            var result = await client.CreatePrimaryOrderAsync("", "", 324m, new() 
            { 
                AddressLine1 = "",
                AddressLine2 = "",
                FirstName = "",
                LastName = "",
                City = "",
                PostalCode = "",
                State = "",
                CountryName = "",
                CountryIsoAlpha2 = ""
            }, 
            PaymentProviderId.Bitpay, new List<IviItemTypeOrder>(), new(), "");
        }
    }
}
