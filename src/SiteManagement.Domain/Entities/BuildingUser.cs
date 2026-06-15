namespace SiteManagement.Domain.Entities;

public class BuildingUser
{
    public int Id { get; private set; }

    public int BuildingId { get; private set; }
    public string UserId { get; private set; } = default!; // Identity user Id (string)

    public string MemberType { get; private set; } = default!; // "Manager" | "Resident"

    private BuildingUser() { }

    public BuildingUser(int buildingId, string userId, string memberType)
    {
        BuildingId = buildingId;
        UserId = userId;
        MemberType = memberType;
    }
}