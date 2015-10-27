using System;
using System.Collections.Generic;
using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;
using System.Linq;
using System.IO;

namespace FlooringProgram.Data.Orders
{
    public class OrderFileRepository : IOrderRepository
    {

        private const string FilePathStart = @"Data\Orders_";
        private const string CurrentDirectory = @"Data\";
        private const string FileNameStart = "Orders_";
        private IFileHandler fileWriter = FileWriterFactory.GetFileWriter(FilePathStart);

        public void AddOrder(Order orderToAdd)
        {
            List<Order> orders = LoadOrdersForDate(orderToAdd.Date);
            //TODO Verify order number is not already present in this file.
            if (orders == null)
            {
                orders = new List<Order>();
            }
            orders.Add(orderToAdd);

            SaveOrdersForDate(orders);
            DeleteFileForDate(orderToAdd.Date, "." + ConfigurationSettings.GetFileWriteFormat().ToLower()); //this way, if they changed the file write format on the same day, there won't be duplicate files
        }

        public void RemoveOrder(Order orderToRemove)
        {
            DateTime dateToRemoveFrom = orderToRemove.Date;
            int orderNumberToRemove = orderToRemove.Number;
            List<Order> ordersForDate = LoadOrdersForDate(dateToRemoveFrom);

            if (!ordersForDate.Any())
            {
                throw new ArgumentException($"Tried to remove an order from a date with no orders.\nDate: {dateToRemoveFrom}");
            }

            IEnumerable<Order> matchingOrders = ordersForDate.Where(o => o.Date == dateToRemoveFrom && o.Number == orderNumberToRemove);
            if (matchingOrders.Count() > 1)
            {
                throw new Exception($"Multiple orders with same date and number discovered during RemoveOrder operation.  Date: {dateToRemoveFrom}");
            }
            if (!matchingOrders.Any())
            {
                throw new ArgumentException($"Tried to remove an order that doesn't exist. \nDate: {dateToRemoveFrom} \nNumber: {orderNumberToRemove}");
            }

            ordersForDate.Remove(matchingOrders.First());
            DeleteFileForDate(orderToRemove.Date);

            if (ordersForDate.Count > 1)
            {
                SaveOrdersForDate(ordersForDate);
            }
        }

        public List<Order> GetOrdersForDate(DateTime date)
        {
            List<Order> orders = LoadOrdersForDate(date);

            //sort by order number
            if (orders.Count > 0)
            {
                orders = orders.OrderBy(o => o.Number).ToList();
            }

            return orders;
        }

        public int NextAvailableOrderNumber(DateTime date)
        {
            List<Order> orders = LoadOrdersForDate(date);
            if (orders.Count == 0)
            {
                return 1;
            }
            IEnumerable<Order> ordersForDate = (from order in orders
                                                where order.Date == date
                                                orderby order.Number
                                                select order);

            return ordersForDate.Last().Number + 1;
        }

        private void SaveOrdersForDate(List<Order> orders)
        {
            //Don't call this with an empty list. If orders list is empty call DeleteFileForDate instead.
            fileWriter.WriteOrdersToFile(orders);
        }

        private List<Order> LoadOrdersForDate(DateTime date)
        {
            string filePath = $"{FilePathStart}{date.Month:00}{date.Day:00}{date.Year:0000}.*"; //TODO fix filepath
            string[] actualFilePath = Directory.GetFiles(@".",filePath);
            if (!actualFilePath.Any())
            {
                return new List<Order>();
            }
            if (actualFilePath.Length > 1)
            {
                throw new Exception($"Multiple files found for date: {date}");
            }

            IFileHandler fileHandler;
            switch (Path.GetExtension(actualFilePath[0]))
            {
                case ".json":
                    fileHandler = new JsonFileHandler(FilePathStart);
                    break;
                case ".csv":
                    fileHandler = new CsvFileHandler(FilePathStart);
                    break;
                case ".xml":
                    fileHandler = new XmlFileHandler(FilePathStart);
                    break;
                default:
                    throw new Exception($"File type not reconized for date: {date}");
            }
            return fileHandler.LoadFromFile(date);
        }

        private void DeleteFileForDate(DateTime date, string fileExtensionToIgnore = "")
        {
            //get all files that match the date, regardless of file type.
            string[] filesToDelete = Directory.GetFiles(CurrentDirectory, $"{FileNameStart}{date.Month:00}{date.Day:00}{date.Year:0000}.*");

            if (filesToDelete.Length == 0)
            {
                throw new ArgumentException($"Tried to delete a file that doesn't exist.  Date:{date}");
            }

            foreach (string filePath in filesToDelete)
            {
                if (Path.GetExtension(filePath) != fileExtensionToIgnore)
                {
                    File.Delete(filePath);
                }
            }
        }

        public List<DateTime> GetValidDates()
        {
            string[] fileNames = Directory.GetFiles(@".", FilePathStart + "*");

            string[] dateStrings = Directory.GetFiles(".", FilePathStart + "*");
            List<DateTime> dates = new List<DateTime>();
            int start = FilePathStart.Length + 2;
            foreach (var s in dateStrings)
            {
                DateTime datetoAdd = DateTime.Parse(s.Substring(start, 2) + "/" + s.Substring(start + 2, 2) + "/" + s.Substring(start + 4,4));
                dates.Add(datetoAdd);
            }
            dates.Sort();
            return dates;
        }
    }
}