using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RetailSuccess.Tax.Models;
using RetailSuccess.Tax.Refund;
using RetailSuccess.Tax.Update;

namespace RetailSuccess.Tax.Adapter.Sovos.Tests
{
    internal static class TestTools
    {
        public static ITaxHandler GetTaxHandler()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ITaxHandler>(x => new SovosImplementation());
            var container = serviceCollection.BuildServiceProvider();
            var taxHandler = container.GetService<ITaxHandler>();
            return taxHandler;
        }

        public static TaxRequest CreateTaxQuoteRequest()
        {
            var taxQuote = new TaxRequest()
            {
                SaleDate = DateTime.Now,

                ShipToInformation = new ShipToInformation()
                {
                    ZipCode = "64057",
                    Country = "USA"
                },
                TaxCalculationType = 1,
                LineItemInformation = new List<TaxLineItemRequest>()
                {
                    new TaxLineItemRequest()
                    {
                        GrossAmount = 100,
                        Quantity = 1,
                        TransactionType = 1,
                    }
                }
            };
            return taxQuote;
        }

        public static TaxRequest CreateTaxPendingRequest()
        {
            var taxQuote = new TaxRequest()
            {
                SaleDate = DateTime.Now,
                TransactionSource = "ERP",
                OrderNumber = "Test" + Guid.NewGuid(),
                InvoiceNumber = "Test",
                ShipToInformation = new ShipToInformation()
                {
                    ZipCode = "64057",
                    Country = "USA"
                },
                TaxCalculationType = 1,
                LineItemInformation = new List<TaxLineItemRequest>()
                {
                    new TaxLineItemRequest()
                    {
                        GrossAmount = 100,
                        Quantity = 1,
                        TransactionType = 1,
                    }
                }
            };
            return taxQuote;
        }

        public static TaxAbortRequestWithTaxSystemId CreateAbortRequestWithTaxSystemId(string sovosId)
        {
            return new TaxAbortRequestWithTaxSystemId()
            {
                PriorTransactionId = sovosId
            };
        }

        public static TaxAbortRequest CreateAbortRequestWithOrderId(string orderId)
        {
            return new TaxAbortRequest()
            {
                OrderId = orderId,
                TransactionSource = "ERP"
            };
        }

        public static TaxCommitRequestWithTaxSystemId CreateCommitRequestWithTaxSystemId(string sovosId)
        {
            return new TaxCommitRequestWithTaxSystemId()
            {
                PriorTransactionId = sovosId
            };
        }

        public static TaxCommitRequest CreateCommitRequestWithOrderId(string orderId)
        {
            return new TaxCommitRequest()
            {
                OrderId = orderId,
                TransactionSource = "ERP"
            };
        }
        public static TaxUpdatePendingTaxRequest CreateUpdateRequest(string orderId)
        {
            var taxQuote = new TaxUpdatePendingTaxRequest()
            {
                SaleDate = DateTime.Now,
                TransactionSource = "ERP",
                OrderNumber = orderId,
                InvoiceNumber = "Test",
                ShipToInformation = new ShipToInformation()
                {
                    ZipCode = "64057",
                    Country = "USA"
                },
                TaxCalculationType = 1,
                LineItemInformation = new List<TaxLineItemRequest>()
                {
                    new TaxLineItemRequest()
                    {
                        GrossAmount = 200,
                        Quantity = 1,
                        TransactionType = 1,
                    }
                }
            };
            return taxQuote;
        }

        public static TaxRefundRequest CreateRefundRequest()
        {
            return new TaxRefundRequest()
            {
                SaleDate = DateTime.Now,
                TransactionSource = "ERP",
                OrderNumber = "Test" + Guid.NewGuid(),
                InvoiceNumber = "Test",
                ShipToInformation = new ShipToInformation()
                {
                    ZipCode = "64057",
                    Country = "USA"
                },
                TaxCalculationType = 1,
                LineItemInformation = new List<TaxLineItemRequest>()
                {
                    new TaxLineItemRequest()
                    {
                        GrossAmount = 100,
                        Quantity = 1,
                        TransactionType = 1,
                    }
                },
                IsRefund = true
                
            };
        }

    }
}
