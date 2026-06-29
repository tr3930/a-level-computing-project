using Microsoft.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Packaging;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static ZXing.QrCode.Internal.Mode;
using System.Net.Mail;
using System.Security.Policy;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace CompSciProjectFinal
{
    internal class DatabaseManagementV2 //This is the class where all database access procedures are stored. The V2 suffix is due to this being an iteration on the design used in
        //my prototype. During development of the final version, the two systems existed in conjunction with each other for a period of time.
    {
        static SqliteConnection connection = new SqliteConnection("Data Source=" + ProgramConfigurationManagement.GetDataPath() + "/database.db"); //Set the connection string

        private static DataTable SelectQueryReturnDataTable(SqliteCommand command) //Generic code which will be used for all select statements
        {
            connection.Open(); //Opens DB connection
            command.Connection = connection; //Adds database connection to the command
            DataTable returnedData = new DataTable(); //Datatable to store the returned data
            returnedData.Load(command.ExecuteReader()); //Fill the datatable
            connection.Close(); //Close connection
            return returnedData; //Return datatable
        }

        private static void NonQueryReturnVoid(SqliteCommand command) //will be used by functions not inserting queries. returns nothing
        {
            connection.Open();
            command.Connection = connection;
            command.ExecuteNonQuery(); //Executes the command
            connection.Close();
        }

        public static UserDetails GetUserDetailsByTextId(string textId) //Used for getting information on a user from the database. Will gather id, text id, name and admin access rights
        {
            SqliteCommand command = new SqliteCommand("SELECT user_id, text_user_id, display_name, admin_access FROM Users WHERE text_user_id = @TextId;"); //Defines command text
            command.Parameters.Add(new SqliteParameter("TextId", textId)); //Adds text id parameter to command
            DataTable returnedData = SelectQueryReturnDataTable(command);
            UserDetails user = new UserDetails()
            {
                user_id = (int)(long)returnedData.Rows[0].ItemArray[0], //Getting details from the datatable
                text_user_id = returnedData.Rows[0].ItemArray[1].ToString(),
                display_name = returnedData.Rows[0].ItemArray[2].ToString(),
                admin_access = (int)(long)returnedData.Rows[0].ItemArray[3]
            }; //Defines a new UserDetails class in which the user's information will be returned
            return user;
        }

        public static UserDetails GetUserDetailsByUserId(int userId) //Used for getting information on a user from the database. Will gather id, text id, name and admin access rights
        {
            SqliteCommand command = new SqliteCommand("SELECT user_id, text_user_id, display_name, admin_access FROM Users WHERE user_id = @UserId;"); //Defines command text
            command.Parameters.Add(new SqliteParameter("UserId", userId)); //Adds text id parameter to command
            DataTable returnedData = SelectQueryReturnDataTable(command);
            UserDetails user = new UserDetails()
            {
                user_id = (int)(long)returnedData.Rows[0].ItemArray[0], //Getting details from the datatable
                text_user_id = returnedData.Rows[0].ItemArray[1].ToString(),
                display_name = returnedData.Rows[0].ItemArray[2].ToString(),
                admin_access = (int)(long)returnedData.Rows[0].ItemArray[3]
            }; //Defines a new UserDetails class in which the user's information will be returned
            return user;
        }

        public static PasswordDetails GetUsersPasswordDetailsByTextId(string textId) //Gets the user's password details for login
        {
            SqliteCommand command = new SqliteCommand("SELECT password, salt FROM Users WHERE text_user_id = @TextId;"); //Command to get password hash and salt
            command.Parameters.Add(new SqliteParameter("TextId", textId)); //Adds parameters to cmd
            DataTable returnedData = SelectQueryReturnDataTable(command); //gets data
            PasswordDetails details = new PasswordDetails() //New password details class
            {
                hash = returnedData.Rows[0].ItemArray[0].ToString(), //gets from datatable
                salt = returnedData.Rows[0].ItemArray[1].ToString()
            };
            return details; //Returns password details
        }

        public static bool VerifyExistenceOfUserByTextId(string textId) //verifies user exists in database
        {
            SqliteCommand command = new SqliteCommand("SELECT count(1) FROM Users WHERE text_user_id = @TextId;");
            command.Parameters.Add(new SqliteParameter("TextId", textId)); //Adds parameters to cmd
            if ((int)(long)SelectQueryReturnDataTable(command).Rows[0].ItemArray[0] == 1) //If there is a row, return 1, else return 0
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<ProductDetails> GetAllProductInformation() //Gets all product information
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM Products");
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<ProductDetails> products = new List<ProductDetails>();
            for (int i = 0; i < returnedData.Rows.Count; i++) //iterates through the datatable and converts each row to a "ProductDetails" oject
            {
                products.Add(new ProductDetails()
                {
                    productId = returnedData.Rows[i].ItemArray[0].ToString(),
                    productName = returnedData.Rows[i].ItemArray[1].ToString(),
                    productCost = (double)returnedData.Rows[i].ItemArray[2]
                });
            }
            return products;
        }

        public static List<ProductTransaction> GetAllProductTransactions() //Gets all product transactions
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM ProductTransactions");
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<ProductTransaction> productTransactions = new List<ProductTransaction>();
            for (int i = 0; i < returnedData.Rows.Count; i++) //converts datatable to product transaction list
            {
                productTransactions.Add(new ProductTransaction()
                {
                    transactionId = (long)returnedData.Rows[i].ItemArray[0],
                    productId = returnedData.Rows[i].ItemArray[1].ToString(),
                    changeAmount = (int)(long)returnedData.Rows[i].ItemArray[2],
                    confirmed = true
                });
                if ((int)(long)returnedData.Rows[i].ItemArray[3] == 0)
                {
                    productTransactions[i].confirmed = false;
                }
            }
            return productTransactions;
        }

        public static List<MaterialDetails> GetAllMaterialInformation() //same as above but used for materials
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM Materials");
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<MaterialDetails> materialDetails = new List<MaterialDetails>();
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                materialDetails.Add(new MaterialDetails()
                {
                    materialId = returnedData.Rows[i].ItemArray[0].ToString(),
                    materialName = returnedData.Rows[i].ItemArray[1].ToString()
                });
            }
            return materialDetails;
        }

        public static ProductDetails GetProductDetailsByProductId(string productId) //gets the details for the product from the database using that product's id
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM Products WHERE product_id = @ProductId");
            command.Parameters.Add(new SqliteParameter("ProductId", productId));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            return new ProductDetails() //Puts results from datatable into a ProductDetails class to be read by the user
            {
                productId = returnedData.Rows[0].ItemArray[0].ToString(),
                productName = returnedData.Rows[0].ItemArray[1].ToString(),
                productCost = (double)returnedData.Rows[0].ItemArray[2]
            };
        }

        public static ProductTransactionData GetTransactionHistoryForProductById(string productId) //Gets every product transaction in the database and returns it as a nice list
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM ProductTransactions WHERE product_id = @ProductId;"); //command
            command.Parameters.Add(new SqliteParameter("ProductId", productId)); //parameters
            DataTable returnedData = SelectQueryReturnDataTable(command); //get datatable with all items
            ProductTransactionData productTransactionData = new ProductTransactionData()
            {
                productTransactions = new List<ProductTransaction>(),
                totalStock = 0,
                totalSellableStock = 0
            };
            
            for (int i = 0; i < returnedData.Rows.Count; i++) //goes through each row in the datatable and adds the details to the list of product transactions
            {
                productTransactionData.productTransactions.Add(new ProductTransaction() //adding details
                {
                    productId = returnedData.Rows[i].ItemArray[1].ToString(),
                    time = DataFunctions.FindPrimaryKeyTime((long)returnedData.Rows[i].ItemArray[0]), //Convert primary key to datetime
                    changeAmount = (int)(long)returnedData.Rows[i].ItemArray[2]
                });
                if ((int)(long)returnedData.Rows[i].ItemArray[3] == 1) //Since it wouldn't let me use a bool in the db, i'll convert it here for use by the program
                {
                    productTransactionData.productTransactions[i].confirmed = true;
                    productTransactionData.totalSellableStock += (int)(long)returnedData.Rows[i].ItemArray[2];
                    productTransactionData.totalStock += (int)(long)returnedData.Rows[i].ItemArray[2];
                }
                else
                {
                    productTransactionData.productTransactions[i].confirmed = false;
                    productTransactionData.totalSellableStock += (int)(long)returnedData.Rows[i].ItemArray[2];
                }
            }
            return productTransactionData;
        }

        public static List<MaterialTransaction> GetAllMaterialTransactions() //Gets a list of every material transaction
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM MaterialTransactions");
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<MaterialTransaction> materialTransactions = new List<MaterialTransaction>();
            for (int i = 0; i < returnedData.Rows.Count; i++) //Same iteration conversion as above
            {
                materialTransactions.Add(new MaterialTransaction()
                {
                    materialId = returnedData.Rows[i].ItemArray[1].ToString(),
                    amount = (int)(long)returnedData.Rows[i].ItemArray[2],
                    transactionId = (long)returnedData.Rows[i].ItemArray[0]
                });
            }
            return materialTransactions;
        }


        public static long InsertNewFinancialTransaction(FinancialTransaction transaction) //Insert a new financial transaction into the database
        {
            long financialTransactionPrimaryKey = DataFunctions.GenerateIdForPrimaryKey(); //Generate primary key for the transaction
            SqliteCommand command = new SqliteCommand("INSERT INTO FinancialTransactions (ft_id, change) VALUES (@PrimaryKey, @Amount);"); //Creates sql command string
            command.Parameters.Add(new SqliteParameter("PrimaryKey", financialTransactionPrimaryKey)); //Add parameters
            command.Parameters.Add(new SqliteParameter("Amount", transaction.amount));
            NonQueryReturnVoid(command); //Executes the command
            return financialTransactionPrimaryKey; //return the primary key for use by the program
        }

        public static void InsertNewProductTransaction(ProductTransaction transaction) //Insert a new product into the database
        {
            long productTransactionPrimaryKey = DataFunctions.GenerateIdForPrimaryKey(); //generate primary key
            SqliteCommand command = new SqliteCommand("INSERT INTO ProductTransactions (pt_id, product_id, change, status) VALUES (@TransactionId, @ProductId, @Change, @Status);"); //sql cmd
            command.Parameters.Add(new SqliteParameter("TransactionId", productTransactionPrimaryKey)); //Addn the parameters to the command
            command.Parameters.Add(new SqliteParameter("ProductId", transaction.productId));
            command.Parameters.Add(new SqliteParameter("Change", transaction.changeAmount));
            int status; //Converting the confirmed status from a bool to an int becuase sqlite didn't allow me to inout a bool
            if (transaction.confirmed)
            {
                status = 1;
            }
            else
            {
                status = 0;
            }
            command.Parameters.Add(new SqliteParameter("Status", status)); //add status parameter
            NonQueryReturnVoid(command); //Execute the command
        }

        public static List<FinancialTransaction> GetAllFinancialTransactions() //Gets every financial transaction from the database
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM FinancialTransactions ORDER BY ft_id DESC;");
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<FinancialTransaction> transactions = new List<FinancialTransaction>();
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                transactions.Add(new FinancialTransaction()
                {
                    time = DataFunctions.FindPrimaryKeyTime((long)returnedData.Rows[i].ItemArray[0]),
                    amount = (double)returnedData.Rows[i].ItemArray[1]
                });
            }
            return transactions;
        }

        public static void AddNewProduct(ProductDetails productDetails) //Function for adding a new product to the database
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO Products (product_id, display_name, cost) VALUES (@ProductId, @DisplayName, @Cost);");
            command.Parameters.Add(new SqliteParameter("ProductId", productDetails.productId)); //Insert parameters into command
            command.Parameters.Add(new SqliteParameter("DisplayName", productDetails.productName));
            command.Parameters.Add(new SqliteParameter("Cost", productDetails.productCost));
            NonQueryReturnVoid(command); //Execute command
        }

        public static void UpdateProductDetails(ProductDetails productDetails) //Function for adding a new product to the database
        {
            SqliteCommand command = new SqliteCommand("UPDATE Products SET display_name = @DisplayName, cost = @Cost WHERE product_id = @ProductId;");
            command.Parameters.Add(new SqliteParameter("ProductId", productDetails.productId)); //Insert parameters into command
            command.Parameters.Add(new SqliteParameter("DisplayName", productDetails.productName));
            command.Parameters.Add(new SqliteParameter("Cost", productDetails.productCost));
            NonQueryReturnVoid(command); //Execute command
        }

        public static List<OverviewSale> GetAllSalesForOverviewScreen(int status) //Gets every sale for the overview and puts it into a list
        {
            SqliteCommand command = new SqliteCommand("SELECT sale_id FROM Sales WHERE status = @Status");
            command.Parameters.Add(new SqliteParameter("Status", status));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<OverviewSale> sales = new List<OverviewSale>();
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                sales.Add(new OverviewSale()
                {
                    saleId = (long)returnedData.Rows[i].ItemArray[0],
                    saleTime = DataFunctions.FindPrimaryKeyTime((long)returnedData.Rows[i].ItemArray[0]).ToString("dd/MM/yyyy HH:mm")
                });
            };
            return sales;
        }

        public static CustomerDetails GetCustomerDetailsByCustomerId(int customerId) //Gets the details for a customer using their id
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM Customers WHERE customer_id = @CustomerId;");
            command.Parameters.Add(new SqliteParameter("CustomerId", customerId));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            CustomerDetails customerDetails = new CustomerDetails() //Put all data into a customerdetails object
            {
                customerId = (int)(long)returnedData.Rows[0].ItemArray[0],
                customerFirstName = returnedData.Rows[0].ItemArray[1].ToString(),
                customerLastName = returnedData.Rows[0].ItemArray[2].ToString(),
                customerAddress1 = returnedData.Rows[0].ItemArray[4].ToString(),
                customerAddress2 = returnedData.Rows[0].ItemArray[5].ToString(),
                customerAddress3 = returnedData.Rows[0].ItemArray[6].ToString(),
                customerPostcode = returnedData.Rows[0].ItemArray[7].ToString(),
                customerEmail = returnedData.Rows[0].ItemArray[3].ToString(),
                customerNotes = returnedData.Rows[0].ItemArray[8].ToString()
            };
            return customerDetails;
        }

        public static SaleDetailsV2 GetSaleDetailsBySaleId(long saleId) //Get all information related to a single sale
        {
            SaleDetailsV2 saleDetails = new SaleDetailsV2() //Defines a new saledetails object to be filled with sale data
            {
                saleId = saleId, //assigns the sale id
                prodTransSaleAsscs = new List<ProdTransSaleAssc>(), //Creates a new list to store each product transaction
                price = 0 //New variable to store total cost of sale
            };
            SqliteCommand command = new SqliteCommand("SELECT user_id, customer_id, status, payment_link, method FROM Sales WHERE sale_id = @SaleId;"); //Command for use with db
            command.Parameters.Add(new SqliteParameter("SaleId", saleId)); //Add parameters
            DataTable returnedSaleInformation = SelectQueryReturnDataTable(command); //Execute command
            saleDetails.userDetails = GetUserDetailsByUserId((int)(long)returnedSaleInformation.Rows[0].ItemArray[0]); //Set user and customer details
            saleDetails.customerDetails = GetCustomerDetailsByCustomerId((int)(long)returnedSaleInformation.Rows[0].ItemArray[1]);
            saleDetails.status = (int)(long)returnedSaleInformation.Rows[0].ItemArray[2];
            saleDetails.method = (int)(long)returnedSaleInformation.Rows[0].ItemArray[4];
            saleDetails.paymentLink = returnedSaleInformation.Rows[0].ItemArray[3].ToString();
            command = new SqliteCommand("SELECT ProductTransactions.pt_id, ProductTransactions.product_id, ProductTransactions.change, ProductTransactions.status, ProductTransactionSalesAssociations.ft_id FROM ProductTransactionSalesAssociations INNER JOIN ProductTransactions ON ProductTransactions.pt_id = ProductTransactionSalesAssociations.pt_id WHERE ProductTransactionSalesAssociations.sale_id = @SaleId;"); //Very long sql command
            command.Parameters.Add(new SqliteParameter("SaleId", saleId)); //Get information on all product transactions
            DataTable returnedProdTransSalesAsscInformation = SelectQueryReturnDataTable(command); //Execute command
            for (int i = 0; i < returnedProdTransSalesAsscInformation.Rows.Count; i++) //Iterates through every product transaction - sales association
            {
                ProdTransSaleAssc prodTransSaleAssc = new ProdTransSaleAssc() //Converts the data table row to a ProdTransSaleAssc object
                {
                    productTransaction = new ProductTransaction()
                    {
                        transactionId = (long)returnedProdTransSalesAsscInformation.Rows[i].ItemArray[0],
                        productId = returnedProdTransSalesAsscInformation.Rows[i].ItemArray[1].ToString(),
                        changeAmount = (int)(long)returnedProdTransSalesAsscInformation.Rows[i].ItemArray[2],
                        time = DataFunctions.FindPrimaryKeyTime((long)returnedProdTransSalesAsscInformation.Rows[i].ItemArray[0])
                    }
                };
                if ((int)(long)returnedProdTransSalesAsscInformation.Rows[i].ItemArray[3] == 1) //COnverts the integer confirmed value from the db into a boolean
                {
                    prodTransSaleAssc.productTransaction.confirmed = true;
                }
                else
                {
                    prodTransSaleAssc.productTransaction.confirmed = false;
                }
                if (prodTransSaleAssc.productTransaction.confirmed) //If the asscoiation has a financial transaction associated with it, (if it is marked as confirmed) get the financial transaction data
                {
                    command = new SqliteCommand("SELECT * FROM FinancialTransactions WHERE ft_id = @FinancialTransId;");
                    command.Parameters.Add(new SqliteParameter("FinancialTransId", (long)returnedProdTransSalesAsscInformation.Rows[0].ItemArray[4]));
                    DataTable returnedFinancialTransInformation = SelectQueryReturnDataTable(command);
                    prodTransSaleAssc.financialTransaction = new FinancialTransaction()
                    {
                        amount = (double)returnedFinancialTransInformation.Rows[0].ItemArray[1],
                        time = DataFunctions.FindPrimaryKeyTime((long)returnedFinancialTransInformation.Rows[0].ItemArray[0])
                    };
                    saleDetails.price += prodTransSaleAssc.financialTransaction.amount; //update the proce within saledetails
                };
                saleDetails.prodTransSaleAsscs.Add(prodTransSaleAssc); //add the accociation to the list
            }
            return saleDetails; //return the sale details
        }

        public static void SetSaleStatus(long saleId, int status) //Change the status of a sale
        {
            SqliteCommand command = new SqliteCommand("UPDATE Sales SET status = @Status WHERE sale_id = @SaleId;");
            command.Parameters.Add(new SqliteParameter("Status", status));
            command.Parameters.Add(new SqliteParameter("SaleId", saleId));
            NonQueryReturnVoid(command);
        }

        public static void UpdateProductTransactionForSale(long productTransId, int amount) //Change the total amount of items for a product transactions
        {
            SqliteCommand command;
            if (amount == 0) //Delete the entry if the amount goes below zero
            {
                command = new SqliteCommand("DELETE FROM ProductTransactionSalesAssociations WHERE pt_id = @ProductTransId; DELETE FROM ProductTransactions WHERE pt_id = @ProductTransId;");
            }
            else
            {
                command = new SqliteCommand("UPDATE ProductTransactions SET change = @Change WHERE pt_id = @ProductTransId;");
                command.Parameters.Add(new SqliteParameter("Change", amount));
            }
            command.Parameters.Add(new SqliteParameter("ProductTransId", productTransId)); //Product transaction will be needed for both scenarios
            NonQueryReturnVoid(command);
        }

        public static void ConfirmSaleDetails(SaleDetailsV2 saleDetails) //Insert a new financial transaction for each product transaction and mark them as confirmed
        {
            for (int i = 0; i < saleDetails.prodTransSaleAsscs.Count; i++) //Do the database management for every entry
            {
                long financialTransId = DataFunctions.GenerateIdForPrimaryKey();
                SqliteCommand command = new SqliteCommand("INSERT INTO FinancialTransactions (ft_id, change) VALUES (@FinancialTransId, @Change); UPDATE ProductTransactionSalesAssociations SET ft_id = @FinancialTransId WHERE pt_id = @ProductTransId; UPDATE ProductTransactions SET status = 1 WHERE pt_id = @ProductTransId");
                command.Parameters.Add(new SqliteParameter("FinancialTransId", financialTransId));
                command.Parameters.Add(new SqliteParameter("ProductTransId", saleDetails.prodTransSaleAsscs[i].productTransaction.transactionId));
                command.Parameters.Add(new SqliteParameter("Change", saleDetails.prodTransSaleAsscs[i].productTransaction.changeAmount));
                NonQueryReturnVoid(command);
            }
        }

        public static void AddNewProductToSale(string productId, long saleId, int amount = -1) //Adding a new product to an existing sale
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO ProductTransactions (pt_id, product_id, change, status) VALUES (@PriKey, @ProductId, @Change, 0); INSERT INTO ProductTransactionSalesAssociations (association_id, pt_id, sale_id) VALUES (@PriKey, @PriKey, @SaleId);");
            command.Parameters.Add(new SqliteParameter("@PriKey", DataFunctions.GenerateIdForPrimaryKey()));//Primary key for entries. It's pointless to use a different one for each because they will both generate in less than 1ms which is the finest detail the keys go to
            command.Parameters.Add(new SqliteParameter("@ProductId", productId)); //Product id
            command.Parameters.Add(new SqliteParameter("@SaleId", saleId)); //Sale id the transaction pertains to
            command.Parameters.Add(new SqliteParameter("@Change", amount)); //Sale id the transaction pertains to
            NonQueryReturnVoid(command);
        }

        public static long CreateNewBlankSaleAndReturnId(UserDetails user) //Insert a new blank sale into the database. Returns the sale id
        {
            long saleId = DataFunctions.GenerateIdForPrimaryKey();
            SqliteCommand command = new SqliteCommand("INSERT INTO Sales (sale_id, user_id) VALUES (@SaleId, @UserId);");
            command.Parameters.Add(new SqliteParameter("SaleId", saleId));
            command.Parameters.Add(new SqliteParameter("UserId", user.user_id));
            NonQueryReturnVoid(command);
            return saleId;
        }

        public static List<CustomerDetails> GetAllCustomerDetails() //Get information on all customers
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM Customers;");
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<CustomerDetails> customers = new List<CustomerDetails>();
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                customers.Add(new CustomerDetails()
                {
                    customerId = (int)(long)returnedData.Rows[i].ItemArray[0],
                    customerFirstName = returnedData.Rows[i].ItemArray[1].ToString(),
                    customerLastName = returnedData.Rows[i].ItemArray[2].ToString(),
                    customerAddress1 = returnedData.Rows[i].ItemArray[4].ToString(),
                    customerAddress2 = returnedData.Rows[i].ItemArray[5].ToString(),
                    customerAddress3 = returnedData.Rows[i].ItemArray[6].ToString(),
                    customerPostcode = returnedData.Rows[i].ItemArray[7].ToString(),
                    customerEmail = returnedData.Rows[i].ItemArray[3].ToString()
                });
            }
            return customers;
        }

        public static void UpdateCustomerForSale(long saleId, int customerId) //Update customer details for a sale
        {
            SqliteCommand command = new SqliteCommand("UPDATE Sales SET customer_id = @CustomerId WHERE sale_id = @SaleId;");
            command.Parameters.Add(new SqliteParameter("CustomerId", customerId));
            command.Parameters.Add(new SqliteParameter("SaleId", saleId));
            NonQueryReturnVoid(command);
        }

        public static void UpdateSaleLink(long saleId, string saleLink) //Update customer details for a sale
        {
            SqliteCommand command = new SqliteCommand("UPDATE Sales SET payment_link = @PaymentLink WHERE sale_id = @SaleId;");
            command.Parameters.Add(new SqliteParameter("PaymentLink", saleLink));
            command.Parameters.Add(new SqliteParameter("SaleId", saleId));
            NonQueryReturnVoid(command);
        }

        public static void InsertCheckoutSale(SaleDetailsV2 saleDetails, UserDetails user) //Insert sale created by checkout into DB
        {
            long saleId = DataFunctions.GenerateIdForPrimaryKey(); //Generate sale id
            SqliteCommand command = new SqliteCommand("INSERT INTO Sales (sale_id, user_id, status, method) VALUES (@SaleId, @UserId, 3, 1);");
            command.Parameters.Add(new SqliteParameter("SaleId", saleId));
            command.Parameters.Add(new SqliteParameter("UserId", user.user_id));
            NonQueryReturnVoid(command);
            for (int i = 0; i < saleDetails.prodTransSaleAsscs.Count; i++) //Add each product transaction
            {
                AddNewProductToSale(saleDetails.prodTransSaleAsscs[i].productTransaction.productId, saleId, -saleDetails.prodTransSaleAsscs[i].productTransaction.changeAmount);
            }
            ConfirmSaleDetails(saleDetails); //Add the financial transactions
        }

        public static void InsertNewSession(long sessionid, UserDetails user) //Inserts a new session into the database
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO Sessions (session_id, user_id) VALUES (@SessionId, @UserId);"); //Doesn't insert end time because the sesson hasn't ended yet
            command.Parameters.Add(new SqliteParameter("@SessionId", sessionid));
            command.Parameters.Add(new SqliteParameter("@UserId", user.user_id));
            NonQueryReturnVoid(command);
        }

        public static void InsertNewSessionEvent(SessionEvent sessionEvent) //Insets a new event for a specified session
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO SessionEvents (event_id, session_id, comment) VALUES (@EventId, @SessionId, @Comment);");
            command.Parameters.Add(new SqliteParameter("EventId", sessionEvent.eventId));
            command.Parameters.Add(new SqliteParameter("SessionId", sessionEvent.session.sessionId));
            command.Parameters.Add(new SqliteParameter("Comment", sessionEvent.comment));
            NonQueryReturnVoid(command);
        }

        public static List<SessionEvent> GetAllEventsForSession(Session session) //Get every event for a certain session
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM SessionEvents WHERE session_id = @SessionId;");
            command.Parameters.Add(new SqliteParameter("SessionId", session.sessionId));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<SessionEvent> sessionEvents = new List<SessionEvent>();
            for (int i = 0; i < returnedData.Rows.Count; i++) //Convert datatable to a list of SessionEvents
            {
                sessionEvents.Add(new SessionEvent()
                {
                    eventId = (long)returnedData.Rows[i].ItemArray[0],
                    session = session,
                    comment = returnedData.Rows[i].ItemArray[2].ToString()
                });
            }
            return sessionEvents;
        }

        public static List<Session> GetAllSessionsForUser(UserDetails userDetails) //Gets all sessions for an individual user
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM Sessions WHERE user_id = @UserId;");
            command.Parameters.Add(new SqliteParameter("UserId", userDetails.user_id));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<Session> sessionList = new List<Session>();
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                sessionList.Add(new Session()
                {
                    sessionId = (long)returnedData.Rows[i].ItemArray[0],
                    userDetails = GetUserDetailsByUserId((int)(long)returnedData.Rows[i].ItemArray[1])
                });
            }
            return sessionList;
        }

        public static List<UserDetails> GetAllUserDetails() //Gets the details for every user in the database
        {
            SqliteCommand command = new SqliteCommand("SELECT user_id, text_user_id, display_name, admin_access FROM Users;");
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<UserDetails> userDetails = new List<UserDetails>();
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                userDetails.Add(new UserDetails()
                {
                    user_id = (int)(long)returnedData.Rows[i].ItemArray[0], //Getting details from the datatable
                    text_user_id = returnedData.Rows[i].ItemArray[1].ToString(),
                    display_name = returnedData.Rows[i].ItemArray[2].ToString(),
                    admin_access = (int)(long)returnedData.Rows[i].ItemArray[3]
                });
            }
            return userDetails;
        }

        public static MaterialDetails GetMaterialDetailsByMaterialId(string materialId) //Gets the details for a specified material
        {
            SqliteCommand command = new SqliteCommand("SELECT display_name FROM Materials WHERE material_id = @MaterialId;");
            command.Parameters.Add(new SqliteParameter("MaterialId", materialId));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            return new MaterialDetails()
            {
                materialId = materialId,
                materialName = returnedData.Rows[0].ItemArray[0].ToString()
            };
        }

        public static List<ProductRequirement> GetAllRequirementsForProduct(ProductDetails productDetails) //Retrieves all requiremnts for a given product
        {
            SqliteCommand command = new SqliteCommand("SELECT requirement_id, material_id FROM ProductRequirements WHERE product_id = @ProductId;");
            command.Parameters.Add(new SqliteParameter("ProductId", productDetails.productId));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            List<ProductRequirement> productRequirements = new List<ProductRequirement>();
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                productRequirements.Add(new ProductRequirement()
                {
                    productDetails = productDetails,
                    requirementId = (int)(long)returnedData.Rows[i].ItemArray[0],
                    materialDetails = GetMaterialDetailsByMaterialId(returnedData.Rows[i].ItemArray[1].ToString())
                });
            }
            return productRequirements;
        }

        public static void RemoveRequirementForProduct(ProductRequirement productRequirement) //Removes a product requirement form the database
        {
            SqliteCommand command = new SqliteCommand("DELETE FROM ProductRequirements WHERE requirement_id = @RequirementId");
            command.Parameters.Add(new SqliteParameter("RequirementId", productRequirement.requirementId));
            NonQueryReturnVoid(command);
        }

        public static void InsertRequirementForProduct(ProductDetails productDetails, string materialId) //Inserts a product requirement into the database
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO ProductRequirements (product_id, material_id) VALUES (@ProductId, @MaterialId);");
            command.Parameters.Add(new SqliteParameter("ProductId", productDetails.productId));
            command.Parameters.Add(new SqliteParameter("MaterialId", materialId));
            NonQueryReturnVoid(command);
        }

        public static void UpdateCustomerDetails(CustomerDetails customerDetails) //Update customer details
        {
            SqliteCommand command = new SqliteCommand("UPDATE Customers SET first_name = @FirstName, last_name = @LastName, email_address = @EmailAddress, address1 = @Address1, address2 = @Address2, address3 = @Address3, postcode = @Postcode, notes = @Notes WHERE customer_id = @CustomerId;");
            command.Parameters.Add(new SqliteParameter("FirstName", customerDetails.customerFirstName));
            command.Parameters.Add(new SqliteParameter("LastName", customerDetails.customerLastName));
            command.Parameters.Add(new SqliteParameter("EmailAddress", customerDetails.customerEmail));
            command.Parameters.Add(new SqliteParameter("Address1", customerDetails.customerAddress1));
            command.Parameters.Add(new SqliteParameter("Address2", customerDetails.customerAddress2));
            command.Parameters.Add(new SqliteParameter("Address3", customerDetails.customerAddress3));
            command.Parameters.Add(new SqliteParameter("Postcode", customerDetails.customerPostcode));
            command.Parameters.Add(new SqliteParameter("Notes", customerDetails.customerNotes));
            command.Parameters.Add(new SqliteParameter("CustomerId", customerDetails.customerId));
            NonQueryReturnVoid(command);
        }

        public static void InsertNewCustomer(CustomerDetails customerDetails) //Insert a new customer
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO Customers (first_name, last_name, email_address, address1, address2, address3, postcode, notes) VALUES (@FirstName, @LastName, @EmailAddress, @Address1, @Address2, @Address3, @Postcode, @Notes);");
            command.Parameters.Add(new SqliteParameter("@FirstName", customerDetails.customerFirstName));
            command.Parameters.Add(new SqliteParameter("@LastName", customerDetails.customerLastName));
            command.Parameters.Add(new SqliteParameter("@EmailAddress", customerDetails.customerEmail));
            command.Parameters.Add(new SqliteParameter("@Address1", customerDetails.customerAddress1));
            command.Parameters.Add(new SqliteParameter("@Address2", customerDetails.customerAddress2));
            command.Parameters.Add(new SqliteParameter("@Address3", customerDetails.customerAddress3));
            command.Parameters.Add(new SqliteParameter("@Postcode", customerDetails.customerPostcode));
            command.Parameters.Add(new SqliteParameter("@Notes", customerDetails.customerNotes));
            NonQueryReturnVoid(command);
        }

        public static void UpdateTextUserId(UserDetails userDetails) //Updates the text id for a specified user
        {
            SqliteCommand command = new SqliteCommand("UPDATE Users SET text_user_id = @TextUserId WHERE user_id = @UserId;");
            command.Parameters.Add(new SqliteParameter("TextUserId", userDetails.text_user_id));
            command.Parameters.Add(new SqliteParameter("UserId", userDetails.user_id));
            NonQueryReturnVoid(command);
        }

        public static void UpdateUserDisplayName(UserDetails userDetails) //Updates the display name for a specified user
        {
            SqliteCommand command = new SqliteCommand("UPDATE Users SET display_name = @DisplayName WHERE user_id = @UserId;");
            command.Parameters.Add(new SqliteParameter("DisplayName", userDetails.display_name));
            command.Parameters.Add(new SqliteParameter("UserId", userDetails.user_id));
            NonQueryReturnVoid(command);
        }

        public static DataTable GetEntireTable(int tableIndex) //Gets the data for an entire database table
        {
            SqliteCommand command = new SqliteCommand();
            if (tableIndex == 0) //Choose which table to get data on
            {
                command = new SqliteCommand("SELECT * FROM Users;");
            }
            else if (tableIndex == 1)
            {
                command = new SqliteCommand("SELECT * FROM Products;");
            }
            else if (tableIndex == 2)
            {
                command = new SqliteCommand("SELECT * FROM Materials;");
            }
            else if (tableIndex == 3)
            {
                command = new SqliteCommand("SELECT * FROM Customers;");
            }
            else if (tableIndex == 4)
            {
                command = new SqliteCommand("SELECT * FROM Sales;");
            }
            else if (tableIndex == 5)
            {
                command = new SqliteCommand("SELECT * FROM ProductTransactions;");
            }
            else if (tableIndex == 6)
            {
                command = new SqliteCommand("SELECT * FROM MaterialTransactions;");
            }
            else if (tableIndex == 7)
            {
                command = new SqliteCommand("SELECT * FROM FinancialTransactions;");
            }
            else if (tableIndex == 8)
            {
                command = new SqliteCommand("SELECT * FROM ProductTransactionSalesAssociations;");
            }
            else if (tableIndex == 9)
            {
                command = new SqliteCommand("SELECT * FROM Sessions;");
            }
            else if (tableIndex == 10)
            {
                command = new SqliteCommand("SELECT * FROM SessionEvents;");
            }
            else if (tableIndex == 11)
            {
                command = new SqliteCommand("SELECT * FROM ProductRequirements;");
            }
            return SelectQueryReturnDataTable(command); //return data
        }

        public static void InsertStructureAndStartingDataIntoBlankDbFile() //Inserts all the required structure and "Unknown Customer" value into a blank db file
        {
            //Most of this ridiculously long sql command was writtrn by db browser for sqlite. I formatted it to work well as a string and added the insert statement
            SqliteCommand command = new SqliteCommand("CREATE TABLE IF NOT EXISTS 'Products' (  'product_id' TEXT NOT NULL UNIQUE,  'display_name' TEXT NOT NULL,  'cost' REAL NOT NULL,  PRIMARY KEY('product_id')); CREATE TABLE IF NOT EXISTS 'ProductTransactions' (  'pt_id' INTEGER NOT NULL UNIQUE,  'product_id' TEXT NOT NULL,  'change' INTEGER NOT NULL DEFAULT 0,  'status' INTEGER NOT NULL DEFAULT 0,  FOREIGN KEY('product_id') REFERENCES 'Products'('product_id'),  PRIMARY KEY('pt_id')); CREATE TABLE IF NOT EXISTS 'MaterialTransactions' (  'mt_id' INTEGER NOT NULL UNIQUE,  'material_id' TEXT NOT NULL,  'change' INTEGER NOT NULL DEFAULT 0,  FOREIGN KEY('material_id') REFERENCES 'Materials'('material_id'),  PRIMARY KEY('mt_id')); CREATE TABLE IF NOT EXISTS 'FinancialTransactions' (  'ft_id' INTEGER NOT NULL UNIQUE,  'change' REAL NOT NULL DEFAULT 0,  PRIMARY KEY('ft_id')); CREATE TABLE IF NOT EXISTS 'Sessions' (  'session_id' INTEGER NOT NULL UNIQUE,  'user_id' INTEGER NOT NULL,  PRIMARY KEY('session_id')); CREATE TABLE IF NOT EXISTS 'SessionEvents' (  'event_id' INTEGER NOT NULL UNIQUE,  'session_id' INTEGER NOT NULL,  'comment' INTEGER NOT NULL,  FOREIGN KEY('session_id') REFERENCES 'Sessions'('session_id'),  PRIMARY KEY('event_id')); CREATE TABLE IF NOT EXISTS 'Users' (  'user_id' INTEGER NOT NULL UNIQUE,  'text_user_id' TEXT NOT NULL UNIQUE,  'display_name' TEXT NOT NULL,  'password' TEXT NOT NULL DEFAULT 'none',  'salt' TEXT NOT NULL DEFAULT 'none',  'admin_access' INTEGER NOT NULL DEFAULT 0,  PRIMARY KEY('user_id' AUTOINCREMENT)); CREATE TABLE IF NOT EXISTS 'Materials' (  'material_id' TEXT NOT NULL UNIQUE,  'display_name' TEXT,  PRIMARY KEY('material_id')); CREATE TABLE IF NOT EXISTS 'ProductTransactionSalesAssociations' (  'association_id' INTEGER NOT NULL UNIQUE,  'pt_id' INTEGER NOT NULL UNIQUE,  'sale_id' INTEGER NOT NULL,  'ft_id' INTEGER,  FOREIGN KEY('ft_id') REFERENCES 'FinancialTransactions'('ft_id'),  FOREIGN KEY('sale_id') REFERENCES 'Sales'('sale_id'),  FOREIGN KEY('pt_id') REFERENCES 'ProductTransactions'('pt_id'),  PRIMARY KEY('association_id')); CREATE TABLE IF NOT EXISTS 'Sales' (  'sale_id' INTEGER NOT NULL UNIQUE,  'customer_id' INTEGER NOT NULL DEFAULT 1,  'user_id' INTEGER NOT NULL,  'payment_link' TEXT DEFAULT 'Not given',  'status' INTEGER NOT NULL DEFAULT 0,  'method' INTEGER NOT NULL DEFAULT 0,  FOREIGN KEY('customer_id') REFERENCES 'Customers'('customer_id'),  FOREIGN KEY('user_id') REFERENCES 'Users'('user_id'),  PRIMARY KEY('sale_id')); CREATE TABLE IF NOT EXISTS 'ProductRequirements' (  'requirement_id' INTEGER NOT NULL UNIQUE,  'product_id' TEXT NOT NULL,  'material_id' TEXT NOT NULL,  FOREIGN KEY('material_id') REFERENCES 'Materials'('material_id'),  FOREIGN KEY('product_id') REFERENCES 'Products'('product_id'),  PRIMARY KEY('requirement_id' AUTOINCREMENT)); CREATE TABLE IF NOT EXISTS 'Customers' (  'customer_id' INTEGER NOT NULL UNIQUE,  'first_name' TEXT NOT NULL,  'last_name' TEXT NOT NULL,  'email_address' TEXT NOT NULL DEFAULT 'none',  'address1' REAL NOT NULL,  'address2' TEXT NOT NULL,  'address3' TEXT NOT NULL,  'postcode' TEXT NOT NULL,  'notes' TEXT NOT NULL,  PRIMARY KEY('customer_id' AUTOINCREMENT)); INSERT INTO Customers (first_name, last_name, email_address, address1, address2, address3, postcode, notes) VALUES ('Unknown', 'Customer', 'none', ' ??? ', ' ??? ', ' ??? ', ' ??? ', 'Unknown Customer');");
            NonQueryReturnVoid(command);
        }

        public static void InsertNewUser(UserDetails userDetails) //Inserts a new user into the database
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO Users (text_user_id, display_name, admin_access) VALUES (@TextId, @DisplayName, @AdminAccess);");
            command.Parameters.Add(new SqliteParameter("TextId", userDetails.text_user_id));
            command.Parameters.Add(new SqliteParameter("DisplayName", userDetails.display_name));
            command.Parameters.Add(new SqliteParameter("AdminAccess", userDetails.admin_access));
            NonQueryReturnVoid(command);
        }

        public static bool CheckIfUserHasPinByTextId(string textId) //A user with a no pin will have the value set as "none". This checks for this value
        {
            SqliteCommand command = new SqliteCommand("SELECT password FROM Users WHERE text_user_id = @TextUserId;");
            command.Parameters.Add(new SqliteParameter("TextUserId", textId));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            if (returnedData.Rows[0].ItemArray[0].ToString() == "none")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void UpdateUserPassword(PasswordDetails passwordDetails, string textId) //Updates a user's password and salt
        {
            SqliteCommand command = new SqliteCommand("UPDATE Users SET password = @Password, salt = @Salt WHERE text_user_id = @TextUserId");
            command.Parameters.Add(new SqliteParameter("Password", passwordDetails.hash));
            command.Parameters.Add(new SqliteParameter("Salt", passwordDetails.salt));
            command.Parameters.Add(new SqliteParameter("TextUserId", textId));
            NonQueryReturnVoid(command);
        }

        public static void InserNewMaterial(MaterialDetails materialDetails) //Insert a new material into the database
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO Materials (material_id,  display_name) VALUES (@MaterialId, @MaterialName);");
            command.Parameters.Add(new SqliteParameter("MaterialId", materialDetails.materialId));
            command.Parameters.Add(new SqliteParameter("MaterialName", materialDetails.materialName));
            NonQueryReturnVoid(command);
        }

        public static void UpdateMaterialName(MaterialDetails materialDetails) //Update a material's name
        {
            SqliteCommand command = new SqliteCommand("UPDATE Materials SET display_name = @DisplayName WHERE material_id = @MaterialId");
            command.Parameters.Add(new SqliteParameter("DisplayName", materialDetails.materialName));
            command.Parameters.Add(new SqliteParameter("MaterialId", materialDetails.materialId));
            NonQueryReturnVoid(command);
        }

        public static MaterialTransactionData GetMaterialTransactionsByMaterialId(string materialId) //Get all material transactions for a material
        {
            SqliteCommand command = new SqliteCommand("SELECT * FROM MaterialTransactions WHERE material_id = @MaterialId;");
            command.Parameters.Add(new SqliteParameter("MaterialId", materialId));
            DataTable returnedData = SelectQueryReturnDataTable(command);
            MaterialTransactionData materialTransactionData = new MaterialTransactionData()
            {
                totalStock = 0,
                materialTransactions = new List<MaterialTransaction>()
            };
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                materialTransactionData.totalStock += (int)(long)returnedData.Rows[i].ItemArray[2];
                materialTransactionData.materialTransactions.Add(new MaterialTransaction()
                {
                    amount = (int)(long)returnedData.Rows[i].ItemArray[2],
                    materialId = returnedData.Rows[i].ItemArray[1].ToString(),
                    transactionId = (long)returnedData.Rows[i].ItemArray[0]
                });
            }
            return materialTransactionData;
        }

        public static void InsertNewMaterialTransaction(MaterialTransaction materialTransaction) //Inserts a new material transaction
        {
            SqliteCommand command = new SqliteCommand("INSERT INTO MaterialTransactions (mt_id, material_id, change) VALUES (@TransactionId, @MaterialId, @Change);");
            command.Parameters.Add(new SqliteParameter("@TransactionId", DataFunctions.GenerateIdForPrimaryKey()));
            command.Parameters.Add(new SqliteParameter("@MaterialId", materialTransaction.materialId));
            command.Parameters.Add(new SqliteParameter("@Change", materialTransaction.amount));
            NonQueryReturnVoid(command);
        }

        public static OverviewScreenStats GetStatsForOverviewPage() //Get the staristics required for the overview screen. This is a modified version of the code included in the prototype
        {
            long thresholdForPastWeek = DataFunctions.GenerateIdForPrimaryKey() - 604800000; //Threshold for primary keys less athan a week ago
            OverviewScreenStats overviewScreenStats = new OverviewScreenStats() //new OverviewScreenStats class
            {
                incomeAllTime = 0,
                salesAllTime = 0,
                salesPastWeek = 0,
                incomePastWeek = 0,
                outgoingsPastWeek = 0
            };
            List<FinancialTransaction> financialTransactions = GetAllFinancialTransactions(); //Get all financial transactions
            for (int i = 0; i < financialTransactions.Count; i++)
            {
                overviewScreenStats.incomeAllTime += financialTransactions[i].amount; //add to total cost
                if (financialTransactions[i].time > DataFunctions.FindPrimaryKeyTime(thresholdForPastWeek))
                {
                    if (financialTransactions[i].amount > 0) //Is the amount classed as an incoming or an outgoing
                    {
                        overviewScreenStats.incomePastWeek += financialTransactions[i].amount;
                    }
                    else
                    {
                        overviewScreenStats.outgoingsPastWeek -= financialTransactions[i].amount;
                    }
                }
            }
            SqliteCommand command = new SqliteCommand("SELECT sale_id FROM Sales;"); //get all sale details
            DataTable returnedData = SelectQueryReturnDataTable(command);
            for (int i = 0; i < returnedData.Rows.Count; i++)
            {
                overviewScreenStats.salesAllTime++; //Adds to all time sales
                if ((long)returnedData.Rows[i].ItemArray[0] > thresholdForPastWeek) //if sale was in past week, increment past week value
                {
                    overviewScreenStats.salesPastWeek++;
                }
            }
            return overviewScreenStats; //return stats
        }
    }

    
}
