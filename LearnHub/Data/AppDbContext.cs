using LearnHub.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Data;

/// <summary>Entity Framework Core context for the local SQLite database.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<LearningResource> Resources => Set<LearningResource>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ContactMessage> Messages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<LearningResource>()
            .HasOne(r => r.Category)
            .WithMany(c => c.Resources)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // A member may only enrol in the same resource once
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.ResourceId })
            .IsUnique();

        // A member may only review a resource once
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.ResourceId })
            .IsUnique();
    }
}
