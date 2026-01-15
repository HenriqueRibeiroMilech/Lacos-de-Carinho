using Ldc.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<WeddingList> WeddingLists { get; set; }
    public DbSet<GiftItem> GiftItems { get; set; }
    public DbSet<Rsvp> Rsvps { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<TemplateGiftItem> TemplateGiftItems { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Tag>().ToTable("Tags");
        
        // WeddingList configuration
        modelBuilder.Entity<WeddingList>()
            .HasOne(w => w.Owner)
            .WithMany(u => u.WeddingLists)
            .HasForeignKey(w => w.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WeddingList>()
            .HasIndex(w => w.ShareableLink)
            .IsUnique();

        // GiftItem configuration
        modelBuilder.Entity<GiftItem>()
            .HasOne(g => g.WeddingList)
            .WithMany(w => w.Items)
            .HasForeignKey(g => g.WeddingListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GiftItem>()
            .HasOne(g => g.ReservedBy)
            .WithMany()
            .HasForeignKey(g => g.ReservedById)
            .OnDelete(DeleteBehavior.SetNull);

        // Rsvp configuration
        modelBuilder.Entity<Rsvp>()
            .HasOne(r => r.WeddingList)
            .WithMany(w => w.Rsvps)
            .HasForeignKey(r => r.WeddingListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rsvp>()
            .HasOne(r => r.Guest)
            .WithMany()
            .HasForeignKey(r => r.GuestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rsvp>()
            .HasIndex(r => new { r.WeddingListId, r.GuestId })
            .IsUnique();

        // Category configuration
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        // TemplateGiftItem configuration
        modelBuilder.Entity<TemplateGiftItem>()
            .HasOne(t => t.Category)
            .WithMany(c => c.TemplateItems)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // PasswordResetToken configuration
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PasswordResetToken>()
            .HasIndex(p => p.Token)
            .IsUnique();
    }
}