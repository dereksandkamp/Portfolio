using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace FlooringProgram.Data.Orders
{
    internal class JsonFileHandler : IFileHandler
    {
        private readonly string FilePathStart;

        public JsonFileHandler(string filePathStart)
        {
            FilePathStart = filePathStart;
        }

        public List<Order> LoadFromFile(DateTime date)
        {
            List<Order> orders = new List<Order>();
            string filePath = $"{FilePathStart}{date.Month:00}{date.Day:00}{date.Year:0000}.json";
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"Json file does not exist for date: {date}");
            }

            string[] orderStrings = File.ReadAllLines(filePath);

            for (int i = 0; i < orderStrings.Length; i++)
            {
                orders.Add(JsonConvert.DeserializeObject<Order>(orderStrings[i]));
            }
            return orders;
        }

        public void WriteOrdersToFile(List<Order> orders)
        {
            DateTime date = orders.First().Date;
            string filePath = $"{FilePathStart}{date.Month:00}{date.Day:00}{date.Year:0000}.json";
            string[] orderStrings = new string[orders.Count()];
            for (int i = 0; i < orderStrings.Length; i++)
            {
                orderStrings[i] = JsonConvert.SerializeObject(orders[i]);
            }
            File.WriteAllLines(filePath, orderStrings);
        }
    }
}
