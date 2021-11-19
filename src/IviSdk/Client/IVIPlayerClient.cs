using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Ivi.Proto.Api.Player;
using Ivi.Proto.Common.Player;
using Ivi.Proto.Common.Sort;
using Ivi.Rpc.Api.Player;
using Ivi.Rpc.Streams;
using Ivi.Rpc.Streams.Player;
using IviSdkCsharp.Client.Executor;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mythical.Game.IviSdkCSharp.Exception;
using Mythical.Game.IviSdkCSharp.Model;

namespace Games.Mythical.Ivi.Sdk.Client
{
    public class IviPlayerClient : AbstractIVIClient
    {
        private readonly ILogger<IviPlayerClient> _logger;
        private readonly IVIPlayerExecutor? _playerExecutor;
        private PlayerService.PlayerServiceClient? _client;
        private PlayerStream.PlayerStreamClient? _streamClient;

        public IviPlayerClient(ILogger<IviPlayerClient>? logger) => _logger = logger ?? new NullLogger<IviPlayerClient>();

        internal IviPlayerClient(ILogger<IviPlayerClient>? logger, HttpClient httpClient)
            : base(httpClient.BaseAddress!, new GrpcChannelOptions{ HttpClient = httpClient }) =>
            _logger = logger ?? new NullLogger<IviPlayerClient>();

        public IVIPlayerExecutor UpdateSubscription
        {
            init
            {
                _playerExecutor = value;
                Task.Run(SubscribeToStream);
            }
        }

        private async Task SubscribeToStream()
        {
            var (waitBeforeRetry, resetRetries) = GetReconnectAwaiter(_logger);
            while (true)
            {
                try
                {
                    _streamClient = new PlayerStream.PlayerStreamClient(Channel);
                    using var call = _streamClient.PlayerStatusStream(new Subscribe {EnvironmentId = EnvironmentId});
                    await foreach (var response in call.ResponseStream.ReadAllAsync())
                    {
                        _logger.LogDebug("Player update subscription for player id {playerId}", response.PlayerId);
                        try
                        {
                            if (_playerExecutor != null)
                            {
                                await _playerExecutor!.UpdatePlayerAsync(response.PlayerId, response.TrackingId, response.PlayerState);
                            }
                            await ConfirmPlayerUpdateAsync(response.PlayerId, response.TrackingId,
                                response.PlayerState);
                            resetRetries();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error calling {nameof(_playerExecutor.UpdatePlayerAsync)}");
                        }
                    }
                    _logger.LogInformation("Player update stream closed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Player update subscription error");
                }
                finally
                {
                    await waitBeforeRetry();
                }
            }
        }

        private async Task ConfirmPlayerUpdateAsync(string playerId, string trackingId, PlayerState playerState)
        {
            await _streamClient!.PlayerStatusConfirmationAsync(new PlayerStatusConfirmRequest
            {
                EnvironmentId = EnvironmentId,
                PlayerId = playerId,
                PlayerState = playerState,
                TrackingId = trackingId
            });
        }

        private PlayerService.PlayerServiceClient Client => _client ??= new PlayerService.PlayerServiceClient(Channel);

        public async Task LinkPlayerAsync(string playerId, string email, string displayName, string requestIp)
        {
            _logger.LogDebug("PlayerClient.linkPlayer called from player: {playerId}:{email}:{displayName}", 
                playerId, email, displayName);
            try
            {
                var request = new LinkPlayerRequest
                {
                    EnvironmentId = EnvironmentId,
                    PlayerId = playerId,
                    Email = email,
                    DisplayName = displayName
                };

                if (!string.IsNullOrWhiteSpace(requestIp))
                {
                    request.RequestIp = requestIp;
                }
                
                var result = await Client.LinkPlayerAsync(request);
                if (_playerExecutor != null)
                {
                    await _playerExecutor!.UpdatePlayerAsync(playerId, result.TrackingId, result.PlayerState);
                }
            }
            catch (RpcException e)
            {
                throw IVIException.FromGrpcException(e);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception calling {nameof(IVIPlayerExecutor.UpdatePlayerAsync)} on {nameof(LinkPlayerAsync)}, player will be in an invalid state!", e);

                throw new IVIException(IVIErrorCode.LOCAL_EXCEPTION);
            }
        }
        
        public async Task<IviPlayer?> GetPlayerAsync(string playerId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("PlayerClient.getPlayer called from player: {playerId}", playerId);
            try
            {
                var result =  await Client.GetPlayerAsync(new GetPlayerRequest
                {
                    EnvironmentId = EnvironmentId,
                    PlayerId = playerId
                }, cancellationToken: cancellationToken);
                return result.Adapt<IviPlayer>();
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.NotFound)
                {
                    return null;
                }
        
                throw IVIException.FromGrpcException(ex);
            }
        }

        public async Task<IList<IviPlayer>?> GetPlayersAsync(DateTimeOffset createdTimestamp, int pageSize, IviSortOrder sortOrder, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("PlayerClient.getPlayers called with params: createdTimestamp {}, pageSize {}, sortOrder {}", createdTimestamp, pageSize, sortOrder);
            try
            {
                var request = new GetPlayersRequest
                {
                    EnvironmentId = EnvironmentId,
                    PageSize = pageSize,
                    SortOrder = sortOrder.Adapt<SortOrder>(),
                    CreatedTimestamp = (ulong) createdTimestamp.ToUnixTimeSeconds()
                };
                var result = await Client.GetPlayersAsync(request, cancellationToken: cancellationToken);
                return result.IviPlayers.Select(x => x.Adapt<IviPlayer>()).ToList();
            }
            catch (RpcException e)
            {
                _logger.LogError("gRPC error from IVI server", e);
                throw IVIException.FromGrpcException(e);
            }
        }
    }
}