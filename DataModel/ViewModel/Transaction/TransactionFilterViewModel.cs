namespace DataModel.ViewModel.Transaction
{
    public class TransactionFilterViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public short OperationType { get; set; }
        public int StatusType { get; set; }
        public decimal Amount { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public short? PaymentMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
