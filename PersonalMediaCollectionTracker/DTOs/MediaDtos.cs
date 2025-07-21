using PersonalMediaTracker.Models;

namespace PersonalMediaTracker.DTOs
{
    public class CreateMediaItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public MediaStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateMediaItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public MediaStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class SearchDto
    {
        public string? Title { get; set; }
        public string? Creator { get; set; }
        public string? Genre { get; set; }
        public MediaType? Type { get; set; }
        public MediaStatus? Status { get; set; }
    }

    public class AIRecommendationDto
    {
        public string SuggestedGenre { get; set; } = string.Empty;
        public List<string> SimilarItems { get; set; } = new();
        public string Reasoning { get; set; } = string.Empty;
    }
}