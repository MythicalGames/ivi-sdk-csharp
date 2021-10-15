using Grpc.Net.Client;
using IviSdkCsharp.Tests.Host;
using Xunit;

namespace IviSdkCsharp.Tests
{
    public class GrpcTestServerFixture
    {
        public const string GrpcTestServerFixtureCollection = "gRPC test server";
        public GrpcTestServerFixture()
        {
            var factory = new TestWebApplicationFactory();
            var client = factory.CreateClient();
            Channel = GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
            {
                HttpClient = client
            });
        }

        public GrpcChannel Channel { get;  }
    }
    
    [CollectionDefinition(GrpcTestServerFixture.GrpcTestServerFixtureCollection)]
    public class DatabaseCollection : ICollectionFixture<GrpcTestServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}