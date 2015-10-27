using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlooringProgram.Models 
{
    public class Order
    {
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public string CustomerName { get; set; }
        public string State { get; set; }
        public decimal TaxPercent { get; set; }
        public string ProductType { get; set; }
        public decimal Area { get; set; }
        public decimal CostPerSquareFoot { get; set; }
        public decimal LaborCostPerSquareFoot { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public Order(DateTime date, int orderNumber, string customerName, string state, decimal taxPercent, string productType, decimal area, decimal costPerSquarFoot, decimal laborCostPerSquareFoot, decimal materialCost, decimal laborCost, decimal tax, decimal total)
        {
            Date = date;
            Number = orderNumber;
            CustomerName = customerName;
            State = state;
            TaxPercent = taxPercent;
            ProductType = productType;
            Area = area;
            CostPerSquareFoot = costPerSquarFoot;
            LaborCostPerSquareFoot = laborCostPerSquareFoot;
            MaterialCost = materialCost;
            LaborCost = laborCost;
            Tax = tax;
            Total = total;
        }

        public Order()
        {
        }
    }
}
