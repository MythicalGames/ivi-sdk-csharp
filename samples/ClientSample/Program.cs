using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Sort;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Mapper;
using Mythical.Game.IviSdkCSharp.Model;
#pragma warning disable CS4014 // for not awaiting SubscribeToStream calls - alternative is Task.Run(...SubscribeToStream...)

namespace ClientSample;

class Program
{
    static async Task Main()
    {
        Setup setup = new ();
        MappersConfig.RegisterMappings();
        await TryRun(async () => await PlayerClient_Usage(setup), setup);
        await TryRun(async () => await ItemTypeClient_Usage(setup), setup);
        await TryRun(async () => await ItemClient_Usage(setup), setup);
        Console.ReadLine();
    }

    private static async Task TryRun(Func<Task> action, Setup setup)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            var logger = setup.CreateLogger<Program>();
            logger.LogError(ex, "Failed to run usage example");
        }
    }

    private static async Task ItemClient_Usage(Setup setup)
    {
        var logger = setup.CreateLogger<IviItemClient>();

        using var itemClient = new IviItemClient(setup.IviConfig, logger);
        itemClient.SubscribeToStream(new LoggingItemUpdateSubscription(logger));
        

        var items = await itemClient.GetItemsAsync(DateTimeOffset.MinValue, 4, SortOrder.Asc);
        logger.LogInformation("GetItemsAsync: {@Items}", items);
    }

    private static async Task PlayerClient_Usage(Setup setup)
    {
        var logger = setup.CreateLogger<IviPlayerClient>();

        using var playerClient = new IviPlayerClient(setup.IviConfig, logger);
        playerClient.SubscribeToStream(new LoggingPlayerUpdateSubscription(logger));
        var players = await playerClient.GetPlayersAsync(DateTimeOffset.MinValue, 3, IviSortOrder.Desc);
        logger.LogInformation("GetPlayersAsync: {@Players}", players);
    }

    private static async Task ItemTypeClient_Usage(Setup setup)
    {
        var logger = setup.CreateLogger<IviItemTypeClient>();

        using var itemTypeClient = new IviItemTypeClient(setup.IviConfig, logger);
        itemTypeClient.SubscribeToStream(new LoggingItemTypeUpdateSubscription(logger));

        var metadataProperties = new Dictionary<string, object>
        {
            {"base_price_usd", 1000},
            {"season_display_name", "winter item types"},
            {"collection", "WINTER"},
            {"item_class", "the class"}
        };

        var iviMetadata = new IviMetadata
        {
            Name = "name",
            Description = "desc",
            Image = "adsfs",
            Properties = metadataProperties
        };

        var iviItemType = new IviItemType
        {
            GameItemTypeId = "itemType.GameItemTypeId2",
            TokenName = "itemType.TokenName2",
            Category = "itemType.Category",
            MaxSupply = 1000,
            IssueTimeSpan = 0,
            Burnable = true,
            Transferable = true,
            Sellable = true,
            Metadata = iviMetadata
        };

        await itemTypeClient.CreateItemTypeAsync(iviItemType);

        var itemTypes = await itemTypeClient.GetItemTypesAsync();

        logger.LogInformation("GetItemTypesAsync: {itemTypes}", itemTypes);
    }
}