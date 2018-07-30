using RetailSuccess.Sovos.Client.Enums;
using RetailSuccess.Tax.Adapter.Sovos.PropertyMaps;
using Xunit;

namespace RetailSuccess.Tax.Adapter.Sovos.Tests
{
    public class TaxAdapterTests
    {
        [Fact]
        public void GetRegularFromInt1()
        {
            var taxType = TaxCalculationTypeMapper.GetTypeFromInt(1);
            Assert.Equal(TaxCalculationType.Regular, taxType);
        }
        [Fact]
        public void GetRegularFromNull()
        {
            var taxType = TaxCalculationTypeMapper.GetTypeFromInt(null);
            Assert.Equal(TaxCalculationType.Regular, taxType);
        }
        [Fact]
        public void GetBackFromInt2()
        {
            var taxType = TaxCalculationTypeMapper.GetTypeFromInt(2);
            Assert.Equal(TaxCalculationType.BackTaxCalculation, taxType);
        }

        [Fact]
        public void GetTransactionSaleFromInt1()
        {
            var transactionType = TransactionTypeMapper.GetTypeFromInt(1);
            Assert.Equal(TransactionType.Sale, transactionType);
        }
        [Fact]
        public void GetTransactionPurchaseFromInt2()
        {
            var transactionType = TransactionTypeMapper.GetTypeFromInt(2);
            Assert.Equal(TransactionType.Purchase, transactionType);
        }
    }
}
