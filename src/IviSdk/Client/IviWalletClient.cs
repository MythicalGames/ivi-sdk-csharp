using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Ivi.Proto.Api.Wallet;
using Ivi.Rpc.Api.Wallet;
using Mapster;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Model;

namespace Games.Mythical.Ivi.Sdk.Client;

public class IviWalletClient : AbstractIVIClient
{
    private WalletService.WalletServiceClient? _client;

    public IviWalletClient(IviConfiguration config, ILogger<IviWalletClient>? logger = null, IChannelProvider? channelProvider = null)
        : base(config, logger: logger, channelProvider: channelProvider) { }

    internal IviWalletClient(IviConfiguration config, ILogger<IviWalletClient>? logger, HttpClient httpClient)
        : base(config, httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient }, logger) { }

    private WalletService.WalletServiceClient Client => _client ??= new(Channel);

    public async Task<IviWallet> GetWalletUserAsync(string playerId, PayoutProviderId payoutProviderId)
    {
        var result = await TryCall(async () => await Client.GetWalletUserAsync(new()
        {
            EnvironmentId = EnvironmentId,
            PlayerId = playerId,
            ProviderId = payoutProviderId
        }));
        return result.Adapt<IviWallet>();
    }

    public async Task<IviUpholdQuote> CreateUpholdQuoteAsync(string playerId, decimal total, string externalCardId)
    {
        var result = await TryCall(async () => await Client.CreateUpholdQuoteAsync(new()
        {
            EnvironmentId = EnvironmentId,
            ExternalCardId = externalCardId,
            PlayerId = playerId,
            Total = total.ToString()
        }));
        return result.Adapt<IviUpholdQuote>();
    }
}