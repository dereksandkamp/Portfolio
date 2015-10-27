using System;
using System.Collections.Generic;
using FlooringProgram.Models;
using FlooringProgram.Data;
using FlooringProgram.Data.Orders;
using FlooringProgram.Models.Interfaces;

namespace FlooringProgram.Operations
{
    public static class OrderOperations
    {
        static readonly IOrderRepository Repo = OrderRepositoryFactory.GetOrderRepository();
        static readonly Logger Logger = new Logger();

        public static List<Order> GetOrdersByDate(DateTime date)
        {
            try
            {
                return Repo.GetOrdersForDate(date);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                throw new Exception("\tThere was an error getting the orders for " + date.ToString("d") + ":\n\t" + e.Message);
            }
        }

        public static void RemoveOrder(Order orderToRemove)
        {
            try
            {
                Repo.RemoveOrder(orderToRemove);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                throw new Exception("\tThere was an error removing the selected order:\n\t" + e.Message);
            }
        }

        public static Order CalculateRemainingOrderFields(Order startedOrder, bool assignDate = true)
        {
            //calculate the remaining fields
            if (assignDate)
            {
                startedOrder.Date = DateTime.Today;
            }
            
            startedOrder.TaxPercent = TaxRateOperations.GetTaxRateFor(startedOrder.State).TaxPercent;

            ProductType orderProductType = ProductTypeOperations.GetProductType(startedOrder.ProductType);
            startedOrder.CostPerSquareFoot = orderProductType.CostPerSquareFoot;
            startedOrder.LaborCostPerSquareFoot = orderProductType.LaborCostPerSquareFoot;

            startedOrder.MaterialCost = startedOrder.CostPerSquareFoot * startedOrder.Area;
            startedOrder.LaborCost = startedOrder.LaborCostPerSquareFoot * startedOrder.Area;
            startedOrder.Tax = (startedOrder.MaterialCost + startedOrder.LaborCost)*(startedOrder.TaxPercent*0.01M);
            startedOrder.Total = startedOrder.MaterialCost + startedOrder.LaborCost + startedOrder.Tax;

           return startedOrder;
        }

        public static Order AssignOrderNumber(Order orderToNumber)
        {
            try
            {
                orderToNumber.Number = Repo.NextAvailableOrderNumber(orderToNumber.Date);
                
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                throw new Exception("\tThere was an error assigining an order number:\n\t" + e.Message);
            }
            return orderToNumber;
        }

        public static void AddOrder(Order orderToAdd)
        {
            try
            {
                Repo.AddOrder(orderToAdd);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                throw new Exception("\tThere was an error adding your order:\n\t" + e.Message);
            }
        }

        public static void EditOrder(Order oldOrder, Order newOrder)
        {
            string userErrorMessage = "\tThere was an error editing your order:\n\t";
            try
            {
                Repo.RemoveOrder(oldOrder);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                throw new Exception(userErrorMessage + e.Message);
            }

            try
            {
                Repo.AddOrder(newOrder);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                throw new Exception(userErrorMessage + e.Message +
                                    "\n\tAlec Wojciechowski said this was an acceptable risk.");
            }
        }

        public static List<DateTime> GetValidDates()
        {
            try
            {
                return Repo.GetValidDates();

            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                throw new Exception("\tThere was an error getting valid dates:\n\t" + e.Message);
            }
        }
    }
}
