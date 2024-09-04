using System.ComponentModel.DataAnnotations;

namespace SampleApi.Entities;

public class Order
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    [Timestamp]
    public uint Version { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public List<OrderItem> Items { get; set; } = [];
}