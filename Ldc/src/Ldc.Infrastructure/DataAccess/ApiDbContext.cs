using Ldc.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }

    override protected void OnModelCreating(ModelBuilder modelBuilder) // Configure Tag entity to map to "Tags" table
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Tag>().ToTable("Tags");
    }
}