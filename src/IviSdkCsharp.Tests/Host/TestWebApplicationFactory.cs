using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace IviSdkCsharp.Tests.Host
{
    public class TestWebApplicationFactory: WebApplicationFactory<Startup>
    {
        // protected override void ConfigureWebHost(IWebHostBuilder builder)
        // {
        //     
        // }

        protected override IHostBuilder CreateHostBuilder() =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}