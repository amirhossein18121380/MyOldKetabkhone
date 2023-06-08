namespace DataModel.ViewModel.Transaction
{
    public class IrGatePaymentResultViewModel
    {
        public int code { get; set; }

        public string? msg { get; set; }

        public string? transaction_id { get; set; }

        public string? order_id { get; set; }

        public string? hash { get; set; }

        public int payment_method { get; set; }

        public string? payment_curreny { get; set; }

        public long amount { get; set; }

        public string? return_currency { get; set; }

        public long return_amount { get; set; }
    }
}
