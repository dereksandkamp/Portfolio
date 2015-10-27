using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlooringProgram.Models.Interfaces
{
    public interface IOrderRepository
    {
        List<Order> GetOrdersForDate(DateTime date);
        void RemoveOrder(Order orderToRemove);
        int NextAvailableOrderNumber(DateTime date);
        void AddOrder(Order orderToAdd);
        List<DateTime> GetValidDates(); //Returns a sorted list of valid dates.
    }
}
