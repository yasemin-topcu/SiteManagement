using System.ComponentModel.DataAnnotations;

namespace SiteManagement.Web.ViewModels;

public class FeeCreateVm
{
    [Required]
    public string Title { get; set; } = default!;

    [Range(0.01, 1000000)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

    // ✅ yeni
    [Required]
    public string FeeType { get; set; } = "Normal"; // Normal | Late | Special

    // Late için
    [Range(0, 1)]
    public decimal? PenaltyRate { get; set; } // 0.10 = %10

    // Special için
    public string? Note { get; set; }
}