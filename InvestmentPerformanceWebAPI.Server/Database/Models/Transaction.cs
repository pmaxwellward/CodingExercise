namespace InvestmentPerformanceWebAPI.Server.Database.Models {
    public enum TransactionType {
        Buy = 1,
        Sell = 2
    }

    public sealed class Transaction {
        public int Id { get; set; }       
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int InvestmentId { get; set; }
        public TransactionType Type { get; set; }
        public decimal NumberOfShares { get; set; }     
        public decimal PricePerShare { get; set; }
        public DateTime ExecutedAtUtc { get; set; }
    }
}
