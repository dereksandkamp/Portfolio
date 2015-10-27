using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Data.TaxRates;
using FlooringProgram.Models.Interfaces;
using NUnit.Framework;

namespace FlooringProgram.Tests
{
    [TestFixture]
    public class TaxRateRepositoryTests
    {
        [Test]
        public void ReadDataFile()
        {
            ITaxRateRepository repository = new TaxRateFileRepository();
            var taxRate = repository.GetTaxRates();

            Assert.AreNotEqual(0, taxRate.Count);
        }

        [Test]
        public void CorrectDataLoaded()
        {
            ITaxRateRepository repository = new TaxRateTestRepository();
            var taxRates = repository.GetTaxRates();

            Assert.AreEqual("OH", taxRates[0].State);
            Assert.AreEqual(6.25M, taxRates[0].TaxPercent);
        }
    }
}
