namespace Sample.Model;

public class Order
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}