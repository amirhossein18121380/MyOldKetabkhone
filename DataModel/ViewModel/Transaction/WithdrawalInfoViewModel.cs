

using DataModel.ViewModel.Common;

namespace DataModel.ViewModel.Transaction
{
    public class WithdrawalInfoViewModel
    {
        public long MinAmount { get; set; }
        public long MaxAmount { get; set; }
        public bool CanWithdrawal { get; set; }
        public string? Message { get; set; }
        public List<ComboItemViewModel>? AccountTypes { get; set; }
        public List<ComboItemViewModel>? Banks { get; set; }
    }
}
