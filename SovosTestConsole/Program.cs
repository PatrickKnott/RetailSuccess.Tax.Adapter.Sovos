using Microsoft.Extensions.DependencyInjection;
using RetailSuccess.Tax;
using RetailSuccess.Tax.Adapter.Sovos;
using RetailSuccess.Tax.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RetailSuccess.Tax.Adapter.Sovos.SetUp;

namespace SovosTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appSettings.json");
            var config = configBuilder.Build();
            var services = new ServiceCollection();
            services.AddSovosTaxAdapter(config);
            var container = services.BuildServiceProvider();
            var taxhandler = container.GetService<ITaxHandler>();
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
            var response = taxhandler.GetTaxQuote(taxQuote).GetAwaiter().GetResult();
            Console.WriteLine(response.InvoiceTotalTaxAmount);
            Console.WriteLine(response.ErrorMessage);
            Console.ReadKey();
        }
    }
}
