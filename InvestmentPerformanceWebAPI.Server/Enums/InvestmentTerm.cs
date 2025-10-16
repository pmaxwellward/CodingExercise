using System.ComponentModel.DataAnnotations;

namespace InvestmentPerformanceWebAPI.Server.Enums {
    public enum InvestmentTerm {
        [Display(Name = "Short Term")]
        ShortTerm = 0,

        [Display(Name = "Long Term")]
        LongTerm = 1
    }
}
