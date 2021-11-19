using Games.Mythical.Ivi.Sdk.Client;
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
            var client = CreateClient();
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
                    AmountPaid = 5m,
                    Currency = "dollars-money",
                    GameInventoryIds = new List<string>{ "truck1" },
                    GameItemTypeId = "type1",
                    ItemName = "the-item",
                    Metadata = new IviMetadata
                    {
                        Name = "meta1",
                        Description = "something",
                        Image = "uri://img.jpg",
                        Properties = new(),
                    }
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

        [Fact]
        public async Task CreateTask_CanMapGrpcToOrder()
        {
            var actualOrder = new Ivi.Proto.Api.Order.Order
            {
                Address = new ()
                {
                    AddressLine1 = "ln1",
                    AddressLine2 = "ln2",
                    City = "cty",
                    CountryIsoAlpha2 = "ctabbr",
                    CountryName = "ct",
                    FirstName = "fn",
                    LastName = "ln",
                    PostalCode = "zip",
                    State = "st",
                },
                BuyerPlayerId = "buyer1",
                CreatedBy = "me",
                CreatedTimestamp = new DateTimeOffset(2000, 5, 5, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds(),
                EnvironmentId = IviConfiguration.EnvironmentId,
                Metadata = new Struct(),
                OrderId = "order1",
                OrderStatus = Ivi.Proto.Common.Order.OrderState.Failed,
                PaymentProviderData = new PaymentProviderOrderProto 
                { 
                    Bitpay = new BitPayProto { Invoice = new Struct() }
                },
                PaymentProviderId = PaymentProviderId.Bitpay,
                PurchasedItems = new (),
                RequestIp = "3.2.1.0",
                StoreId = "store1",
                Tax = "1",
                Total = "2",
            };
            actualOrder.Metadata.Fields.Add(new Dictionary<string, Value>
            {
                ["truck"] = Value.ForString("foo"),
            });
            actualOrder.PaymentProviderData.Bitpay.Invoice.Fields.Add("truck3", Value.ForString("ok"));
            actualOrder.PurchasedItems.PurchasedItems.Add(new ItemTypeOrder
            {
                AmountPaid = "234",
                Currency = "coins",
                GameItemTypeId = "id1",
                ItemName = "some-item",
                Metadata = new Ivi.Proto.Common.Metadata
                {
                    Name = "meta1",
                    Description = "the item",
                    Image = "uri://something.jpg",
                    Properties = new Struct()
                }
            });
            actualOrder.PurchasedItems.PurchasedItems[0].Metadata.Properties.Fields.Add("what", Value.ForString("no"));
            actualOrder.PurchasedItems.PurchasedItems[0].GameInventoryIds.Add("inv1");

            var mock = new Mock<FakeOrderService>();
            mock.Setup(s => s.CreateOrder(It.IsAny<CreateOrderRequest>(), It.IsAny<ServerCallContext>()))
                .Returns(() => Task.FromResult(actualOrder));

            var client = CreateClient();
            FakeOrderService.UseMock(mock.Object);

            var expectedOrder = new IviOrder
            {
                Address = new IviAddress
                {
                    AddressLine1 = "ln1",
                    AddressLine2 = "ln2",
                    City = "cty",
                    CountryIsoAlpha2 = "ctabbr",
                    CountryName = "ct",
                    FirstName = "fn",
                    LastName = "ln",
                    PostalCode = "zip",
                    State = "st",
                },
                BuyerPlayerId = "buyer1",
                CreatedBy = "me",
                CreatedTimestamp = new DateTimeOffset(2000, 5, 5, 0, 0, 0, TimeSpan.Zero),
                EnvironmentId = IviConfiguration.EnvironmentId,
                ListingId = "",
                Metadata = new ()
                {
                    ["truck"] = "foo",
                },
                OrderId = "order1",
                OrderStatus = Ivi.Proto.Common.Order.OrderState.Failed,
                PaymentProviderData = new IviBitpayOrder
                {
                    Invoice = new ()
                    {
                        ["truck3"] = "ok"
                    }
                },
                PaymentProviderId = PaymentProviderId.Bitpay,
                PurchasedItems = new ()
                {
                    new ()
                    {
                        AmountPaid = 234m,
                        Currency = "coins",
                        GameInventoryIds = new() { "inv1" },
                        GameItemTypeId = "id1",
                        ItemName = "some-item",
                        Metadata = new()
                        {
                            Name = "meta1",
                            Description = "the item",
                            Image = "uri://something.jpg",
                            Properties = new()
                            {
                                ["what"] = "no"
                            },
                        }
                    },
                },
                RequestIp = "3.2.1.0",
                StoreId = "store1",
                Tax = 1m,
                Total = 2m,
            };

            var resultOrder = await client.CreatePrimaryOrderAsync("", "", 0m, expectedOrder.Address, PaymentProviderId.Bitpay, new(), new(), "");

            resultOrder.ShouldBeEquivalentTo(expectedOrder);            
        }

        [Fact]
        public async Task GetOrder_CanMapToRequest()
        {
            var client = CreateClient();
            var mock = new Mock<FakeOrderService>();
            GetOrderRequest request = null;
            mock.Setup(f => f.GetOrder(It.IsAny<GetOrderRequest>(), It.IsAny<ServerCallContext>()))
                .Callback<GetOrderRequest, ServerCallContext>((req, ctx) => { request = req; })
                .Returns(() => Task.FromResult(new Ivi.Proto.Api.Order.Order { }));

            FakeOrderService.UseMock(mock.Object);
            var result = await client.GetOrder("asdf");
            request.EnvironmentId.ShouldBe(IviConfiguration.EnvironmentId);
            request.OrderId.ShouldBe("asdf");
        }

        [Fact]
        public async Task GetOrder_CanMapGrpcToOrder()
        {
            var mock = new Mock<FakeOrderService>();
            mock.Setup(s => s.GetOrder(It.IsAny<GetOrderRequest>(), It.IsAny<ServerCallContext>()))
                .Returns(() => Task.FromResult(new Ivi.Proto.Api.Order.Order 
                { 
                    OrderId = "asdf"
                }));

            var client = CreateClient();
            FakeOrderService.UseMock(mock.Object);

            var result = await client.GetOrder("23");
            result.OrderId.ShouldBe("asdf");
        }

        [Fact]
        public async Task FinalizeOrder_CanMapBitpayRequest()
        {
            FinalizeOrderRequest request = null;
            var mock = new Mock<FakeOrderService>();
            mock.Setup(s => s.FinalizeOrder(It.IsAny<FinalizeOrderRequest>(), It.IsAny<ServerCallContext>()))
                .Callback<FinalizeOrderRequest, ServerCallContext>((req, ctx) => { request = req; })
                .Returns(() => Task.FromResult(new FinalizeOrderAsyncResponse {  }));
            FakeOrderService.UseMock(mock.Object);

            var client = CreateClient();
            await client.FinalizeBitpayOrderAsync("order1", "inv1", "sess1");

            request.EnvironmentId.ShouldBe(IviConfiguration.EnvironmentId);
            request.FraudSessionId.ShouldBe("sess1");
            request.OrderId = "order1";
            request.PaymentRequestData.ShouldBe(new PaymentRequestProto
            {
                Bitpay = new BitPayPaymentRequestProto
                {
                    InvoiceId = "inv1"
                }
            });
        }

        [Fact]
        public async Task FinalizeOrder_CanMapGrpcToFinalizeResponse()
        {
            var actualResp = new FinalizeOrderAsyncResponse
            {
                FraudScore = new FraudResultProto
                {
                    FraudOmniscore = "omni1",
                    FraudScore = 12,
                },
                OrderStatus = Ivi.Proto.Common.Order.OrderState.Failed,
                PaymentInstrumentType = "type1",
                PendingIssuedItems = new(),
                ProcessorResponse = "proc1",
                Success = true,
                TransactionId = "tran1"
            };
            actualResp.PendingIssuedItems.PurchasedItems.Add(new IssuedItem()
            {
                AmountPaid = "234",
                Currency = "money",
                GameInventoryId = "inv1",
                GameItemTypeId = "item-type1",
                ItemName = "item-one",
                Metadata = new Ivi.Proto.Common.Metadata
                {
                    Name = "name1",
                    Description = "desc1",
                    Image = "img.jpg",
                    Properties = new Struct(),
                }
            });
            actualResp.PendingIssuedItems.PurchasedItems[0].Metadata.Properties.Fields.Add("foot", Value.ForString("lettuce"));
            var mock = new Mock<FakeOrderService>();
            mock.Setup(s => s.FinalizeOrder(It.IsAny<FinalizeOrderRequest>(), It.IsAny<ServerCallContext>()))
                .Returns(() => Task.FromResult(actualResp));
            FakeOrderService.UseMock(mock.Object);

            var client = CreateClient();
            var resultResponse = await client.FinalizeBitpayOrderAsync("order1", "inv1", "sess1");

            var expectedResp = new IviFinalizeOrderResponse
            {
                FraudScore = new IviFraudResult { FraudOmniscore = "omni1", FraudScore = 12 },
                OrderStatus = Ivi.Proto.Common.Order.OrderState.Failed,
                PaymentInstrumentType = "type1",
                PendingIssuedItems = new()
                {
                    new ()
                    {
                        AmountPaid = 234m,
                        Currency = "money",
                        GameInventoryId = "inv1",
                        GameItemTypeId = "item-type1",
                        ItemName = "item-one",
                        Metadata = new()
                        {
                            Name = "name1",
                            Description = "desc1",
                            Image = "img.jpg",
                            Properties = new()
                            {
                                ["foot"] = "lettuce"
                            }
                        }
                    }
                },
                ProcessorResponse = "proc1",
                Success = true,
                TransactionId = "tran1",
            };

            resultResponse.ShouldBeEquivalentTo(expectedResp);
        }

        private IviOrderClient CreateClient()
            => new(NullLogger<IviOrderClient>.Instance, _fixture.Client);
    }
}
