using RetailSuccess.Sovos.Client;
using RetailSuccess.Sovos.Client.Enums;
using RetailSuccess.Sovos.Client.TaxCalculation;
using RetailSuccess.Tax.Models;
using System;
using System.Collections.Generic;
using RetailSuccess.Tax.Refund;

namespace RetailSuccess.Tax.Adapter.Sovos.PropertyMaps
{
    public static class TaxRelayMapper
    {
        //TODO Where does StoreId go?


        /// <summary>
        /// This creates the request that will not be submitted into the pending table.
        /// </summary>
        /// <param name="taxInformation"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static SovosTaxCalcRequest CreateSovosTaxQuote(TaxRequest taxInformation, SovosTaxClientOptions userInfo)
        {
            var sovosLineItems = new List<SovosTaxCalcRequestLine>();

            foreach (var lineItem in taxInformation.LineItemInformation)
            {
                var sovosLineItem =
                    new SovosTaxCalcRequestLine()
                    {
                        TransactionType = TransactionType.Sale,
                        OrganizationCode = "10002173",
                        GrossAmount = lineItem.GrossAmount,
                        Quantity = lineItem.Quantity,
                        GoodServiceCode = lineItem.SKU,
                        GoodServiceCodeDescription = lineItem.Description,
                        LineItemId = lineItem.LineItemId,
                        CustomerVendorIdCode = taxInformation.MerchantInformation?.TaxExemptionId,
                        //TODO see below
                        #region Bill To
                        BillToTaxId =
                            taxInformation.BillingInformation?.FullName, 
                        BillToStreetNameWithNumber =
                            taxInformation.BillingInformation?.AddressLineOne + " " +
                            taxInformation.BillingInformation?.AddressLineTwo,
                        BillToCity = taxInformation.BillingInformation?.City,
                        BillToStateOrProvince = taxInformation.BillingInformation?.State,
                        BillToPostalCode =
                            taxInformation.BillingInformation?.ZipCode,
                        BillToCountry = taxInformation.BillingInformation?.Country,
                        #endregion
                        #region Ship To
                        ShipToTaxId = taxInformation.ShipToInformation?.FullName,
                        ShipToStreetNameWithNumber =
                            taxInformation.ShipToInformation?.AddressLineOne + " " +
                            taxInformation.ShipToInformation?.AddressLineTwo,
                        ShipToCity = taxInformation.ShipToInformation?.City,
                        ShipToStateOrProvince = taxInformation.ShipToInformation?.State,
                        ShipToPostalCode = taxInformation.ShipToInformation?.ZipCode,
                        ShipToCountry = taxInformation.ShipToInformation?.Country,
                        #endregion
                        #region Ship From
                        ShipFromTaxId = taxInformation.ShipFromInformation?.FullName,
                        ShipFromStreetNameWithNumber =
                            taxInformation.ShipFromInformation?.AddressLineOne + " " +
                            taxInformation.ShipFromInformation?.AddressLineTwo,
                        ShipFromCity = taxInformation.ShipFromInformation?.City,
                        ShipFromStateOrProvince = taxInformation.ShipFromInformation?.State,
                        ShipFromPostalCode = taxInformation.ShipFromInformation?.ZipCode,
                        ShipFromCountry = taxInformation.ShipFromInformation?.Country
                        #endregion

                    };
                sovosLineItems.Add(sovosLineItem);
            }

            var request = new SovosTaxCalcRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                CurrencyCode = "USD",
                TaxCalculationType = TaxCalculationTypeMapper.GetTypeFromInt(taxInformation.TaxCalculationType),
                ResultLevel = ResultLevel.ConciseSetofResultsIncludingOnlyAmounts,
                IsAudit = false,

                LineItems = sovosLineItems
            };
            return request;

        }

        /// <summary>
        /// This creates the request to go into the pending table to be further committed or aborted later.
        /// </summary>
        /// <param name="taxInformation"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static SovosTaxCalcRequest CreateSovosTaxRequest(TaxRequest taxInformation, SovosTaxClientOptions userInfo)
        {
            //TODO StoreID  This could be MerchantId or transactionSource.

            var sovosLineItems = new List<SovosTaxCalcRequestLine>();
            foreach (var lineItem in taxInformation.LineItemInformation)
            {
                var sovosLineItem =
                    new SovosTaxCalcRequestLine()
                    {
                        TransactionType = TransactionTypeMapper.GetTypeFromInt(lineItem.TransactionType),
                        OrganizationCode = "10002173", //DirectTax.ClientUnit
                        GrossAmount = lineItem.GrossAmount,
                        Quantity = lineItem.Quantity,
                        GoodServiceCode = lineItem.SKU,
                        GoodServiceCodeDescription = lineItem.Description,
                        CustomerVendorIdCode = taxInformation.MerchantInformation?.TaxExemptionId,  //per squire docs.
                        LineItemId = lineItem.LineItemId,
                        //TODO check what fullname is vs. tax id.
                        #region Bill To
                        BillToTaxId =
                            taxInformation.BillingInformation?.FullName,
                        BillToStreetNameWithNumber =
                            taxInformation.BillingInformation?.AddressLineOne + " " +
                            taxInformation.BillingInformation?.AddressLineTwo,
                        BillToCity = taxInformation.BillingInformation?.City,
                        BillToStateOrProvince = taxInformation.BillingInformation?.State,
                        BillToPostalCode =
                            taxInformation.BillingInformation?.ZipCode,
                        BillToCountry = taxInformation.BillingInformation?.Country,
                        #endregion
                        #region Ship To
                        ShipToTaxId = taxInformation.ShipToInformation?.FullName,
                        ShipToStreetNameWithNumber =
                            taxInformation.ShipToInformation?.AddressLineOne + " " +
                            taxInformation.ShipToInformation?.AddressLineTwo,
                        ShipToCity = taxInformation.ShipToInformation?.City,
                        ShipToStateOrProvince = taxInformation.ShipToInformation?.State,
                        ShipToPostalCode = taxInformation.ShipToInformation?.ZipCode,
                        ShipToCountry = taxInformation.ShipToInformation?.Country,
                        #endregion
                        #region Ship From
                        ShipFromTaxId = taxInformation.ShipFromInformation?.FullName,
                        ShipFromStreetNameWithNumber =
                            taxInformation.ShipFromInformation?.AddressLineOne + " " +
                            taxInformation.ShipFromInformation?.AddressLineTwo,
                        ShipFromCity = taxInformation.ShipFromInformation?.City,
                        ShipFromStateOrProvince = taxInformation.ShipFromInformation?.State,
                        ShipFromPostalCode = taxInformation.ShipFromInformation?.ZipCode,
                        ShipFromCountry = taxInformation.ShipFromInformation?.Country
                        #endregion
                    };
                sovosLineItems.Add(sovosLineItem);
            }

            return new SovosTaxCalcRequest()
            {
                UserName = userInfo.User,
                Password = userInfo.Password,
                TransactionId = taxInformation.OrderNumber, 
                TransactionSource = taxInformation.TransactionSource,
                //TransactionDocumentMessagingRequired = true,  //TODO defaults to false, per docs true? do I need this? What does this do?
                TransactionDocumentNumber = taxInformation.InvoiceNumber, 
                DocumentDate = taxInformation.SaleDate,
                CurrencyCode = "USD",
                TaxCalculationType = TaxCalculationTypeMapper.GetTypeFromInt(taxInformation.TaxCalculationType),  
                ResultLevel = ResultLevel.ConciseSetofResultsIncludingOnlyAmounts,
                LineItems = sovosLineItems
            };
        }

        /// <summary>
        /// This maps the response back into Retail Success's response expectations.
        /// </summary>
        /// <param name="taxInformation"></param>
        /// <param name="saleDate"></param>
        /// <returns></returns>
        public static TaxResponse CreateTaxResponseFromSovos(SovosTaxCalcResponse taxInformation, DateTime? saleDate)
        {
            var lineItems = new List<TaxLineItemResponse>();
            foreach (var lineResult in taxInformation.LineResults)
            {
                var lineResponse = new TaxLineItemResponse()
                {
                    GrossAmount = lineResult.GrossAmount,
                    LineItemTax = lineResult.TotalTaxAmount,
                    OrderItemId = lineResult.LineId 
                };
                lineItems.Add(lineResponse);
            }

            return new TaxResponse()
            {
                InvoiceNumber = taxInformation.TransactionDocumentNumber,  
                InvoiceTotalTaxAmount = taxInformation.TaxAmount,
                TransactionDate = saleDate,
                TransactionId = taxInformation.SovosTransactionDocumentId,
                LineItemResponse = lineItems
            };
        }

        /// <summary>
        /// This maps the refund response back into Retail Success's response expectations.
        /// </summary>
        /// <param name="taxInformation"></param>
        /// <param name="saleDate"></param>
        /// <returns></returns>
        public static TaxRefundResponse CreateTaxRefundResponseFromSovos(SovosTaxCalcResponse taxInformation, DateTime? saleDate)
        {
            var lineItems = new List<TaxLineItemResponse>();
            foreach (var lineResult in taxInformation.LineResults)
            {
                var lineResponse = new TaxLineItemResponse()
                {
                    GrossAmount = lineResult.GrossAmount,
                    LineItemTax = lineResult.TotalTaxAmount,
                    OrderItemId = lineResult.LineId
                };
                lineItems.Add(lineResponse);
            }
            return new TaxRefundResponse()
            {
                InvoiceTotalTaxAmount = taxInformation.TaxAmount,
                TransactionDate = saleDate,
                TransactionId = taxInformation.SovosTransactionDocumentId,
                RefundDate = DateTime.Now,  //TODO check to ensure this is valid?
                LineItemResponse = lineItems
            };
        }
    }
}