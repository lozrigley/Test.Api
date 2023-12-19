namespace Test.Core.Models;

public abstract record Entity
{
    public Guid Id { get; init; }
}

public record Customer
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    
    public List<Order> Orders { get; init; }
}

public record Product
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string SKU { get; init; }
    
    public List<Order> Orders { get; init; }
}

public record Order
{
    public Guid Id { get; init; }
    public Guid CustomerId{ get; init; }
    public Guid ProductId { get; init; }
    
    public Product Product { get; init; }
    public Customer Customer { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? UpdatedDate { get; init; }
}

public enum OrderStatus
{
    New,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}