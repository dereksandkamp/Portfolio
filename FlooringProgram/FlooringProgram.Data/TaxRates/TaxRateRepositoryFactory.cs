using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models.Interfaces;

namespace FlooringProgram.Data.TaxRates
{
    public class TaxRateRepositoryFactory
    {
        public static ITaxRateRepository GetTaxRateRepository()
        {
            switch (ConfigurationSettings.GetMode().ToUpper())
            {
                case "PROD":
                    return new TaxRateFileRepository();
                case "TEST":
                    return new TaxRateTestRepository();
            }

            return null;
        }
    }
}
