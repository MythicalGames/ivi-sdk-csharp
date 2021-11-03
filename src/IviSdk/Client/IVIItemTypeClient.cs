using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using IviSdkCsharp.Client.Executor;
using IviSdkCsharp.Exception;
using Mapster;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp;
using Mythical.Game.IviSdkCSharp.Model;
using ProtoBuf.Grpc.Client;
using Metadata = Mythical.Game.IviSdkCSharp.Metadata;

namespace Games.Mythical.Ivi.Sdk.Client
{
    public class IviItemTypeClient : AbstractIVIClient
    {
        private readonly ILogger<IviItemTypeClient>? _logger;
        private readonly IVIItemTypeExecutor? _itemTypeExecutor;
        private IItemTypeService? _client;
        private IItemTypeStatusStream? _streamClient;

        public IviItemTypeClient(ILogger<IviItemTypeClient>? logger)
        {
            _logger = logger;
        }

        internal IviItemTypeClient(ILogger<IviItemTypeClient>? logger, HttpClient httpClient) : base(httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient })
        {
            _logger = logger;
        }
        private IItemTypeService Client => _client ??= Channel.CreateGrpcService<IItemTypeService>();

        public IVIItemTypeExecutor UpdateSubscription
        {
            init
            {
                _itemTypeExecutor = value;
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
                    _streamClient = Channel.CreateGrpcService<IItemTypeStatusStream>();
                    var call =  _streamClient.ItemTypeStatusStreamAsync(new Subscribe { EnvironmentId = EnvironmentId });
                    await foreach (var response in call)
                    {
                        _logger.LogDebug("ItemType update subscription for itemType id {itemTypeId}", response.GameItemTypeId);
                        try
                        {
                            _itemTypeExecutor?.UpdateItemType(response.GameItemTypeId, response.CurrentSupply, response.IssuedSupply, response.BaseUri, response.IssueTimeSpan, response.TrackingId, response.ItemTypeState);
                            await ConfirmItemTypeUpdateAsync(response.GameItemTypeId, response.TrackingId, response.ItemTypeState);
                            resetRetries();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error calling {nameof(_itemTypeExecutor.UpdateItemType)}");
                        }
                    }
                    _logger.LogInformation("ItemType update stream closed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ItemType update subscription error");
                }
                finally
                {
                    await waitBeforeRetry();
                }
            }
        }

        private async Task ConfirmItemTypeUpdateAsync(string itemTypeId, string trackingId, ItemTypeState itemTypeState)
        {
            var itemTypeStatusConfirmRequest = new ItemTypeStatusConfirmRequest
            {
                EnvironmentId = EnvironmentId,
                GameItemTypeId = itemTypeId,
                ItemTypeState = itemTypeState,
                TrackingId = trackingId
            };

            await _streamClient!.ItemTypeStatusConfirmationAsync(itemTypeStatusConfirmRequest);
        }

        
        public async Task<ItemType?> GetItemTypeAsync(string gameItemTypeId, CancellationToken cancellationToken = default)
        {
            _logger?.LogDebug("ItemTypeClient.getItemType called with param: gameItemTypeId {}", gameItemTypeId);
            var itemTypeList =  await GetItemTypesAsync(new List<string>{gameItemTypeId}, cancellationToken);
            return itemTypeList?.FirstOrDefault();
        }

        public async Task<IList<ItemType>?> GetItemTypesAsync(CancellationToken cancellationToken = default)
        {
            return await GetItemTypesAsync(new List<string>(), cancellationToken);
        }

        public async Task<IList<ItemType>?> GetItemTypesAsync(List<string> gameItemTypeIds, CancellationToken cancellationToken = default)
        {
            _logger?.LogDebug("ItemTypeClient.getItemTypes called with params: gameItemTypeIds {}", gameItemTypeIds);
            try
            { 
                GetItemTypesRequest request = new ()
                {
                    EnvironmentId = EnvironmentId
                };
                request.GameItemTypeIds.AddRange(gameItemTypeIds);

                var result = await Client.GetItemTypesAsync(request);
                return result.item_types;
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.NotFound)
                {
                    return null;
                }
                _logger?.LogError(ex, "gRPC error from IVI server");
                throw IVIException.FromGrpcException(ex);
            }
        }
        
        public async Task CreateItemTypeAsync(CreateItemTypeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                request.EnvironmentId = EnvironmentId;
                _logger?.LogDebug("ItemTypeClient.CreateItemTypeAsync called with params: {request}", request);
                
                var result = await Client.CreateItemTypeAsync(request);
                _itemTypeExecutor?.SavedItemTypeStatus(result);
            }
            catch (RpcException ex)
            {
                _logger?.LogError(ex, "gRPC error from IVI server" );
                throw IVIException.FromGrpcException(ex);
            }
        }

        public void FreezeItemTypeAsync(string gameItemTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var freezeItemTypeRequest = new FreezeItemTypeRequest()
                {
                    EnvironmentId = EnvironmentId,
                    GameItemTypeId = gameItemTypeId
                };
                _logger?.LogDebug($"ItemTypeClient.FreezeItemType called with params: {freezeItemTypeRequest}", freezeItemTypeRequest);
                Client.FreezeItemTypeAsync(freezeItemTypeRequest);

            }
            catch (RpcException ex)
            {
                _logger?.LogError(ex, "gRPC error from IVI server");
                throw IVIException.FromGrpcException(ex);
            }
        }

        public void UpdateItemTypeMetadataAsync(string gameItemTypeId, Metadata? metadata, CancellationToken cancellationToken = default)
        {
            try
            {
                var updateItemTypeMetadataPayload = new UpdateItemTypeMetadataPayload()
                {
                    EnvironmentId = EnvironmentId,
                    GameItemTypeId = gameItemTypeId,
                    Metadata = metadata
                };
                _logger?.LogDebug($"ItemTypeClient.UpdateItemTypeMetadataAsync called with params: {updateItemTypeMetadataPayload}", updateItemTypeMetadataPayload);
                Client.UpdateItemTypeMetadataAsync(updateItemTypeMetadataPayload);

            }
            catch (RpcException ex)
            {
                _logger?.LogError(ex, "gRPC error from IVI server");
                throw IVIException.FromGrpcException(ex);
            }
        }

    }
}