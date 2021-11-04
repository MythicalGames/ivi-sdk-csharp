using System;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Ivi.Proto.Common.Sort;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp;
using Mythical.Game.IviSdkCSharp.Mapper;
using ProtoBuf.Meta;

// using Mythical.Game.IviSdkCSharp.Model;

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
            var metadataProperties = new Struct
            {
                Fields =
                {
                    {"base_price_usd", Value.ForNumber(1000)},
                    {"season_display_name", Value.ForString("winter item types")},
                    {"collection", Value.ForString("WINTER")},
                    {"item_class", Value.ForString("the class")}
                }
            };

            var iviMetadata = new Metadata{
                Name = "name",
                Description = "desc",
                Image = "sdsfs", 
                Properties = JsonFormatter.Default.Format(metadataProperties)
            };

            var iviItemType = new CreateItemTypeRequest
            {
                GameItemTypeId = "itemType.GameItemTypeId3",
                TokenName = "itemType.TokenName3",
                Category = "itemType.Category3",
                MaxSupply = 1000,
                IssueTimeSpan = 1,
                Burnable = true,
                Transferable = true,
                Sellable = true,
                Metadata = iviMetadata
            };

            var schema = RuntimeTypeModel.Default.GetSchema(typeof(CreateItemTypeRequest));

            await itemTypeClient.CreateItemTypeAsync(iviItemType);

            var itemTypes = await itemTypeClient.GetItemTypesAsync();

            logger.LogInformation("GetItemTypesAsync: {itemTypes}", itemTypes);
        }
    }


}
