using System;
using RetailSuccess.Sovos.Client;
using RetailSuccess.Sovos.Client.Abort;
using RetailSuccess.Sovos.Client.Commit;
using RetailSuccess.Tax.Abort;
using RetailSuccess.Tax.Commit;
using RetailSuccess.Tax.Models;


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
                TransactionId = commitRequest.OrderId,
                TransactionSource = commitRequest.TransactionSource
            };
        }
        public static CommitRequest CreateCommitRequestWithSovosId(TaxCommitRequestWithTaxSystemId commitRequest, SovosTaxClientOptions userInfo)
        {
            return new CommitRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                SovosTransactionDocumentId = commitRequest.PriorTransactionId
            };
        }
        public static AbortRequest CreateAbortRequest(TaxAbortRequest abortRequest, SovosTaxClientOptions userInfo)
        {
            return new AbortRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                TransactionId = abortRequest.OrderId,
                TransactionSource = abortRequest.TransactionSource,
            };
        }
        public static AbortRequest CreateAbortRequestWithSovosId(TaxAbortRequestWithTaxSystemId abortRequest, SovosTaxClientOptions userInfo)
        {
            return new AbortRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                SovosTransactionDocumentId = abortRequest.PriorTransactionId
            };
        }

        public static TaxCommitResponse CreateCommitResponse(CommitResponse sovosResponse, DateTime? commitDate = null)
        {
            return new TaxCommitResponse()
            {
                TransactionMessage = sovosResponse.TransactionStateDescription,
                TransactionId = sovosResponse.SovosTransactionDocumentId,
                TransactionSource = sovosResponse.TransactionSource,
                TransactionCode = sovosResponse.TransactionStateCode.ToString(),
                OrderId = sovosResponse.TransactionId,
                CommitDate = commitDate
            };
        }
        public static TaxAbortResponse CreateAbortResponse(AbortResponse sovosResponse)
        {
            return new TaxAbortResponse()
            {
                TransactionMessage = sovosResponse.TransactionStateDescription,
                TransactionId = sovosResponse.SovosTransactionDocumentId,
                TransactionSource = sovosResponse.TransactionSource,
                TransactionCode = sovosResponse.TransactionStateCode.ToString(),
                OrderId = sovosResponse.TransactionId
            };
        }

        public static TaxResponseAbstract CreateErrorResponse(this TaxResponseAbstract response, string message)
        {
            response.ErrorMessage = message;
            return response;
        }
    }
}
