using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InvestmentPerformanceWebAPI.Server.Database.Models;

namespace InvestmentPerformanceWebAPI.Server.Database.Repository {
    public interface IInvestmentRepository {
        Task<IReadOnlyList<Investment>> GetByUserAsync(int userId, CancellationToken ct = default);

        Task<Investment?> GetByIdAsync(int investmentId, CancellationToken ct = default);
    }
}
