using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Ivi.Proto.Api.Order.Payment;
using Ivi.Proto.Api.Payment;
using Ivi.Rpc.Api.Payment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Games.Mythical.Ivi.Sdk.Client
{
    public class IviPaymentClient : AbstractIVIClient
    {
        private readonly ILogger<IviPaymentClient> _logger;
        private PaymentService.PaymentServiceClient? _client;

        public IviPaymentClient(ILogger<IviPaymentClient>? logger) => _logger = logger ?? new NullLogger<IviPaymentClient>();

        internal IviPaymentClient(ILogger<IviPaymentClient>? logger, HttpClient httpClient)
            : base(httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient }) =>
            _logger = logger ?? new NullLogger<IviPaymentClient>();

        private PaymentService.PaymentServiceClient Client => _client ??= new(Channel);

        public Task<string> GetToken(PaymentProviderId paymentProviderId, string playerId)
        {
            switch (paymentProviderId)
            {
                case PaymentProviderId.Braintree: return GetBraintreeToken(playerId);
            }
            throw new InvalidOperationException($"Cannot get token for {paymentProviderId}");
        }

        private async Task<string> GetBraintreeToken(string playerId)
        {
            var result = await Client.GenerateClientTokenAsync(new CreateTokenRequest
            {
                EnvironmentId = EnvironmentId,
                Braintree = new BraintreeTokenRequest
                {
                    PlayerId = playerId,
                }
            });
            return result.Braintree.Token;
        }
    }
}