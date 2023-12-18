using Microsoft.Extensions.DependencyInjection;
using Test.Core.Models;
using Test.DataAccess;
using Xunit;

namespace Test.FunctionalTests.Api;

public class ApiTest
{
    [Fact]
    public async Task HelloWorld()
    {
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();
            var product = new Product
                { Id = Guid.NewGuid(), Name = "Test Product", Description = "Whatever", SKU = "asda1" };
            context.Products.Add(product);

            context.Database.EnsureCreated();
            await context.SaveChangesAsync();
        }

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal("Hello World!", responseString);
    }
}