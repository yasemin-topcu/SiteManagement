namespace SiteManagement.Domain.Entities;

public class ApartmentUnit
{
    public int Id { get; private set; }
    public int BuildingId { get; private set; }
    public string DoorNumber { get; private set; } = default!;

    private ApartmentUnit() { }

    public ApartmentUnit(int buildingId, string doorNumber)
    {
        BuildingId = buildingId;
        DoorNumber = doorNumber.Trim();
    }
}