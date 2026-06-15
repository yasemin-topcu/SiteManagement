namespace SiteManagement.Domain.Entities;

public class LateFee : Fee
{
    public decimal PenaltyRate { get; private set; } // örn 0.10 = %10

    private LateFee() { } // EF

    public LateFee(int buildingId, string title, decimal baseAmount, DateTime dueDate, string createdByUserId, decimal penaltyRate)
        : base(buildingId, title, baseAmount, dueDate, createdByUserId)
    {
        PenaltyRate = penaltyRate;
    }

    public override decimal CalculateAmount(DateTime? asOf = null)
    {
        var now = asOf ?? DateTime.UtcNow;

        if (now.Date <= DueDate.Date)
            return BaseAmount;

        return BaseAmount + (BaseAmount * PenaltyRate);
    }
}