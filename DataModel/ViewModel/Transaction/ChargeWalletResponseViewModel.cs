namespace DataModel.ViewModel.Transaction
{
    public class ChargeWalletResponseViewModel
    {
        public long Amount { get; set; }
        public short PaymentMethod { get; set; }
        public string? PaymentMethodName { get; set; }
        public long UserId { get; set; }
        public DateTime? VerifyDate { get; set; }
        public short StatusType { get; set; }
        public string? IbanNumber { get; set; }
    }
}
