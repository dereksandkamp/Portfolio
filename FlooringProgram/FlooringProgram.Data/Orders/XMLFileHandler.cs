using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;
using System.Xml;

namespace FlooringProgram.Data.Orders
{
    public class XmlFileHandler : IFileHandler
    {
        private readonly string FilePathStart;

        public XmlFileHandler(string filePathStart)
        {
            this.FilePathStart = filePathStart;
        }

        public List<Order> LoadFromFile(DateTime date)
        {
            List<Order> orders = new List<Order>();
            Order orderToAdd = null;
            string filePath = $"{FilePathStart}{date.Month:00}{date.Day:00}{date.Year:0000}.xml";

            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File does not exist.");
            }

            //Derek: I did it with an XmlReader because I couldn't figure out how to deserialize from an XElement to a list of Orders
            //I later realized that I could have read the text to an XDocument, then looped through the XDocument to create the Order objects.
            using (XmlReader reader = new XmlTextReader(filePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Order":
                                //add the previous order and create a new order for this time through
                                if (orderToAdd != null)
                                {
                                    orders.Add(orderToAdd);
                                }
                                orderToAdd = new Order();
                                break;
                            case "Date":
                                orderToAdd.Date = DateTime.Parse(reader.ReadInnerXml());
                                break;
                            case "OrderNumber":
                                orderToAdd.Number = int.Parse(reader.ReadInnerXml());
                                break;
                            case "CustomerName":
                                orderToAdd.CustomerName = reader.ReadElementContentAsString();
                                break;
                            case "State":
                                orderToAdd.State = reader.ReadInnerXml();
                                break;
                            case "TaxPercent":
                                orderToAdd.TaxPercent = decimal.Parse(reader.ReadInnerXml());
                                break;
                            case "ProductType":
                                orderToAdd.ProductType = reader.ReadInnerXml();
                                break;
                            case "Area":
                                orderToAdd.Area = decimal.Parse(reader.ReadInnerXml());
                                break;
                            case "CostPerSquareFoot":
                                orderToAdd.CostPerSquareFoot = decimal.Parse(reader.ReadInnerXml());
                                break;
                            case "LaborCostPerSquareFoot":
                                orderToAdd.LaborCostPerSquareFoot = decimal.Parse(reader.ReadInnerXml());
                                break;
                            case "MaterialCost":
                                orderToAdd.MaterialCost = decimal.Parse(reader.ReadInnerXml());
                                break;
                            case "LaborCost":
                                orderToAdd.LaborCost = decimal.Parse(reader.ReadInnerXml());
                                break;
                            case "Tax":
                                orderToAdd.Tax = decimal.Parse(reader.ReadInnerXml());
                                break;
                            case "Total":
                                orderToAdd.Total = decimal.Parse(reader.ReadInnerXml());
                                break;
                        }
                    }
                }
            }

            ///add last order
            if (orderToAdd != null)
            {
                orders.Add(orderToAdd);
            }
            return orders;
        }

        public void WriteOrdersToFile(List<Order> orders)
        {
            DateTime date = orders.First().Date;
            string filePath = $"{FilePathStart}{date.Month:00}{date.Day:00}{date.Year:0000}.xml";

            //convert to xelements
            XElement xmlTree = new XElement("DateWithOrders",
                orders.Select(o => new XElement("Order",
                    new XElement("Date", o.Date.ToString("d")),
                    new XElement("OrderNumber", o.Number),
                    new XElement("CustomerName", o.CustomerName),
                    new XElement("State", o.State),
                    new XElement("TaxPercent", o.TaxPercent),
                    new XElement("ProductType", o.ProductType),
                    new XElement("Area", o.Area),
                    new XElement("CostPerSquareFoot", o.CostPerSquareFoot),
                    new XElement("LaborCostPerSquareFoot", o.LaborCostPerSquareFoot),
                    new XElement("MaterialCost", o.MaterialCost),
                    new XElement("LaborCost", o.LaborCost),
                    new XElement("Tax", o.Tax),
                    new XElement("Total", o.Total)
                    ))
                );
            //write the file
            xmlTree.Save(filePath);
        }
    }
}
