using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Test.Core.Models;
using Test.DataAccess;
using Xunit;

namespace Test.FunctionalTests.Api;

public class ProductTests
{
    public ProductTests()
    {
        if (DatabaseContext.IsRefreshed) return;
        var factory = new ApiWebApplicationFactory();
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();
            if (!DatabaseContext.IsRefreshed);
            {
                context.Products.RemoveRange(context.Products);
                context.SaveChanges();
            }
            DatabaseContext.IsRefreshed = true;
        }
        
    }
    [Fact]
    public async Task HelloWorld()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        //Act
        var response = await client.GetAsync("/");

        //Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal("Hello World!", responseString);
    }

    [Fact]
    public async Task GetsProductWhenItExists()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();
            var product = new Product
                { Id = guid, Name = "Test Product", Description = "Whatever", SKU = "asda1" };
            context.Products.Add(product);

            context.Database.EnsureCreated();
            await context.SaveChangesAsync();
        }
        
        //Act
        var response = await client.GetAsync($"/products/{guid}");
        
        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.GetProperty("id").GetGuid().Should().Be(guid);
        jsonDocument.RootElement.GetProperty("name").GetString().Should().Be("Test Product");
        jsonDocument.RootElement.GetProperty("description").GetString().Should().Be("Whatever");
        jsonDocument.RootElement.GetProperty("sku").GetString().Should().Be("asda1");
    }
    
    [Fact]
    public async Task GetProductReturnsNotFoundWhenProductDoesNotExist()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        
        //Act
        var response = await client.GetAsync($"/products/{guid}");
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdatesProductSuccesfully()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        var product = new Product
            { Id = guid, Name = "Test Product", Description = "Whatever", SKU = "ABC1" };
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();

            context.Products.Add(product);

            context.Database.EnsureCreated();
            await context.SaveChangesAsync();
        }
        var stringContent = new StringContent(JsonSerializer.Serialize(product with { Description = "Whatever2" }), Encoding.UTF8, "application/json");
        
        //Act
        var response = await client.PutAsync($"/products/{guid}", stringContent);
        
        
        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.GetProperty("id").GetGuid().Should().Be(guid);
        jsonDocument.RootElement.GetProperty("name").GetString().Should().Be("Test Product");
        jsonDocument.RootElement.GetProperty("description").GetString().Should().Be("Whatever2");
        jsonDocument.RootElement.GetProperty("sku").GetString().Should().Be("ABC1");
    }
    
    [Fact]
    public async Task UpdateReturnsNotFoundWhenProductDoesNotExist()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        var product = new Product
            { Id = guid, Name = "Test Product", Description = "Whatever", SKU = "ABC1" };
        var stringContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        
        //Act
        var response = await client.PutAsync($"/products/{guid}", stringContent);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteIsSuccessful()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        var product = new Product
            { Id = guid, Name = "Test Product", Description = "Whatever", SKU = "ABC1" };
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TestApiDbContext>();

            context.Products.Add(product);

            context.Database.EnsureCreated();
            await context.SaveChangesAsync();
        }
        
        //Act
        var response = await client.DeleteAsync($"/products/{guid}");
        
        //Assert
        response.EnsureSuccessStatusCode();
        var response2 = await client.GetAsync($"/products/{guid}");
        response2.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
    
    [Fact]
    public async Task DeleteReturnsNotFoundWhenProductDoesNotExist()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        
        //Act
        var response = await client.DeleteAsync($"/products/{guid}");
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task PostProductReturnsNewProductWithId()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        var product = new Product
            { Name = "Test Product", Description = "Whatever", SKU = "ABC1" };
        var stringContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        
        //Act
        var response = await client.PostAsync($"/products", stringContent);
        
        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.GetProperty("id").GetGuid().Should().NotBeEmpty();
        jsonDocument.RootElement.GetProperty("name").GetString().Should().Be("Test Product");
        jsonDocument.RootElement.GetProperty("description").GetString().Should().Be("Whatever");
        jsonDocument.RootElement.GetProperty("sku").GetString().Should().Be("ABC1");
    }
    
    [Fact]
    public async Task PostProductReturnsBadRequestWhenNameIsMissing()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        var product = new Product
            { Description = "Whatever", SKU = "ABC1" };
        var stringContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        
        //Act
        var response = await client.PostAsync($"/products", stringContent);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    

}