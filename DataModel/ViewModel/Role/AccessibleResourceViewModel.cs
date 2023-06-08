namespace DataModel.ViewModel.Role
{
    public class AccessibleResourceViewModel
    {
        public long ResourceId { get; set; }
        public string? ResourceName { get; set; }
        public string? ResourceKey { get; set; }
        public bool IsAccess { get; set; }
    }
}