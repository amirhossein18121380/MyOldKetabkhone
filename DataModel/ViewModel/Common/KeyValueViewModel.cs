namespace DataModel.ViewModel.Common
{
    public class KeyValueViewModel
    {
        public KeyValueViewModel(long key, string value)
        {
            Key = key;
            Value = value;
        }

        public long Key { get; set; }
        public string Value { get; set; }
    }
}
