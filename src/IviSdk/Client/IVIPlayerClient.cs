﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Ivi.Proto.Api.Player;
using Ivi.Proto.Common.Sort;
using Ivi.Rpc.Api.Player;
using IviSdkCsharp.Client.Executor;
using IviSdkCsharp.Exception;
using Microsoft.Extensions.Logging;

namespace Games.Mythical.Ivi.Sdk.Client
{
    public class IviPlayerClient : AbstractIVIClient
    {
        private readonly IVIPlayerExecutor _playerExecutor;
        private readonly ILogger<IviPlayerClient> _logger;
        private PlayerService.PlayerServiceClient? _client;

        public IviPlayerClient(IVIPlayerExecutor playerExecutor, ILogger<IviPlayerClient> logger)
        {
            _logger = logger;

            this._playerExecutor = playerExecutor;
            var options = new GrpcChannelOptions();

            //options.Credentials
            this._channel = GrpcChannel.ForAddress($"{host}:{port}", options );

            //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(keepAlive));
            //.KeepAliveTime(keepAlive, TimeUnit.SECONDS).Build()
            
        }

        internal IviPlayerClient(IVIPlayerExecutor playerExecutor, ILogger<IviPlayerClient> logger, GrpcChannel channel)
        {
            _logger = logger;
            _playerExecutor = playerExecutor;
            _channel = channel;
        }
        private PlayerService.PlayerServiceClient Client => _client ??= new PlayerService.PlayerServiceClient(_channel);

        public virtual void LinkPlayer(string playerId, string email, string displayName, string requestIp)
        {
            _logger.LogDebug("PlayerClient.linkPlayer called from player: {}:{}:{}", playerId, email, displayName);
            try
            {
                var request = new LinkPlayerRequest
                {
                    EnvironmentId = environmentId,
                    PlayerId = playerId,
                    Email = email,
                    DisplayName = displayName
                };

                if (!string.IsNullOrEmpty(requestIp))
                {
                    request.RequestIp = requestIp;
                }

                CallOptions options = new CallOptions();
                //options.CancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(keepAlive));
                var result = Client.LinkPlayer(request, options);
                _playerExecutor.UpdatePlayer(playerId, result.TrackingId, result.PlayerState);
            }
            catch (RpcException e)
            {
                throw IVIException.FromGrpcException(e);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception calling updatePlayerState on linkPlayer, player will be in an invalid state!", e);

                throw new IVIException(IVIErrorCode.LOCAL_EXCEPTION);
            }
        }
        
        public async Task<IVIPlayer?> GetPlayerAsync(string playerId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("PlayerClient.getPlayer called from player: {}", playerId);

            try
            {
                return await Client.GetPlayerAsync(new GetPlayerRequest
                {
                    EnvironmentId = environmentId,
                    PlayerId = playerId
                }, cancellationToken: cancellationToken);
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

        public virtual IList<IVIPlayer> GetPlayers(DateTimeOffset createdTimestamp, int pageSize, SortOrder sortOrder)
        {
            _logger.LogDebug("PlayerClient.getPlayers called with params: createdTimestamp {}, pageSize {}, sortOrder {}", createdTimestamp, pageSize, sortOrder);
            try
            {

                var request = new GetPlayersRequest
                {
                    EnvironmentId = environmentId,
                    PageSize = pageSize,
                    SortOrder = sortOrder,
                    CreatedTimestamp = (ulong) createdTimestamp.Ticks
                };
                var result = Client.GetPlayers(request);
                return result.IviPlayers;
            }
            catch (RpcException e)
            {
                _logger.LogError("gRPC error from IVI server", e);
                throw IVIException.FromGrpcException(e);
            }
        }
    }
}