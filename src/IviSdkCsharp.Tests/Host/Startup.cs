using System.Security.Authentication;
using IviSdkCsharp.Tests.Item.Services;
using IviSdkCsharp.Tests.ItemType.Services;
using IviSdkCsharp.Tests.Order.Services;
using IviSdkCsharp.Tests.Payment.Services;
using IviSdkCsharp.Tests.Player.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IviSdkCsharp.Tests.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddGrpc();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.TryGetValue("api-key", out var apiKey)
                    && apiKey == TestWebApplicationFactory.ApiKey)
                {
                    await next();
                }
                else
                {
                    throw new AuthenticationException("API key was not set!");
                }
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<FakePlayerService>();
                endpoints.MapGrpcService<FakeItemTypeService>();
                endpoints.MapGrpcService<FakeOrderService>();
                endpoints.MapGrpcService<FakeItemService>();
                endpoints.MapGrpcService<FakePaymentService>();
            });
        }
    }
}