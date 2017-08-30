using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace U_R_DRINK_orderStockControl
{
    class Program
    {
        //Global variables

        static string[] products = new string[0]; //holds products
        static int[] stockLevels = new int[0]; //holds stock levels
        static double[] price = new double[0]; //holds prices

        static int[] orderLine_Products; //holds ordered products
        static int[] orderLine_Quant; //holds ordered quantities

        //customer details

        static string customerName = "";
        static string customerAddress_Line1 = "";
        static string customerAddress_Line2 = "";
        static string customerPostcode = "";

        //line order variables

        static int orderNo;
        static double orderTotal = 0.00;
        static double lineTotal = 0.00;
        static int lowStockWarning = 5;

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("U R DRINK Order Processing Program");
            Thread.Sleep(2000);

            readFiles(); //load product data from file into arrays
            mainMenu(); //load main menu
            writeFiles(); //[when exiting program] save changes to products and orders

        }

        static void mainMenu()
        {
            ConsoleKeyInfo menuOption; //menu option

            do {
                Console.Clear();

                Console.WriteLine("--------------------");
                Console.WriteLine("\n[1] Manage products file");
                Console.WriteLine("[2] Place an order");
                Console.WriteLine("[3] Help");
                Console.WriteLine("\n[x] Exit program");
                menuOption = Console.ReadKey();

                switch (menuOption.KeyChar)
                {
                    case '1':
                        productMenu(); //procedure for managing products
                        break;

                    case '2':
                        orderInput(); //procedure for placing orders and printing invoices
                        break;

                    case '3':
                        help(); //procedure for explaining how to use the program
                        break;
                    
                    case 'x': //exit menu loop
                        break;

                    default: //invalid menu option
                        Console.WriteLine("\"{0}\" not recognised; press ENTER and try again.");
                        Console.ReadLine();
                        break;
                }
            }
            while (menuOption.KeyChar != 'x');
        }

        static void help()
        {
            ConsoleKeyInfo menuOption;

            do
            {
                Console.Clear();
                Console.WriteLine("--------------------");
                Console.WriteLine("Help menu");
                Console.WriteLine("\nWith what do you need help?");


                Console.WriteLine("[1] Adding products");
                Console.WriteLine("[2] Amending products");
                Console.WriteLine("[3] Placing an order");
                Console.WriteLine("\n[x] Exit menu");
                menuOption = Console.ReadKey();

                switch (menuOption.KeyChar)
                {
                    case '1':
                        Console.Clear();
                        Console.WriteLine("Adding products:\n\nWhen creating products, you will enter the following details:" +
                            "\n\nProduct name;\nProduct stock level;\nProduct price (GBP)." +
                            "\n\nProducts will be displayed above the product menu options." +
                            "\n\nPress ENTER to return to the help menu.");
                        Console.ReadLine();
                        break;

                    case '2':
                        Console.Clear();
                        Console.WriteLine("Amending products:\n\nWhen amending products, you will re-enter a product's details. These" +
                            "\nchanges will be reflected in the display and the product text file." +
                            "\n\nPress ENTER to return to the help menu.");
                        Console.ReadLine();
                        break;

                    case '3':
                        Console.Clear();
                        Console.WriteLine("Placing an order:\n\nWhen placing orders, you will enter the following details:" + 
                            "\n\nCustomer name;\nAddress line 1;\nAddress line 2;\nPostcode;\nOrder number." + 
                            "\n\nYou will then see a list of products. Enter the product number to order\nthat product, and " + 
                            "enter \"x\" to end the order.\n\nYou will then see a complete invoice of the order, showing every" +
                            "\nproduct ordered, the quantity, the price, and the total price, along with\nthe customer's details." +
                            "\n\nPress ENTER to return to the help menu.");
                        Console.ReadLine();
                        break;

                    case 'x':
                        break;

                    default:
                        Console.WriteLine("Invalid option entered. Press ENTER and try again.");
                        Console.ReadLine();
                        break;
                }
            }
            while (menuOption.KeyChar != 'x');
        }

        static void productMenu()
        {
            ConsoleKeyInfo menuOption;

            do
            {
                displayProducts();
                Console.WriteLine("--------------------------");
                Console.WriteLine("\n[1] Add new product");
                Console.WriteLine("[2] Amend existing product");
                Console.WriteLine("\n[x] Return to main menu");
                menuOption = Console.ReadKey();

                switch (menuOption.KeyChar)
                {
                    case '1':
                        addNewProduct(); //procedure for adding new product
                        break;

                    case '2':
                        selectAndAmendProduct(); //procedure for amending a product
                        break;

                    case 'x': //exit menu loop
                        break;

                    default: //invalid menu option
                        Console.WriteLine("{0} not recognised; press ENTER and try again.", menuOption.KeyChar);
                        Console.ReadLine();
                        break;
                }
            }
            while (menuOption.KeyChar != 'x');
        }

        static void displayProducts() //displays products above product menu options
        {
            Console.Clear();
            Console.WriteLine("Current products:\n");

            for (int i = 0; i < products.Length; i++)
            {
                Console.WriteLine("Product > {0,3:N0} : {1,-30} : Price > {2,7:C2} : Stock > {3:N0}",
                    i, products[i], price[i], stockLevels[i]); //display each product in array
            }
        }

        static void extendProductArrays() //resize arrays to fit new order
        {
            int newArrayLength = products.Length + 1; //determine new array length

            Array.Resize(ref products, newArrayLength); //resize products array
            Array.Resize(ref stockLevels, newArrayLength); //resize stockLevels array
            Array.Resize(ref price, newArrayLength); //resize price array
        }

        static void addNewProduct()
        {
            extendProductArrays(); //resize arrays to accomodate for new order

            //default entries
            products[products.Length - 1] = "New product";
            stockLevels[products.Length - 1] = 0;
            price[products.Length - 1] = 0;

            amendProduct(products.Length - 1); //amend product details to user input
        }

        static void selectAndAmendProduct()
        {
            Console.Write("\nProduct number: ");
            string userInput = Console.ReadLine();

            if (!(String.IsNullOrEmpty(userInput)))
            {
                try
                {
                    int productID = int.Parse(userInput);
                    if (productID < 0 || productID >= products.Length)
                    {
                        Console.WriteLine("\nProduct ID out of range; press ENTER and try again.");
                        Console.ReadLine();
                    }
                    else
                    {
                        amendProduct(productID);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nInvalid product ID entry; press ENTER and try again.");
                    Console.ReadLine();
                }
            }
        }

        static void amendProduct(int productID)
        {
            Console.Write("Product {0:N0} : Name: {1, -20} \n? New product name: ",
                productID, products[productID]);
            string userInputProductDesc = Console.ReadLine();

            if (!(String.IsNullOrEmpty(userInputProductDesc)))
            {
                products[productID] = userInputProductDesc;
            }

            bool validData = true;

            do
            {
                Console.Write("Product {0:N0}\tCurrent stock level: {1:N0} --> ? New stock level: ",
                    productID, stockLevels[productID]);
                string userInputStockLevel = Console.ReadLine();

                if (!(String.IsNullOrEmpty(userInputStockLevel)))
                {
                    try
                    {
                        stockLevels[productID] = int.Parse(userInputStockLevel);
                        validData = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Invalid stock level entered for \"{0}\": value entered {1}",
                            productID, userInputStockLevel);
                        validData = false;
                    }
                }
                checkForLowStock(productID);
            }
            while (!validData);

            do
            {
                Console.Write("Product {0:N0}\tCurrent price: {1:C2} --> ? New price: £",
                    productID, price[productID]);
                string userInputPrice = Console.ReadLine();

                if (!(String.IsNullOrEmpty(userInputPrice)))
                {
                    try
                    {
                        price[productID] = double.Parse(userInputPrice);
                        validData = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Invalid price entered for \"{0}\": value entered {1}",
                            productID, userInputPrice);
                        validData = false;
                    }
                }
            }
            while (!validData);

            writeFiles();
        }

        static void checkForLowStock(int productID)
        {
            if (stockLevels[productID] < lowStockWarning)
            {
                Console.WriteLine("WARNING: Low stock for \"{0}\": {1}. Stock at {2}. Please re-order.\nPress ENTER to continue.",
                    productID, products[productID], stockLevels[productID]);
                Console.ReadLine();
            }
        }

        static void extendOrderArrays()
        {
            int newArrayLength = orderLine_Products.Length + 1;

            Array.Resize(ref orderLine_Products, newArrayLength);
            Array.Resize(ref orderLine_Quant, newArrayLength);
        }

        static int orderLineQuantInput(int productID)
        {
            bool validData = false;
            int inputQuant = 0;

            do
            {
                Console.Write("Enter quantity: ");
                string userInput = Console.ReadLine();

                if (!(String.IsNullOrEmpty(userInput)))
                {
                    try
                    {
                        inputQuant = int.Parse(userInput);
                        validData = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in entered quantity. Please enter a positive whole number.");
                        Debug.Print(e.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a positive whole number.");
                }
            }
            while (!validData);

            if (inputQuant > stockLevels[productID])
            {
                Console.WriteLine("Insufficient stock to cover order. Quantity reset to {0}. Press ENTER to continue.",
                    stockLevels[productID]);
                Console.ReadLine();
                inputQuant = stockLevels[productID];
            }

            stockLevels[productID] -= inputQuant;

            checkForLowStock(productID);

            writeFiles();

            return inputQuant;
        }

        static void displayOrderLines()
        {
            lineTotal = 0.00;
            orderTotal = 0.00;

            Console.WriteLine("\nItem product               Price (£)   Qty.    Total (£)");

            for (int i = 0; i < orderLine_Products.Length; i++)
            {
                lineTotal = Math.Round((price[orderLine_Products[i]] * orderLine_Quant[i]), 2);
                Console.WriteLine("{0,3}   {1,-23}  £{2,5:N2}  {3,5:N0}   {4,6:N2}",
                    i, products[orderLine_Products[i]], price[orderLine_Products[i]], orderLine_Quant[i], lineTotal);
                orderTotal += lineTotal;
            }
        }

        static void displayInvoice()
        {
            string companyName = "U R Drink";
            string companyAddress1 = "Lichfield Road";
            string companyAddress2 = "Sutton Coldfield";
            string companyPostcode = "B74 3NW";
            string companyPhoneNo = "0121 355 5671";

            Console.Clear();
            Console.WriteLine("INVOICE \tOrder no.: {0,-10}\t\t{1}", orderNo, companyName);
            Console.WriteLine("\t\t\t\t\t\n{0}", companyAddress1);
            Console.WriteLine("CUSTOMER \t{0,-20}\t\t\t{1}", customerName, companyAddress2);
            Console.WriteLine("\t\t{0,-20}\n\t\t{1,-20}\t\tTel: {2}", customerAddress_Line2, customerPostcode, companyPhoneNo);
            displayOrderLines();
            Console.WriteLine("\n\t\t\t     Invoice total: {0,10:C2}", orderTotal);
            Console.ReadLine();
        }

        static void writeOrderToFile()
        {
            try
            {
                string dataLine;

                StreamWriter file = new StreamWriter(@".\orders.txt", true);
                dataLine = String.Format("ORDER:,{0},{1},{2},{3},{4},{5}",
                    orderNo, orderTotal, customerName, customerAddress_Line1, customerAddress_Line2, customerPostcode);
                file.WriteLine(dataLine);

                double lineTotal = 0.00;

                for (int i = 0; i < orderLine_Products.Length; i++)
                {
                    lineTotal = Math.Round((price[orderLine_Products[i]] * orderLine_Quant[i]), 2);
                    dataLine = String.Format("{0},{1},{2},{3},{4}",
                        i, products[orderLine_Products[i]], price[orderLine_Products[i]], orderLine_Quant[i], lineTotal);

                    file.WriteLine(dataLine);
                }

                file.Flush();
                file.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error in writing order data for order number {0}.", orderNo);
                Debug.Print(e.ToString());
            }

        }

        static void readFiles()
        {
            //open file and reach each line into products and stock levels arrays
            //use a variable to hold the line of data

            string dataLine;
            string[] dataArray;

            //products file

            try
            {
                StreamReader file = new StreamReader(@".\products.txt"); //read products file

                //loop read data into array until the end of file

                while (!file.EndOfStream)
                {
                    dataLine = file.ReadLine(); //read record from file
                    dataArray = dataLine.Split(',');
                    //split record whenever a comma is found; [0] will be product description, [1] will be stock level

                    if (dataArray.Length != 3)
                    {
                        foreach (string dataSplit in dataArray)
                        {
                            Console.WriteLine("Error in loading data from file record no. {0}" + "\n{1}",
                                products.Length, dataSplit);
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        //extend both arrays to handle new record data
                        extendProductArrays();

                        int productID = products.Length - 1; //ID of newly added product
                        products[productID] = dataArray[0];

                        //update price array
                        try
                        {
                            price[productID] = double.Parse(dataArray[1]); //parse price data as double
                        }
                        catch (Exception e) //if price fails to read
                        {
                            Console.WriteLine("Invalid price for product {0}. Value entered: {1}; set to zero.",
                                productID, dataArray[1]);
                            price[productID] = 0.00; //set blank price
                        }

                        //update stock levels arry
                        try
                        {
                            stockLevels[productID] = int.Parse(dataArray[2]); //parse stock level as integer
                        }
                        catch (Exception e) //if stock level fails to read
                        {
                            Console.WriteLine("Invalid stock level for product {0}. Value entered: {1}; set to zero.",
                                productID, dataArray[2]);
                            stockLevels[productID] = 0; //set empty stock
                        }
                        checkForLowStock(productID);
                    }
                }
                file.Close();
            }
            
            catch (Exception e)
            {
                Console.WriteLine("Error in reading products file. Unable to import data.");
                Console.ReadLine();
            }
        }

        static void writeFiles()
        {
            try
            {
                string dataLine;

                StreamWriter file = new StreamWriter(@".\products.txt"); //read and write products to file

                for (int i = 0; i < products.Length; i++)
                {
                    //format output as comma-separated values
                    dataLine = String.Format("{0},{1},{2}", products[i], price[i], stockLevels[i]);

                    file.WriteLine(dataLine); //write each product in the array
                }
                file.Flush();
                file.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\nError in writing to products file. Data not saved.");
                Debug.Print(e.ToString());
                Console.ReadLine();
            }
        }

        static void orderInput()
        {
            //enter customer details
            enterNameAndAddress();
            //enter order lines
            enterOrderLines();
            //display invoice
            displayInvoice();
            //write order to file
            writeOrderToFile();
        }

        static void enterNameAndAddress()
        {
            Console.Write("Customer name: ");
            customerName = Console.ReadLine();
            Console.Write("Customer address line 1: ");
            customerAddress_Line1 = Console.ReadLine();
            Console.Write("Customer address line 2: ");
            customerAddress_Line2 = Console.ReadLine();
            Console.Write("Customer postcode: ");
            customerPostcode = Console.ReadLine();
            bool validData = true;
            do
            {
                try
                {
                    Console.Write("Order number: ");
                    orderNo = int.Parse(Console.ReadLine());
                    validData = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nINVALID ORDER NUMBER. Press ENTER and try again.");
                    validData = false;
                    Console.ReadLine();
                }
            }
            while (validData != true);
        }

        static void enterOrderLines()
        {
            //initialise as new array
            orderLine_Products = new int[0];
            orderLine_Quant = new int[0];
            string userInput = "";

            do
            {
                displayProducts();
                Console.WriteLine("------------------------------");
                Console.WriteLine("Products ordered so far:");
                displayOrderLines();
                Console.WriteLine("------------------------------");

                //prompt product number entry
                Console.Write("Enter product number, or press X to complete the order. ");
                userInput = Console.ReadLine();

                if(userInput != "X" && userInput != "x")
                {
                    //if string is not null then select product
                    if (!(string.IsNullOrEmpty(userInput)))
                    {
                        try
                        {
                            int productID = int.Parse(userInput);
                            if (productID < 0 || productID >= products.Length)
                            {
                                Console.WriteLine("Product ID out of range. Try again.");
                                Console.ReadLine();
                            }
                            else
                            {
                                //order selection
                                extendOrderArrays();
                                orderLine_Products[orderLine_Products.Length - 1] = productID;

                                //enter valid quantity
                                orderLine_Quant[orderLine_Products.Length - 1] = orderLineQuantInput(productID);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Invalid product entry. Try again.");
                            Console.ReadLine();
                        }
                    }
                }
               
            }
            while (userInput != "X" && userInput != "x");
        }
    }
}
   


