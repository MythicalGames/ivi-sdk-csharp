using Games.Mythical.Ivi.Sdk.Client;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ivi.Proto.Api.Order;
using Ivi.Proto.Api.Order.Payment;
using IviSdkCsharp.Tests.Order.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Model;
using Shouldly;
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
        public async Task CreateOrder_CanMapInputToRequestType()
        {
            var client = new IviOrderClient(NullLogger<IviOrderClient>.Instance, _fixture.Client);
            CreateOrderRequest request = null;
            var mock = new Mock<FakeOrderService>();
            mock.Setup(s => s.CreateOrder(It.IsAny<CreateOrderRequest>(), It.IsAny<ServerCallContext>()))
                .Callback<CreateOrderRequest, ServerCallContext>((req, ctx) => { request = req; })
                .Returns(() => Task.FromResult(new Ivi.Proto.Api.Order.Order { }));

            FakeOrderService.UseMock(mock.Object);
            var inputAddress = new IviAddress()
            {
                AddressLine1 = "add1",
                AddressLine2 = "add2",
                FirstName = "fn",
                LastName = "ln",
                City = "ct",
                PostalCode = "zip",
                State = "st",
                CountryName = "co",
                CountryIsoAlpha2 = "coabbr"
            };
            var inputItems = new List<IviItemTypeOrder>
            {
                new()
                {
                    AmountPaid = "5",
                    Currency = "dollars-money",
                    GameInventoryIds = new List<string>{ "truck1" },
                    GameItemTypeId = "type1",
                    ItemName = "the-item",
                    Metadata = new IviMetadata("meta1", "something", "uri://img.jpg", new())
                }
            };
            var inputMetadata = new Dictionary<string, object>()
            {
                ["blah"] = "ok",
                ["wow"] = 12,
                ["nope"] = true,                
            };
            await client.CreatePrimaryOrderAsync("123store", "123player", 324m, inputAddress, PaymentProviderId.Bitpay, inputItems, inputMetadata, "1.0.0.2");

            var expectedAddress = new Address
            {
                AddressLine1 = "add1",
                AddressLine2 = "add2",
                FirstName = "fn",
                LastName = "ln",
                City = "ct",
                PostalCode = "zip",
                State = "st",
                CountryName = "co",
                CountryIsoAlpha2 = "coabbr"
            };
            request.Address.ShouldBe(expectedAddress);
            request.BuyerPlayerId.ShouldBe("123player");
            request.EnvironmentId.ShouldBe(IviConfiguration.EnvironmentId);
            var expectedMeta = new Struct();
            expectedMeta.Fields.Add("blah", Value.ForString("ok"));
            expectedMeta.Fields.Add("wow", Value.ForNumber(12));
            expectedMeta.Fields.Add("nope", Value.ForBool(true));
            request.Metadata.ShouldBe(expectedMeta);
            request.PaymentProviderId.ShouldBe(PaymentProviderId.Bitpay);
            var expectedItemTypeOrder = new ItemTypeOrder
            {
                AmountPaid = "5",
                Currency = "dollars-money",
                GameItemTypeId = "type1",
                ItemName = "the-item",
                Metadata = new Ivi.Proto.Common.Metadata
                {
                    Name = "meta1",
                    Description = "something",
                    Image = "uri://img.jpg",
                    Properties = new(),
                }
            };
            expectedItemTypeOrder.GameInventoryIds.Add("truck1");
            request.PurchasedItems.PurchasedItems.Count.ShouldBe(1);
            request.PurchasedItems.PurchasedItems[0].ShouldBe(expectedItemTypeOrder);
            request.RequestIp.ShouldBe("1.0.0.2");
            request.StoreId.ShouldBe("123store");
            request.SubTotal.ShouldBe("324");
        }
    }
}
