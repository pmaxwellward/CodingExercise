// Services/InvestmentQueryService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvestmentPerformanceWebAPI.Server.Database.Models;
using InvestmentPerformanceWebAPI.Server.DTOs;
using InvestmentPerformanceWebAPI.Server.Enums;
using InvestmentPerformanceWebAPI.Server.Services;
using InvestmentPerformanceWebAPI.Server.Database.Repository;
using InvestmentPerformanceWebAPI.Server.Extensions;

namespace InvestmentPerformanceWebAPI.Server.Services {
    public sealed class InvestmentQueryService : IInvestmentQueryService {
        private readonly IInvestmentRepository _investments;
        private readonly ITransactionRepository _transactions;
        private readonly IPriceService _prices;

        public InvestmentQueryService(
            IInvestmentRepository investments,
            ITransactionRepository transactions,
            IPriceService prices) {
            _investments = investments;
            _transactions = transactions;
            _prices = prices;
        }

        public async Task<IReadOnlyList<InvestmentListItemDto>> GetInvestmentsForUserAsync(int userId, CancellationToken ct = default) {
            var list = await _investments.GetByUserAsync(userId, ct);

            var dtos = list
                .OrderBy(i => i.Name)
                .Select(i => new InvestmentListItemDto {
                    InvestmentId = i.Id,
                    Name = i.Name
                })
                .ToList();

            return dtos;
        }

        public async Task<InvestmentDetailsDto?> GetInvestmentDetailsAsync(int userId, int investmentId, CancellationToken ct = default) {
            var inv = await _investments.GetByIdAsync(investmentId, ct);
            if (inv is null || inv.UserId != userId) {
                return null;
            }

            var txns = await _transactions.GetByInvestmentAsync(investmentId, ct);
            if (txns.Count == 0) {
                var priceEmpty = await _prices.GetCurrentPriceAsync(inv.Name, ct);

                return new InvestmentDetailsDto {
                    InvestmentId = inv.Id,
                    Name = inv.Name,
                    NumberOfShares = 0m,
                    CostBasisPerShare = 0m,
                    CurrentPrice = priceEmpty,
                    CurrentValue = 0m,
                    Term = InvestmentTerm.ShortTerm.GetDisplayName(),
                    TotalGainLoss = 0m
                };
            }

            var ordered = txns.OrderBy(t => t.ExecutedAtUtc).ToList();

            // Build FIFO buy lots, consume by sells
            var lots = new List<(decimal Shares, decimal PricePerShare, DateTime BoughtAtUtc)>();

            foreach (var t in ordered) {
                if (t.Type == TransactionType.Buy) {
                    lots.Add((t.NumberOfShares, t.PricePerShare, t.ExecutedAtUtc));
                } else {
                    decimal toSell = t.NumberOfShares;
                    int idx = 0;

                    while (toSell > 0m && idx < lots.Count) {
                        var lot = lots[idx];

                        if (lot.Shares <= toSell) {
                            toSell -= lot.Shares;
                            lots.RemoveAt(idx);
                            // do not increment idx; next lot shifts into current index
                        } else {
                            decimal remaining = lot.Shares - toSell;
                            lots[idx] = (remaining, lot.PricePerShare, lot.BoughtAtUtc);
                            toSell = 0m;
                        }
                    }
                }
            }

            decimal remainingShares = lots.Sum(l => l.Shares);
            decimal totalCostRemaining = lots.Sum(l => l.Shares * l.PricePerShare);

            if (remainingShares <= 0m) {
                var priceZeroPos = await _prices.GetCurrentPriceAsync(inv.Name, ct);
                
                return new InvestmentDetailsDto {
                    InvestmentId = inv.Id,
                    Name = inv.Name,
                    NumberOfShares = 0m,
                    CostBasisPerShare = 0m,
                    CurrentPrice = priceZeroPos,
                    CurrentValue = 0m,
                    Term = InvestmentTerm.ShortTerm.GetDisplayName(),
                    TotalGainLoss = 0m
                };
            }

            decimal costBasisPerShare = totalCostRemaining / remainingShares;
            DateTime oldestLotDate = lots.Min(l => l.BoughtAtUtc);
            InvestmentTerm term = (DateTime.UtcNow - oldestLotDate).TotalDays > 365.0
                ? InvestmentTerm.LongTerm
                : InvestmentTerm.ShortTerm;

            decimal currentPrice = await _prices.GetCurrentPriceAsync(inv.Name, ct);
            decimal currentValue = remainingShares * currentPrice;
            decimal totalGainLoss = currentValue - totalCostRemaining;

            return new InvestmentDetailsDto {
                InvestmentId = inv.Id,
                Name = inv.Name,
                NumberOfShares = decimal.Round(remainingShares, 6),
                CostBasisPerShare = decimal.Round(costBasisPerShare, 6),
                CurrentPrice = decimal.Round(currentPrice, 6),
                CurrentValue = decimal.Round(currentValue, 2),
                Term = term.GetDisplayName(),
                TotalGainLoss = decimal.Round(totalGainLoss, 2)
            };
        }
    }
}
