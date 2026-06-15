using System.ComponentModel.DataAnnotations;

namespace SiteManagement.Web.ViewModels;

public class FaultCreateVm
{
    [Required]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = default!;

    [Required]
    [Display(Name = "Açıklama")]
    public string Description { get; set; } = default!;
}