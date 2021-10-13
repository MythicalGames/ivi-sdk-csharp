using System.Threading.Tasks;
using IviSdkCsharp.Tests.Host;
using Shouldly;
using Xunit;

namespace IviSdkCsharp.Tests
{
    public class UnitTest1: IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public UnitTest1(TestWebApplicationFactory factory) => _factory = factory;

        [Fact]
        public async Task Test1()
        {
            var client = _factory.CreateClient();
            var result = await client.GetAsync("/");

            result.EnsureSuccessStatusCode();
            (await result.Content.ReadAsStringAsync()).ShouldBe("Hello World!");
        }
    }
}