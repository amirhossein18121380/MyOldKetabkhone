namespace DataModel.Models.Transaction;

public class Currency
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Symbol { get; set; }
    public string? Label { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreateOn { get; set; }
}

