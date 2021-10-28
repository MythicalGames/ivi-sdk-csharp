﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Ivi.Proto.Api.Itemtype;
using Ivi.Proto.Common.Itemtype;
using Ivi.Rpc.Api.Itemtype;
using Ivi.Rpc.Streams;
using Ivi.Rpc.Streams.Itemtype;
using IviSdkCsharp.Client.Executor;
using IviSdkCsharp.Exception;
using Mapster;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Model;
using Metadata = Ivi.Proto.Common.Metadata;

namespace Games.Mythical.Ivi.Sdk.Client
{
    public class IVIItemTypeClient : AbstractIVIClient
    {
        private readonly ILogger<IVIItemTypeClient>? _logger;
        private readonly IVIItemTypeExecutor? _itemTypeExecutor;
        private ItemTypeService.ItemTypeServiceClient? _client;
        private ItemTypeStatusStream.ItemTypeStatusStreamClient? _streamClient;

        public IVIItemTypeClient(ILogger<IVIItemTypeClient>? logger)
        {
            _logger = logger;
        }

        internal IVIItemTypeClient(ILogger<IVIItemTypeClient>? logger, HttpClient httpClient) : base(httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient })
        {
            _logger = logger;
        }
        private ItemTypeService.ItemTypeServiceClient Client => _client ??= new ItemTypeService.ItemTypeServiceClient(Channel);

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
                    _streamClient = new ItemTypeStatusStream.ItemTypeStatusStreamClient(Channel);
                    using var call = _streamClient.ItemTypeStatusStream(new Subscribe { EnvironmentId = EnvironmentId });
                    await foreach (var response in call.ResponseStream.ReadAllAsync())
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
            var itemTypeList =  await GetItemTypesAsync(new[] {gameItemTypeId}, cancellationToken);
            return itemTypeList?.Count > 0 ? itemTypeList[0] : null;
        }

        public async Task<IList<ItemType>?> GetItemTypesAsync(CancellationToken cancellationToken = default)
        {
            return await GetItemTypesAsync(new List<string>(), cancellationToken);
        }

        public async Task<IList<ItemType>?> GetItemTypesAsync(IEnumerable<string> gameItemTypeIds, CancellationToken cancellationToken = default)
        {
            _logger?.LogDebug("ItemTypeClient.getItemTypes called with params: gameItemTypeIds {}", gameItemTypeIds);
            try
            {
                var request = new GetItemTypesRequest()
                {
                    EnvironmentId = EnvironmentId,
                    GameItemTypeIds = { gameItemTypeIds }
                };

                var result = await Client.GetItemTypesAsync(request, cancellationToken: cancellationToken);
                return result.ItemTypes_;
            }
            catch (RpcException e)
            {
                _logger?.LogError(e, "gRPC error from IVI server");
                throw IVIException.FromGrpcException(e);
            }
        }

        public void CreateItemTypeAsync(string gameItemTypeId, string tokenName, string category, int maxSupply, int issueTimeSpan, bool burnable, bool transferable, bool sellable, IviMetadata metadata, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new CreateItemTypeRequest()
                {
                    EnvironmentId = EnvironmentId,
                    GameItemTypeId = gameItemTypeId,
                    TokenName = tokenName,
                    Category = category,
                    MaxSupply = maxSupply,
                    IssueTimeSpan = issueTimeSpan,
                    Burnable = burnable,
                    Transferable = transferable,
                    Sellable = sellable,
                    Metadata = metadata.Adapt<Metadata>()
                };

                _logger?.LogDebug("ItemTypeClient.CreateItemTypeAsync called with params: {request}", request);


                Client.CreateItemTypeAsync(request, cancellationToken: cancellationToken);
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
                Client.FreezeItemTypeAsync(freezeItemTypeRequest, cancellationToken: cancellationToken);

            }
            catch (RpcException ex)
            {
                _logger?.LogError(ex, "gRPC error from IVI server");
                throw IVIException.FromGrpcException(ex);
            }
        }

        public void UpdateItemTypeMetadataAsync(string gameItemTypeId, IviMetadata metadata, CancellationToken cancellationToken = default)
        {
            try
            {
                var updateItemTypeMetadataPayload = new UpdateItemTypeMetadataPayload()
                {
                    EnvironmentId = EnvironmentId,
                    GameItemTypeId = gameItemTypeId,
                    Metadata = metadata.Adapt<Metadata>()
                };
                _logger?.LogDebug($"ItemTypeClient.UpdateItemTypeMetadataAsync called with params: {updateItemTypeMetadataPayload}", updateItemTypeMetadataPayload);
                Client.UpdateItemTypeMetadataAsync(updateItemTypeMetadataPayload, cancellationToken: cancellationToken);

            }
            catch (RpcException ex)
            {
                _logger?.LogError(ex, "gRPC error from IVI server");
                throw IVIException.FromGrpcException(ex);
            }
        }

    }
}