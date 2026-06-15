using System.ComponentModel.DataAnnotations.Schema;
namespace SiteManagement.Domain.Entities;

public abstract class Fee
{
    public int Id { get; protected set; }
    public int BuildingId { get; protected set; }

    public string Title { get; protected set; } = default!;
    public decimal BaseAmount { get; protected set; }
    public DateTime DueDate { get; protected set; }

    public DateTime CreatedAt { get; protected set; }
    public string CreatedByUserId { get; protected set; } = default!;

    protected Fee() { } // EF

    protected Fee(int buildingId, string title, decimal baseAmount, DateTime dueDate, string createdByUserId)
    {
        BuildingId = buildingId;
        Title = title.Trim();
        BaseAmount = baseAmount;
        DueDate = dueDate;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }

    // ✅ Polymorphism: her tür farklı hesaplayacak
    public abstract decimal CalculateAmount(DateTime? asOf = null);

    
}