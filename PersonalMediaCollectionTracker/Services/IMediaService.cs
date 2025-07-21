using PersonalMediaTracker.DTOs;
using PersonalMediaTracker.Models;

namespace PersonalMediaTracker.Services
{
    public interface IMediaService
    {
        Task<IEnumerable<MediaItem>> GetAllAsync(int userId);
        Task<MediaItem?> GetByIdAsync(int id, int userId);
        Task<MediaItem> CreateAsync(CreateMediaItemDto dto, int userId);
        Task<MediaItem?> UpdateAsync(int id, UpdateMediaItemDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<IEnumerable<MediaItem>> SearchAsync(SearchDto searchDto, int userId);
    }
}