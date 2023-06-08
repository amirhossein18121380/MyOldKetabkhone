namespace DataModel.ViewModel.Transaction;
public class GatewayCurrencyViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public short PaymentMethod { get; set; }
    public string MerchantId { get; set; } = null!;
    public long? TerminalNumber { get; set; }
    public long ImageId { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public float RateValue { get; set; }
    public bool IsActive { get; set; }
    public string? SourceCurrencyTitle { get; set; }
    public string? DestinationCurrencyTitle { get; set; }

}

