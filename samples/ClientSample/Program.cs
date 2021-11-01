using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common;
using Ivi.Proto.Common.Sort;
using IviSdkCsharp.Client.Executor;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Mapper;
using Mythical.Game.IviSdkCSharp.Model;

namespace ClientSample
{
    class Program
    {
        static async Task Main()
        {
            Setup setup = new ();
            setup.SetIviConfiguration();
            MappersConfig.RegisterMappings();

            await PlayerClient_Usage(setup);
            await ItemTypeClient_Usage(setup);

            Console.ReadLine();
        }

        private static async Task PlayerClient_Usage(Setup setup)
        {
            var logger = setup.CreateLogger<IviPlayerClient>();

            var playerClient = new IviPlayerClient(logger)
            {
                UpdateSubscription = new LoggingPlayerUpdateSubscription(logger)
            };
            var players = await playerClient.GetPlayersAsync(DateTimeOffset.MinValue, 3, SortOrder.Desc);
            logger.LogInformation("GetPlayersAsync: {@Players}", players);
        }

        private static async Task ItemTypeClient_Usage(Setup setup)
        {
            var logger = setup.CreateLogger<IviItemTypeClient>();

            var itemTypeClient = new IviItemTypeClient(logger)
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

            var iviMetadata = new IviMetadata("name", "desc", "sdsfs", metadataProperties);

            var iviItemType = new IviItemType
            {
                GameItemTypeId = "itemType.GameItemTypeId",
                TokenName = "itemType.TokenName",
                Category = "itemType.Category",
                MaxSupply = 1000,
                IssueTimeSpan = 0,
                Burnable = true,
                Transferable = true,
                Sellable = true,
                //Metadata = iviMetadata
            };

            await itemTypeClient.CreateItemTypeAsync(iviItemType);

            var itemTypes = await itemTypeClient.GetItemTypesAsync();

            logger.LogInformation("GetItemTypesAsync: {itemTypes}", itemTypes);
        }
    }
}
