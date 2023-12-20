using System.ComponentModel.DataAnnotations;
using Test.Core.Models;

namespace Test.Api.Models;

public record OrderResponse
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public Guid ProductId { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public OrderStatus Status { get; init; }
    
    public static OrderResponse CreateFrom(Order order) => new()
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        ProductId = order.ProductId,
        CreatedDate = order.CreatedDate,
        UpdatedDate = order.UpdatedDate,
        Status = order.Status
    };
}

public record OrderRequest
{
    [Required]
    public Guid CustomerId { get; init; }
    [Required]
    public Guid ProductId { get; init; }
    [Required]
    public OrderStatus Status { get; init; }
}