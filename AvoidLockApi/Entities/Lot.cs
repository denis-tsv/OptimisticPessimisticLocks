namespace AvoidLockApi.Entities;

public class Lot
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public decimal Price { get; set; }
    public DateTime UpdatedAt { get; set; }
}