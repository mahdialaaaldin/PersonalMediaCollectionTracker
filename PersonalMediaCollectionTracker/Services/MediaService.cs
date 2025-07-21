using Microsoft.EntityFrameworkCore;
using PersonalMediaTracker.Data;
using PersonalMediaTracker.DTOs;
using PersonalMediaTracker.Models;

namespace PersonalMediaTracker.Services
{
    public class MediaService : IMediaService
    {
        private readonly MediaContext _context;

        public MediaService(MediaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaItem>> GetAllAsync(int userId)
        {
            return await _context.MediaItems
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<MediaItem?> GetByIdAsync(int id, int userId)
        {
            return await _context.MediaItems
                .Where(m => m.Id == id && m.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<MediaItem> CreateAsync(CreateMediaItemDto dto, int userId)
        {
            var mediaItem = new MediaItem
            {
                Title = dto.Title,
                Creator = dto.Creator,
                ReleaseDate = dto.ReleaseDate,
                Genre = dto.Genre,
                Type = dto.Type,
                Status = dto.Status,
                Notes = dto.Notes,
                UserId = userId, // Use the current user's ID
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MediaItems.Add(mediaItem);
            await _context.SaveChangesAsync();
            return mediaItem;
        }

        public async Task<MediaItem?> UpdateAsync(int id, UpdateMediaItemDto dto, int userId)
        {
            var mediaItem = await _context.MediaItems
                .Where(m => m.Id == id && m.UserId == userId)
                .FirstOrDefaultAsync();

            if (mediaItem == null) return null;

            mediaItem.Title = dto.Title;
            mediaItem.Creator = dto.Creator;
            mediaItem.ReleaseDate = dto.ReleaseDate;
            mediaItem.Genre = dto.Genre;
            mediaItem.Type = dto.Type;
            mediaItem.Status = dto.Status;
            mediaItem.Notes = dto.Notes;
            mediaItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return mediaItem;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var mediaItem = await _context.MediaItems
                .Where(m => m.Id == id && m.UserId == userId)
                .FirstOrDefaultAsync();

            if (mediaItem == null) return false;

            _context.MediaItems.Remove(mediaItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MediaItem>> SearchAsync(SearchDto searchDto, int userId)
        {
            var query = _context.MediaItems
                .Where(m => m.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchDto.Title))
            {
                query = query.Where(m => m.Title.Contains(searchDto.Title));
            }

            if (!string.IsNullOrEmpty(searchDto.Creator))
            {
                query = query.Where(m => m.Creator.Contains(searchDto.Creator));
            }

            if (!string.IsNullOrEmpty(searchDto.Genre))
            {
                query = query.Where(m => m.Genre.Contains(searchDto.Genre));
            }

            if (searchDto.Type.HasValue)
            {
                query = query.Where(m => m.Type == searchDto.Type.Value);
            }

            if (searchDto.Status.HasValue)
            {
                query = query.Where(m => m.Status == searchDto.Status.Value);
            }

            return await query.OrderBy(m => m.Title).ToListAsync();
        }
    }
}