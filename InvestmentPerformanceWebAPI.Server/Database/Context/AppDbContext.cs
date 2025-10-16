using InvestmentPerformanceWebAPI.Server.Database.Models;
using InvestmentPerformanceWebAPI.Server.Database.Models.YourProject.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPerformanceWebAPI.Server.Database.Context {
    public sealed class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }

        public DbSet<User> Users {
            get { return Set<User>(); }
        }

        public DbSet<Transaction> Transactions {
            get { return Set<Transaction>(); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>(b => {

                // Seed users
                b.HasData(
                    new User {
                        Id = 1,
                        Name = "Alice"
                    },
                    new User {
                        Id = 2,
                        Name = "Bob"
                    }
                );
            });

            modelBuilder.Entity<Transaction>(b => {

                b.HasData(
                    // --- Alice (UserId = 1) ---
                    new Transaction {
                        Id = 1,
                        UserId = 1,
                        Symbol = "ACME",
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)10.00,
                        CostBasisPerShare = (decimal)100.00,
                        ExecutedAtUtc = new DateTime(2024, 01, 15, 10, 00, 00, DateTimeKind.Utc)
                    },
                    // Same symbol (ACME) purchased again at a different price
                    new Transaction {
                        Id = 2,
                        UserId = 1,
                        Symbol = "ACME",
                        Type = TransactionType.Sell,
                        NumberOfShares = (decimal)3.00,
                        CostBasisPerShare = (decimal)120.000000,
                        ExecutedAtUtc = new DateTime(2024, 12, 01, 15, 30, 00, DateTimeKind.Utc)
                    },
                    new Transaction {
                        Id = 3,
                        UserId = 1,
                        Symbol = "ACME",
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)5.000000,
                        CostBasisPerShare = (decimal)110.000000,
                        ExecutedAtUtc = new DateTime(2025, 03, 10, 09, 00, 00, DateTimeKind.Utc)
                    },

                    new Transaction {
                        Id = 4,
                        UserId = 1,
                        Symbol = "BETA",
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)8.000000,
                        CostBasisPerShare = (decimal)40.000000,
                        ExecutedAtUtc = new DateTime(2025, 05, 20, 14, 30, 00, DateTimeKind.Utc)
                    },

                    // --- Bob (UserId = 2) ---
                    new Transaction {
                        Id = 5,
                        UserId = 2,
                        Symbol = "BETA",
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)20.000000,
                        CostBasisPerShare = (decimal)45.000000,
                        ExecutedAtUtc = new DateTime(2025, 02, 20, 14, 00, 00, DateTimeKind.Utc)
                    },
                    // Same symbol (BETA) purchased again at a different price
                    new Transaction {
                        Id = 6,
                        UserId = 2,
                        Symbol = "BETA",
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)10.000000,
                        CostBasisPerShare = (decimal)50.000000,
                        ExecutedAtUtc = new DateTime(2025, 06, 01, 16, 45, 00, DateTimeKind.Utc)
                    },
                    // Partial sell to make the position interesting
                    new Transaction {
                        Id = 7,
                        UserId = 2,
                        Symbol = "BETA",
                        Type = TransactionType.Sell,
                        NumberOfShares = (decimal)5.000000,
                        CostBasisPerShare = (decimal)55.000000,
                        ExecutedAtUtc = new DateTime(2025, 08, 15, 13, 15, 00, DateTimeKind.Utc)
                    }
                );
            });
        }
    }
}
