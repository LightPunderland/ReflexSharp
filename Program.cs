using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();

var host = builder.Configuration["PSI_PROJECT_HOST"];
var database = builder.Configuration["PSI_PROJECT_DATABASE"];
var user = builder.Configuration["PSI_PROJECT_USER"];
var password = builder.Configuration["PSI_PROJECT_PASSWORD"];
var connectionString = $"Host={host};Database={database};Username={user};Password={password}";


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();

}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
