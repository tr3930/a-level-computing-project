using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.DirectoryServices;

namespace CompSciProjectFinal
{
    

    internal class DatabaseManagementLegacy
    {
        static SqliteConnection connection = new SqliteConnection("Data Source="+ProgramConfigurationManagement.GetDataPath()+"/database.db");

        public static void openConnection()
        {
            connection.Open();
        }

        public static void closeConnection()
        {
            connection.Close();
        }

        public static DataTable GetUserInfoById(int user_id)
        {
            connection.Open();
            DataTable dataTable = new DataTable();
            SqliteCommand sqliteCommand = new SqliteCommand("SELECT * FROM Users where user_id=@user_id;", connection);
            sqliteCommand.Parameters.Add(new SqliteParameter("user_id", user_id));
            SqliteDataReader reader = sqliteCommand.ExecuteReader();
            dataTable.Load(reader);
            connection.Close();
            return dataTable;
        }

        public static DataTable getAllProductData()
        {
            connection.Open();
            DataTable dataTable = new DataTable();
            SqliteCommand sqliteCommand = new SqliteCommand("SELECT * FROM Products;", connection);
            SqliteDataReader reader = sqliteCommand.ExecuteReader();
            dataTable.Load(reader);
            return dataTable;
        }

        public static UserDetails GetUserInfoByTextId(string text_id)
        {
            connection.Open();
            DataTable dataTable = new DataTable();
            SqliteCommand sqliteCommand = new SqliteCommand("SELECT * FROM Users WHERE text_user_id = @user_id;", connection);
            sqliteCommand.Parameters.Add(new SqliteParameter("@user_id", text_id));
            SqliteDataReader reader = sqliteCommand.ExecuteReader();
            dataTable.Load(reader);
            UserDetails userDetails = new UserDetails()
            {
                user_id = (int)(long)dataTable.Rows[0].ItemArray[0],
                admin_access = (int)(long)dataTable.Rows[0].ItemArray[4],
                text_user_id = dataTable.Rows[0].ItemArray[1].ToString(),
                display_name = dataTable.Rows[0].ItemArray[2].ToString()
               
            };
            return userDetails;
        }

        public static string GetPasswordByTextId(string text_id)
        {
            connection.Open();
            DataTable dataTable = new DataTable();
            SqliteCommand sqliteCommand = new SqliteCommand("SELECT Password FROM Users WHERE text_user_id = '" + text_id + "';", connection);
            SqliteDataReader reader = sqliteCommand.ExecuteReader();
            dataTable.Load(reader);
            return dataTable.Rows[0].ItemArray[0].ToString();
        }

        public static void CreateNewSale(SaleDetails saleDetails)
        {
            connection.Open();
            SqliteCommand sqliteCommand = new SqliteCommand("INSERT into Sales (sale_id, customer_id, user_id) VALUES ("+saleDetails.id.ToString()+", '"+saleDetails.customerId.ToString()+"', '"+ saleDetails.userId.ToString()+"');", connection);
            sqliteCommand.ExecuteNonQuery();
            for (int i = 0; i < saleDetails.entries.Count; i++)
            {
                long productTransactionId = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey();
                long financialTransactionId = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey();
                long associationId = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey();
                sqliteCommand = new SqliteCommand("INSERT INTO FinancialTransactions (ft_id, change) VALUES (" + financialTransactionId.ToString() + ", " + saleDetails.entries[i].cost.ToString() + "); INSERT into ProductTransactions (pt_id, product_id, change, ft_id) VALUES (" + productTransactionId.ToString() + ", '" + saleDetails.entries[i].itid + "', " + (-saleDetails.entries[i].amount).ToString() + ", " + financialTransactionId.ToString() + "); INSERT INTO ProductTransactionSalesAssociations (association_id, sale_id, pt_id) VALUES (" + associationId.ToString() + ", " + saleDetails.id.ToString() + ", " + productTransactionId.ToString() + ");", connection);
                sqliteCommand.ExecuteNonQuery();
            }
        }

        public static SaleDetails GetSaleById(long saleId)
        {
            SaleDetails sale = new SaleDetails()
            {
                id = saleId,
                entries = new List<OrderEntry>()
            };
            connection.Open();
            SqliteCommand sqliteCommand = new SqliteCommand("SELECT user_id, customer_id FROM Sales WHERE sale_id = " + saleId.ToString() + ";", connection);
            SqliteDataReader reader = sqliteCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(reader);
            sale.userId = (int)(long)data.Rows[0].ItemArray[0];
            sale.customerId = (int)(long)data.Rows[0].ItemArray[1];
            data = new DataTable();
            sqliteCommand = new SqliteCommand("SELECT ProductTransactions.product_id, ProductTransactions.change, FinancialTransactions.change FROM ProductTransactionSalesAssociations INNER JOIN ProductTransactions ON ProductTransactionSalesAssociations.pt_id = ProductTransactions.pt_id INNER JOIN FinancialTransactions ON ProductTransactions.ft_id = FinancialTransactions.ft_id WHERE ProductTransactionSalesAssociations.sale_id = " + saleId.ToString() + ";",connection);
            reader = sqliteCommand.ExecuteReader();
            data.Load(reader);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                sale.entries.Add(new OrderEntry()
                {
                    itid = data.Rows[i].ItemArray[0].ToString(),
                    amount = -(int)(long)data.Rows[i].ItemArray[1],
                    cost = (double)data.Rows[i].ItemArray[2]
                });
            }
            return sale;
        }

        public static List<object> GetStatsForOverviewPage()
        {
            long thresholdForPastWeek = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey() - 604800000;
            double incomeAllTime = 0;
            int salesAllTime = 0;
            int salesPastWeek = 0;
            double incomePastWeek = 0;
            double outgoingsPastWeek = 0;
            connection.Open();
            SqliteCommand sqliteCommand = new SqliteCommand("SELECT * FROM FinancialTransactions", connection);
            SqliteDataReader dataReader = sqliteCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(dataReader);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                incomeAllTime += (double)data.Rows[i].ItemArray[1];
                if ((long)data.Rows[i].ItemArray[0] > thresholdForPastWeek)
                {
                    if ((double)data.Rows[i].ItemArray[1] > 0)
                    {
                        incomePastWeek += (double)data.Rows[i].ItemArray[1];
                    }
                    else
                    {
                        outgoingsPastWeek -= (double)data.Rows[i].ItemArray[1];
                    }
                }
            }
            sqliteCommand = new SqliteCommand("SELECT sale_id FROM Sales", connection);
            dataReader = sqliteCommand.ExecuteReader();
            data = new DataTable();
            data.Load(dataReader);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                salesAllTime ++;
                if ((long)data.Rows[i].ItemArray[0] > thresholdForPastWeek)
                {
                    salesPastWeek++;
                }
            }
            return new List<object>()
            {
                incomeAllTime,
                salesAllTime,
                salesPastWeek,
                incomePastWeek,
                outgoingsPastWeek
            };
        }

        public static DataTable GetAllSalesForOverview()
        {
            DataTable data = new DataTable();
            connection.Open();
            SqliteCommand command = new SqliteCommand("SELECT Sales.sale_id, Users.display_name, Customers.email_address FROM Sales INNER JOIN Users ON Sales.user_id = Users.user_id INNER JOIN Customers ON Sales.customer_id = Customers.customer_id ORDER BY sale_id DESC;", connection);
            SqliteDataReader dataReader = command.ExecuteReader();
            data.Load(dataReader);
            return data;
        }

        public static DataTable GetAllCustomerDetails()
        {
            DataTable data = new DataTable();
            connection.Open();
            SqliteCommand command = new SqliteCommand("SELECT * FROM Customers;", connection);
            SqliteDataReader dataReader = command.ExecuteReader();
            data.Load(dataReader);
            return data;
        }

        public static DataTable GetAllProductTransactions()
        {
            DataTable data = new DataTable();
            connection.Open();
            SqliteCommand command = new SqliteCommand("SELECT * FROM ProductTransactions;", connection);
            SqliteDataReader dataReader = command.ExecuteReader();
            data.Load(dataReader);
            return data;
        }

        public static DataTable GetAllMaterialTransactions()
        {
            DataTable data = new DataTable();
            connection.Open();
            SqliteCommand command = new SqliteCommand("SELECT * FROM MaterialsTransactions;", connection);
            SqliteDataReader dataReader = command.ExecuteReader();
            data.Load(dataReader);
            return data;
        }

        public static DataTable GetAllMaterials()
        {
            DataTable data = new DataTable();
            connection.Open();
            SqliteCommand command = new SqliteCommand("SELECT * FROM Materials;", connection);
            SqliteDataReader dataReader = command.ExecuteReader();
            data.Load(dataReader);
            return data;
        }

        public static void NewProductTransaction(string productId, int amount, double cost)
        {
            long ptId = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey();
            long ftId = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey();
            connection.Open();
            SqliteCommand command = new SqliteCommand("INSERT INTO FinancialTransactions (ft_id, change) VALUES ("+ ftId.ToString() + ", "+ (-cost).ToString() +"); INSERT INTO ProductTransactions (pt_id, product_id, change, ft_id) VALUES (" + ptId.ToString() + ", '" + productId + "', " + amount.ToString() + ", " + ftId.ToString() + ");", connection);
            command.ExecuteNonQuery();
        }

        public static void NewMaterialTransaction(int productId, int amount, double cost)
        {
            long ptId = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey();
            long ftId = DataProcessingAndOtherSimilarFunctions.GenerateIdForPrimaryKey();
            connection.Open();
            SqliteCommand command = new SqliteCommand("INSERT INTO FinancialTransactions (ft_id, change) VALUES (" + ftId.ToString() + ", "+ (-cost).ToString() +"); INSERT INTO MaterialsTransactions (mt_id, material_id, change, ft_id) VALUES (" + ptId.ToString() + ", " + productId.ToString() + ", " + amount.ToString() + ", " + ftId.ToString() + ");", connection);
            command.ExecuteNonQuery();
        }

        public static void InsertNewProduct(string productId, string displayName, double cost)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand("INSERT INTO Products (product_id, display_name, cost) VALUES ('" + productId + "', '" + displayName + "', " + cost.ToString() + ");", connection);
            command.ExecuteNonQuery();
        }

        public static void InsertNewMaterial(string displayName)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand("INSERT INTO Materials (display_name) VALUES ('" + displayName + "');", connection);
            command.ExecuteNonQuery();
        }

        public static void UpdateProduct(string productId, string displayName, double cost)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand("UPDATE Products SET display_name = '" + displayName + "', cost = " + cost.ToString() + " WHERE product_id = '" + productId + "';", connection);
            command.ExecuteNonQuery();
        }

        public static void UpdateMaterial(int materialId, string displayName)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand("UPDATE Materials SET display_name = '" + displayName + "' WHERE material_id = " + materialId.ToString() + ";", connection);
            command.ExecuteNonQuery();
        }

        public static void UpdatePassword(string pin, int user_id)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand("UPDATE Users SET password = '"+pin+"' WHERE user_id = "+user_id.ToString()+";", connection);
            command.ExecuteNonQuery();
        }

        public static void UpdateTextUserId(string text_user_id, int user_id)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand("UPDATE Users SET text_user_id = '" + text_user_id + "' WHERE user_id = " + user_id.ToString() + ";", connection);
            command.ExecuteNonQuery();
        }

        public static void UpdateDisplayName(string display_name, int user_id)
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand("UPDATE Users SET display_name = '" + display_name + "' WHERE user_id = " + user_id.ToString() + ";", connection);
            command.ExecuteNonQuery();
        }
    }



}
