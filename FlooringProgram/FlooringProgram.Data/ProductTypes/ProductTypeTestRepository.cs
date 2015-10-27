using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;

namespace FlooringProgram.Data.ProductTypes
{
    class ProductTypeTestRepository : IProductTypeRepository
    {
        public List<ProductType> GetProductTypes()
        {
            return new List<ProductType>()
            {
                new ProductType() {Type = "Wood", CostPerSquareFoot = 5.15M, LaborCostPerSquareFoot = 4.75M },
                new ProductType() {Type = "Tile", CostPerSquareFoot = 3.50M, LaborCostPerSquareFoot = 4.15M }
            };
        }
    }
}
