namespace DataModel.Models.Transaction;

public class CurrencyRate
{
    public int Id { get; set; }
    public int SourceCurrencyId { get; set; }
    public int DestinationCurrencyId { get; set; }
    public decimal RateValue { get; set; }
    public bool IsActive { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreateOn { get; set; }
}

