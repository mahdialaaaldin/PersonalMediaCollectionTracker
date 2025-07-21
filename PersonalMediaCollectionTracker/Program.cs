using Microsoft.EntityFrameworkCore;
using PersonalMediaTracker.Data;
using PersonalMediaTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite Database (persistent file storage)
builder.Services.AddDbContext<MediaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IAIService, AIService>();

// Configure HttpClient for AI service
builder.Services.AddHttpClient<IAIService, AIService>();

// Add memory cache for sessions
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

// Simple session-based auth for demo
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("http://localhost:*", "https://localhost:*")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");

// Serve static files
app.UseStaticFiles();

// Enable default files (index.html)
app.UseDefaultFiles();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MediaContext>();

    try
    {
        var created = context.Database.EnsureCreated();

        if (created)
        {
            Console.WriteLine("Database created successfully with new schema!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database creation error: {ex.Message}");
    }
}

app.Run();