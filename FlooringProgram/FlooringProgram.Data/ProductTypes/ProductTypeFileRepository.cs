using System;
using System.Collections.Generic;
using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;
using System.IO;

namespace FlooringProgram.Data.ProductTypes
{
    internal class ProductTypeFileRepository : IProductTypeRepository
    {
        public List<ProductType> GetProductTypes()
        {
            List<ProductType> products = new List<ProductType>();

            string[] data = File.ReadAllLines(@"Data\Products.txt");
            for (int i = 1; i < data.Length; i++)
            {
                string[] row = data[i].Split(',');

                ProductType toAdd = new ProductType();
                toAdd.Type = row[0];
                toAdd.CostPerSquareFoot = decimal.Parse(row[1]);
                toAdd.LaborCostPerSquareFoot = decimal.Parse(row[2]);

                products.Add(toAdd);
            }

            return products;
        }
    }
}