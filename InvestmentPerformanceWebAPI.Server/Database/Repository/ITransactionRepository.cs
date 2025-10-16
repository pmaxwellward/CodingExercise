using InvestmentPerformanceWebAPI.Server.Database.Models;

namespace InvestmentPerformanceWebAPI.Server.Database.Repository {
    public interface ITransactionRepository {
        Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Transaction>> GetByUserAsync(int userId, CancellationToken ct = default);
        Task<IReadOnlyList<Transaction>> GetByInvestmentAsync(int inventmestId, CancellationToken ct = default);
    }
}
