namespace AvoidLockApi.Entities;

public class Bid
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LotId { get; set; }
    public string Status { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public Lot Lot { get; set; }
}