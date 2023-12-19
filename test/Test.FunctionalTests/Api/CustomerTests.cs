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
                FirstName = "GetAllCustomersReturnsAllCustomers1",
                LastName = "Customer",
                Email = "bob@gmail.com",
                Phone = "1234567890"
            };
            var customer2 = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "GetAllCustomersReturnsAllCustomers2",
                LastName = "Customer 2",
                Email = "bob@gmail.com",
                Phone = "1234567890"
            };
            var customer3 = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "GetAllCustomersReturnsAllCustomers3",
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
        var customerResponses = await response.Content.ReadFromJsonAsync<List<CustomerResponse>>();
        customerResponses.FirstOrDefault(c => c.FirstName == "GetAllCustomersReturnsAllCustomers1")!.Should().NotBeNull();
        customerResponses.FirstOrDefault(c => c.FirstName == "GetAllCustomersReturnsAllCustomers2")!.Should().NotBeNull();
        customerResponses.FirstOrDefault(c => c.FirstName == "GetAllCustomersReturnsAllCustomers3")!.Should().NotBeNull();
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

    [Fact]
    public async Task ReturnsBadRequestWhenRequiredFieldsAreNotPopulated()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var customer = new
        {

        };
        
        //Act
        var response = await client.PostAsJsonAsync("/customers", customer);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutCustomerUpdatesCustomer()
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
                Email = "kangaroo@bounce.com",
                Phone = "1234567890"
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

        }

        var customerToUpdate = new
        {
            FirstName = "Test",
            LastName = "Customer",
            Email = "bird@swim.com"
        };

        //Act
        var response = await client.PutAsJsonAsync($"/customers/{guid}", customerToUpdate);

        //Assert
        response.EnsureSuccessStatusCode();
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        jsonDocument!.RootElement.GetProperty("id").GetGuid().Should().Be(guid);
        jsonDocument!.RootElement.GetProperty("firstName").GetString().Should().Be("Test");
        jsonDocument!.RootElement.GetProperty("lastName").GetString().Should().Be("Customer");
        jsonDocument!.RootElement.GetProperty("email").GetString().Should().Be("bird@swim.com");
        jsonDocument!.RootElement.GetProperty("phone").GetString().Should().Be("");
    }

    [Fact]
    public async Task PutCustomerReturnsNotFoundWhenCustomerDoesNotExist()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        var customerToUpdate = new
        {
            FirstName = "Test",
            LastName = "Customer",
            Email = "some text",
            Phone = "1234567890"
        };

        //Act
        var response = await client.PutAsJsonAsync($"/customers/{guid}", customerToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task PutCustomerReturnsBadRequestWhenRequiredFieldsAreNotPopulated()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();
        var customerToUpdate = new
        {

        };

        //Act
        var response = await client.PutAsJsonAsync($"/customers/{guid}", customerToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCustomerDeletesCustomer()
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
                Email = "whatever",
                Phone = "1234567890"
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
        }

        //Act
        var response = await client.DeleteAsync($"/customers/{guid}");

        //Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task DeleteCustomerReturnsNotFoundWhenCustomerDoesNotExist()
    {
        //Arrange
        var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var guid = Guid.NewGuid();

        //Act
        var response = await client.DeleteAsync($"/customers/{guid}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}