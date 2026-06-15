namespace SiteManagement.Domain.Entities;

public class Building
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string JoinCode { get; private set; } = default!;

    private Building() { } // EF için

    public Building(string name, string joinCode)
    {
        Name = name.Trim();
        JoinCode = joinCode.Trim();
    }
}