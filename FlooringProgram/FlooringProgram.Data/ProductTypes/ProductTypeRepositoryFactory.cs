using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models.Interfaces;

namespace FlooringProgram.Data.ProductTypes
{
    public class ProductTypeRepositoryFactory
    {
        public static IProductTypeRepository GetProductTypeRepository()
        {
            switch (ConfigurationSettings.GetMode().ToUpper())
            {
                case "PROD":
                    return new ProductTypeFileRepository();
                case "TEST":
                    return new ProductTypeTestRepository();
            }

            return null;
        }
    }
}
