using InvestmentPerformanceWebAPI.Server.DTOs;

namespace InvestmentPerformanceWebAPI.Server.Services {
    public interface IInvestmentQueryService {
        Task<IReadOnlyList<InvestmentListItemDto>> GetInvestmentsForUserAsync(int userId, CancellationToken ct = default);

        Task<InvestmentDetailsDto?> GetInvestmentDetailsAsync(int userId, int investmentId, CancellationToken ct = default);
    }
}
