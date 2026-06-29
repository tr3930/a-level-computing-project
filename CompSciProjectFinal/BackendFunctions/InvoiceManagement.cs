using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompSciProjectFinal
{
    internal class InvoiceManagement
    {
        public static string GenerateInvoiceFromSaleDetails(SaleDetailsV2 saleDetails) //Exports an invoice to a text file
        {
            //FOrmats the sale details into an invoice string in the formatting of the string shown in section 3.2
            string invoiceString = "Sales Invoice:" + System.Environment.NewLine + "==============================================================" + System.Environment.NewLine + "Sale ID: " + saleDetails.saleId.ToString() + System.Environment.NewLine + "Sale Recorded Time: " + DataFunctions.FindPrimaryKeyTime(saleDetails.saleId).ToString("dd/MM/yyyy HH:mm") + System.Environment.NewLine + "Customer: " + saleDetails.customerDetails.customerFirstName + " " + saleDetails.customerDetails.customerLastName + " " + "(" + saleDetails.customerDetails.customerEmail + ")" + System.Environment.NewLine + "Seller: " + saleDetails.userDetails.display_name + System.Environment.NewLine + "==============================================================" + System.Environment.NewLine + "Item List:" + System.Environment.NewLine;
            List<ProductDetails> productDetails = DatabaseManagementV2.GetAllProductInformation();
            double totalCost = 0;
            for (int i = 0; i < saleDetails.prodTransSaleAsscs.Count; i++) //Iterate through the sale items and add them to the list
            {
                for (int e = 0; i < productDetails.Count; e++)
                {
                    if (productDetails[e].productId == saleDetails.prodTransSaleAsscs[i].productTransaction.productId)
                    {
                        invoiceString += productDetails[e].productName + " x" + (-saleDetails.prodTransSaleAsscs[i].productTransaction.changeAmount).ToString() + " - " + (saleDetails.prodTransSaleAsscs[i].financialTransaction.amount).ToString("C") + System.Environment.NewLine;
                        totalCost += -saleDetails.prodTransSaleAsscs[i].productTransaction.changeAmount;
                        break;
                    }
                }
            }
            invoiceString += "Total Cost: " + totalCost.ToString("C") + System.Environment.NewLine + "==============================================================" + System.Environment.NewLine + "Thanks for shopping with us!";
            return invoiceString;
        }

        public static void SaveInvoiceToFile(string invoiceString, long saleId) //Save the invoice to a file
        {
            FileProcessing.SaveTextFile(invoiceString, ProgramConfigurationManagement.GetDataPath() + "/Invoices/" + saleId.ToString() + ".txt");
        }
    }
}
