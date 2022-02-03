using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Ivi.Proto.Api.Item;
using Ivi.Proto.Common.Item;
using Ivi.Proto.Common.Sort;
using Ivi.Rpc.Api.Item;
using Ivi.Rpc.Streams;
using Ivi.Rpc.Streams.Item;
using IviSdkCsharp.Client.Executor;
using Mapster;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Config;
using Mythical.Game.IviSdkCSharp.Exception;
using Mythical.Game.IviSdkCSharp.Model;
using Metadata = Ivi.Proto.Common.Metadata;

namespace Games.Mythical.Ivi.Sdk.Client;

public class IviItemClient : AbstractIVIClient
{
    private readonly IVIItemExecutor? _itemExecutor;
    private ItemService.ItemServiceClient? _client;
    private ItemStream.ItemStreamClient? _streamClient;

    public IviItemClient(IviConfiguration config, ILogger<IviItemClient>? logger)
        : base(config, logger: logger) { }

    internal IviItemClient(IviConfiguration config, ILogger<IviItemClient>? logger, HttpClient httpClient)
        : base(config, httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient }, logger) { }

    public IVIItemExecutor UpdateSubscription
    {
        init
        {
            _itemExecutor = value;
        }
    }

    public async Task SubscribeToStream()
    {
        if (_itemExecutor is null) throw new InvalidOperationException($"Cannot subscribe, {nameof(UpdateSubscription)} is not set. ");
        var (waitBeforeRetry, resetRetries) = GetReconnectAwaiter(_logger);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _streamClient = new ItemStream.ItemStreamClient(Channel);
                using var call = _streamClient.ItemStatusStream(new Subscribe { EnvironmentId = EnvironmentId }, cancellationToken: cancellationToken);
                await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    _logger.LogDebug("Item update subscription for item id {itemId}", response.GameInventoryId);
                    try
                    {
                        _itemExecutor?.UpdateItemAsync(response.GameInventoryId, response.GameItemTypeId, response.PlayerId, response.DgoodsId, response.SerialNumber, response.MetadataUri, response.TrackingId, response.ItemState);
                        await ConfirmItemUpdateAsync(response.GameInventoryId, response.TrackingId,
                            response.ItemState);
                        resetRetries();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error calling {nameof(_itemExecutor.UpdateItemAsync)}");
                    }
                }
                _logger.LogInformation("Item update stream closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Item update subscription error");
            }
            finally
            {
                await waitBeforeRetry();
            }
        }
    }
    private async Task ConfirmItemUpdateAsync(string gameInventoryId, string trackingId, ItemState itemState)
    {
        await _streamClient!.ItemStatusConfirmationAsync(new ItemStatusConfirmRequest
        {
            EnvironmentId = EnvironmentId,
            GameInventoryId = gameInventoryId,
            ItemState = itemState,
            TrackingId = trackingId
        }, cancellationToken: cancellationToken);
    }

    private ItemService.ItemServiceClient Client => _client ??= new ItemService.ItemServiceClient(Channel);

    public async Task IssueItemAsync(string gameInventoryId, string playerId, string itemName, string gameItemTypeId,
        decimal amountPaid, string currency, IviMetadata metadata, string storeId, string orderId, string requestIp, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug(
            "ItemClient.issueItem called with params: gameInventoryId {}, playerId {}, itemName {}, gameItemTypeId {}, amountPaid {}, currency {}, metadata {}, storeId {}, orderId {}, requestIp {}",
            gameInventoryId, playerId, itemName, gameItemTypeId, amountPaid, currency, metadata, storeId, orderId,
            requestIp);
        try
        {
            var request = new IssueItemRequest()
            {
                EnvironmentId = EnvironmentId,
                GameInventoryId = gameInventoryId,
                PlayerId = playerId,
                ItemName = itemName,
                GameItemTypeId = gameItemTypeId,
                Metadata = metadata.Adapt<Metadata>(),
                AmountPaid = amountPaid.ToString(CultureInfo.InvariantCulture),
                StoreId = storeId,
            };

            if (!string.IsNullOrEmpty(orderId))
            {
                request.OrderId = orderId;
            }


            if (!string.IsNullOrWhiteSpace(requestIp))
            {
                request.RequestIp = requestIp;
            }

            var results = await Client.IssueItemAsync(request, cancellationToken: cancellationToken);
            if (_itemExecutor != null)
            {
                await _itemExecutor!.UpdateItemStateAsync(gameInventoryId, results.TrackingId, results.ItemState);
            }
        }
        catch (RpcException e)
        {
            throw IVIException.FromGrpcException(e);
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception calling {nameof(IVIItemExecutor.UpdateItemStateAsync)} on {nameof(IssueItemAsync)}, player will be in an invalid state!", e);

            throw new IVIException(IVIErrorCode.LOCAL_EXCEPTION);
        }
    }

    public async Task TransferItemAsync(string gameInventoryId, string sourcePlayerId, string destPlayerId,
        string storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ItemClient.transferItem called with params: gameInventoryId {}, sourcePlayerId {}, destPlayerId {}, storeId {}", gameInventoryId, sourcePlayerId, destPlayerId, storeId);
        try
        {
            var request = new TransferItemRequest()
            {
                EnvironmentId = EnvironmentId,
                GameItemInventoryId = gameInventoryId,
                DestinationPlayerId = destPlayerId,
                SourcePlayerId = sourcePlayerId,
                StoreId = storeId
            };

            var result = await Client.TransferItemAsync(request, cancellationToken: cancellationToken);
            if (_itemExecutor != null)
            {
                await _itemExecutor!.UpdateItemStateAsync(gameInventoryId, result.TrackingId, result.ItemState);
            }

        }
        catch (RpcException e)
        {
            throw IVIException.FromGrpcException(e);
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception calling {nameof(IVIItemExecutor.UpdateItemStateAsync)} on {nameof(TransferItemAsync)}, player will be in an invalid state!", e);

            throw new IVIException(IVIErrorCode.LOCAL_EXCEPTION);
        }
    }

    public async Task BurnItemAsync(string gameInventoryId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ItemClient.burnItem called with param: gameInventoryId {}", gameInventoryId);
        try
        {
            var request = new BurnItemRequest()
            {
                EnvironmentId = EnvironmentId,
                GameItemInventoryId = gameInventoryId
            };
            var result = await Client.BurnItemAsync(request, cancellationToken: cancellationToken);
            if (_itemExecutor != null)
            {
                await _itemExecutor!.UpdateItemStateAsync(gameInventoryId, result.TrackingId, result.ItemState);
            }
        }
        catch (RpcException e)
        {
            throw IVIException.FromGrpcException(e);
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception calling {nameof(IVIItemExecutor.UpdateItemStateAsync)} on {nameof(BurnItemAsync)}, player will be in an invalid state!", e);

            throw new IVIException(IVIErrorCode.LOCAL_EXCEPTION);
        }
    }

    public async Task<IviItem?> GetItemAsync(string gameInventoryId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("ItemClient.getItem called with param: gameInventoryId {}", gameInventoryId);
        return await GetItemAsync(gameInventoryId, false, cancellationToken);
    }

    public async Task<IviItem?> GetItemAsync(string gameInventoryId, Boolean history, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("ItemClient.getItem called with params: gameInventoryId {}", gameInventoryId);
        try
        {
            GetItemRequest? request = null;

            if (!string.IsNullOrEmpty(gameInventoryId))
            {
                request = new GetItemRequest()
                {
                    EnvironmentId = EnvironmentId,
                    GameInventoryId = gameInventoryId,
                    History = history
                };
            }

            var response = await Client.GetItemAsync(request, cancellationToken: cancellationToken);
            return response.Adapt<IviItem>();
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

    public async Task<IList<IviItem>?> GetItemsAsync(DateTimeOffset createdTimestamp, int pageSize, SortOrder sortOrder, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("ItemClient.getItems called with params: createdTimestamp {}, pageSize {}, sortOrder {}", createdTimestamp, pageSize, sortOrder);
        try
        {
            var request = new GetItemsRequest()
            {
                EnvironmentId = EnvironmentId,
                PageSize = pageSize,
                SortOrder = sortOrder,
                CreatedTimestamp = (ulong)createdTimestamp.ToUnixTimeSeconds()
            };
            var result = await Client.GetItemsAsync(request, cancellationToken: cancellationToken);
            return result.Adapt<List<IviItem>>();
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

    public async Task UpdateItemMetadataAsync(String gameInventoryId, IviMetadata metadata, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("ItemClient.updateItemMetadata called with params: gameInventoryId {}, metadata {}", gameInventoryId, metadata);
        try
        {
            List<UpdateItemMetadata>? updateList = new() { new UpdateItemMetadata() { GameInventoryId = gameInventoryId, Metadata = metadata.Adapt<Metadata>() } };

            await _updateItemMetadataAsync(updateList, cancellationToken);
        }
        catch (RpcException e)
        {
            _logger?.LogError("gRPC error from IVI server", e);
            throw IVIException.FromGrpcException(e);
        }
    }

    public async Task UpdateItemMetadataAsync(List<IviMetadataUpdate> updates,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("ItemClient.updateItemMetadata called with param: updates {}", updates);
        try
        {
            List<UpdateItemMetadata> updateList = new();
            foreach (var iviMetadataUpdate in updates)
            {
                updateList.Add(new UpdateItemMetadata() { GameInventoryId = iviMetadataUpdate.GameInventoryId, Metadata = iviMetadataUpdate.Metadata.Adapt<Metadata>() });
            }

            await _updateItemMetadataAsync(updateList, cancellationToken);
        }
        catch (RpcException e)
        {
            _logger?.LogError("gRPC error from IVI server", e);
            throw IVIException.FromGrpcException(e);
        }
    }

    private async Task<UpdateItemMetadataResponse> _updateItemMetadataAsync(List<UpdateItemMetadata>? updateList,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("ItemClient.updateItemMetadata called with param: updateList {} ", updateList);
        try
        {
            var request = new UpdateItemMetadataRequest()
            {
                EnvironmentId = EnvironmentId,
                UpdateItems = { updateList }

            };
            var result = await Client.UpdateItemMetadataAsync(request, cancellationToken: cancellationToken);
            return result;
        }
        catch (RpcException e)
        {
            _logger?.LogError("gRPC error from IVI server", e);
            throw IVIException.FromGrpcException(e);
        }
    }
}