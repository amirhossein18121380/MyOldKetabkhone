namespace DataModel.ViewModel.Transaction
{
    public class GetTransactionReportFilterViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
