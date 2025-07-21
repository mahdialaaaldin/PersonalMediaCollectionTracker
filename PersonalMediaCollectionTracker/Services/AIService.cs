using PersonalMediaTracker.DTOs;
using System.Text;
using System.Text.Json;

namespace PersonalMediaTracker.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration["GeminiAPI:ApiKey"] ?? "";
            _baseUrl = _configuration["GeminiAPI:BaseUrl"] ?? "";
        }

        public async Task<AIRecommendationDto> GetRecommendationAsync(string title, string creator)
        {
            try
            {
                var prompt = $@"Analyze this media item: '{title}' by '{creator}'. 
                Provide a JSON response with:
                1. Most appropriate genre (single word)
                2. 3 similar items you'd recommend
                3. Brief reasoning (1 sentence)

                Respond ONLY with valid JSON in this exact format:
                {{
                    ""suggestedGenre"": ""genre"",
                    ""similarItems"": [""item1"", ""item2"", ""item3""],
                    ""reasoning"": ""explanation""
                }}

                Do not include any text outside the JSON object.";

                var response = await CallGeminiAPI(prompt);

                // Clean and parse the JSON response
                try
                {
                    // Remove markdown code blocks and extra formatting
                    var cleanResponse = response.Trim();
                    cleanResponse = cleanResponse.Replace("```json", "").Replace("```", "").Trim();

                    // Find JSON object boundaries
                    var startIndex = cleanResponse.IndexOf('{');
                    var endIndex = cleanResponse.LastIndexOf('}');

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        cleanResponse = cleanResponse.Substring(startIndex, endIndex - startIndex + 1);
                    }

                    var jsonResponse = JsonSerializer.Deserialize<AIRecommendationDto>(cleanResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return jsonResponse ?? CreateFallbackRecommendation(title);
                }
                catch (Exception ex)
                {
                    // For debugging - you can remove this later
                    Console.WriteLine($"JSON Parse Error: {ex.Message}");
                    Console.WriteLine($"Raw response: {response}");
                    return CreateFallbackRecommendation(title);
                }
            }
            catch
            {
                return CreateFallbackRecommendation(title);
            }
        }

        public async Task<string> SuggestGenreAsync(string title, string creator)
        {
            try
            {
                var prompt = $"What genre best describes '{title}' by '{creator}'? Respond with only the genre name (one or two words max).";
                var response = await CallGeminiAPI(prompt);
                return response.Trim().Replace("\"", "");
            }
            catch
            {
                return "Unknown";
            }
        }

        public async Task<bool> CheckDuplicateAsync(string title, string creator, IEnumerable<string> existingTitles)
        {
            try
            {
                var existingList = string.Join(", ", existingTitles.Take(10)); // Limit for prompt size
                var prompt = $@"Is '{title}' by '{creator}' likely the same as any of these existing items: {existingList}? 
                Consider variations in spelling, abbreviations, and alternative titles. 
                Respond with only 'YES' or 'NO'.";

                var response = await CallGeminiAPI(prompt);
                return response.Trim().ToUpper().Contains("YES");
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> CallGeminiAPI(string prompt)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                throw new InvalidOperationException("Gemini API key not configured");
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}?key={_apiKey}", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Gemini API call failed: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);

            var text = responseJson.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "";
        }

        private static AIRecommendationDto CreateFallbackRecommendation(string title)
        {
            return new AIRecommendationDto
            {
                SuggestedGenre = "Unknown",
                SimilarItems = new List<string> { "No recommendations available" },
                Reasoning = "AI service temporarily unavailable"
            };
        }
    }
}