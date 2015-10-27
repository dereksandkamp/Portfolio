using FlooringProgram.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlooringProgram.Data.Orders
{
    public class OrderRepositoryFactory
    {
        public static IOrderRepository GetOrderRepository()
        {
            switch (ConfigurationSettings.GetMode().ToUpper())
            {
                case "PROD":
                    return new OrderFileRepository();
                case "TEST":
                    return new OrderTestRepository();
            }

            return null;
        }
    }
}
