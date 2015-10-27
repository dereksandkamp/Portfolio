using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlooringProgram.Data.Orders
{
    internal class CsvFileHandler : IFileHandler
    {
        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private const string EscapedComma = Quote + "," + Quote;
        private readonly string FilePathStart;

        public CsvFileHandler(string filePathStart)
        {
            FilePathStart = filePathStart;
        }

        private IEnumerable<string> SplitLine(string line)
        {
            int fieldStart = 0;
            for (int i = fieldStart; i < line.Length; i++)
            {
                if (line[i] == ',' || i == line.Length - 1)
                {
                    string returnString = line.Substring(fieldStart, i - fieldStart);
                    returnString = returnString.Replace(EscapedComma, ",");
                    returnString = returnString.Replace(EscapedQuote, Quote);

                    yield return returnString;
                    fieldStart = i + 1;
                }
                else if (line[i] == '"')
                {
                    if (line[i + 1] == '"') //if there are 2 quotes in a row, that is an escaped single quote. We should read it.
                    {
                        i++; //this should skip both quote marks
                    }
                    else
                    {
                        do //otherwise, ignore whatever is in quotes - it is escaped.
                        {
                            i++;
                        } while (line[i] != '"');
                    }
                }
            }
        }

        private string EscapeString(string s)
        {
            s = s.Replace(Quote, EscapedQuote);
            s = s.Replace(",", EscapedComma);

            return s;
        }

        public void WriteOrdersToFile(List<Order> orders)
        {
            //Don't call this with an empty list. If orders list is empty call DeleteFileForDate instead.
            DateTime date = orders.First().Date;
            string filePath = $"{FilePathStart}{date.Month:00}{date.Day:00}{date.Year:0000}.csv";
            string[] orderStrings = new string[orders.Count() + 1]; //+1 for the header
            orderStrings[0] = "Date,OrderNumber,CustomerName,State,TaxRate,ProductType,Area,CostPerSquareFoot,LaborCostPerSquareFoot,MaterialCost,LaborCost,Tax,Total";
            for (int i = 1; i < orderStrings.Length; i++)
            {
                Order order = orders[i - 1];

                PropertyInfo[] props = order.GetType().GetProperties();

                Dictionary<string, string> escapedFields = props.Select(p => new { Name = p.Name, Value = EscapeString(p.GetValue(order).ToString()) }).ToDictionary(field => field.Name, field => field.Value);

                string orderStringToAdd = $"{escapedFields["Date"]},{escapedFields["Number"]},{escapedFields["CustomerName"]},{escapedFields["State"]},{escapedFields["TaxPercent"]},{escapedFields["ProductType"]},{escapedFields["Area"]},{escapedFields["CostPerSquareFoot"]},{escapedFields["LaborCostPerSquareFoot"]},{escapedFields["MaterialCost"]},{escapedFields["LaborCost"]},{escapedFields["Tax"]},{escapedFields["Total"]}";
                orderStrings[i] = orderStringToAdd;
            }
            File.WriteAllLines(filePath, orderStrings);
        }

        public List<Order> LoadFromFile(DateTime date)
        {
            List<Order> orders = new List<Order>();
            string filePath = $"{FilePathStart}{date.Month:00}{date.Day:00}{date.Year:0000}.csv";
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File does not exist.");
            }

            string[] orderStrings = File.ReadAllLines(filePath);

            for (int i = 1; i < orderStrings.Length; i++)
            {
                List<string> orderFields = SplitLine(orderStrings[i]).ToList();

                if (orderFields.Count() != 13)
                {
                    throw new Exception($"{orderFields.Count()} fields found in order, should be 13.\n" + orderStrings[i]);
                }
                Order orderToAdd = new Order();

                orderToAdd.Date = DateTime.Parse(orderFields[0]);
                orderToAdd.Number = Int32.Parse(orderFields[1]);
                orderToAdd.CustomerName = orderFields[2];
                orderToAdd.State = orderFields[3];
                orderToAdd.TaxPercent = Decimal.Parse(orderFields[4]);
                orderToAdd.ProductType = orderFields[5];
                orderToAdd.Area = Decimal.Parse(orderFields[6]);
                orderToAdd.CostPerSquareFoot = Decimal.Parse(orderFields[7]);
                orderToAdd.LaborCostPerSquareFoot = Decimal.Parse(orderFields[8]);
                orderToAdd.MaterialCost = Decimal.Parse(orderFields[9]);
                orderToAdd.LaborCost = Decimal.Parse(orderFields[10]);
                orderToAdd.Tax = Decimal.Parse(orderFields[11]);
                orderToAdd.Total = Decimal.Parse(orderFields[12]);

                orders.Add(orderToAdd);
            } //for each line in file.
            return orders;
        }
    }
}
