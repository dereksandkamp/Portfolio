using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;
using FlooringProgram.Data.ProductTypes;

namespace FlooringProgram.Operations
{
    static public class ProductTypeOperations
    {
        static public bool IsProductType(string productType)
        {
            IProductTypeRepository repository = ProductTypeRepositoryFactory.GetProductTypeRepository();
            var allProductTypes = repository.GetProductTypes();

            return allProductTypes.Any(t => t.Type.ToUpper() == productType.ToUpper());
        }

        static public ProductType GetProductType(string productType)
        {
            IProductTypeRepository repository = ProductTypeRepositoryFactory.GetProductTypeRepository();
            var allProductTypes = repository.GetProductTypes();

            return allProductTypes.FirstOrDefault(t => t.Type.ToUpper() == productType.ToUpper());
        }

        public static List<ProductType> GetAllProductTypes()
        {
            IProductTypeRepository repository = ProductTypeRepositoryFactory.GetProductTypeRepository();
            return repository.GetProductTypes();
        }
    }
}
