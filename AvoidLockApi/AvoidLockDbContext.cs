using AvoidLockApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace AvoidLockApi;

public class AvoidLockDbContext : DbContext
{
    public AvoidLockDbContext(DbContextOptions<AvoidLockDbContext> options) : base(options)
    {
    }
    
    public DbSet<Lot> Lots { get; set; }
    
    public DbSet<Bid> Bids { get; set; }
}