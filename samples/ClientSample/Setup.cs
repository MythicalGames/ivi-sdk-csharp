using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mythical.Game.IviSdkCSharp.Config;
using Serilog;

namespace ClientSample
{
    class Setup
    {
        private readonly ServiceProvider _services;

        public Setup()
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Config).CreateLogger();
            var services = new ServiceCollection();
            services.AddLogging(x => x.AddSerilog());
            _services = services.BuildServiceProvider();
            IviConfig = new IviConfiguration
            {
                EnvironmentId = Config.GetSection("IviConfiguration:EnvironmentId").Value,
                ApiKey = Config.GetSection("IviConfiguration:ApiKey").Value,
                Host = Config.GetSection("IviConfiguration:Host").Value
            };
        }

        public IviConfiguration IviConfig { get; }

        public IConfiguration Config { get; } = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<Program>()
            .Build();

        public ILogger<T> CreateLogger<T>() => _services!.GetRequiredService<ILogger<T>>();
    }
}