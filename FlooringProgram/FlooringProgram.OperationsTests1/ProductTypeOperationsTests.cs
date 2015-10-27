using NUnit.Framework;
using FlooringProgram.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models;

namespace FlooringProgram.Operations.Tests
{
    [TestFixture]
    public class ProductTypeOperationsTests
    {
        [TestCase("Wood")]
        [TestCase("Tile")]
        public void IsProductTypeTest(string productType)
        {
            var result = ProductTypeOperations.IsProductType(productType);

            Assert.IsTrue(result);
        }

        [TestCase("Wood")]
        [TestCase("Tile")]
        public void GetProductTypeTest(string productName)
        {
            ProductType result = ProductTypeOperations.GetProductType(productName);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Type, productName);
        }

        public void GetAllProductTypesTest()
        {
            var result = ProductTypeOperations.GetAllProductTypes();

            Assert.AreEqual(result.Count(), 2);
        }
    }
}