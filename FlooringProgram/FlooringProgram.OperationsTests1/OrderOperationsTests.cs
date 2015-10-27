using NUnit.Framework;
using FlooringProgram.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models;
using NUnit.Framework.Constraints;

namespace FlooringProgram.Operations.Tests
{
    [TestFixture]
    public class OrderOperationsTests
    {

        [TestCase(2013, 06, 01, 2, "Wise")]
        [TestCase(1000, 01, 01, 0, null)]
        public void GetOrdersByDateTest(int year, int month, int day, int expectedCount, string expectedName1)
        {
            List<Order> actualDates = OrderOperations.GetOrdersByDate(new DateTime(year, month, day));

            Assert.AreEqual(expectedCount, actualDates.Count);
            if (actualDates.Count > 0)
            {
                Assert.AreEqual(expectedName1, actualDates[0].CustomerName);
            }
        }

        [TestCase(-1)]
        public void RemoveOrderTest(int expectedDifferenceInNumberOfOrders)
        {
            int numberOfOrdersBefore = OrderOperations.GetOrdersByDate(new DateTime(2013, 06, 01)).Count;

            Order orderToRemove = OrderOperations.GetOrdersByDate(new DateTime(2013, 06, 01))[0];
            OrderOperations.RemoveOrder(orderToRemove);

            int numberOfOrdersAfter = OrderOperations.GetOrdersByDate(new DateTime(2013, 06, 01)).Count;
            int actualDifferenceInNumberOfOrders = numberOfOrdersAfter - numberOfOrdersBefore;

            Assert.AreEqual(expectedDifferenceInNumberOfOrders, actualDifferenceInNumberOfOrders);
        }

        [TestCase("OH", "Wood", 50, 525.94)]
        public void CalculateRemainingOrderFieldsTest(string state, string productType, decimal area, decimal expectedTotal)
        {
            Order orderToTest = new Order();
            orderToTest.State = state;
            orderToTest.ProductType = productType;
            orderToTest.Area = area;
            orderToTest = OrderOperations.CalculateRemainingOrderFields(orderToTest);

            Assert.AreEqual(expectedTotal, decimal.Round(orderToTest.Total, 2));
        }

        [Test]
        public void AssignOrderNumberTest()
        {
            Order order = new Order();
            order.Date = new DateTime(2013, 06, 01);

            order = OrderOperations.AssignOrderNumber(order);
            int numberOfOrders = OrderOperations.GetOrdersByDate(new DateTime(2013, 06, 01)).Count;

            Assert.AreEqual(order.Number, numberOfOrders + 1);
        }
    }
}