using System.ComponentModel.DataAnnotations;

namespace SiteManagement.Web.ViewModels;

public class CreateBuildingVm
{
    [Required, Display(Name="Apartman Adı")]
    public string Name { get; set; } = default!;
}