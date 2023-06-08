namespace DataModel.Models.Transaction;

public class Withdrawal
{
    public long Id { get; set; }
    public long TransactionId { get; set; }
    public long UserId { get; set; }
    public short StatusType { get; set; }
    public decimal Amount { get; set; }
    public short AccountType { get; set; }
    public string? AccountValue { get; set; }
    public int? BankId { get; set; }
    public string? FullName { get; set; }
    public long? ModifierId { get; set; }
    public DateTime? ModifyDateTime { get; set; }
    public decimal ExchangeAmount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? VoucherNumber { get; set; }
    public string? VoucherCode { get; set; }
    public string? Comment { get; set; }
    public DateTime CreateOn { get; set; }
}

