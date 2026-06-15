namespace SiteManagement.Domain.Entities;

public class FeePayment
{
    public int Id { get; private set; }

    public int FeeId { get; private set; }
    public string UserId { get; private set; } = default!;

    public bool IsPaid { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private FeePayment() { }

    public FeePayment(int feeId, string userId)
    {
        FeeId = feeId;
        UserId = userId;
        IsPaid = false;
    }

    public void MarkPaid()
    {
        IsPaid = true;
        PaidAt = DateTime.UtcNow;
    }

    public void MarkUnpaid()
    {
        IsPaid = false;
        PaidAt = null;
    }
}