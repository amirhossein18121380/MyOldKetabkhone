using Common.Enum;
using Common.Extension;

namespace DataModel.ViewModel.Transaction
{
    public class TransactionInfoViewModel
    {
        public long Id { get; set; }
        public short OperationType { get; set; }
        public int StatusType { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentBalance { get; set; }
        public long WalletId { get; set; }
        public long UserId { get; set; }
        public long TrCode { get; set; }
        public short? PaymentMethod { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateOn { get; set; }

        public string OperationTitle
        {
            get
            {
                var opType = EnumExtension.EnumToList<TransactionOperationTypeEnum>();

                var result = opType.FirstOrDefault(c => c.IntValueMember == OperationType);

                return result?.StringValueMember ?? OperationType.ToString();
            }
        }

        public string StatusTitle
        {
            get
            {
                var stType = EnumExtension.EnumToList<TransactionStatusType>();

                var result = stType.FirstOrDefault(c => c.IntValueMember == StatusType);

                return result?.StringValueMember ?? StatusType.ToString();
            }
        }

        //public string PaymentMethodTitle
        //{
        //    get
        //    {
        //        if (!PaymentMethod.HasValue)
        //        {
        //            return string.Empty;
        //        }

        //        var pmType = EnumExtension.EnumToList<PaymentMethodEnum>();

        //        var result = pmType.FirstOrDefault(c => c.IntValueMember == PaymentMethod.Value);

        //        return result?.StringValueMember ?? PaymentMethod.Value.ToString();
        //    }
        //}
    }
}
