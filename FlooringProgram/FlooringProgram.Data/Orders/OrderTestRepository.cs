using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models;
using System.IO;
using FlooringProgram.Models.Interfaces;

namespace FlooringProgram.Data.Orders
{
    public class OrderTestRepository : IOrderRepository
    {
        private List<Order> orders = new List<Order>();
        private const string _dataFilePath = @"Data\Orders_06012013.csv";

        public OrderTestRepository()
        {
            string[] orderStrings = File.ReadAllLines(_dataFilePath);

            for (int i=1; i < orderStrings.Length; i++)
            {
                //TODO: allow saved entries to have a , in the string.
                string[] stringOrderDetails = orderStrings[i].Split(',');
                if (stringOrderDetails.Count() != 13)
                {
                    throw new Exception($"{stringOrderDetails.Count()} fields found while loading order, should be 13.  \nPlease check the file to see if the format has been altered.  \n" + orderStrings[i]);
                }
                Order orderToAdd = new Order();

                orderToAdd.Date = DateTime.Parse(stringOrderDetails[0]);
                orderToAdd.Number = Int32.Parse(stringOrderDetails[1]);
                orderToAdd.CustomerName = stringOrderDetails[2];
                orderToAdd.State = stringOrderDetails[3];
                orderToAdd.TaxPercent = Decimal.Parse(stringOrderDetails[4]);
                orderToAdd.ProductType = stringOrderDetails[5];
                orderToAdd.Area = Decimal.Parse(stringOrderDetails[6]);
                orderToAdd.CostPerSquareFoot = Decimal.Parse(stringOrderDetails[7]);
                orderToAdd.LaborCostPerSquareFoot = Decimal.Parse(stringOrderDetails[8]);
                orderToAdd.MaterialCost = Decimal.Parse(stringOrderDetails[9]);
                orderToAdd.LaborCost = Decimal.Parse(stringOrderDetails[10]);
                orderToAdd.Tax = Decimal.Parse(stringOrderDetails[11]);
                orderToAdd.Total = Decimal.Parse(stringOrderDetails[12]);

                orders.Add(orderToAdd);
            } //for each line in file.
        }

        public Order GetOrder(DateTime date, int orderNumber)
        {
            IEnumerable<Order> matchingOrders = from order in orders
                                               where order.Date == date
                                               where order.Number == orderNumber
                                               select order;
            if (matchingOrders.Count() == 0)
            {
                throw new Exception($"No orders matching {date} and {orderNumber}");
            }
            else if (matchingOrders.Count() > 1)
            {
                throw new Exception($"Multiple orders matching {date} and {orderNumber}");
            }
            return matchingOrders.First();
        }

        public List<Order> GetOrdersForDate(DateTime date)
        {
            List<Order> ordersForDate = (from order in orders
                                        where order.Date == date
                                        orderby order.Number
                                        select order).ToList();
            return ordersForDate;
        } 

        public void RemoveOrder(Order orderToRemove)
        {
            if (orders.Contains(orderToRemove))
            {
                orders.Remove(orderToRemove);
            }
            else
            {
                throw new ArgumentException("Tried to remove an order that does not exist.");
            }
        }

        public void AddOrder(Order orderToAdd)
        {
            orders.Add(orderToAdd);
        }

        public int NextAvailableOrderNumber (DateTime date)
        {
            IEnumerable<Order> ordersForDate = (from order in orders
                                                where order.Date == date
                                                orderby order.Number
                                                select order);
            if (!ordersForDate.Any())
            {
                return 1;
            }
            return ordersForDate.Last().Number + 1;
        }

        public List<DateTime> GetValidDates()
        {
            var dates = new List<DateTime>();
            foreach (Order order in orders)
            {
                if (!dates.Contains(order.Date))
                {
                    dates.Add(order.Date);
                }
            }
            dates.Sort();
            return dates;
        }
    }
}
