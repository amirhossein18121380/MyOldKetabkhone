namespace DataModel.ViewModel.Transaction
{
    public class UserTransactionReportViewModel
    {
        public long Id { get; set; }
        public short OperationType { get; set; }
        public int StatusType { get; set; }
        public decimal Amount { get; set; }
        public long WalletId { get; set; }
        public long UserId { get; set; }
        public long TrCode { get; set; }
        public string? RefCode { get; set; }
        public long? ReserveNumber { get; set; }
        public int? GatewayId { get; set; }
        public short? PaymentMethod { get; set; }
        public string? Token { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string? Comment { get; set; }
        public long? CreatorId { get; set; }
        public DateTime CreateOn { get; set; }
        public int? WithdrawalStatus { get; set; }
    }
}
