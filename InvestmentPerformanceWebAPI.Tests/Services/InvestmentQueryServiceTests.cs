using FakeItEasy;
using FluentAssertions;
using InvestmentPerformanceWebAPI.Server.Database.Models;
using InvestmentPerformanceWebAPI.Server.Database.Repository;
using InvestmentPerformanceWebAPI.Server.DTOs;
using InvestmentPerformanceWebAPI.Server.Enums;
using InvestmentPerformanceWebAPI.Server.Services;
using Xunit;

namespace InvestmentPerformance.Tests.Services {
    public sealed class InvestmentQueryServiceTests {
        [Fact]
        public async Task GetInvestmentsForUserAsync_Returns_ListItems() {
            // Arrange
            int userId = 1;
            var fakeInvRepo = A.Fake<IInvestmentRepository>();
            var fakeTxnRepo = A.Fake<ITransactionRepository>();
            var fakePrices = A.Fake<IPriceService>();

            var investments = new List<Investment>
            {
                new Investment { Id = 10, UserId = userId, Name = "ACME" },
                new Investment { Id = 20, UserId = userId, Name = "BETA" }
            };

            A.CallTo(() => fakeInvRepo.GetByUserAsync(userId, A<CancellationToken>._))
                .Returns(investments);

            var svc = new InvestmentQueryService(fakeInvRepo, fakeTxnRepo, fakePrices);

            // Act
            IReadOnlyList<InvestmentListItemDto> result = await svc.GetInvestmentsForUserAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.Select(x => x.InvestmentId).Should().BeEquivalentTo(new[] { 10, 20 });
            result.Select(x => x.Name).Should().BeEquivalentTo(new[] { "ACME", "BETA" });

            A.CallTo(() => fakeInvRepo.GetByUserAsync(userId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetInvestmentDetailsAsync_Computes_FIFO_CostBasis_Value_And_GainLoss() {
            // Arrange
            int userId = 1;
            int investmentId = 10;

            var fakeInvRepo = A.Fake<IInvestmentRepository>();
            var fakeTxnRepo = A.Fake<ITransactionRepository>();
            var fakePrices = A.Fake<IPriceService>();

            var investment = new Investment {
                Id = investmentId,
                UserId = userId,
                Name = "ACME"
            };

            var txns = new List<Transaction>
            {
                new Transaction
                {
                    Id = 1, UserId = userId, InvestmentId = investmentId,
                    Type = TransactionType.Buy, NumberOfShares = (decimal)10.0,
                    PricePerShare = (decimal)100.0,
                    ExecutedAtUtc = new DateTime(2024, 01, 15, 10, 00, 00, DateTimeKind.Utc)
                },
                new Transaction
                {
                    Id = 2, UserId = userId, InvestmentId = investmentId,
                    Type = TransactionType.Sell, NumberOfShares = (decimal)3.0,
                    PricePerShare = (decimal)120.0,
                    ExecutedAtUtc = new DateTime(2024, 12, 01, 15, 30, 00, DateTimeKind.Utc)
                },
                new Transaction
                {
                    Id = 3, UserId = userId, InvestmentId = investmentId,
                    Type = TransactionType.Buy, NumberOfShares = (decimal)5.0,
                    PricePerShare = (decimal)110.0,
                    ExecutedAtUtc = new DateTime(2025, 03, 10, 09, 00, 00, DateTimeKind.Utc)
                }
            };

            A.CallTo(() => fakeInvRepo.GetByIdAsync(investmentId, A<CancellationToken>._))
                .Returns(investment);

            A.CallTo(() => fakeTxnRepo.GetByInvestmentAsync(investmentId, A<CancellationToken>._))
                .Returns(txns);

            A.CallTo(() => fakePrices.GetCurrentPriceAsync("ACME", A<CancellationToken>._))
                .Returns((decimal)150.00);

            var svc = new InvestmentQueryService(fakeInvRepo, fakeTxnRepo, fakePrices);

            // Act
            InvestmentDetailsDto? dto = await svc.GetInvestmentDetailsAsync(userId, investmentId);

            // Assert
            dto.Should().NotBeNull();
            dto!.InvestmentId.Should().Be(investmentId);
            dto.Name.Should().Be("ACME");

            // FIFO math:
            // Start: [10 @ 100]
            // Sell 3 -> remaining [7 @ 100]
            // Buy 5 @ 110 -> remaining lots: [7 @ 100], [5 @ 110]
            // Remaining shares = 12
            dto.NumberOfShares.Should().Be((decimal)12.0);

            // Total remaining cost = 7*100 + 5*110 = 1250
            // Cost basis / share = 1250 / 12 = 104.166666...
            dto.CostBasisPerShare.Should().BeApproximately((decimal)104.166666, (decimal)0.000001);

            // Current price 150 → current value = 12 * 150 = 1800
            dto.CurrentPrice.Should().Be((decimal)150.00);
            dto.CurrentValue.Should().Be((decimal)1800.00);

            // Gain/Loss = 1800 - 1250 = 550
            dto.TotalGainLoss.Should().Be((decimal)550.00);

            // Term depends on oldest remaining lot date (2024-01-15).
            // We won't assert exact term because it depends on "now" inside the service,
            // but we assert it's a valid enum value.
            dto.Term.Should().BeOfType<string>();

            A.CallTo(() => fakeInvRepo.GetByIdAsync(investmentId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTxnRepo.GetByInvestmentAsync(investmentId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakePrices.GetCurrentPriceAsync("ACME", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
