namespace SiteManagement.Domain.Entities;

public class FaultReport
{
    public int Id { get; private set; }

    public int BuildingId { get; private set; }

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    public string CreatedByUserId { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    public bool IsResolved { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedByUserId { get; private set; }

    private FaultReport() { }

    public FaultReport(int buildingId, string title, string description, string createdByUserId)
    {
        BuildingId = buildingId;
        Title = title.Trim();
        Description = description.Trim();
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        IsResolved = false;
    }

    public void MarkResolved(string resolvedByUserId)
    {
        IsResolved = true;
        ResolvedAt = DateTime.UtcNow;
        ResolvedByUserId = resolvedByUserId;
    }
}