using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Data;
using Features.User.DTOs;
using Features.User.Entities;


public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureServices(services =>
    {
       
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb");
        });

        
        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            GoogleId = "google123",
            Email = "test@example.com",
            DisplayName = "TestUser",
            Rank = Rank.Noob,
            Gold = 100,
            XP = 200
        });

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            GoogleId = "google456",
            Email = "test2@example.com",
            DisplayName = "AnotherTestUser",
            Rank = Rank.Pro,
            Gold = 50,
            XP = 100
        });

        db.SaveChanges();
    });
}

}
