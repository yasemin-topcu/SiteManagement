using System.ComponentModel.DataAnnotations;

namespace SiteManagement.Web.ViewModels;

public class AnnouncementCreateVm
{
    [Required]
    public string Title { get; set; } = default!;

    [Required]
    public string Content { get; set; } = default!;
}