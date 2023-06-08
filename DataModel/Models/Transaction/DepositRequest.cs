namespace DataModel.Models.Transaction;

public class DepositRequest
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public int DepositType { get; set; }
    public long TraceNumber { get; set; }
    public string? AccountIdentity { get; set; }
    public int StatusType { get; set; }
    public DateTime VerifyDate { get; set; }
    public DateTime CreateOn { get; set; }
}

