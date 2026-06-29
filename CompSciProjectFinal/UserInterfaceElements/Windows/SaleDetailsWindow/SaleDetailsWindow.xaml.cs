using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for SaleDetailsWindow.xaml
    /// </summary>
    public partial class SaleDetailsWindow : Window
    {
        public SaleDetailsV2 saleDetails;
        public SaleDetailsWindow()
        {
            InitializeComponent();
        }

        public long saleId;
        public MainWindow containingWindow;
        private void btnClose_Click(object sender, RoutedEventArgs e) //these are commented in other windows
        {
            this.Close();
        }

        private void btnMinimise_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        
        private void bdrTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {

            };

        }

        public void RefreshWindow() //Gets current data from the database and populates the window with the data
        {
            saleDetails = DatabaseManagementV2.GetSaleDetailsBySaleId(saleId);
            if (saleDetails != null)
            {
                stpItems.Children.Clear();
                txbTitle.Text = "Details for Sale #" + saleId.ToString();
                txbSaleId.Text = "#" + saleId.ToString();
                txbSeller.Text = saleDetails.userDetails.display_name;
                txbSaleTime.Text = DataFunctions.FindPrimaryKeyTime(saleId).ToString("dd/MM/yyyy HH:mm");
                txtPaymentLink.Text = saleDetails.paymentLink;
                if (saleDetails.method == 0)
                {
                    txbSaleMethod.Text = "Online Order";
                }
                else if (saleDetails.method == 1)
                {
                    txbSaleMethod.Text = "Checkout";
                }
                btnCustomer.Content = saleDetails.customerDetails.customerFirstName + " " + saleDetails.customerDetails.customerLastName;
                if (saleDetails.status == 0) //Setting the sale status
                {
                    txbSale.Text = "Unconfirmed";
                    btnAdvance.Content = "Confirm Sale";
                    bdrStatusBg.Background = Brushes.DarkOrange;
                }
                else if (saleDetails.status == 1)
                {
                    txbSale.Text = "Awating Payment";
                    btnAdvance.Content = "Mark as paid";
                    bdrStatusBg.Background = Brushes.DarkSalmon;
                }
                else if (saleDetails.status == 2)
                {
                    txbSale.Text = "Awating Postage";
                    btnAdvance.Content = "Mark as posted";
                    bdrStatusBg.Background = Brushes.Lavender;
                }
                else if (saleDetails.status == 3)
                {
                    txbSale.Text = "Complete";
                    bdrStatusBg.Background = Brushes.MediumSeaGreen;
                    btnAdvance.IsEnabled = false;
                    btnAdvance.Content = "Sale is complete";
                }
                bool isEditable = true;
                if (saleDetails.status != 0) //Sales will only be editable if they are unconfirmed
                {
                    btnCustomer.IsEnabled = false;
                    txtPaymentLink.IsEnabled= false;
                    btnAdd.IsEnabled = false;
                    isEditable = false;
                }
                for (int i = 0; i < saleDetails.prodTransSaleAsscs.Count; i++)
                {
                    ProductDetails productDetails = DatabaseManagementV2.GetProductDetailsByProductId(saleDetails.prodTransSaleAsscs[i].productTransaction.productId);
                    if (saleDetails.prodTransSaleAsscs[i].financialTransaction == null)
                    {
                        saleDetails.prodTransSaleAsscs[i].financialTransaction = new FinancialTransaction()
                        {
                            amount = saleDetails.prodTransSaleAsscs[i].productTransaction.changeAmount * productDetails.productCost //Calculate price
                        };
                    }
                    stpItems.Children.Add(new ProductTransUserControl()
                    {
                        productDetails = productDetails,
                        PictureLocation = FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/Images/Products/" + saleDetails.prodTransSaleAsscs[i].productTransaction.productId.ToString() + ".png"),
                        Amount = -saleDetails.prodTransSaleAsscs[i].productTransaction.changeAmount,
                        Cost = (-saleDetails.prodTransSaleAsscs[i].financialTransaction.amount),
                        Margin = new Thickness(3, 3, 3, 3), //Spacing out the ui elements,
                        isEditable = isEditable,
                        containingWindow = this,
                        indexInList = i,
                        productTransactionId = saleDetails.prodTransSaleAsscs[i].productTransaction.transactionId
                    });
                }
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //Runs when window is loaded
        {
            RefreshWindow(); //Populates data
        }

        private void btnAdvance_Click(object sender, RoutedEventArgs e)
        {
            if (saleDetails.prodTransSaleAsscs.Count > 0)
            {
                string message = ""; //Dialog message
                string emailSubject = "";
                string emailMessage = "";
                string analyticsMessage = "";
                if (saleDetails.status == 0)
                {
                    analyticsMessage = "Confirmed sale #" + saleDetails.saleId.ToString();
                    message = "Do you want to confirm this sale?"; //message for warning screen
                    if (ProgramConfigurationManagement.IsEmailEnabled())
                    {
                        emailSubject = "Order awaiting payment (ID: " + saleDetails.saleId.ToString() + ")"; //email subject line
                        emailMessage = EmailManagement.GetEmailMessages().order_confirm_email_text + System.Environment.NewLine + System.Environment.NewLine + InvoiceManagement.GenerateInvoiceFromSaleDetails(saleDetails); //email body text
                    }

                }
                else if (saleDetails.status == 1)
                {

                    analyticsMessage = "Marked sale #" + saleDetails.saleId.ToString() + " as paid";
                    message = "Do you want to mark this sale as paid for?";
                    if (ProgramConfigurationManagement.IsEmailEnabled())
                    {
                        emailSubject = "Order confirmed (ID: " + saleDetails.saleId.ToString() + ")";
                        emailMessage = EmailManagement.GetEmailMessages().order_paid_email_text;
                    }

                }
                else if (saleDetails.status == 2)
                {
                    analyticsMessage = "Marked sale #" + saleDetails.saleId.ToString() + " as posted";
                    message = "Do you want to mark this sale as posted?";
                    if (ProgramConfigurationManagement.IsEmailEnabled())
                    {
                        emailSubject = "Order posted (ID: " + saleDetails.saleId.ToString() + ")";
                        emailMessage = EmailManagement.GetEmailMessages().order_posted_email_text;
                    }
                }
                else if (saleDetails.status == 3)
                {
                    analyticsMessage = "Marked sale #" + saleDetails.saleId.ToString() + " as complete";
                }

                YesNoDialog yesNoDialog = new YesNoDialog() //I kept accidentally advancing sales when I was developing this program so I implemented a choice message to stop this
                {
                    message = message + System.Environment.NewLine + "This action cannot be undone"
                };
                yesNoDialog.ShowDialog();
                if (yesNoDialog.choice)
                {
                    if (saleDetails.status < 3 && ProgramConfigurationManagement.IsEmailEnabled())
                    {
                        emailMessage = emailMessage.Replace("CUSTOMERFIRSTNAME", saleDetails.customerDetails.customerFirstName);
                        emailMessage = emailMessage.Replace("CUSTOMERLASTNAME", saleDetails.customerDetails.customerLastName);
                        emailMessage = emailMessage.Replace("ORDERID", saleDetails.saleId.ToString());
                        emailMessage = emailMessage.Replace("PAYMENTLINK", saleDetails.paymentLink.ToString());
                        if (saleDetails.customerDetails.customerEmail != "none")
                        {
                            EmailManagement.SendEmail(saleDetails.customerDetails.customerEmail, emailSubject, emailMessage);
                        }

                    }
                    if (containingWindow.session != null)
                    {
                        SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, analyticsMessage);
                    }

                    saleDetails.status++; //Increase status
                    DatabaseManagementV2.SetSaleStatus(saleDetails.saleId, saleDetails.status);
                    if (saleDetails.status == 2)
                    {
                        DatabaseManagementV2.ConfirmSaleDetails(saleDetails);
                    }
                    RefreshWindow(); //Reload all data on the window
                }
            }
            else
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Item list is empty"
                };
                errorDialog.ShowDialog();
            }   
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductSelectionWindow productSelectionWindow = new ProductSelectionWindow();
            productSelectionWindow.ShowDialog();
            if (productSelectionWindow.selectedProductId != null) //Has the user actually selected something?
            {
                
                string productId = productSelectionWindow.selectedProductId;
                if (containingWindow.session != null)
                {
                    SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Added " + productId + " to sale #" + saleId.ToString());
                }
                bool isProductAlreadyPartOfSale = false;
                for (int i = 0; i < saleDetails.prodTransSaleAsscs.Count; i++)
                {
                    if (saleDetails.prodTransSaleAsscs[i].productTransaction.productId == productId) //Is the product already within the sale?
                    {
                        isProductAlreadyPartOfSale = true;
                        ErrorDialog errorDialog = new ErrorDialog() //Error message if product is already part of sale
                        {
                            errorMessage = "Product is already part of sale"
                        };
                        errorDialog.ShowDialog();
                    }
                }
                if (!isProductAlreadyPartOfSale) 
                {
                    DatabaseManagementV2.AddNewProductToSale(productId, saleId);
                    RefreshWindow();
                }
            }
        }

        private void btnCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerSelectionWindow customerSelectionWindow = new CustomerSelectionWindow();
            customerSelectionWindow.ShowDialog();
            if (customerSelectionWindow.selectedCustomerId != 0)
            {
                DatabaseManagementV2.UpdateCustomerForSale(saleId, customerSelectionWindow.selectedCustomerId);
                if (containingWindow.session != null)
                {
                    SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Set customer for sale #" + saleId.ToString() + " to ID: " + customerSelectionWindow.selectedCustomerId.ToString());
                }
                RefreshWindow();
            }
        }

        private void txtPaymentLink_TextChanged(object sender, TextChangedEventArgs e)
        {
            DatabaseManagementV2.UpdateSaleLink(saleId, txtPaymentLink.Text);
        }

        private void btnSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            InvoiceManagement.SaveInvoiceToFile(InvoiceManagement.GenerateInvoiceFromSaleDetails(saleDetails), saleId);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Plain Text File (*.txt)|*.txt";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                if (File.Exists(saveFileDialog.FileName)) //Stops an error if the file already exists
                {
                    File.Delete(saveFileDialog.FileName);
                }
                File.Copy(ProgramConfigurationManagement.GetDataPath() + "/Invoices/" + saleId.ToString() + ".txt", saveFileDialog.FileName);
                MessageDialog messageDialog = new MessageDialog()
                {
                    message = "Invoice exported to:" + System.Environment.NewLine + saveFileDialog.FileName
                };
                messageDialog.ShowDialog();
            }
        }
    }


}
