using InvestmentPerformanceWebAPI.Server.Database.Context;
using InvestmentPerformanceWebAPI.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPerformanceWebAPI.Server.Database.Repository {
    public sealed class TransactionRepository : ITransactionRepository {
        private readonly AppDbContext _db;

        public TransactionRepository(AppDbContext db) {
            _db = db;
        }

        public Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default) {
            return _db.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<IReadOnlyList<Transaction>> GetByUserAsync(int userId, CancellationToken ct = default) {
            var list = await _db.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.ExecutedAtUtc)
                .ToListAsync(ct);

            return list;
        }

        public async Task<IReadOnlyList<Transaction>> GetByInvestmentAsync(int investmentId, CancellationToken ct = default) {
            var list = await _db.Transactions
                .AsNoTracking()
                .Where(t => t.InvestmentId == investmentId)
                .OrderBy(t => t.ExecutedAtUtc)
                .ToListAsync(ct);

            return list;
        }
    }
}
