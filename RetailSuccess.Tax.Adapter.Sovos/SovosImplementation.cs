using RetailSuccess.Sovos.Client;
using RetailSuccess.Sovos.Client.Enums;
using RetailSuccess.Tax.Abort;
using RetailSuccess.Tax.Adapter.Sovos.PropertyMaps;
using RetailSuccess.Tax.Adapter.Sovos.SetUp;
using RetailSuccess.Tax.Commit;
using RetailSuccess.Tax.Refund;
using RetailSuccess.Tax.Update;
using System.Threading.Tasks;

namespace RetailSuccess.Tax.Adapter.Sovos
{
    public class SovosImplementation : ITaxHandler
    {
        private SovosTaxClientOptions _sovosSettings;
        private ClientFactory _factory;
        private SovosTaxClient _client;

        public SovosImplementation(SovosTaxClientOptions sovosSettings)
        {
            _sovosSettings = sovosSettings;
            _factory = new ClientFactory(_sovosSettings);
            _client = _factory.CreateClient();
        }


        /// <summary>
        /// Do not provide the transactionId, source, or invoice number.
        /// </summary>
        /// <param name="taxInformation"></param>
        /// <returns></returns>
        public async Task<TaxResponse> GetTaxQuote(TaxRequest taxInformation)
        {
            var taxRequest = TaxRelayMapper.CreateSovosTaxQuote(taxInformation, _sovosSettings);
            try
            {
                var response =
                    await _client.GetTaxDeterminationAsync(taxRequest);
                var taxResponse = TaxRelayMapper.CreateTaxResponseFromSovos(response, taxInformation.SaleDate);
                return taxResponse;
            }
            catch (Refit.ApiException ex)
            {
                var response = new TaxResponse();
                return (TaxResponse)response.CreateErrorResponse(ex.Content);
            }
        }
        /// <summary>
        /// Ensure the transactionId and source are provided. This may include the invoice number.
        /// </summary>
        /// <param name="taxInformation"></param>
        /// <returns></returns>
        public async Task<TaxResponse> GetPendingTaxCalculation(TaxRequest taxInformation)
        {
            var taxRequest = TaxRelayMapper.CreateSovosTaxRequest(taxInformation, _sovosSettings);
            try { 
                var response = 
                    await _client.GetTaxDeterminationAsync(taxRequest);
                var taxResponse = TaxRelayMapper.CreateTaxResponseFromSovos(response, taxInformation.SaleDate);
                return taxResponse;
            }
            catch (Refit.ApiException ex)
            {
                var response = new TaxResponse();
                return (TaxResponse) response.CreateErrorResponse(ex.Content);
            }
}
        /// <summary>
        /// Commits a pending tax request for audit.  
        /// </summary>
        public async Task<TaxCommitResponse> CommitPendingTaxCalculation(TaxCommitRequest commitRequest)
        {
            var sovosCommitRequest = PendingTaxRelayMapper.CreateCommitRequest(commitRequest, _sovosSettings);
            try
            {
                var sovosCommitResponse =
                    await _client.CommitWithResponsePendingGTDAuditTransactionAsync(sovosCommitRequest);
                var commitResponse = PendingTaxRelayMapper.CreateCommitResponse(sovosCommitResponse, commitRequest.CommitDate);
                return commitResponse;
            }
            catch (Refit.ApiException ex)
            {
                var commitResponse = new TaxCommitResponse();
                return (TaxCommitResponse)commitResponse.CreateErrorResponse(ex.Content);
            }
        }

        /// <summary>
        /// Uses SovosId; Commits a pending tax request for audit.  
        /// </summary>
        public async Task<TaxCommitResponse> CommitPendingTaxCalculationUsingSovosId(TaxCommitRequestWithTaxSystemId commitRequest)
        {
            var sovosCommitRequest = PendingTaxRelayMapper.CreateCommitRequestWithSovosId(commitRequest, _sovosSettings);
            try
            {
                var sovosCommitResponse =
                    await _client.CommitWithResponsePendingGTDAuditTransactionAsync(sovosCommitRequest);
                var commitResponse = PendingTaxRelayMapper.CreateCommitResponse(sovosCommitResponse);
                return commitResponse;
            }
            catch (Refit.ApiException ex)
            {
                var commitResponse = new TaxCommitResponse();
                return (TaxCommitResponse)commitResponse.CreateErrorResponse(ex.Content);
            }
        }

        /// <summary>
        /// Aborts a pending tax request, purging it.
        /// </summary>
        public async Task<TaxAbortResponse> AbortPendingTaxCalculation(TaxAbortRequest abortRequest)
        {
            var sovosAbortRequest = PendingTaxRelayMapper.CreateAbortRequest(abortRequest, _sovosSettings);
            try
            {
                var sovosAbortResponse =
                    await _client.AbortWithResponsePendingGTDAuditTransactionAsync(sovosAbortRequest);
                var abortResponse = PendingTaxRelayMapper.CreateAbortResponse(sovosAbortResponse);
                return abortResponse;
            }
            catch (Refit.ApiException ex)
            {
                var abortResponse = new TaxAbortResponse();
                return (TaxAbortResponse) abortResponse.CreateErrorResponse(ex.Content);
            }
        }

        /// <summary>
        /// Uses SovosId; Aborts a pending tax request, purging it.
        /// </summary>
        public async Task<TaxAbortResponse> AbortPendingTaxCalculationUsingSovosId(TaxAbortRequestWithTaxSystemId abortRequest)
        {
            var sovosAbortRequest = PendingTaxRelayMapper.CreateAbortRequestWithSovosId(abortRequest, _sovosSettings);
            try
            { 
                var sovosAbortResponse = 
                    await _client.AbortWithResponsePendingGTDAuditTransactionAsync(sovosAbortRequest);
                var abortResponse = PendingTaxRelayMapper.CreateAbortResponse(sovosAbortResponse);
                return abortResponse;
            }
            catch (Refit.ApiException ex)
            {
                var abortResponse = new TaxAbortResponse();
                return (TaxAbortResponse)abortResponse.CreateErrorResponse(ex.Content);
            }
        }

        /// <summary>
        /// Updates a pending tax request; does not commit/abort it.
        /// Requires TransactionId, Transaction Source AND Invoice Number.
        /// Aborts the existing request by TransactionId, then submits a new request with the provided information, providing
        /// a new transactionId.
        /// </summary>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        public async Task<TaxResponse> UpdatePendingTaxCalculation(TaxUpdatePendingTaxRequest updateRequest)
        {
            var taxRequest = TaxRelayMapper.CreateSovosTaxRequest(updateRequest, _sovosSettings);
            taxRequest.TransactionSource = "OVER_" + taxRequest.TransactionSource;
            try
            {
                var response = 
                    await _client.GetTaxDeterminationAsync(taxRequest);
                var taxResponse = TaxRelayMapper.CreateTaxResponseFromSovos(response, updateRequest.SaleDate);
                return taxResponse;
            }
            catch (Refit.ApiException ex)
            {
                var response = new TaxResponse();
                return (TaxResponse)response.CreateErrorResponse(ex.Content);
            }
        }

        public async Task<TaxRefundResponse> RefundRequest(TaxRefundRequest refundRequest)
        {
            var taxRequest = TaxRelayMapper.CreateSovosTaxRequest(refundRequest, _sovosSettings);
            if (refundRequest.IsRefund)
            {
                foreach (var line in taxRequest.LineItems)
                {
                    line.DebitCreditIndicator = DebitCreditIndicator.Credit;
                }
            }

            try
            {
                var response =
                    await _client.GetTaxDeterminationAsync(taxRequest);
                var taxResponse = TaxRelayMapper.CreateTaxRefundResponseFromSovos(response, refundRequest.SaleDate);
                return taxResponse;
            }
            catch (Refit.ApiException ex)
            {
                var response = new TaxResponse();
                return (TaxRefundResponse)response.CreateErrorResponse(ex.Content);
            }
        }
    }
}
