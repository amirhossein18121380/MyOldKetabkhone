namespace DataModel.Models.Transaction;

public class Gateway
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public short PaymentMethod { get; set; }
    public string? MerchantId { get; set; }
    public long? TerminalNumber { get; set; }
    public long ImageId { get; set; }
    public string? ImageName { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int CurrencyRateId { get; set; }
    public bool IsActive { get; set; }
    public string? Comment { get; set; }
    public DateTime CreateOn { get; set; }
}

