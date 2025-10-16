using System.Collections.Generic;

namespace InvestmentPerformanceWebAPI.Server.Database.Models 
{
    public class User 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
