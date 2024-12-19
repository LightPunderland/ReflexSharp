using Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Features.Wardrobe.Services;
using Features.TraficMonitoring;
using Features.TraficMonitoring.Middleware;
using System.Threading.Tasks;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5050");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();

Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(50000); // 50 sec
        Console.Clear();
        Console.WriteLine($"=== API Monitor Active === {DateTime.Now}");
        Console.WriteLine("Listening...");
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var host = builder.Configuration["PSI_PROJECT_HOST"];
var database = builder.Configuration["PSI_PROJECT_DATABASE"];
var user = builder.Configuration["PSI_PROJECT_USER"];
var password = builder.Configuration["PSI_PROJECT_PASSWORD"];
var connectionString = $"Host={host};Database={database};Username={user};Password={password}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
builder.Services.AddScoped<IWardrobeService, WardrobeService>();

var app = builder.Build();
app.UseMiddleware<TraficMonitoringMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
