using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Test.Core;
using Test.Core.Models;

namespace Test.Api;

public static class Endpoints
{
    public static void AddProductEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("/products", async (IProductRepository productRepository) =>
        {
            var products = await productRepository.GetAllAsync();
            return Results.Ok(products);
        });

        webApplication.MapGet("/products/{id}", async (IProductRepository productRepository, Guid id) =>
        {
            var result = await productRepository.GetByIdAsync(id);
            return result.Match(
                product => Results.Ok(product),
                notFound => Results.NotFound()
            );
        });

        webApplication.MapPost("/products", async ([FromServices] IProductRepository productRepository, Product product) =>
        {
            var createdProduct = await productRepository.CreateAsync(product);
            return Results.Created($"/products/{createdProduct.Id}", createdProduct);
        });

        webApplication.MapPut("/products/{id}", async ([FromServices] IProductRepository productRepository, Guid id, Product productRequest) =>
        {
            if (productRequest.Id != id) return Results.BadRequest();
            
            var result = await productRepository.UpdateAsync(productRequest);
            return result.Match(
                product => Results.Ok(product),
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
}