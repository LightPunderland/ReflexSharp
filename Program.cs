using Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Features.Wardrobe.Services;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5050");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();

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
