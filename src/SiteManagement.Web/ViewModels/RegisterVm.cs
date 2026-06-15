using System.ComponentModel.DataAnnotations;

namespace SiteManagement.Web.ViewModels;

public class RegisterVm
{
    [Required, Display(Name="Ad Soyad")]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress, Display(Name="E-posta")]
    public string Email { get; set; } = default!;

    [Required, Phone, Display(Name="Telefon")]
    public string PhoneNumber { get; set; } = default!;

    [Required, Display(Name="Kullanıcı Tipi")]
    public string UserType { get; set; } = "Resident"; // Manager | Resident

    // Resident ise apartmana katılmak için kod isteyebilirsin
    [Display(Name="Apartman Katılım Kodu (Daire sakini için)")]
    public string? JoinCode { get; set; }

    [Required, DataType(DataType.Password), Display(Name="Şifre")]
    public string Password { get; set; } = default!;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name="Şifre (Tekrar)")]
    public string ConfirmPassword { get; set; } = default!;
}