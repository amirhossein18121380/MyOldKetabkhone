namespace DataModel.ViewModel.Common
{
    public class LogResponseViewModel
    {
        public short Level { get; set; }
        public string? LevelTitle { get; set; }
        public string? MethodName { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
