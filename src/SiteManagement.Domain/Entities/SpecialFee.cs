namespace SiteManagement.Domain.Entities;

public class SpecialFee : Fee
{
    public string? Note { get; private set; } // örn: "Asansör bakımı"

    private SpecialFee() { } // EF

    public SpecialFee(int buildingId, string title, decimal baseAmount, DateTime dueDate, string createdByUserId, string? note)
        : base(buildingId, title, baseAmount, dueDate, createdByUserId)
    {
        Note = note;
    }

    public override decimal CalculateAmount(DateTime? asOf = null)
        => BaseAmount;
}