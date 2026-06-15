namespace SiteManagement.Web.ViewModels;

public class DashboardVm
{
    public string BuildingName { get; set; } = default!;
    public string JoinCode { get; set; } = default!;

    public int UnresolvedFaultCount { get; set; }
    public List<FaultItem> LatestFaults { get; set; } = new();

    // Bildirim alanları
    public List<AnnouncementItem> LatestAnnouncements { get; set; } = new();
    public List<UnpaidFeeItem> UnpaidFees { get; set; } = new();

    public int TotalAnnouncementsCount { get; set; }
    public int TotalFeesCount { get; set; }

    public class AnnouncementItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

    public class UnpaidFeeItem
    {
        public int FeeId { get; set; }
        public string Title { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
    public class FaultItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}