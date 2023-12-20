using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Test.Api.Models;
using Test.Core;
using Test.Core.Models;

namespace Test.Api;

public static class Endpoints
{
    public static void AddProductEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("/products", async (IProductRepository productRepository) =>
        {
            var products = (await productRepository.GetAllAsync())
                .Select(ProductResponse.CreateFrom);
            return Results.Ok(products);
        });

        webApplication.MapGet("/products/{id}", async (IProductRepository productRepository, Guid id) =>
        {
            var result = await productRepository.GetByIdAsync(id);
            return result.Match(
                product => Results.Ok(ProductResponse.CreateFrom(product)),
                notFound => Results.NotFound()
            );
        });

        webApplication.MapPost("/products", async ([FromServices] IProductRepository productRepository, ProductRequest productRequest) =>
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(productRequest, null, null);

            if (!Validator.TryValidateObject(productRequest, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }
            
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productRequest.Name,
                Description = productRequest.Description,
                SKU = productRequest.SKU
            };
            try
            {
                var createdProduct = await productRepository.CreateAsync(product);

                return Results.Created($"/products/{createdProduct.Id}", createdProduct);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        });

        webApplication.MapPut("/products/{id}", async ([FromServices] IProductRepository productRepository, Guid id, ProductRequest productRequest) =>
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(productRequest, null, null);

            if (!Validator.TryValidateObject(productRequest, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }
            
            var productUpdate = new Product
            {
                Id = id,
                Name = productRequest.Name,
                Description = productRequest.Description ?? string.Empty,
                SKU = productRequest.SKU
            };
            
            var result = await productRepository.UpdateAsync(productUpdate);
            return result.Match(
                product => Results.Ok(ProductResponse.CreateFrom(product)),
                notFound => Results.NotFound()
            );
        });

        webApplication.MapDelete("/products/{id}", async (IProductRepository productRepository, Guid id) =>
        {
            var result = await productRepository.DeleteAsync(id);
            return result.Match(
                success => Results.Ok(),
                notFound => Results.NotFound()
            );
        });
    }
    
    public static void AddCustomerEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("/customers", async (ICustomerRepository customerRepository) =>
        {
            var customers = (await customerRepository.GetAllAsync())
                .Select(CustomerResponse.CreateFrom);
            return Results.Ok(customers);
        });

        webApplication.MapGet("/customers/{id}", async (ICustomerRepository customerRepository, Guid id) =>
        {
            var result = await customerRepository.GetByIdAsync(id);
            return result.Match(
                customer => Results.Ok(CustomerResponse.CreateFrom(customer)),
                notFound => Results.NotFound()
            );
        });

        webApplication.MapPost("/customers", async (ICustomerRepository customerRepository, CustomerRequest customerRequest) =>
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(customerRequest, null, null);
            
            if (!Validator.TryValidateObject(customerRequest, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }
            
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = customerRequest.FirstName,
                LastName = customerRequest.LastName,
                Email = customerRequest.Email,
                Phone = customerRequest.Phone ?? string.Empty
            };
            try
            {
                var createdCustomer = await customerRepository.CreateAsync(customer);
        
                return Results.Created($"/products/{createdCustomer.Id}", createdCustomer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        
        });
        
        webApplication.MapPut("/customers/{id}", async (ICustomerRepository customerRepository, Guid id, CustomerRequest customerRequest) =>
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(customerRequest, null, null);
            
            if (!Validator.TryValidateObject(customerRequest, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }
            
            var customerUpdate = new Customer
            {
                Id = id,
                FirstName = customerRequest.FirstName,
                LastName = customerRequest.LastName,
                Email = customerRequest.Email,
                Phone = customerRequest.Phone ?? string.Empty
            };
            
            var result = await customerRepository.UpdateAsync(customerUpdate);
            return result.Match(
                customer => Results.Ok(CustomerResponse.CreateFrom(customer)),
                notFound => Results.NotFound()
            );
        });
        
        webApplication.MapDelete("/customers/{id}", async (ICustomerRepository customerRepository, Guid id) =>
        {
            var result = await customerRepository.DeleteAsync(id);
            return result.Match(
                success => Results.Ok(),
                notFound => Results.NotFound()
            );
        });
    }

    public static void AddOrderEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("/orders", async (IOrderRepository orderRepository) =>
        {
            var order = (await orderRepository.GetAllAsync())
                .Select(OrderResponse.CreateFrom);
            return Results.Ok(order);
        });
        
        webApplication.MapGet("/orders/{id}", async (IOrderRepository orderRepository, Guid id) =>
        {
            var result = await orderRepository.GetByIdAsync(id);
            return result.Match(
                order => Results.Ok(OrderResponse.CreateFrom(order)),
                notFound => Results.NotFound()
            );
        });
        
        webApplication.MapPost("/orders", async (IOrderRepository orderRepository, OrderRequest orderRequest) =>
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(orderRequest, null, null);
            
            if (!Validator.TryValidateObject(orderRequest, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }
            
            var orderDto = new OrderDto
            {
                Id = Guid.NewGuid(),
                CustomerId = orderRequest.CustomerId,
                ProductId = orderRequest.ProductId,
                Status = orderRequest.Status
            };

            var response = await orderRepository.CreateAsync(orderDto);
            return response.Match(
                order => Results.Created($"/orders/{order.Id}", OrderResponse.CreateFrom(order)),
                missingReferenceData => Results.BadRequest("Missing reference data")
            );

        
        });
        
        webApplication.MapPut("/orders/{id}", async (IOrderRepository orderRepository, Guid id, OrderRequest orderRequest) =>
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(orderRequest, null, null);
            
            if (!Validator.TryValidateObject(orderRequest, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults);
            }
            
            var orderDto= new OrderDto
            {
                Id = id,
                CustomerId = orderRequest.CustomerId,
                ProductId = orderRequest.ProductId,
                Status = orderRequest.Status
            };
            
            var result = await orderRepository.UpdateAsync(orderDto);
            return result.Match(
                order => Results.Ok(OrderResponse.CreateFrom(order)),
                notFound => Results.NotFound(),
                missingReferenceData => Results.BadRequest("Missing reference data")
            );
        });
        
        webApplication.MapDelete("/orders/{id}", async (IOrderRepository orderRepository, Guid id) =>
        {
            var result = await orderRepository.DeleteAsync(id);
            return result.Match(
                success => Results.Ok(),
                notFound => Results.NotFound()
            );
        });
        
    }
}