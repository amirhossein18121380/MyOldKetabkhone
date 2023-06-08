namespace DataModel.ViewModel.Transaction;
public class GetTransactionReportViewModel
{
    public long Id { get; set; }
    public short OperationType { get; set; }
    public string? OperationTitle { get; set; }
    public int StatusType { get; set; }
    public string? StatusTitle { get; set; }
    public decimal Amount { get; set; }
    public long TrCode { get; set; }
    public string? Comment { get; set; }
    public short DocType { get; set; }
    public DateTime TrDate { get; set; }
}

