using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Test.Api.Models;
using Test.Core.Models;
using Test.DataAccess;
using Xunit;

namespace Test.FunctionalTests.Api;

public class OrderTests
{
    [Fact]
    public async Task GetOrdersReturnsAllOrders()
    {
        //Arrange
        var factory = new ApiWebApplicationFactoryWithInMemoryDatabase();
        var client = factory.CreateClient();
        
        var product1 = new Product
        {   Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "OrderTest",
            SKU = "ABC1"
        };
        var customer1 = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "OrderTest",
            Email = "hello@goodbye.com",
            Phone = "1234567890"
        };
        
        
        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            ProductId = product1.Id,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Status = OrderStatus.New
        };
        
        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            ProductId = product1.Id,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Status = OrderStatus.Delivered
        };
        var order3 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            ProductId = product1.Id,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Status = OrderStatus.Cancelled
        };
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();
            
            context.Products.Add(product1);
            context.Customers.Add(customer1);
            context.Orders.Add(order1);
            context.Orders.Add(order2);
            context.Orders.Add(order3);
            await context.SaveChangesAsync();
        }

        //Act
        var response = await client.GetAsync("/orders");

        //Assert
        response.EnsureSuccessStatusCode();
        
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument.RootElement.EnumerateArray().FirstOrDefault(j => j.GetProperty("id").GetGuid() == order1.Id)!.Should().NotBeNull();
        jsonDocument.RootElement.EnumerateArray().FirstOrDefault(j => j.GetProperty("id").GetGuid() == order2.Id)!.Should().NotBeNull();
        jsonDocument.RootElement.EnumerateArray().FirstOrDefault(j => j.GetProperty("id").GetGuid() == order3.Id)!.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOrderReturnsOrder()
    {
        //Arrange
        var factory = new ApiWebApplicationFactoryWithInMemoryDatabase();
        var client = factory.CreateClient();

        var product1 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "OrderTest",
            SKU = "ABC1"
        };
        var customer1 = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "OrderTest",
            Email = "a@a.com",
            Phone = "1234567890"
        };
        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            ProductId = product1.Id,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Status = OrderStatus.New
        };
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();

            context.Products.Add(product1);
            context.Customers.Add(customer1);
            context.Orders.Add(order1);
            await context.SaveChangesAsync();
        }

        //Act
        var response = await client.GetAsync($"/orders/{order1.Id}");

        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.GetProperty("id").GetGuid().Should().Be(order1.Id);
        jsonDocument!.RootElement.GetProperty("customerId").GetGuid().Should().Be(customer1.Id);
        jsonDocument!.RootElement.GetProperty("productId").GetGuid().Should().Be(product1.Id);
        jsonDocument!.RootElement.GetProperty("status").GetString().Should().Be("New");
        //Getting late. Not going to test dates
    }

    [Fact]
    public async Task GetOrdersReturnsNotFoundWhenOrderDoesNotExist()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        
        //Act
        var response = await client.GetAsync($"/orders/{guid}");
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}