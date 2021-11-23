using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Sort;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Mapper;
using Mythical.Game.IviSdkCSharp.Model;

namespace ClientSample;

class Program
{
    static async Task Main()
    {
        Setup setup = new ();
        MappersConfig.RegisterMappings();

        await PlayerClient_Usage(setup);
        await ItemTypeClient_Usage(setup);
        await ItemClient_Usage(setup);
        Console.ReadLine();
    }

    private static async Task ItemClient_Usage(Setup setup)
    {
        var logger = setup.CreateLogger<IviItemClient>();

        var itemClient = new IviItemClient(setup.IviConfig, logger)
        {
            UpdateSubscription = new LoggingItemUpdateSubscription(logger)
        };

        var items = await itemClient.GetItemsAsync(DateTimeOffset.MinValue, 4, SortOrder.Asc);
        logger.LogInformation("GetItemsAsync: {@Items}", items);
    }

    private static async Task PlayerClient_Usage(Setup setup)
    {
        var logger = setup.CreateLogger<IviPlayerClient>();

        var playerClient = new IviPlayerClient(setup.IviConfig, logger)
        {
            UpdateSubscription = new LoggingPlayerUpdateSubscription(logger)
        };
        var players = await playerClient.GetPlayersAsync(DateTimeOffset.MinValue, 3, IviSortOrder.Desc);
        logger.LogInformation("GetPlayersAsync: {@Players}", players);
    }

    private static async Task ItemTypeClient_Usage(Setup setup)
    {
        var logger = setup.CreateLogger<IviItemTypeClient>();

        var itemTypeClient = new IviItemTypeClient(setup.IviConfig, logger)
        {
            UpdateSubscription = new LoggingItemTypeUpdateSubscription(logger)
        };

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