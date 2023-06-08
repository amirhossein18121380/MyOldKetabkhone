
namespace DataModel.Models;

public class Like
{
    public long Id { get; set; }
    public long UserId { get; set; }
    //show a special group i.e Users, Translator, Authors
    public short EntityType { get; set; }
    //show a specific person you vote or review
    public long EntityId { get; set; }
    public DateTime CreateOn { get; set; }
    //show a specific action i.e like, dislike, celebrate, support, love, insightful, curiuos
    public short Type { get; set; }
}