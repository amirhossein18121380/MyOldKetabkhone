namespace DataModel.ViewModel.Transaction
{
    public class ChargeWalletRequestViewModel
    {
        public long UserId { get; set; }
        public long Amount { get; set; }
        public short PaymentMethod  { get; set; }
        public string? BankType { get; set; }
        
    }
}
