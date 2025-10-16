// Database/Repository/InvestmentRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvestmentPerformanceWebAPI.Server.Database.Context;
using InvestmentPerformanceWebAPI.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPerformanceWebAPI.Server.Database.Repository {
    public sealed class InvestmentRepository : IInvestmentRepository {
        private readonly AppDbContext _db;

        public InvestmentRepository(AppDbContext db) {
            _db = db;
        }

        public async Task<IReadOnlyList<Investment>> GetByUserAsync(int userId, CancellationToken ct = default) {
            List<Investment> list = await _db.Investments
                .AsNoTracking()
                .Where(i => i.UserId == userId)
                .OrderBy(i => i.Name)
                .ToListAsync(ct);

            return list;
        }

        public Task<Investment?> GetByIdAsync(int investmentId, CancellationToken ct = default) {
            return _db.Investments
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == investmentId, ct);
        }
    }
}
