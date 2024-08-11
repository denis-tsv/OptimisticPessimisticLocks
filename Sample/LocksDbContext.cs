using Microsoft.EntityFrameworkCore;
using Sample.Model;

namespace Sample;

public class LocksDbContext : DbContext
{
    public LocksDbContext(DbContextOptions<LocksDbContext> options) : base(options)
    {
    }
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}