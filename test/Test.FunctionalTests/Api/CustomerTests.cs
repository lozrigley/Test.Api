using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Test.Core.Models;
using Test.DataAccess;
using Xunit;

namespace Test.FunctionalTests.Api;

public class CustomerTests
{
    [Fact]
    public async Task GetsCustomerWhenItExists()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        using (var scope = factory.Services.CreateScope())
        {
            
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();
            context.Database.EnsureCreated();
            var customer = new Customer
            {
                Id = guid,
                FirstName = "Test",
                LastName = "Customer",
                Email = "hello@home.com",
                Phone = "1234567890"
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
        }
        
        //Act
        var response = await client.GetAsync($"/customers/{guid}");
        
        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.GetProperty("id").GetGuid().Should().Be(guid);
        jsonDocument!.RootElement.GetProperty("firstName").GetString().Should().Be("Test");
        jsonDocument!.RootElement.GetProperty("lastName").GetString().Should().Be("Customer");
        jsonDocument!.RootElement.GetProperty("email").GetString().Should().Be("hello@home.com");
        jsonDocument!.RootElement.GetProperty("phone").GetString().Should().Be("1234567890");

    }
}