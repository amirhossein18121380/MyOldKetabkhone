
namespace DataModel.Models.Transaction;

public class Wallet
{
    public long Id { get; set; }
    public short EntityType { get; set; }
    public long EntityId { get; set; }
    public short WalletType { get; set; }
    public decimal LastBalance { get; set; }
    public DateTime CreateOn { get; set; }
}