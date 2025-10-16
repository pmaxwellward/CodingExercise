namespace InvestmentPerformanceWebAPI.Server.DTOs {
    public sealed class InvestmentListItemDto {
        public int InvestmentId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
