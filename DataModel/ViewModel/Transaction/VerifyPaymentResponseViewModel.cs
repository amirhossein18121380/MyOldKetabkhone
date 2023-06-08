namespace DataModel.ViewModel.Transaction
{
    public class VerifyPaymentResponseViewModel
    {
        public short Status { get; set; } //0 => Fail, 1 => Success, 2 => Pending
        public string? PaymentMessage { get; set; }
        public long TrCode { get; set; }
        public long Amount { get; set; }
        public long LastBalance { get; set; }
        public DateTime PaymentDateTime { get; set; }
    }
}
