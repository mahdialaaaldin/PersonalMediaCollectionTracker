using Microsoft.AspNetCore.Mvc;
using PersonalMediaTracker.DTOs;
using PersonalMediaTracker.Services;

namespace PersonalMediaTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IAIService _aiService;

        public MediaController(IMediaService mediaService, IAIService aiService)
        {
            _mediaService = mediaService;
            _aiService = aiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var items = await _mediaService.GetAllAsync(userId.Value);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var item = await _mediaService.GetByIdAsync(id, userId.Value);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMediaItemDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check for duplicates first
            var existingItems = await _mediaService.GetAllAsync(userId.Value);
            var existingTitles = existingItems.Select(i => i.Title).ToList();
            var isDuplicate = await _aiService.CheckDuplicateAsync(dto.Title, dto.Creator, existingTitles);

            if (isDuplicate)
            {
                return Conflict(new { message = "A similar item already exists in your collection" });
            }

            // AI: Suggest genre if not provided
            if (string.IsNullOrEmpty(dto.Genre))
            {
                try
                {
                    dto.Genre = await _aiService.SuggestGenreAsync(dto.Title, dto.Creator);
                }
                catch
                {
                    dto.Genre = "Unknown";
                }
            }

            var item = await _mediaService.CreateAsync(dto, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMediaItemDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check for duplicates (excluding the current item)
            var existingItems = await _mediaService.GetAllAsync(userId.Value);
            var existingTitles = existingItems
                .Where(i => i.Id != id)  // Exclude the item being updated
                .Select(i => i.Title)
                .ToList();

            var isDuplicate = await _aiService.CheckDuplicateAsync(dto.Title, dto.Creator, existingTitles);
            if (isDuplicate)
            {
                return Conflict(new { message = "Another item with this title/creator already exists" });
            }

            var item = await _mediaService.UpdateAsync(id, dto, userId.Value);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var success = await _mediaService.DeleteAsync(id, userId.Value);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchDto searchDto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var items = await _mediaService.SearchAsync(searchDto, userId.Value);
            return Ok(items);
        }

        [HttpPost("{id}/ai-recommendation")]
        public async Task<IActionResult> GetAIRecommendation(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var item = await _mediaService.GetByIdAsync(id, userId.Value);
            if (item == null) return NotFound();

            var recommendation = await _aiService.GetRecommendationAsync(item.Title, item.Creator);
            return Ok(recommendation);
        }
    }
}