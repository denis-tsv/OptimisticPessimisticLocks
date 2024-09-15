using System.ComponentModel.DataAnnotations;

namespace SampleApi.Entities;

public class Order
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    [ConcurrencyCheck]
    public int? LockOwnerId { get; set; }
    
    public List<OrderItem> Items { get; set; } = [];
}