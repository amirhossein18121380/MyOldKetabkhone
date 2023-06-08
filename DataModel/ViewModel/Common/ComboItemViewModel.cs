namespace DataModel.ViewModel.Common
{
    public class ComboItemViewModel
    {
        public ComboItemViewModel(string id, string name, string message = null)
        {
            int.TryParse(id, out var idValue);
            Id = idValue;
            Name = name;

            if (message != null)
                Message = message;
        }

        public ComboItemViewModel(int id, string name, string message = null)
        {
            Id = id;
            Name = name;

            if (message != null)
                Message = message;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
