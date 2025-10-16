// Database/Repository/UserRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvestmentPerformanceWebAPI.Server.Database.Context;
using InvestmentPerformanceWebAPI.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPerformanceWebAPI.Server.Database.Repository {
    public sealed class UserRepository : IUserRepository {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) {
            _db = db;
        }

        public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) {
            return _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) {
            var list = await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .ToListAsync(ct);

            return list;
        }
    }
}
