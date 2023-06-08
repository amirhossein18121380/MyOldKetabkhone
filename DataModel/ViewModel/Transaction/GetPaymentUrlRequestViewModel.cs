namespace DataModel.ViewModel.Transaction
{
    public class GetPaymentUrlRequestViewModel
    {
        public long Amount { get; set; }
        public int GatewayId { get; set; }
        public string DomainName { get; set; } = null!;
    }
}
