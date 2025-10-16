using System.ComponentModel.DataAnnotations.Schema;

namespace InvestmentPerformanceWebAPI.Server.Database.Models 
{
    // Database/Models/Transaction.cs
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace YourProject.Database.Models {
        public enum TransactionType {
            Buy = 1,
            Sell = 2
        }

        public sealed class Transaction {
            public int Id { get; set; }

            public int UserId { get; set; }

            public User User { get; set; } = null!;

            public required string Symbol { get; set; }

            public TransactionType Type { get; set; }

            public decimal NumberOfShares { get; set; }

            // Price paid per share at purchase time
            public decimal CostBasisPerShare { get; set; }

            public DateTime ExecutedAtUtc { get; set; }

            // Transient: populated by a pricing service at query time
            [NotMapped]
            public decimal? CurrentPrice { get; set; }

            [NotMapped]
            public decimal CurrentValue {
                get {
                    if (CurrentPrice.HasValue == false) {
                        return 0m;
                    }

                    return NumberOfShares * CurrentPrice.Value;
                }
            }

            [NotMapped]
            public string Term {
                get {
                    TimeSpan age = DateTime.UtcNow - ExecutedAtUtc;
                    return age.TotalDays > 365.0 ? "LongTerm" : "ShortTerm";
                }
            }

            [NotMapped]
            public decimal TotalGainLoss {
                get {
                    if (CurrentPrice.HasValue == false) {
                        return 0m;
                    }

                    decimal amountPaidForAllShares = NumberOfShares * CostBasisPerShare;
                    decimal currentValue = NumberOfShares * CurrentPrice.Value;
                    return currentValue - amountPaidForAllShares;
                }
            }
        }
    }
}
