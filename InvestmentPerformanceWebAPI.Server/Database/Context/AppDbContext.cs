using InvestmentPerformanceWebAPI.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPerformanceWebAPI.Server.Database.Context {
    public sealed class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }
        public DbSet<Investment> Investments {
            get { return Set<Investment>(); }
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

            // Investments seed (so InvestmentId in Transactions is valid)
            modelBuilder.Entity<Investment>(b =>
            {
                b.HasData(
                    // Alice's investments
                    new Investment {
                        Id = 1,
                        UserId = 1,
                        Name = "ACME"
                    },
                    new Investment {
                        Id = 2,
                        UserId = 1,
                        Name = "BETA"
                    },

                    // Bob's investments
                    new Investment {
                        Id = 3,
                        UserId = 2,
                        Name = "BETA"
                    }
                );
            });
            
            modelBuilder.Entity<Transaction>(b =>
            {
                b.HasData(
                    // --- Alice (UserId = 1) ---
                    new Transaction {
                        Id = 1,
                        UserId = 1,
                        InvestmentId = 1,                           // ACME (Alice)
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)10.00,
                        PricePerShare = (decimal)100.00,
                        ExecutedAtUtc = new DateTime(2024, 01, 15, 10, 00, 00, DateTimeKind.Utc)
                    },
                    // Same investment bought/sold at different prices
                    new Transaction {
                        Id = 2,
                        UserId = 1,
                        InvestmentId = 1,                           // ACME (Alice)
                        Type = TransactionType.Sell,
                        NumberOfShares = (decimal)3.00,
                        PricePerShare = (decimal)120.00,
                        ExecutedAtUtc = new DateTime(2024, 12, 01, 15, 30, 00, DateTimeKind.Utc)
                    },
                    new Transaction {
                        Id = 3,
                        UserId = 1,
                        InvestmentId = 1,                           // ACME (Alice)
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)5.00,
                        PricePerShare = (decimal)110.00,
                        ExecutedAtUtc = new DateTime(2025, 03, 10, 09, 00, 00, DateTimeKind.Utc)
                    },

                    new Transaction {
                        Id = 4,
                        UserId = 1,
                        InvestmentId = 2,                           // BETA (Alice)
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)8.00,
                        PricePerShare = (decimal)40.00,
                        ExecutedAtUtc = new DateTime(2025, 05, 20, 14, 30, 00, DateTimeKind.Utc)
                    },

                    // --- Bob (UserId = 2) ---
                    new Transaction {
                        Id = 5,
                        UserId = 2,
                        InvestmentId = 3,                           // BETA (Bob)
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)20.00,
                        PricePerShare = (decimal)45.00,
                        ExecutedAtUtc = new DateTime(2025, 02, 20, 14, 00, 00, DateTimeKind.Utc)
                    },
                    // Same investment again at different price
                    new Transaction {
                        Id = 6,
                        UserId = 2,
                        InvestmentId = 3,                           // BETA (Bob)
                        Type = TransactionType.Buy,
                        NumberOfShares = (decimal)10.00,
                        PricePerShare = (decimal)50.00,
                        ExecutedAtUtc = new DateTime(2025, 06, 01, 16, 45, 00, DateTimeKind.Utc)
                    },
                    // Partial sell
                    new Transaction {
                        Id = 7,
                        UserId = 2,
                        InvestmentId = 3,                           // BETA (Bob)
                        Type = TransactionType.Sell,
                        NumberOfShares = (decimal)5.00,
                        PricePerShare = (decimal)55.00,
                        ExecutedAtUtc = new DateTime(2025, 08, 15, 13, 15, 00, DateTimeKind.Utc)
                    }
                );
            });

        }
    }
}
