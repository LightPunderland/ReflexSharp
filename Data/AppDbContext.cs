using Data.Configurations;
using Features.Score;
using Features.User.Entities;
using Microsoft.EntityFrameworkCore;
using Features.Sprite.Entities;
using Features.Audio.Entities;
using Features.Wardrobe.Entities;

namespace Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ScoreEntity> Scores { get; set; }
        public DbSet<SpriteEntity> Sprites { get; set; }
        public DbSet<AudioFileEntity> AudioFiles { get; set; }
         public DbSet<WardrobeItem> WardrobeItems { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpriteEntity>()
                .ToTable("sprites")
                .HasKey(s => s.Id);

            modelBuilder.Entity<AudioFileEntity>()
                .ToTable("audiofiles")
                .HasKey(a => a.Id);

            modelBuilder.Entity<ScoreEntity>()
                .HasIndex(s => s.Score)
                .HasDatabaseName("idx_score")
                .IsUnique(false);

            modelBuilder.Entity<WardrobeItem>()
                .ToTable("wardrobe_items")
                .HasKey(w => w.Id);

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

    }
}
