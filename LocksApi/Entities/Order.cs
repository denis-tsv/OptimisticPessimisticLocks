using System.ComponentModel.DataAnnotations;

namespace LocksApi.Entities;

public class Order
{
    public int Id { get; set; }

    [ConcurrencyCheck]
    public int? LockOwnerId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}