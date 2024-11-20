using Features.Score;
using Features.User.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<ScoreEntity> Scores { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
