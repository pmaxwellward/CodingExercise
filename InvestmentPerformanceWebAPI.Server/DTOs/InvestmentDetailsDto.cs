// DTOs/InvestmentDetailsDto.cs
namespace InvestmentPerformanceWebAPI.Server.DTOs {
    public sealed class InvestmentDetailsDto {
        public int InvestmentId { get; set; }

        public string Name { get; set; } = string.Empty;        

        public decimal NumberOfShares { get; set; }

        public decimal CostBasisPerShare { get; set; }

        public decimal CurrentPrice { get; set; }

        public decimal CurrentValue { get; set; }

        public string Term { get; set; } = string.Empty; // "ShortTerm" or "LongTerm"

        public decimal TotalGainLoss { get; set; }
    }
}
