using System.ComponentModel.DataAnnotations;

namespace LocksApi.Entities;

public class Order
{
    public int Id { get; set; }

    [Timestamp]
    public uint Version { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}