using System;
using System.Threading.Tasks;
using RetailSuccess.Sovos.Client;
using RetailSuccess.Tax.Adapter.Sovos.PropertyMaps;
using RetailSuccess.Tax.Adapter.Sovos.SetUp;

namespace RetailSuccess.Tax.Adapter.Sovos
{
    public class SovosImplementation : ITaxHandler
    {
        private SovosTaxClientOptions _sovosSettings;
        private ClientFactory _factory;
        private SovosTaxClient _client;

        public SovosImplementation()
        {
            _sovosSettings = ConfigurationReader.GetAppSettings();
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
            var response = await _client.GetTaxDeterminationAsync(taxRequest);
            var taxResponse = TaxRelayMapper.CreateTaxResponseFromSovos(response, taxInformation.SaleDate);
            return taxResponse;
        }
        /// <summary>
        /// Ensure the transactionId and source are provided. This may include the invoice number.
        /// </summary>
        /// <param name="taxInformation"></param>
        /// <returns></returns>
        public async Task<TaxResponse> GetPendingTaxCalculation(TaxRequest taxInformation)
        {
            var taxRequest = TaxRelayMapper.CreateSovosTaxRequest(taxInformation, _sovosSettings);
            var response = await _client.GetTaxDeterminationAsync(taxRequest);
            var taxResponse = TaxRelayMapper.CreateTaxResponseFromSovos(response, taxInformation.SaleDate);
            return taxResponse;
        }
        /// <summary>
        /// Commits a pending tax request for audit.  
        /// </summary>
        public async Task<string> CommitPendingTaxCalculation(TaxCommitRequest commitRequest)
        {
            var sovosCommitRequest = PendingTaxRelayMapper.CreateCommitRequest(commitRequest, _sovosSettings);
            var sovosCommitResponse = await _client.CommitWithResponsePendingGTDAuditTransactionAsync(sovosCommitRequest);
            return sovosCommitResponse.TransactionStateDescription;
        }

        /// <summary>
        /// Uses SovosId; Commits a pending tax request for audit.  
        /// </summary>
        public async Task<string> CommitPendingTaxCalculationUsingSovosId(TaxCommitRequestWithSovosId commitRequest)
        {
            var sovosCommitRequest = PendingTaxRelayMapper.CreateCommitRequestWithSovosId(commitRequest, _sovosSettings);
            var sovosCommitResponse = await _client.CommitWithResponsePendingGTDAuditTransactionAsync(sovosCommitRequest);
            return sovosCommitResponse.TransactionStateDescription;
        }

        /// <summary>
        /// Aborts a pending tax request, purging it.
        /// </summary>
        public async Task<string> AbortPendingTaxCalculation(TaxAbortRequest abortRequest)
        {
            var sovosAbortRequest = PendingTaxRelayMapper.CreateAbortRequest(abortRequest, _sovosSettings);
            var sovosAbortResponse = await _client.AbortWithResponsePendingGTDAuditTransactionAsync(sovosAbortRequest);
            return sovosAbortResponse.TransactionStateDescription;
        }

        /// <summary>
        /// Uses SovosId; Aborts a pending tax request, purging it.
        /// </summary>
        public async Task<string> AbortPendingTaxCalculationUsingSovosId(TaxAbortRequestWithSovosId abortRequest)
        {
            var sovosAbortRequest = PendingTaxRelayMapper.CreateAbortRequestWithSovosId(abortRequest, _sovosSettings);
            var sovosAbortResponse = await _client.AbortWithResponsePendingGTDAuditTransactionAsync(sovosAbortRequest);
            return sovosAbortResponse.TransactionStateDescription;
        }

        /// <summary>
        /// Updates a pending tax request; does not commit/abort it.
        /// Requires TransactionId and Transaction Source.
        /// Aborts the existing request by TransactionId, then submits a new request with the provided information.
        /// </summary>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        public async Task<TaxResponse> UpdatePendingTaxCalculation(TaxRequest updateRequest)
        {
            var taxRequest = TaxRelayMapper.CreateSovosTaxRequest(updateRequest, _sovosSettings, true);
            var response = await _client.GetTaxDeterminationAsync(taxRequest);
            var taxResponse = TaxRelayMapper.CreateTaxResponseFromSovos(response, updateRequest.SaleDate);
            return taxResponse;
        }

    }
}
