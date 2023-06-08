namespace PokerNet.DataModel.ViewModel.Security
{
    public class InviteHistoryViewModel
    {
        public long Id { get; set; }
        public long ParentUserId { get; set; }
        public long UserId { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool IsGetGift { get; set; }
        public decimal? GiftAmount { get; set; }
        public DateTime? GiftDate { get; set; }
        public bool EmailVerified { get; set; }
        public bool HasSuccessDeposit { get; set; }
    }
}
