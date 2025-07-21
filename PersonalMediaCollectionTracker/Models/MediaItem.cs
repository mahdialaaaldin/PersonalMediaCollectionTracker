using System.ComponentModel.DataAnnotations;

namespace PersonalMediaTracker.Models
{
    public class MediaItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Creator { get; set; } = string.Empty;

        public DateTime? ReleaseDate { get; set; }

        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;

        public MediaType Type { get; set; }

        public MediaStatus Status { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Simple UserId for demo (no navigation property)
        public int UserId { get; set; }
    }

    public enum MediaType
    {
        Movie = 1,
        Music = 2,
        Game = 3
    }

    public enum MediaStatus
    {
        Wishlist = 1,
        Owned = 2,
        CurrentlyUsing = 3,
        Completed = 4
    }
}