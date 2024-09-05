namespace ExclusiveLockApi.Entities;

public class Account
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required decimal Volume { get; set; }
    public required decimal BlockedVolume { get; set; }
}