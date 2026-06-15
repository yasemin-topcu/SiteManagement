namespace SiteManagement.Domain.Entities;

public class Announcement
{
    public int Id { get; private set; }
    public int BuildingId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public string CreatedByUserId { get; private set; } = default!;

    private Announcement() { }

    public Announcement(int buildingId, string title, string content, string createdByUserId)
    {
        BuildingId = buildingId;
        Title = title.Trim();
        Content = content.Trim();
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string content)
    {
        Title = title.Trim();
        Content = content.Trim();
    }
}