namespace SiteManagement.Domain.Entities;

public class NormalFee : Fee
{
    private NormalFee() { } // EF

    public NormalFee(int buildingId, string title, decimal baseAmount, DateTime dueDate, string createdByUserId)
        : base(buildingId, title, baseAmount, dueDate, createdByUserId)
    {
    }

    public override decimal CalculateAmount(DateTime? asOf = null)
        => BaseAmount;
}