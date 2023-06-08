namespace DataModel.ViewModel.Transaction
{
    public class GetWithdrawalListViewModel
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public long UserId { get; set; }
        public short StatusType { get; set; }
        public decimal Amount { get; set; }
        public short AccountType { get; set; }
        public string? AccountValue { get; set; }
        public int? BankId { get; set; }
        public string? FullName { get; set; }
        public long? ModifierId { get; set; }
        public DateTime? ModifyDateTime { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateOn { get; set; }
        public string? UserName { get; set; }
        public string? ModifierUserName { get; set; }
    }
}
