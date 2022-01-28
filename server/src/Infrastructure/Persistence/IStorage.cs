using System.Threading;
using System.Threading.Tasks;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence
{
    public interface IStorage
    {
        DatabaseFacade Database { get; }
        DbSet<User> Users { get; }
        DbSet<CipherLogin> CipherLogins { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}