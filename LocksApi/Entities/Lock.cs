namespace SampleApi.Entities;

public class Lock
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public DateTime CreatedAt { get; set; }
}