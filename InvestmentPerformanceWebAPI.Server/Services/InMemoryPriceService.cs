// Services/InMemoryPriceService.cs
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace InvestmentPerformanceWebAPI.Server.Services {
    public sealed class InMemoryPriceService : IPriceService {
        private readonly ConcurrentDictionary<string, decimal> _prices;

        public InMemoryPriceService() {
            _prices = new ConcurrentDictionary<string, decimal>();

            // Seed a couple of example prices; adjust freely
            _prices["ACME"] = (decimal)123.45;
            _prices["BETA"] = (decimal)47.80;
        }

        public Task<decimal> GetCurrentPriceAsync(string name, CancellationToken ct = default) {
            if (_prices.TryGetValue(name, out decimal price)) {
                return Task.FromResult(price);
            }

            // Default to zero if unknown; you could choose to throw instead
            return Task.FromResult((decimal)0.0);
        }

        // Optional helpers for tests or admin endpoints:

        public void SetPrice(string name, decimal price) {
            _prices[name] = price;
        }

        public bool TryGetPrice(string name, out decimal price) {
            return _prices.TryGetValue(name, out price);
        }
    }
}
