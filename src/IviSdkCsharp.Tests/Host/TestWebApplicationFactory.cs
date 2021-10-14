using IviSdkCsharp.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace IviSdkCsharp.Tests.Host
{
    public class TestWebApplicationFactory: WebApplicationFactory<Startup>
    {
        public TestWebApplicationFactory()
        {
            IviConfiguration.EnvironmentId = "Test environment id";
            IviConfiguration.ApiKey = "Test api key";
        }

        protected override IHostBuilder CreateHostBuilder() =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}