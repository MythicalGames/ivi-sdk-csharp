using System;
using System.Threading.Tasks;
using Games.Mythical.Ivi.Sdk.Client;
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
            var player = await playerClient.GetPlayerAsync("JohnDoe");
            logger.LogInformation("GetPlayerAsync: {@Player}", player);

            Console.ReadLine();
        }
    }
}
