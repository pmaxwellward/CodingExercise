namespace InvestmentPerformanceWebAPI.Server.Services {
    public interface IPriceService {
        Task<decimal> GetCurrentPriceAsync(string name, CancellationToken ct = default);
    }
}
