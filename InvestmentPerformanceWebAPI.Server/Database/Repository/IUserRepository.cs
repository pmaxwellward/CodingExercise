using InvestmentPerformanceWebAPI.Server.Database.Models;

namespace InvestmentPerformanceWebAPI.Server.Database.Repository {
    public interface IUserRepository {
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    }
}
