using LocksApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocksApi;

public class LocksDbContext : DbContext
{
    public LocksDbContext(DbContextOptions<LocksDbContext> options) : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}