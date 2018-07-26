using System;
using System.Collections.Generic;
using System.Text;
using RetailSuccess.Sovos.Client.Enums;

namespace RetailSuccess.Tax.Adapter.Sovos.PropertyMaps
{
    public static class TaxCalculationTypeMapper
    {
        /// <summary>
        /// Per Squire, 1 is regular//forward, 2 is back//reverse. 
        /// </summary>
        /// <param name="taxCalculationInt"></param>
        /// <returns></returns>
        public static TaxCalculationType GetTypeFromInt(int? taxCalculationInt)
        {
            var taxCalculationType = TaxCalculationType.Regular;
            if (taxCalculationInt.HasValue)
            {
                taxCalculationType = (TaxCalculationType)taxCalculationInt;
            }
            return taxCalculationType;
        }
    }
}
