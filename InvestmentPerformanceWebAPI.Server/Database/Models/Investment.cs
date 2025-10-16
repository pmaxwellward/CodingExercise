using System.ComponentModel.DataAnnotations;

namespace InvestmentPerformanceWebAPI.Server.Database.Models {
    public sealed class Investment {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;
    }
}
