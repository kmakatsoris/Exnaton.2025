using Exnaton.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exnaton.Models;

public class AppDbContext : DbContext
{
    public DbSet<MeasurementDataEntity> MeasurementData { get; set; }
    public DbSet<TagsEntity> Tags { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeasurementDataEntity>()
            .HasIndex(m => new { m.Measurement, m.TagsMUId, m.Timestamp });
        
        modelBuilder.Entity<TagsEntity>()
            .HasIndex(t => t.Muid);
        
        // modelBuilder.Entity<MeasurementDataEntity>()
        //     .HasOne(m => m.Tags)
        //     .WithMany()
        //     .HasForeignKey(m => m.TagsMUId);
    }
}