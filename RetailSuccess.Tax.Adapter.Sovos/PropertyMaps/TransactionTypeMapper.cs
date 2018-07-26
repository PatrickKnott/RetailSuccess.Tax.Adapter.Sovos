using RetailSuccess.Sovos.Client.Enums;

namespace RetailSuccess.Tax.Adapter.Sovos.PropertyMaps
{
    public static class TransactionTypeMapper
    {
        /// <summary>
        /// this was tax class in squire.  1 for sales, 2 for purchase.
        /// </summary>
        /// <param name="transactionInt"></param>
        /// <returns></returns>
        public static TransactionType GetTypeFromInt(int? transactionInt)
        {
            var transactionType = TransactionType.Sale;
            if (transactionInt.HasValue)
            {
                transactionType = (TransactionType)transactionInt;
            }
            return transactionType;
        }
    }
}
