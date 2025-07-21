using Microsoft.EntityFrameworkCore;
using PersonalMediaTracker.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PersonalMediaTracker.Data
{
    public class MediaContext : DbContext
    {
        public MediaContext(DbContextOptions<MediaContext> options) : base(options)
        {
        }

        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure MediaItem
            modelBuilder.Entity<MediaItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Creator).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Genre).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();

            });

            // Seed default user and data for the demo user
            var defaultUserId = 1;
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = defaultUserId,
                    Username = "demo",
                    Email = "demo@example.com",
                    PasswordHash = "demo123", 
                    FirstName = "Demo",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed some sample data
            modelBuilder.Entity<MediaItem>().HasData(
                new MediaItem
                {
                    Id = 1,
                    Title = "The Matrix",
                    Creator = "The Wachowskis",
                    ReleaseDate = new DateTime(1999, 3, 31),
                    Genre = "Sci-Fi",
                    Type = MediaType.Movie,
                    Status = MediaStatus.Owned,
                    Notes = "Classic sci-fi movie",
                    UserId = defaultUserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new MediaItem
                {
                    Id = 2,
                    Title = "Cyberpunk 2077",
                    Creator = "CD Projekt Red",
                    ReleaseDate = new DateTime(2020, 12, 10),
                    Genre = "RPG",
                    Type = MediaType.Game,
                    Status = MediaStatus.Completed,
                    Notes = "Open world RPG",
                    UserId = defaultUserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new MediaItem
                {
                    Id = 3,
                    Title = "Abbey Road",
                    Creator = "The Beatles",
                    ReleaseDate = new DateTime(1969, 9, 26),
                    Genre = "Rock",
                    Type = MediaType.Music,
                    Status = MediaStatus.Wishlist,
                    Notes = "Iconic album",
                    UserId = defaultUserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}