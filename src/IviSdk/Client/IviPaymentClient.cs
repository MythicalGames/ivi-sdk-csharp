using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Ivi.Proto.Api.Order;
using Ivi.Proto.Api.Order.Payment;
using Ivi.Proto.Api.Payment;
using Ivi.Rpc.Api.Payment;
using Mapster;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Model;

namespace Games.Mythical.Ivi.Sdk.Client;

public class IviPaymentClient : AbstractIVIClient
{
    private PaymentService.PaymentServiceClient? _client;

    public IviPaymentClient(IviConfiguration config, ILogger<IviPaymentClient>? logger)
        : base(config, logger: logger) { }

    internal IviPaymentClient(IviConfiguration config, ILogger<IviPaymentClient>? logger, HttpClient httpClient)
        : base(config, httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient }, logger) { }

    private PaymentService.PaymentServiceClient Client => _client ??= new(Channel);

    public async Task<IviToken> GetTokenAsync(PaymentProviderId paymentProviderId, string playerId, string origin = "")
    {
        var request = paymentProviderId switch
        {
            PaymentProviderId.Braintree => GetBraintreeTokenRequest(playerId),
            PaymentProviderId.Cybersource => GetCybersourceTokenRequest(origin),
            _ => new() { EnvironmentId = EnvironmentId }
        };
        var result = await TryCall(async () => await Client.GenerateClientTokenAsync(request));
        return result.Adapt<IviToken>();
    }

    public async Task<IviPaymentMethod> CreateCybersourcePaymentMethodAsync(
        string playerId,
        string cardType,
        string expirationMonth,
        string expirationYear,
        string instrumentId,
        IviAddress address)
    {
        var result = await TryCall(async () => await Client.CreatePaymentMethodAsync(new()
        {
            EnvironmentId = EnvironmentId,
            PlayerId = playerId,
            Address = address.Adapt<Address>(),
            CardPaymentData = new()
            {
                Cybersource = new()
                {
                    CardType = cardType,
                    ExpirationMonth = expirationMonth,
                    ExpirationYear = expirationYear,
                    InstrumentId = instrumentId,
                }
            }
        }));
        return result.Adapt<IviPaymentMethod>();
    }

    public async Task<List<IviPaymentMethod>> GetPaymentMethodsAsync(string playerId, string token, PaymentProviderId paymentProviderId)
    {
        var result = await TryCall(async () => await Client.GetPaymentMethodsAsync(new()
        {
            EnvironmentId = EnvironmentId,
            PaymentProviderId = paymentProviderId,
            PlayerId = playerId,
            Token = token
        }));
        return result.PaymentMethods.Adapt<List<IviPaymentMethod>>();
    }

    public async Task<IviPaymentMethod> UpdateCybersourcePaymentMethodAsync(
        string playerId,
        string cardType,
        string token,
        string expirationMonth,
        string expirationYear,
        string instrumentId,
        IviAddress address)
    {
        var result = await TryCall(async () => await Client.UpdatePaymentMethodAsync(new()
        {
            EnvironmentId = EnvironmentId,
            PaymentProviderId = PaymentProviderId.Cybersource,
            PlayerId = playerId,
            Token = token,
            Address = address.Adapt<Address>(),
            CardPaymentData = new()
            {
                Cybersource = new()
                {
                    CardType = cardType,
                    ExpirationMonth = expirationMonth,
                    ExpirationYear = expirationYear,
                    InstrumentId = instrumentId,
                }
            }
        }));
        return result.Adapt<IviPaymentMethod>();
    }

    public async Task DeletePaymentMethodAsync(string playerId, string token, PaymentProviderId paymentProviderId)
    {
        await TryCall(async () => await Client.DeletePaymentMethodAsync(new()
        {
            EnvironmentId = EnvironmentId,
            PaymentProviderId = paymentProviderId,
            PlayerId = playerId,
            Token = token,
        }));
    }

    private CreateTokenRequest GetBraintreeTokenRequest(string playerId)
        => new()
        {
            EnvironmentId = EnvironmentId,
            Braintree = new()
            {
                PlayerId = playerId,
            }
        };

    private CreateTokenRequest GetCybersourceTokenRequest(string origin)
        => new()
        {
            EnvironmentId = EnvironmentId,
            Cybersource = new()
            {
                Origin = origin,
            }
        };
}