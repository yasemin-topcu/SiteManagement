using System.ComponentModel.DataAnnotations;

namespace SiteManagement.Web.ViewModels;

public class PayFeeVm
{
    [Required]
    public int FeeId { get; set; }

    [Required, Display(Name="Kart Üzerindeki İsim")]
    public string CardHolderName { get; set; } = default!;

    [Required, Display(Name="Kart Numarası")]
    public string CardNumber { get; set; } = default!;

    [Required, Display(Name="Son Kullanma (AA/YY)")]
    public string Expiry { get; set; } = default!;

    [Required, Display(Name="CVV")]
    public string Cvv { get; set; } = default!;
}