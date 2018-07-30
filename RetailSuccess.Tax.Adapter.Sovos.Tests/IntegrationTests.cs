using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace RetailSuccess.Tax.Adapter.Sovos.Tests
{
    public class IntegrationTests
    {
        private readonly ITaxHandler _taxHandler;
        private string _transactionId;
        public string _orderId;

        public IntegrationTests()
        {
            _taxHandler = TestTools.GetTaxHandler();
        }
        #region Get
        [Fact]
        public async Task CanGetTaxQuoteWhenGross100()
        {
            var taxRequest = TestTools.CreateTaxQuoteRequest();
            var response = await _taxHandler.GetTaxQuote(taxRequest);
            Assert.Null(response.ErrorMessage);
            Assert.NotEqual(0, response.InvoiceTotalTaxAmount);
        }
        [Fact]
        public async Task CanGetTaxPendingWhenGross100()
        {
            var taxRequest = TestTools.CreateTaxPendingRequest();
            _orderId = taxRequest.OrderNumber;
            var response = await _taxHandler.GetPendingTaxCalculation(taxRequest);
            Assert.Null(response.ErrorMessage);
            _transactionId = response.TransactionId;
            Assert.NotEqual(0, response.InvoiceTotalTaxAmount);
        }
        #endregion
        #region Abort
        [Fact]
        public async Task AbortPendingTaxIdUsingSovosId()
        {
            if (string.IsNullOrEmpty(_transactionId))
            {
                await CanGetTaxPendingWhenGross100();
                Assert.NotNull(_transactionId);
            }

            var abortRequest = TestTools.CreateAbortRequestWithTaxSystemId(_transactionId);
            var response = await _taxHandler.AbortPendingTaxCalculationUsingSovosId(abortRequest);
            Assert.Null(response.ErrorMessage);
            _transactionId = null;
            _orderId = null;
            Assert.Equal("Aborted Successfully", response.TransactionMessage);
        }
        [Fact]
        public async Task AbortPendingTaxIdUsingOrderId()
        {
            if (string.IsNullOrEmpty(_orderId))
            {
                await CanGetTaxPendingWhenGross100();
                Assert.NotNull(_orderId);
            }
            var abortRequest = TestTools.CreateAbortRequestWithOrderId(_orderId);
            var response = await _taxHandler.AbortPendingTaxCalculation(abortRequest);
            Assert.Null(response.ErrorMessage);
            _transactionId = null;
            _orderId = null;
            Assert.Equal("Aborted Successfully", response.TransactionMessage);
        }
        [Fact]
        public async Task AbortThrowsErrorWhenInvalidId()
        {
            var abortRequest = TestTools.CreateAbortRequestWithOrderId("FakeId123");
            var response = await _taxHandler.AbortPendingTaxCalculation(abortRequest);
            Assert.NotNull(response.ErrorMessage);
            Assert.Equal("{\n  \"errorCode\" : \"9968\",\n  \"errorMessage\" : \"Could not abort the Document. Abort failed.Transaction was not found.\"\n}", response.ErrorMessage);
        }
        #endregion
        #region Commit
        [Fact]
        public async Task CommitPendingTaxIdUsingSovosId()
        {
            if (string.IsNullOrEmpty(_transactionId))
            {
                await CanGetTaxPendingWhenGross100();
                Assert.NotNull(_transactionId);
            }

            var commitRequest = TestTools.CreateCommitRequestWithTaxSystemId(_transactionId);
            var response = await _taxHandler.CommitPendingTaxCalculationUsingSovosId(commitRequest);
            Assert.Null(response.ErrorMessage);
            _transactionId = null;
            _orderId = null;
            Assert.Equal("Committed Successfully", response.TransactionMessage);
        }
        [Fact]
        public async Task CommitPendingTaxIdUsingOrderId()
        {
            if (string.IsNullOrEmpty(_orderId))
            {
                await CanGetTaxPendingWhenGross100();
                Assert.NotNull(_orderId);
            }

            var commitRequest = TestTools.CreateCommitRequestWithOrderId(_orderId);
            var response = await _taxHandler.CommitPendingTaxCalculation(commitRequest);
            Assert.Null(response.ErrorMessage);
            _transactionId = null;
            _orderId = null;
            Assert.Equal("Committed Successfully", response.TransactionMessage);
        }
        [Fact]
        public async Task CommitThrowsErrorWhenInvalidId()
        {
            var commitRequest = TestTools.CreateCommitRequestWithOrderId("FakeId123c");
            var response = await _taxHandler.CommitPendingTaxCalculation(commitRequest);
            Assert.NotNull(response.ErrorMessage);
            Assert.Equal("{\n  \"errorCode\" : \"9968\",\n  \"errorMessage\" : \"Could not commit the Document. Commit failed. Transaction was not found.\"\n}", response.ErrorMessage);
        }
        #endregion
        #region Update
        /// <summary>
        /// TODO WARNING This is an issue that needs to be addressed by Sovos.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateKeepsOldSovosIdAndProvidesNewSovosId()
        {
            if (string.IsNullOrEmpty(_orderId))
            {
                await CanGetTaxPendingWhenGross100();
                Assert.NotNull(_orderId);
                Assert.NotNull(_transactionId);
            }

            var updateRequest = TestTools.CreateUpdateRequest(_orderId);
            var response = await _taxHandler.UpdatePendingTaxCalculation(updateRequest);
            Assert.Null(response.ErrorMessage);

            //shows the new transactionId is different.
            Assert.NotNull(response.TransactionId);
            Assert.NotEqual(_transactionId, response.TransactionId);

            //shows the previous ID is still valid.
            var abortRequest = TestTools.CreateAbortRequestWithTaxSystemId(_transactionId);
            var abortResponse = await _taxHandler.AbortPendingTaxCalculationUsingSovosId(abortRequest);
            Assert.Null(abortResponse.ErrorMessage);
            
            //shows the new ID is ALSO valid.
            var abortRequest2 = TestTools.CreateAbortRequestWithTaxSystemId(response.TransactionId);
            var abortResponse2 = await _taxHandler.AbortPendingTaxCalculationUsingSovosId(abortRequest2);
            Assert.Null(abortResponse2.ErrorMessage);
        }
        #endregion
        #region Refund
        /// <summary>
        /// TODO Warning Refund does not refund nor display the money to be returned for taxes.  
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RefundCreatesNewCreditResponseButDoesNotProvideTaxInfo()
        {
            var refundRequest = TestTools.CreateRefundRequest();
            var response = await _taxHandler.RefundRequest(refundRequest);
            Assert.Null(response.ErrorMessage);
            Assert.NotNull(response.TransactionId);
            Assert.False(response.InvoiceTotalTaxAmount != 0);
        }
        #endregion
    }
}
