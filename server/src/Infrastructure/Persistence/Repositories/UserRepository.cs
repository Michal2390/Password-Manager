using System.Threading;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IStorage _storage;

        public UserRepository(IStorage storage)
        {
            _storage = storage;
        }

        public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _storage.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        
        public async Task<User?> GetByEmailOrUsernameAsync(string email, string username,
            CancellationToken cancellationToken = default)
        {
            return await _storage.Users
                .FirstOrDefaultAsync(u => u.Email.Equals(email.ToLower()) || u.Username.Equals(username.ToLower()),
                    cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _storage.Users.FirstOrDefaultAsync(u => u.Username.Equals(username.ToLower()),
                cancellationToken);
        }

        public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            await _storage.Users.AddAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
        {
            await Task.FromResult(_storage.Users.Remove(entity));
        }
    }
}