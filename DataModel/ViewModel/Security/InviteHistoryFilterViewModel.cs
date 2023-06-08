namespace PokerNet.DataModel.ViewModel.Security
{
    public class InviteHistoryFilterViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public long ParentUserId { get; set; }
        public long? UserId { get; set; }
        public DateTime? FromRegisterDate { get; set; }
        public DateTime? ToRegisterDate { get; set; }
        public bool? IsGetGift { get; set; }
        public bool? EmailVerified { get; set; }
    }
}
