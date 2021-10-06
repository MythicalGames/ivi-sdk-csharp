using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using Ivi.Proto.Api.Player;
using Ivi.Proto.Common.Sort;
using Ivi.Rpc.Api.Player;
using Ivi.Rpc.Streams.Player;
using IviSdkCsharp.Client.Executor;
using IviSdkCsharp.Exception;
using Microsoft.Extensions.Logging;

namespace Games.Mythical.Ivi.Sdk.Client
{
    public class IviPlayerClient : AbstractIVIClient
    {
        private readonly IVIPlayerExecutor _playerExecutor;
        private readonly ILogger<IviPlayerClient> _logger;
        private PlayerService.PlayerServiceClient _serviceBlockingStub = null;

        public IviPlayerClient(IVIPlayerExecutor playerExecutor, ILogger<IviPlayerClient> logger) : base()
        {
            _logger = logger;

            this._playerExecutor = playerExecutor;
            var options = new GrpcChannelOptions();

            //options.Credentials
            this.channel = GrpcChannel.ForAddress($"{host}:{port}", options );

            //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(keepAlive));
            //.KeepAliveTime(keepAlive, TimeUnit.SECONDS).Build()
            
        }

        private IviPlayerClient(IVIPlayerExecutor playerExecutor, GrpcChannel channel)
        {
            this._playerExecutor = playerExecutor;
            this.channel = channel;
        }
        private PlayerService.PlayerServiceClient ServiceBlockingStub
        {
            get
            {
                return _serviceBlockingStub ??= new PlayerService.PlayerServiceClient(channel);
            }
        }

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
                var result = ServiceBlockingStub.LinkPlayer(request, options);
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
        
        public virtual IVIPlayer GetPlayer(string playerId)
        {
            _logger.LogDebug("PlayerClient.getPlayer called from player: {}", playerId);

            try
            {
                var request = new GetPlayerRequest
                {
                    EnvironmentId = environmentId,
                    PlayerId = playerId
                };
        
                var result = ServiceBlockingStub.GetPlayer(request);
                return result;
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
                var result = ServiceBlockingStub.GetPlayers(request);
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