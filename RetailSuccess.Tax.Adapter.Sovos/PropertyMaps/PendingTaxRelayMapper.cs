using RetailSuccess.Sovos.Client;
using RetailSuccess.Sovos.Client.Abort;
using RetailSuccess.Sovos.Client.Commit;


namespace RetailSuccess.Tax.Adapter.Sovos.PropertyMaps
{
    public static class PendingTaxRelayMapper
    {
        public static CommitRequest CreateCommitRequest(TaxCommitRequest commitRequest, SovosTaxClientOptions userInfo)
        {
            return new CommitRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                TransactionId = commitRequest.TransactionId,
                TransactionSource = commitRequest.TransactionSource
            };
        }
        public static CommitRequest CreateCommitRequestWithSovosId(TaxCommitRequestWithSovosId commitRequest, SovosTaxClientOptions userInfo)
        {
            return new CommitRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                SovosTransactionDocumentId = commitRequest.SovosTransactionId
            };
        }
        public static AbortRequest CreateAbortRequest(TaxAbortRequest abortRequest, SovosTaxClientOptions userInfo)
        {
            return new AbortRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                TransactionId = abortRequest.TransactionId,
                TransactionSource = abortRequest.TransactionSource,
            };
        }
        public static AbortRequest CreateAbortRequestWithSovosId(TaxAbortRequestWithSovosId abortRequest, SovosTaxClientOptions userInfo)
        {
            return new AbortRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                SovosTransactionDocumentId = abortRequest.SovosTransactionId
            };
        }
    }
}
