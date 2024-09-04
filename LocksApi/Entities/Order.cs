using System.ComponentModel.DataAnnotations;

namespace SampleApi.Entities;

public class Order
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    [Timestamp]
    public uint Version { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}