using ExclusiveLockApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveLockApi;

public class ExclusiveLockDbContext : DbContext
{
    public ExclusiveLockDbContext(DbContextOptions<ExclusiveLockDbContext> options) : base(options)
    {
    }
    
    public DbSet<Account> Accounts { get; set; }
    
    public DbSet<RenderImage> RenderImages { get; set; }
}