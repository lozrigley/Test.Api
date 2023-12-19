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

    [Fact]
    public async Task GetAllCustomersReturnsAllCustomers()
    {
        //Arrange
        var factory = new ApiWebApplicationFactoryWithInMemoryDatabase();
        var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope())
        {

            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();
            context.Database.EnsureCreated();
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Customer",
                Email = "bob@gmail.com",
                Phone = "1234567890"
            };
            var customer2 = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Customer 2",
                Email = "bob@gmail.com",
                Phone = "1234567890"
            };
            var customer3 = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Customer 3",
                Email = "bob@gmail.com",
                Phone = "1234567890"
            };
            context.Customers.Add(customer);
            context.Customers.Add(customer2);
            context.Customers.Add(customer3);

            await context.SaveChangesAsync();
        }
        
        //Act
        var response = await client.GetAsync("/customers");

        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.EnumerateArray().Count().Should().Be(3);
        jsonDocument.RootElement[0].GetProperty("lastName").GetString().Should().Be("Customer");
        jsonDocument.RootElement[1].GetProperty("lastName").GetString().Should().Be("Customer 2");
        jsonDocument.RootElement[2].GetProperty("lastName").GetString().Should().Be("Customer 3");
    }

    [Fact]
    public async Task CreateCustomerCreatesCustomer()
    {
        //Arrange
        var factory = new ApiWebApplicationFactoryWithInMemoryDatabase();
        var client = factory.CreateClient();
        var customer = new
        {
            FirstName = "Test",
            LastName = "Customer",
            Email = "pete@live.co.uk",
            Phone = "1234567890"
        };

        //Act
        var response = await client.PostAsJsonAsync("/customers", customer);

        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.GetProperty("firstName").GetString().Should().Be("Test");
        jsonDocument!.RootElement.GetProperty("lastName").GetString().Should().Be("Customer");
        jsonDocument!.RootElement.GetProperty("email").GetString().Should().Be("pete@live.co.uk");
        jsonDocument!.RootElement.GetProperty("phone").GetString().Should().Be("1234567890");
    }
}