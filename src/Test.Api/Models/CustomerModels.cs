using System.ComponentModel.DataAnnotations;
using Test.Core.Models;

namespace Test.Api.Models;

public record CustomerResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    
    public static CustomerResponse CreateFrom(Customer customer) => new()
    {
        Id = customer.Id,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        Email = customer.Email,
        Phone = customer.Phone
    };
}

public record CustomerRequest
{
    [Required]
    public string FirstName { get; init; }
    [Required]
    public string LastName { get; init; }
    [Required]
    public string Email { get; init; }
    public string? Phone { get; init; }
}

