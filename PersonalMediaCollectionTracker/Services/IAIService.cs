using PersonalMediaTracker.DTOs;

namespace PersonalMediaTracker.Services
{
    public interface IAIService
    {
        Task<AIRecommendationDto> GetRecommendationAsync(string title, string creator);
        Task<string> SuggestGenreAsync(string title, string creator);
        Task<bool> CheckDuplicateAsync(string title, string creator, IEnumerable<string> existingTitles);
    }
}