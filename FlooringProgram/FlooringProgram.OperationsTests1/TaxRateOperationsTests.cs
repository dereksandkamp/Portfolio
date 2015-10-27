using NUnit.Framework;
using FlooringProgram.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models.Interfaces;
using FlooringProgram.Data.TaxRates;
using FlooringProgram.Models;

namespace FlooringProgram.Operations.Tests
{
    [TestFixture]
    public class TaxRateOperationsTests
    {
        [Test]
        public void IsAllowedStateTest()
        {
            var result = TaxRateOperations.IsAllowedState("OH");

            Assert.IsTrue(result);
        }

        [Test]
        public void GetTaxRateForTest()
        {
            TaxRate result = TaxRateOperations.GetTaxRateFor("OH");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.State, "OH");
            Assert.AreEqual(result.TaxPercent, 6.25M);
        }
    }
}