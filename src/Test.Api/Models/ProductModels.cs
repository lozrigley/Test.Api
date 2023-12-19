using System.ComponentModel.DataAnnotations;
using Test.Core.Models;

namespace Test.Api.Models;

public record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string SKU { get; init; } = null!;
    
    public static ProductResponse CreateFrom(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        SKU = product.SKU
    };
}

public record ProductRequest
{
    [Required]
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    [Required]
    public string SKU { get; init; }
}

