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
            var products = (await customerRepository.GetAllAsync())
                .Select(CustomerResponse.CreateFrom);
            return Results.Ok(products);
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
    }
}