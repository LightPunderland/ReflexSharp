using Data.Configurations;
using Features.Score;
using Features.User.Entities;
using Microsoft.EntityFrameworkCore;
using Features.Sprite.Entities;


namespace Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<ScoreEntity> Scores { get; set; }

        public DbSet<SpriteEntity> Sprites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpriteEntity>()
                .ToTable("sprites")
                .HasKey(s => s.Id);



            modelBuilder.Entity<ScoreEntity>()
                .HasIndex(s => s.Score)
                .HasDatabaseName("idx_score")
                .IsUnique(false);

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

    }
}
