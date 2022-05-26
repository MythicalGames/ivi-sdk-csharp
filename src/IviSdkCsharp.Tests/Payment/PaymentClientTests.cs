using Games.Mythical.Ivi.Sdk.Client;
using Grpc.Core;
using Ivi.Proto.Api.Payment;
using IviSdkCsharp.Tests.Payment.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Mythical.Game.IviSdkCSharp.Model;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IviSdkCsharp.Tests.Payment;

[Collection(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
public class PaymentClientTests
{
    private readonly GrpcTestServerFixture _fixture;
    public PaymentClientTests(GrpcTestServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetToken_CanMapCybersourceToProto()
    {
        using var client = CreateClient();
        CreateTokenRequest request = null;
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.GenerateClientToken(It.IsAny<CreateTokenRequest>(), It.IsAny<ServerCallContext>()))
            .Callback<CreateTokenRequest, ServerCallContext>((req, ctx) => { request = req; })
            .Returns(() => Task.FromResult(new Token { }));
        FakePaymentService.UseMock(mock.Object);

        await client.GetTokenAsync(Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource, "asdf", "what");

        var expected = new CreateTokenRequest
        {
            EnvironmentId = GrpcTestServerFixture.Config.EnvironmentId,
            Cybersource = new()
            {
                Origin = "what"
            }
        };

        request.ShouldBe(expected);
    }

    [Fact]
    public async Task GetToken_CanMapCybersourceTokenToIviToken()
    {
        using var client = CreateClient();
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.GenerateClientToken(It.IsAny<CreateTokenRequest>(), It.IsAny<ServerCallContext>()))
            .Returns(() => Task.FromResult(new Token
            {
                Cybersource = new()
                {
                    Jwk = new()
                    {
                        E = "e",
                        Kid = "kid",
                        Kty = "kty",
                        N = "n",
                        Use = "use"
                    }
                },
            }));
        FakePaymentService.UseMock(mock.Object);

        var expected = new IviToken
        {
            CybersourceJwk = new()
            {
                E = "e",
                Kid = "kid",
                Kty = "kty",
                N = "n",
                Use = "use"
            }
        };

        var actual = await client.GetTokenAsync(Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource, "asdf");

        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateCybersourcePaymentMethod_CanMapToProto()
    {
        using var client = CreateClient();
        CreatePaymentMethodRequest request = null;
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.CreatePaymentMethod(It.IsAny<CreatePaymentMethodRequest>(), It.IsAny<ServerCallContext>()))
            .Callback<CreatePaymentMethodRequest, ServerCallContext>((req, ctx) => request = req)
            .Returns(() => Task.FromResult(new PaymentMethodResponse { }));
        FakePaymentService.UseMock(mock.Object);

        var expected = new CreatePaymentMethodRequest
        {
            Address = new() { AddressLine1 = "l1" },
            CardPaymentData = new()
            {
                Cybersource = new()
                {
                    CardType = "ct",
                    ExpirationMonth = "12",
                    ExpirationYear = "21",
                    InstrumentId = "in1"
                }
            },
            EnvironmentId = GrpcTestServerFixture.Config.EnvironmentId,
            PlayerId = "p1"
        };
        await client.CreateCybersourcePaymentMethodAsync("p1", "ct", "12", "21", "in1", new() { AddressLine1 = "l1" });

        request.ShouldBe(expected);
    }

    [Fact]
    public async Task CreateCybersourcePaymentMethod_CanMapToPaymentMethod()
    {
        using var client = CreateClient();
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.CreatePaymentMethod(It.IsAny<CreatePaymentMethodRequest>(), It.IsAny<ServerCallContext>()))
            .Returns(() => Task.FromResult(new PaymentMethodResponse
            {
                Address = new() { AddressLine1 = "l1" },
                CardType = "ct",
                ExpirationMonth = "12",
                ExpirationYear = "21",
                LastFour = "1234",
                MaskedNumber = "***3",
                Token = "t1"
            }));
        FakePaymentService.UseMock(mock.Object);

        var actual = await client.CreateCybersourcePaymentMethodAsync("", "", "", "", "", new());

        actual.ShouldBeEquivalentTo(new IviPaymentMethod
        {
            Address = new() { AddressLine1 = "l1" },
            CardType = "ct",
            ExpirationMonth = "12",
            ExpirationYear = "21",
            LastFour = "1234",
            MaskedNumber = "***3",
            Token = "t1"
        });
    }

    [Fact]
    public async Task GetPaymentMethods_CanMapToProto()
    {
        using var client = CreateClient();
        GetPaymentMethodRequest request = null;
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.GetPaymentMethods(It.IsAny<GetPaymentMethodRequest>(), It.IsAny<ServerCallContext>()))
            .Callback<GetPaymentMethodRequest, ServerCallContext>((req, ctx) => request = req)
            .Returns(() => Task.FromResult(new GetPaymentMethodResponse { }));
        FakePaymentService.UseMock(mock.Object);

        var expected = new GetPaymentMethodRequest
        {
            EnvironmentId = GrpcTestServerFixture.Config.EnvironmentId,
            PaymentProviderId = Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource,
            PlayerId = "p1",
            Token = "t1"
        };
        await client.GetPaymentMethodsAsync("p1", "t1", Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource);

        request.ShouldBe(expected);
    }

    [Fact]
    public async Task GetPaymentMethods_CanMapToPaymentMethod()
    {
        using var client = CreateClient();
        var mock = new Mock<FakePaymentService>();
        var response = new GetPaymentMethodResponse();
        response.PaymentMethods.Add(new PaymentMethodResponse()
        {
            CardType = "ct1",
        });
        response.PaymentMethods.Add(new PaymentMethodResponse()
        {
            CardType = "ct2",
        });
        mock.Setup(s => s.GetPaymentMethods(It.IsAny<GetPaymentMethodRequest>(), It.IsAny<ServerCallContext>()))
            .Returns(() => Task.FromResult(response));
        FakePaymentService.UseMock(mock.Object);

        var actual = await client.GetPaymentMethodsAsync("", "", Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource);

        actual.ShouldBeEquivalentTo(new List<IviPaymentMethod>
        {
            new IviPaymentMethod
            {
                CardType = "ct1",
            },
            new IviPaymentMethod
            {
                CardType = "ct2",
            }
        });
    }

    [Fact]
    public async Task UpdateCybersourcePayment_CanMapToProto()
    {
        using var client = CreateClient();
        UpdatePaymentMethodRequest request = null;
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.UpdatePaymentMethod(It.IsAny<UpdatePaymentMethodRequest>(), It.IsAny<ServerCallContext>()))
            .Callback<UpdatePaymentMethodRequest, ServerCallContext>((req, ctx) => request = req)
            .Returns(() => Task.FromResult(new PaymentMethodResponse { }));
        FakePaymentService.UseMock(mock.Object);

        var expected = new UpdatePaymentMethodRequest
        {
            EnvironmentId = GrpcTestServerFixture.Config.EnvironmentId,
            PaymentProviderId = Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource,
            PlayerId = "p1",
            Token = "t1",
            Address = new() { AddressLine1 = "l1" },
            CardPaymentData = new()
            {
                Cybersource = new()
                {
                    CardType = "ct",
                    ExpirationMonth = "12",
                    ExpirationYear = "21",
                    InstrumentId = "in1"
                }
            },
        };
        await client.UpdateCybersourcePaymentMethodAsync("p1", "ct", "t1", "12", "21", "in1", new() { AddressLine1 = "l1" });

        request.ShouldBe(expected);
    }

    [Fact]
    public async Task UpdateCybersourcePayment_CanMapToPaymentMethod()
    {
        using var client = CreateClient();
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.UpdatePaymentMethod(It.IsAny<UpdatePaymentMethodRequest>(), It.IsAny<ServerCallContext>()))
            .Returns(() => Task.FromResult(new PaymentMethodResponse
            {
                CardType = "ct",
            }));
        FakePaymentService.UseMock(mock.Object);

        var actual = await client.UpdateCybersourcePaymentMethodAsync("", "", "", "", "", "", new());

        actual.ShouldBeEquivalentTo(new IviPaymentMethod
        {
            CardType = "ct",
        });
    }

    [Fact]
    public async Task DeletePaymentMethod_CanMapToProto()
    {
        using var client = CreateClient();
        DeletePaymentMethodRequest request = null;
        var mock = new Mock<FakePaymentService>();
        mock.Setup(s => s.DeletePaymentMethod(It.IsAny<DeletePaymentMethodRequest>(), It.IsAny<ServerCallContext>()))
            .Callback<DeletePaymentMethodRequest, ServerCallContext>((req, ctx) => request = req)
            .Returns(() => Task.FromResult(new DeletePaymentMethodResponse { }));
        FakePaymentService.UseMock(mock.Object);

        var expected = new DeletePaymentMethodRequest
        {
            EnvironmentId = GrpcTestServerFixture.Config.EnvironmentId,
            PaymentProviderId = Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource,
            PlayerId = "p1",
            Token = "t1",
        };
        await client.DeletePaymentMethodAsync("p1", "t1", Ivi.Proto.Api.Order.Payment.PaymentProviderId.Cybersource);

        request.ShouldBe(expected);
    }

    private IviPaymentClient CreateClient()
        => new(GrpcTestServerFixture.Config, NullLogger<IviPaymentClient>.Instance, _fixture.Client);
}