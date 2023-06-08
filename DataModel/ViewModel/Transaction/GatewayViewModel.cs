namespace DataModel.ViewModel.Transaction
{
    public class GatewayViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public long ImageId { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string? CurrencyTitle { get; set; }
    }
}
