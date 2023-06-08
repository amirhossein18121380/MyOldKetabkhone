namespace DataModel.ViewModel.Transaction
{
    public class GetDepositResultViewModel
    {
        public long TransactionId { get; set; }
        public short OperationType { get; set; }
        public int StatusType { get; set; }
        public decimal Amount { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public long TrCode { get; set; }
        public short? PaymentMethod { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string? Comment { get; set; }
        public long? CreatorId { get; set; }
        public string? CreatorUserName { get; set; }
        public DateTime CreateOn { get; set; }
    }
}
