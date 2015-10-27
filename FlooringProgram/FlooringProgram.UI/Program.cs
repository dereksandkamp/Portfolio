using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlooringProgram.Models;
using FlooringProgram.Models.Interfaces;
using FlooringProgram.Operations;
using FlooringProgram.Data.Orders;

namespace FlooringProgram.UI
{
    internal class Program
    {
        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private const ConsoleColor EmphasisColor = ConsoleColor.Cyan;
        private const ConsoleColor PromptColor = ConsoleColor.Yellow;
        private const ConsoleColor SuccessColor = ConsoleColor.Green;

        private static void Main(string[] args)
        {
            Initialize();
            MainMenu();
        }

        private static void Initialize()
        {
            Console.Title = "SWC Corp Flooring Orders";

            if (Console.LargestWindowWidth >= 60 && Console.LargestWindowHeight >= 30)
            {
                Console.SetWindowSize(60, 30);
                Console.BufferWidth = 60;
                Console.BufferHeight = 30;
            }
            else
            {
                if (Console.LargestWindowWidth >= 60)
                {
                    Console.SetWindowSize(60, Console.LargestWindowHeight);
                    Console.SetBufferSize(60, Console.LargestWindowHeight);
                }
                else
                {
                    Console.SetWindowSize(Console.LargestWindowWidth, 30);
                    Console.SetBufferSize(Console.LargestWindowWidth, 30);
                }
            }
        }


        //UI for main options **********************************************************************

        private static void MainMenu()
        {
            bool quit = false;
            bool error = false;
            while (!quit)
            {
                OffsetTop();
                Console.WriteLine("\t\t****************************");
                Console.Write("\t\t|");
                Console.ForegroundColor = EmphasisColor;
                Console.Write(" SWC Corp Flooring Orders ");
                Console.ResetColor();
                Console.WriteLine("|");
                Console.WriteLine("\t\t****************************");
                //Console.WriteLine();

                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t\t    1. ");
                Console.ResetColor();
                Console.WriteLine("Display Orders");
                Console.ResetColor();

                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t\t    2. ");
                Console.ResetColor();
                Console.WriteLine("Add an Order");
                Console.ResetColor();

                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t\t    3. ");
                Console.ResetColor();
                Console.WriteLine("Edit an Order");
                Console.ResetColor();

                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t\t    4. ");
                Console.ResetColor();
                Console.WriteLine("Remove an Order");
                Console.ResetColor();

                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t\t    5. ");
                Console.ResetColor();
                Console.WriteLine("Quit");
                Console.ResetColor();
                Console.WriteLine();

                //error message
                if (error)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\t\t    Invalid choice.");
                    Console.ResetColor();
                    Console.WriteLine();
                }

                //prompt
                Console.ForegroundColor = PromptColor;
                Console.Write("\t\t    Enter an option: ");
                Console.ForegroundColor = EmphasisColor;
                string userInput = Console.ReadLine().Trim();
                Console.ResetColor();

                try
                {
                    switch (userInput)
                    {
                        case "1":
                            Console.Clear();
                            PromptDisplayOrders();
                            break;
                        case "2":
                            Console.Clear();
                            PromptAddOrder();
                            break;
                        case "3":
                            Console.Clear();
                            PromptEditOrder();
                            break;
                        case "4":
                            Console.Clear();
                            PromptRemoveOrder();
                            break;
                        case "5":
                            quit = true;
                            break;
                        default:
                            error = true;
                            Console.Clear();
                            continue;
                    } //switch (userInput)
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    PressEnterToContinueToMain();
                }
            }//while (!quit)
        }

        private static void PromptDisplayOrders()
        {
            DateTime? date = PromptForValidOrderDate();
            if (date == null)
            {
                return;
            }

            List<Order> potentialOrders = OrderOperations.GetOrdersByDate((DateTime)date);
            Order orderToDisplay = SelectFromOrders(potentialOrders);

            bool validInput = false;
            bool error = false;
            while (!validInput)
            {
                DisplayOrder(orderToDisplay);

                //give options to edit or remove the order
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t1. ");
                Console.ResetColor();
                Console.WriteLine("Edit this order");
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t2. ");
                Console.ResetColor();
                Console.WriteLine("Remove this order");
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t3. ");
                Console.ResetColor();
                Console.WriteLine("Return to the main menu");
                Console.WriteLine();

                if (error)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("Enter 1, 2, or 3.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                Console.ForegroundColor = PromptColor;
                Console.Write("\tEnter an option: ");
                Console.ForegroundColor = EmphasisColor;

                string userInput = Console.ReadLine().Trim();
                Console.ResetColor();
                Console.Clear();

                switch (userInput)
                {
                    case "1":
                    case "1.":
                        PromptEditOrder(orderToDisplay);
                        validInput = true;
                        break;
                    case "2":
                    case "2.":
                        PromptRemoveOrder(orderToDisplay);
                        validInput = true;
                        break;
                    case "3":
                    case "3.":
                    case "":
                        return;
                    default:
                        error = true;
                        continue;
                }
            }
        }

        private static void PromptAddOrder()
        {
            string customerName = "";
            string state = "";
            ProductType productType = null;
            decimal area = 0M;

            while (string.IsNullOrEmpty(customerName))
            {
                OffsetTop();
                Console.ForegroundColor = PromptColor;
                Console.Write("\tEnter the customer name: ");
                Console.ForegroundColor = EmphasisColor;
                customerName = Console.ReadLine();
                Console.ResetColor();
                Console.Clear();
            }

            state = PromptForValidState("Enter the customer state: ", true);
            productType = PromptForValidProductType("Enter the product type: ", true);
            area = PromptForValidArea(productType, true);

            Order orderToAdd = new Order()
            {
                CustomerName = customerName,
                State = state,
                ProductType = productType.Type,
                Area = area
            };

            //calculate remaining fields and assign order number
            orderToAdd = OrderOperations.CalculateRemainingOrderFields(orderToAdd);
            orderToAdd = OrderOperations.AssignOrderNumber(orderToAdd);
            if (PromptConfirmOrder(orderToAdd, "Would you like to add the order? "))
            {
                OrderOperations.AddOrder(orderToAdd);
                Console.Clear();
                DisplayOrder(orderToAdd);
                Console.ForegroundColor = SuccessColor;
                Console.WriteLine("\tOrder added successfully.");
                Console.WriteLine();
                PressEnterToContinueToMain();
            }
            else
            {
                Console.Clear();
                orderToAdd = EditOrder(orderToAdd);  //TODO: Check what happens if an error occurs during edit?
                if (orderToAdd != null) //if it's null, they chose to cancel and return to the main menu
                {
                    OrderOperations.AddOrder(orderToAdd);
                    DisplayOrder(orderToAdd);
                    Console.ForegroundColor = SuccessColor;
                    Console.WriteLine("\tOrder added successfully.");
                    PressEnterToContinueToMain();
                }
                Console.Clear();
            }
        }

        private static void PromptEditOrder(Order oldOrder = null)
        {
            if (oldOrder == null) //prompt to choose order. They might have passed in an order from the promptdisplayorders method.
            {
                DateTime? date = PromptForValidOrderDate();
                if (date == null) //there are no orders in the system.
                {
                    return;
                }
                List<Order> potentialOrders = OrderOperations.GetOrdersByDate((DateTime) date);
                oldOrder = SelectFromOrders(potentialOrders);
            }

            Order orderToEdit = new Order()
            {
                Date = oldOrder.Date,
                Number = oldOrder.Number,
                CustomerName = oldOrder.CustomerName,
                State = oldOrder.State,
                TaxPercent = oldOrder.TaxPercent,
                ProductType = oldOrder.ProductType,
                Area = oldOrder.Area,
                CostPerSquareFoot = oldOrder.CostPerSquareFoot,
                LaborCostPerSquareFoot = oldOrder.LaborCostPerSquareFoot,
                MaterialCost = oldOrder.MaterialCost,
                LaborCost = oldOrder.LaborCost,
                Tax = oldOrder.Tax,
                Total = oldOrder.Total
            };

            orderToEdit = EditOrder(orderToEdit);
            //if Edit order returns null, they chose to cancel and go back to main menu
            if (orderToEdit != null)
            {
                OrderOperations.EditOrder(oldOrder, orderToEdit);

                DisplayOrder(orderToEdit, oldOrder);
                Console.ForegroundColor = SuccessColor;
                Console.WriteLine("\tOrder successfully changed.");
                PressEnterToContinueToMain();
            }
        }

        private static void PromptRemoveOrder(Order orderToRemove = null)
        {
            if (orderToRemove == null) //have them select an order. an order can be passed in from the promptdisplayorder method
            {
                DateTime? date = PromptForValidOrderDate();
                if (date == null) // there are no orders in the system
                {
                    return;
                }
                List<Order> potentialOrders = OrderOperations.GetOrdersByDate((DateTime) date);

                orderToRemove = SelectFromOrders(potentialOrders);
            }

            if (PromptConfirmOrder(orderToRemove, "Remove this order? ", ErrorColor))
            {
                OrderOperations.RemoveOrder(orderToRemove);
                Console.Clear();
                OffsetTop();
                Console.ForegroundColor = SuccessColor;
                Console.WriteLine("\tOrder Removed Sucessfully.");
                PressEnterToContinueToMain();
            }
            Console.Clear();
        }


        //order selection/display methods *********************************************************
        
        private static DateTime? PromptForValidOrderDate()
        {
            DateTime dateToReturn = DateTime.Today; //initialized value should never be used 
            DateTime dateToGenerateSuggestions = DateTime.Today;
            List<DateTime> closeValidDates;
            bool validInput = false;
            bool wasCorrectFormat = true;
            bool noOrdersFound = false;
            while (!validInput)
            {
                Console.Clear();
                OffsetTop();
                closeValidDates = DisplayCloseValidDates(dateToGenerateSuggestions);
                if (closeValidDates.Count == 0)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tThere are no orders in the system.");
                    Console.WriteLine("\tPlease add an order.");
                    Console.WriteLine();
                    Console.ForegroundColor = PromptColor;
                    Console.Write("\tPress enter to continue...");
                    Console.ResetColor();
                    Console.ReadLine();
                    Console.Clear();
                    return null;
                }
                Console.WriteLine("\t(Press enter for most recent valid date)");
                Console.WriteLine();

                if (!wasCorrectFormat)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tPlease format your date as MM/DD/YYYY\n");
                    Console.ResetColor();
                }
                wasCorrectFormat = true;

                if (noOrdersFound)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tNo orders were found for that date.");
                    Console.WriteLine("\tTry one of the dates above.");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                noOrdersFound = false;

                //prompt
                Console.ForegroundColor = PromptColor;
                Console.Write("\tWhat date is the order? ");

                //get user input
                Console.ForegroundColor = EmphasisColor;
                string userInput = Console.ReadLine().Trim();
                Console.ResetColor();
                int userNumberSelection = 0;

                if (String.IsNullOrEmpty(userInput))
                { //If input is empty then use most recent date.
                    validInput = true;
                    dateToReturn = OrderOperations.GetValidDates().Last();
                }
                else if (int.TryParse(userInput, out userNumberSelection) && userNumberSelection > 0 && userNumberSelection <= closeValidDates.Count)
                {
                    //they selected one of the displayed dates
                    dateToReturn = closeValidDates[userNumberSelection - 1];
                    validInput = true;
                }
                else
                {
                    wasCorrectFormat = DateTime.TryParse(userInput, out dateToReturn);

                    if (wasCorrectFormat)
                    {
                        if (OrderOperations.GetOrdersByDate(dateToReturn).Any())
                        {//orders were found and we got a valid date
                            validInput = true;
                        }
                        else
                        {
                            noOrdersFound = true;
                            dateToGenerateSuggestions = DateTime.Parse(userInput);
                        }
                    }
                }
            }
            Console.Clear();
            return dateToReturn;
        }

        private static List<DateTime> DisplayCloseValidDates(DateTime targetDate)
        {
            //Return four dates, two ahead and two behind the entered date.  If none ahead of the date return four behind.
            List<DateTime> validDates = OrderOperations.GetValidDates();
            var datesToDisplay = new List<DateTime>();
            if (validDates.Count() < 5)
            {
                datesToDisplay = validDates;
            }
            else
            {
                int indexToStart = 0;
                DateTime dateToStart = validDates.LastOrDefault(date => date <= targetDate);
                //TODO: Redo this IF branching statement smarter with math, floor, and ceiling.
                if (!(dateToStart == default(DateTime)))
                {
                    indexToStart = validDates.FindLastIndex(date => date <= targetDate);
                    if (indexToStart < 3)
                    {//targetDate is near the beginning of validDates, show first four valid dates.
                        indexToStart = 0;
                    }
                    else
                    {
                        if (indexToStart + 2 >= validDates.Count())
                        {//targetDate is near the end of Valid Dates, show the last four valid dates.
                            indexToStart = validDates.Count - 5;
                        }
                        else
                        {//targetDate has two dates before it and after it. (Or targetDate is valid and has one date after it.)
                            indexToStart -= 2;
                        }
                    }
                }//indexToStart is set.
                for (int i = indexToStart; i < indexToStart + 5; i++)
                {
                    datesToDisplay.Add(validDates[i]);
                }
            }
            Console.ForegroundColor = EmphasisColor;
            Console.WriteLine("\tSuggested Dates:");
            Console.ResetColor();

            for (int i = 0; i < datesToDisplay.Count; i++)
            {
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t  " + (i + 1) + ". ");
                Console.ResetColor();
                Console.WriteLine(datesToDisplay[i].ToString("d"));
            }

            return datesToDisplay;
        }

        private static Order SelectFromOrders(List<Order> potentialOrders)
        {
            Order selectedOrder = null;
            bool validInput = false;
            bool hadError = false;
            bool displayedList = false;
            while (!validInput)
            {
                if (!displayedList)
                {
                    OffsetTop();
                    Console.ForegroundColor = EmphasisColor;
                    Console.Write("\t" + potentialOrders[0].Date.ToString("MMMM dd, yyyy"));
                    Console.ResetColor();
                    Console.Write(" - {0} order", potentialOrders.Count);
                    if (potentialOrders.Count > 1)
                    {
                        Console.Write("s");
                    }
                    Console.WriteLine();
                    //Console.WriteLine();
                    Console.WriteLine("\t(Press enter to list all orders on this date)");
                    Console.WriteLine();
                }

                if (hadError)
                {
                    if (displayedList) //they got an error after they displayed the list. Let's put the list above the error message
                    {
                        DisplayOrderThumbnailsForDate(potentialOrders);
                    }

                    //error message
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tThat order number does not exist.");
                    Console.WriteLine();
                    Console.ResetColor();
                }

                //prompt for order number
                if (displayedList)
                {
                    Console.ForegroundColor = PromptColor;
                    Console.Write("\tEnter one of the order numbers above: ");
                    int cursorLeft = Console.CursorLeft;
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.SetCursorPosition(cursorLeft, Console.CursorTop - 3);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = PromptColor;
                    Console.Write("\tEnter the order number: ");
                    Console.ResetColor();
                }
                Console.ForegroundColor = EmphasisColor;
                string inputString = Console.ReadLine();
                Console.ResetColor();
                Console.Clear();
                Console.BufferHeight = 30; //reset after displaying thumbnails

                int orderNumberSelected;
                validInput = Int32.TryParse(inputString, out orderNumberSelected);

                if (string.IsNullOrEmpty(inputString)) //they want to see all the orders for that day
                {
                    DisplayOrderThumbnailsForDate(potentialOrders);
                    displayedList = true;
                    hadError = false;
                }
                else if (validInput) //they entered a valid int
                {
                    //check if it's an order number on the list
                    var selectedOrders = potentialOrders.Where(o => o.Number == orderNumberSelected).ToList();

                    if (selectedOrders.Count > 0)
                    {
                        selectedOrder = selectedOrders[0];
                    }
                    else //not on the list
                    {
                        hadError = true;
                        validInput = false;
                    }
                }
                else
                {
                    //They entered a non-empty string, but an invalid int.  We will loop and display error message.
                    hadError = true;
                }
            } //while(!validInput)
            return selectedOrder;
        }

        private static void DisplayOrderThumbnailsForDate(List<Order> potentialOrders)
        {
            OffsetTop();
            //display the date at the top of the list, then display thumbnails below it
            Console.ForegroundColor = EmphasisColor;
            Console.WriteLine("\tOrders for {0}", potentialOrders[0].Date.ToString("MMMM dd, yyyy"));
            Console.WriteLine();
            Console.ResetColor();

            if (potentialOrders.Count > 4)
            {
                Console.BufferHeight = (potentialOrders.Count * 5) + 7;
            }

            foreach (var order in potentialOrders)
            {
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t#{0}. ", order.Number);
                Console.ResetColor();
                Console.WriteLine("{0} - {1}", order.CustomerName, order.State);
                Console.WriteLine("\t    {0:n0} sqaure feet of {1}", order.Area, order.ProductType);
                Console.WriteLine("\t    Total: {0:C}", order.Total);
                Console.WriteLine("\t--------------------------------");
                Console.WriteLine();
            }
        }

        private static void DisplayOrder(Order orderToDisplay, Order oldOrder = null)
        {
            bool nameWasChanged = false;
            bool stateWasChanged = false;
            bool productWasChanged = false;
            bool areaWasChanged = false;
            bool wasEdit = oldOrder != null;
            if (wasEdit)
            {
                nameWasChanged = oldOrder.CustomerName != orderToDisplay.CustomerName;
                stateWasChanged = oldOrder.State != orderToDisplay.State;
                productWasChanged = oldOrder.ProductType != orderToDisplay.ProductType;
                areaWasChanged = oldOrder.Area != orderToDisplay.Area;
            }
            //display full order for confirmation purposes
            int fieldNameFormatter = 23;
            int fieldValueLength = 0;
            OffsetTop();
            Console.ForegroundColor = EmphasisColor;
            //Console.Write("\t" + orderToDisplay.Date.ToString("d") + ":");
            //Console.WriteLine("\t {0,10}", "Order #"+ orderToDisplay.Number);

            Console.Write("\t\t      " + orderToDisplay.Date.ToString("d") + ":");
            Console.WriteLine(" Order #" + orderToDisplay.Number);
            Console.WriteLine("\t--------------------------------------------");
            Console.ResetColor();
            
            if (wasEdit && nameWasChanged)
            {
                Console.ForegroundColor = SuccessColor;
            }
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + "}", "Customer Name", orderToDisplay.CustomerName);
            Console.ResetColor();
            if (wasEdit && stateWasChanged)
            {
                Console.ForegroundColor = SuccessColor;
            }
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + "}", "State", orderToDisplay.State);
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":P}", "Tax Percent", orderToDisplay.TaxPercent * 0.01M);
            Console.ResetColor();
            if (wasEdit && productWasChanged)
            {
                Console.ForegroundColor = SuccessColor;
            }
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + "}", "Product Type", orderToDisplay.ProductType);
            Console.ResetColor();
            if (wasEdit && areaWasChanged)
            {
                Console.ForegroundColor = SuccessColor;
            }
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":n0}" + " sq feet", "Area", orderToDisplay.Area);
            Console.ResetColor();
            if (wasEdit && productWasChanged)
            {
                Console.ForegroundColor = SuccessColor;
            }
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":C}", "Cost per square foot", orderToDisplay.CostPerSquareFoot);
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":C}", "Labor cost per sq ft", orderToDisplay.LaborCostPerSquareFoot);
            if (wasEdit && areaWasChanged) //if product was changed, color is already green
            {
                Console.ForegroundColor = SuccessColor;
            }
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":C}", "Material Cost", orderToDisplay.MaterialCost);
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":C}", "Labor Cost", orderToDisplay.LaborCost);
            if (wasEdit && stateWasChanged)//if product or area was changed, color is already green
            {
                Console.ForegroundColor = SuccessColor;
            }
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":C}", "Tax", orderToDisplay.Tax);
            Console.WriteLine("\t{0," + fieldNameFormatter + "}: {1," + fieldValueLength + ":C}", "Total", orderToDisplay.Total);
            Console.ResetColor();
            Console.ForegroundColor = EmphasisColor;
            Console.WriteLine("\t--------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();
        }


        //helper methods **************************************************************************

        private static bool PromptConfirmOrder(Order orderToConfirm, string prompt, ConsoleColor promptColor = PromptColor)
        {
            bool validInput = false;
            bool confirmed = false; //Initialized value should never be returned.
            bool hadError = false;
            while (!validInput)
            {
                DisplayOrder(orderToConfirm);
                if (hadError)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tPlease type Y or N.\n");
                    Console.ResetColor();
                }
                Console.ForegroundColor = promptColor;
                Console.Write("\t" + prompt);
                Console.ForegroundColor = EmphasisColor;
                string inputString = Console.ReadLine().ToUpper().Trim();
                Console.Clear();
                Console.ResetColor();

                if (!String.IsNullOrEmpty(inputString))
                {
                    switch (inputString[0])
                    {
                        case 'Y':
                            confirmed = true;
                            validInput = true;
                            break;
                        case 'N':
                            confirmed = false;
                            validInput = true;
                            break;
                        default:
                            //validInput is false
                            hadError = true;
                            break;
                    } //switch
                }
                else //input is empty.
                {
                    hadError = true;
                }

            } //while not validInput
            return confirmed;
        }

        private static Order EditOrder(Order orderToEdit)
        {
            bool doneEditing = false;
            bool hadError = false;
            int fieldChanged = 0;
            while (!doneEditing)
            {
                //display order info with edit options, coloring items that have been changed
                OffsetTop();
                int fieldNameFormatter = -18;
                int fieldValueLength = 0;
                Console.ForegroundColor = EmphasisColor;
                Console.WriteLine("\tEdit Order");
                Console.WriteLine("\t-----------------------------------");
                Console.ResetColor();
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + "}", "Order Date:", orderToEdit.Date.ToString("d"));
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + "}", "Order #:", orderToEdit.Number);
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t1. ");
                Console.ResetColor();
                if (fieldChanged == 1)
                {
                    Console.ForegroundColor = SuccessColor;
                }
                Console.WriteLine("{0," + fieldNameFormatter + "}{1," + fieldValueLength + "}", "Customer Name:", orderToEdit.CustomerName);
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t2. ");
                Console.ResetColor();
                if (fieldChanged == 2)
                {
                    Console.ForegroundColor = SuccessColor;
                }
                Console.WriteLine("{0," + fieldNameFormatter + "}{1," + fieldValueLength + "}", "State:", orderToEdit.State);
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + ":P}", "Tax Percent:", orderToEdit.TaxPercent * 0.01M);
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t3. ");
                Console.ResetColor();
                if (fieldChanged == 3)
                {
                    Console.ForegroundColor = SuccessColor;
                }
                Console.WriteLine("{0," + fieldNameFormatter + "}{1," + fieldValueLength + "}", "Product Type:", orderToEdit.ProductType);
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t4. ");
                Console.ResetColor();
                if (fieldChanged == 4)
                {
                    Console.ForegroundColor = SuccessColor;
                }
                Console.WriteLine("{0," + fieldNameFormatter + "}{1," + fieldValueLength + ":n0}" + " sq feet", "Area:", orderToEdit.Area);
                Console.ResetColor();
                if (fieldChanged == 3)
                {
                    Console.ForegroundColor = SuccessColor;
                }
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + ":C}", "Cost per sq ft:", orderToEdit.CostPerSquareFoot);
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + ":C}", "Labor cost/sq ft:", orderToEdit.LaborCostPerSquareFoot);
                if (fieldChanged == 4)
                {
                    Console.ForegroundColor = SuccessColor;
                }
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + ":C}", "Material Cost:", orderToEdit.MaterialCost);
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + ":C}", "Labor Cost:", orderToEdit.LaborCost);
                if (fieldChanged == 2)
                {
                    Console.ForegroundColor = SuccessColor;
                }
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + ":C}", "Tax:", orderToEdit.Tax);
                Console.WriteLine("\t" + "   {0," + fieldNameFormatter + "}{1," + fieldValueLength + ":C}", "Total:", orderToEdit.Total);
                Console.ResetColor();
                Console.ForegroundColor = EmphasisColor;
                Console.WriteLine("\t-----------------------------------");

                //display confirm/cancel options
                Console.ForegroundColor = EmphasisColor;
                Console.Write("\t5. ");
                Console.WriteLine("Confirm edit");
                Console.Write("\t6. ");
                Console.WriteLine("Cancel and return to menu");
                Console.ResetColor();
                Console.WriteLine();

                //display error message
                if (hadError)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tInvalid entry.");
                    Console.WriteLine();
                    Console.ResetColor();
                }

                // get user input
                Console.ForegroundColor = PromptColor;
                Console.Write("\tEnter an option: ");
                Console.ForegroundColor = EmphasisColor;
                string userInput = Console.ReadLine().Trim();
                Console.ResetColor();

                switch (userInput)
                {
                    case "1":
                    case "1.":
                        //enter new name
                        Console.Clear();
                        string newName = "";
                        while (string.IsNullOrEmpty(newName))
                        {
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.ForegroundColor = PromptColor;
                            Console.Write("\tEnter the new customer name: ");
                            Console.ForegroundColor = EmphasisColor;
                            newName = Console.ReadLine();
                            Console.ResetColor();
                            Console.Clear();
                        }
                        orderToEdit.CustomerName = newName;
                        fieldChanged = 1;
                        hadError = false;
                        break;
                    case "2":
                    case "2.":
                        //enter new state
                        Console.Clear();
                        orderToEdit.State = PromptForValidState("Enter the new state: ");
                        orderToEdit = OrderOperations.CalculateRemainingOrderFields(orderToEdit, false);
                        Console.Clear();
                        fieldChanged = 2;
                        hadError = false;
                        break;
                    case "3":
                    case "3.":
                        //enter new product type
                        Console.Clear();
                        orderToEdit.ProductType = PromptForValidProductType("Enter the new product type: ").Type;
                        orderToEdit = OrderOperations.CalculateRemainingOrderFields(orderToEdit, false);
                        Console.Clear();
                        fieldChanged = 3;
                        hadError = false;
                        break;
                    case "4":
                    case "4.":
                        //enter new area
                        Console.Clear();
                        ProductType currentProductType = ProductTypeOperations.GetProductType(orderToEdit.ProductType);
                        orderToEdit.Area = PromptForValidArea(currentProductType);
                        orderToEdit = OrderOperations.CalculateRemainingOrderFields(orderToEdit, false);
                        Console.Clear();
                        fieldChanged = 4;
                        hadError = false;
                        break;
                    case "5":
                    case "5.":
                        //confirm order
                        Console.Clear();
                        doneEditing = true;
                        break;
                    case "6":
                    case "6.":
                        //cancel and return to main menu
                        Console.Clear();
                        return null;
                    default:
                        //invalid input
                        Console.Clear();
                        hadError = true;
                        break;
                }
            }

            return orderToEdit;
        }

        private static void OffsetTop()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void PressEnterToContinueToMain()
        {
            Console.WriteLine();
            Console.ForegroundColor = PromptColor;
            Console.Write("\tPress enter to return to Main Menu...");
            Console.ResetColor();
            Console.ReadLine();
            Console.Clear();
        }


        //methods to prompt for certain input *****************************************************

        private static decimal PromptForValidArea(ProductType currentProductType, bool extraInfo = false)
        {
            decimal area = 0M;
            bool error = false;
            bool validArea = false;
            while (!validArea)
            {
                if (!extraInfo || error)
                {
                    OffsetTop();
                }

                if (error)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tInvalid entry.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                Console.ForegroundColor = PromptColor;
                Console.Write("\tEnter the area of {0} (in sq ft): ", currentProductType.Type.ToLower());
                Console.ForegroundColor = EmphasisColor;
                string userInput = Console.ReadLine().Trim();
                Console.Clear();
                Console.ResetColor();

                if (decimal.TryParse(userInput, out area) && area >= 0M)
                {
                    validArea = true;
                }
                else
                {
                    Console.Clear();
                    error = true;
                }
            }

            return area;
        }

        private static ProductType PromptForValidProductType(string prompt, bool extraInfo = false)
        {
            ProductType productType = null;
            List<ProductType> productTypes = ProductTypeOperations.GetAllProductTypes();
            string userInput = "";
            bool validProduct = false;
            bool error = false;
            bool displayedAllProducts = false;
            while (!validProduct)
            {
                if (!displayedAllProducts && (!extraInfo || error))
                {
                    OffsetTop();
                }

                //display 10 current products and give them the chance to see all of them
                if (!displayedAllProducts)
                {
                    DisplayProductTypes(false);
                }

                //if there was an error in the last time through the loop
                if (error)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tInvalid Product.");
                    Console.WriteLine();
                }

                //get the user input
                Console.ForegroundColor = PromptColor;
                Console.Write("\t" + prompt);
                Console.ForegroundColor = EmphasisColor;
                userInput = Console.ReadLine().Trim();
                Console.ResetColor();
                Console.Clear();

                if (userInput == "") //they want to display all product types
                {
                    OffsetTop();
                    DisplayProductTypes(true);
                    displayedAllProducts = true;
                }
                else if (ProductTypeOperations.IsProductType(userInput))
                {
                    productType = ProductTypeOperations.GetProductType(userInput);
                    Console.Clear();

                    if (extraInfo)
                    {
                        OffsetTop();
                        Console.WriteLine("\t{0} cost per square foot: {1:C}", productType.Type, productType.CostPerSquareFoot);
                        Console.WriteLine("\t{0} labor cost per square foot: {1:C}", productType.Type, productType.LaborCostPerSquareFoot);
                        Console.WriteLine("\t----------------------------------------");
                        Console.WriteLine();
                    }

                    validProduct = true;
                }
                else
                {
                    error = true;
                    if (displayedAllProducts) //the console was just cleared, so we don't want the error list to go away
                    {
                        OffsetTop();
                        DisplayProductTypes(true);
                    }
                }
            }
            return productType;
        }

        private static void DisplayProductTypes(bool displayAllProducts)
        {
            List<ProductType> productTypes = ProductTypeOperations.GetAllProductTypes();

            const int prodNameWidth = -12;
            const int prodCostWidth = -15;
            const int prodLaborWidth = -10;

            //Console.ForegroundColor = EmphasisColor;
            Console.WriteLine("\t\t    CURRENT PRODUCTS");
            Console.WriteLine();
            Console.ResetColor();
            Console.Write("\t{0," + prodNameWidth + "}", "Name");
            Console.WriteLine("{0," + prodCostWidth + "}{1," + prodLaborWidth + "}", "Cost/Sq Ft", "Labor Cost/Sq Ft");

            if (displayAllProducts)
            {
                for (int i = 0; i < productTypes.Count; i++)
                {
                    Console.ForegroundColor = EmphasisColor;
                    Console.Write("\t{0," + prodNameWidth + "}", productTypes[i].Type);
                    Console.ResetColor();
                    Console.WriteLine("  {0," + prodCostWidth + ":C}     {1," + prodLaborWidth + ":C}", productTypes[i].CostPerSquareFoot, productTypes[i].LaborCostPerSquareFoot);
                }
            }
            else //They should only see the first 10, with the option to see the rest
            {
                for (int i = 0; i <= 10 && i < productTypes.Count; i++)
                {
                    Console.ForegroundColor = EmphasisColor;
                    Console.Write("\t{0," + prodNameWidth + "}", productTypes[i].Type);
                    Console.ResetColor();
                    Console.WriteLine("  {0," + (prodCostWidth +2) + ":C}     {1," + prodLaborWidth + ":C}", productTypes[i].CostPerSquareFoot, productTypes[i].LaborCostPerSquareFoot);

                }

                //now we've displayed up to 10 products. If there are more. . . 
                if (productTypes.Count > 10)
                {
                    Console.WriteLine("\tContinued... (Press enter to display all current product types)");
                }
            }
            Console.WriteLine();
        }

        private static string PromptForValidState(string prompt, bool extraInfo = false)
        {
            string state = "";
            bool validState = false;
            bool error = false;
            while (!validState)
            {
                OffsetTop();
                DisplayValidStates();

                if (error)
                {
                    Console.ForegroundColor = ErrorColor;
                    Console.WriteLine("\tInvalid State.");
                    Console.WriteLine();
                }

                Console.ForegroundColor = PromptColor;
                Console.Write("\t" + prompt);
                Console.ForegroundColor = EmphasisColor;
                state = Console.ReadLine().Trim().ToUpper();
                Console.Clear();
                Console.ResetColor();

                if (TaxRateOperations.IsAllowedState(state))
                {
                    if (extraInfo)
                    {
                        TaxRate rate = TaxRateOperations.GetTaxRateFor(state);

                        OffsetTop();
                        Console.WriteLine("\tThe tax rate for {0} is {1:p}", rate.State, rate.TaxPercent * 0.01M);
                        Console.WriteLine("\t----------------------------------------");
                        Console.WriteLine();
                    }

                    validState = true;
                }
                else
                {
                    error = true;
                }
            }

            return state;
        }

        private static void DisplayValidStates()
        {
            List<TaxRate> taxRates = TaxRateOperations.GetTaxRates();
            taxRates = taxRates.OrderBy(tr => tr.State).ToList();

            
            Console.WriteLine("\t\t      Valid States");
            Console.ForegroundColor = EmphasisColor;
            foreach (var rate in taxRates)
            {
                Console.WriteLine("\t\t       " + "    " + rate.State);
            }
            Console.WriteLine();
            //TODO: display in columns based on how many states there are
        }
    }
}
