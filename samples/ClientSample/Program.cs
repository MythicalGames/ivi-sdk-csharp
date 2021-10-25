using System;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
using Ivi.Proto.Common.Sort;
using Microsoft.Extensions.Logging;

namespace ClientSample
{
    class Program
    {
        static async Task Main()
        {
            Setup setup = new ();
            setup.SetIviConfiguration();
            var logger = setup.CreateLogger<IviPlayerClient>();

            var playerClient = new IviPlayerClient(logger)
            {
                UpdateSubscription = new LoggingPlayerUpdateSubscription(logger)
            };
            var players = await playerClient.GetPlayersAsync(DateTimeOffset.MinValue, 3, SortOrder.Desc);
            logger.LogInformation("GetPlayersAsync: {@Players}", players);

            Console.ReadLine();
        }
    }
}
