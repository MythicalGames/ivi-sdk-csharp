using System;
using System.Net.Http;
using IviSdkCsharp.Tests.Host;
using Microsoft.AspNetCore.Mvc.Testing;
using Mythical.Game.IviSdkCSharp.Config;
using Xunit;

namespace IviSdkCsharp.Tests;

public class GrpcTestServerFixture
{
    public const string GrpcTestServerFixtureCollection = "gRPC test server";
    public GrpcTestServerFixture()
    {
        var factory = new TestWebApplicationFactory();
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

    }

    public HttpClient Client { get; }

    public static IviConfiguration Config { get; } = new()
    {
        ApiKey = TestWebApplicationFactory.ApiKey,
        EnvironmentId = "test environment id"
    };
}

[CollectionDefinition(GrpcTestServerFixture.GrpcTestServerFixtureCollection, DisableParallelization = true)]
public class DatabaseCollection : ICollectionFixture<GrpcTestServerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}