using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Ivi.Proto.Api.Order;
using Ivi.Proto.Api.Order.Payment;
using Ivi.Rpc.Api.Order;
using Ivi.Rpc.Streams;
using Ivi.Rpc.Streams.Order;
using IviSdkCsharp.Client.Executor;
using Mapster;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Model;

namespace Games.Mythical.Ivi.Sdk.Client;

public class IviOrderClient : AbstractIVIClient, IIviSubcribable<IVIOrderExecutor>
{
    private OrderService.OrderServiceClient? _client;
    private OrderStream.OrderStreamClient? _streamClient;

    public IviOrderClient(IviConfiguration config, ILogger<IviOrderClient>? logger, IChannelProvider? channelProvider = null)
        : base(config, logger: logger, channelProvider: channelProvider) { }

    internal IviOrderClient(IviConfiguration config, ILogger<IviOrderClient>? logger, HttpClient httpClient)
        : base(config, httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient }, logger) { }

    private OrderService.OrderServiceClient Client => _client ??= new(Channel);

    public async Task<IviOrder?> GetOrder(string orderId, CancellationToken cancellationToken = default)
    {
        var result = await TryCall(async () => await Client.GetOrderAsync(new() { EnvironmentId = EnvironmentId, OrderId = orderId }, cancellationToken: cancellationToken));
        return result.Adapt<IviOrder>();
    }

    public async Task<IviOrder> CreatePrimaryOrderAsync(string storeId,
        string buyerPlayerId,
        decimal subTotal,
        IviAddress address,
        PaymentProviderId paymentProviderId,
        List<IviItemTypeOrder> purchasedItems,
        Dictionary<string, object> metadata,
        string? requestIp, 
        CancellationToken cancellationToken = default)
    {
        var result = await TryCall(async () => await Client.CreateOrderAsync(new()
        {
            EnvironmentId = EnvironmentId,
            StoreId = storeId,
            BuyerPlayerId = buyerPlayerId,
            SubTotal = subTotal.ToString(),
            Address = address.Adapt<Address>(),
            Metadata = metadata.Adapt<Struct>(),
            PaymentProviderId = paymentProviderId,
            PurchasedItems = purchasedItems.Adapt<ItemTypeOrders>(),
            RequestIp = requestIp ?? string.Empty,
        }, cancellationToken: cancellationToken));
        return result.Adapt<IviOrder>();
    }

    public Task<IviFinalizeOrderResponse> FinalizeBitpayOrderAsync(string orderId, string invoiceId, string fraudSessionId, CancellationToken cancellationToken = default)
        => FinalizeOrder(orderId, fraudSessionId, new() { Bitpay = new() { InvoiceId = invoiceId } }, cancellationToken);

    public Task<IviFinalizeOrderResponse> FinalizeCybersourceOrder(string orderId,
        string cardType,
        string expirationMonth,
        string expirationYear,
        string instrumentId,
        string paymentMethodTokenId,
        string fraudSessionId, 
        CancellationToken cancellationToken = default)
        => FinalizeOrder(orderId, fraudSessionId, new PaymentRequestProto
        {
            Cybersource = new()
            {
                CardType = cardType,
                ExpirationMonth = expirationMonth,
                ExpirationYear = expirationYear,
                InstrumentId = instrumentId,
                PaymentMethodTokenId = paymentMethodTokenId,
            }
        }, cancellationToken);

    public Task<IviFinalizeOrderResponse> FinalizeUpholdOrder(string orderId, string upholdExternalCardId, string quoteId, string fraudSessionId, CancellationToken cancellationToken = default)
        => FinalizeOrder(orderId, fraudSessionId, new PaymentRequestProto
        {
            Uphold = new()
            {
                ExternalCardId = upholdExternalCardId,
                QuoteId = quoteId,
            }
        }, cancellationToken);

    private async Task<IviFinalizeOrderResponse> FinalizeOrder(string orderId, string fraudSessionId, PaymentRequestProto paymentRequest, CancellationToken cancellationToken = default)
    {
        var result = await TryCall(async () => await Client.FinalizeOrderAsync(new FinalizeOrderRequest
        {
            EnvironmentId = EnvironmentId,
            PaymentRequestData = paymentRequest,
            FraudSessionId = fraudSessionId,
            OrderId = orderId
        }, cancellationToken: cancellationToken));
        return result.Adapt<IviFinalizeOrderResponse>();
    }

    public async Task SubscribeToStream(IVIOrderExecutor orderExecutor)
    {
        ArgumentNullException.ThrowIfNull(orderExecutor, nameof(orderExecutor));
        var (waitBeforeRetry, resetRetries) = GetReconnectAwaiter(_logger);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _streamClient = new OrderStream.OrderStreamClient(Channel);
                using var call = _streamClient.OrderStatusStream(new Subscribe { EnvironmentId = EnvironmentId }, cancellationToken: cancellationToken);
                await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    _logger.LogDebug($"Order update subscription for order id {response.OrderId}");
                    try
                    {
                        if (orderExecutor != null)
                        {
                            await orderExecutor!.UpdateOrderAsync(response.OrderId, response.OrderState);
                        }
                        await _streamClient.OrderStatusConfirmationAsync(new OrderStatusConfirmRequest
                        {
                            EnvironmentId = EnvironmentId,
                            OrderId = response.OrderId,
                            OrderState = response.OrderState
                        });

                        resetRetries();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error executing or confirming UpdateOrder");
                    }
                }
                _logger.LogInformation("Order update stream closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Order update subscription error");
            }
            finally
            {
                await waitBeforeRetry();
            }
        }
    }
}