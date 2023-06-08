namespace DataModel.ViewModel.Transaction;
public class WithdrawalGetListFilterViewModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public long? UserId { get; set; }
    public string? UserName { get; set; }
    public short? StatusType { get; set; }
    public long? Amount { get; set; }
    public short? AccountType { get; set; }
    public string? AccountValue { get; set; }
    public DateTime? Date { get; set; }
}

