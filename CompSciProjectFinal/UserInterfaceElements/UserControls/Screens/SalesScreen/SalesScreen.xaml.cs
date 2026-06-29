using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for SalesScreen.xaml
    /// </summary>
    public partial class SalesScreen : UserControl
    {
        public MainWindow containingWindow;
        public SalesScreen()
        {
            InitializeComponent();
        }

        private void RefreshSalesList(int saleStatus)
        {
            if (stpItems != null)
            {
                stpItems.Children.Clear();
                List<OverviewSale> sales = DatabaseManagementV2.GetAllSalesForOverviewScreen(saleStatus);
                if (sales.Count > 0)
                {
                    for (int i = 0; i < sales.Count; i++) //This goes through each item in the datatable and adds it to the user interface
                    {
                        stpItems.Children.Add(new SaleUserControl() //New usercontrol added to stackpanel
                        {
                            saleId = sales[i].saleId,
                            time = sales[i].saleTime,
                            containingWindow = containingWindow,
                            Margin = new Thickness(3, 3, 3, 3), //Spacing out the ui elements
                        });
                        stpItems.MinHeight = 0;
                    }
                }
                else
                {
                    stpItems.Children.Add(new Border() //Sales doesn't exist message
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 180,
                        Height = 40,
                        Child = new TextBlock()
                        {
                            FontSize = 16,
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = "There's nothing here"
                        },
                        Background = new SolidColorBrush(Color.FromRgb(246, 161, 94)),
                        CornerRadius = new CornerRadius(10,10,10,10),
                        Margin = new Thickness(0,40,0,0)
                    });
                }
                
            }
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) //Refresh sales list on load
        {
            RefreshSalesList(0);
        }

        private void rdbUnconfirmed_Checked(object sender, RoutedEventArgs e) //Show unconfirmed sales
        {
            RefreshSalesList(0);
        }

        private void rdbAwaitPay_Checked(object sender, RoutedEventArgs e) //Show sales awaiting payment
        {
            RefreshSalesList(1);
        }

        private void rdbAwaitShip_Checked(object sender, RoutedEventArgs e) //Show sales awaiting postage
        {
            RefreshSalesList(2);
        }

        private void rdbComplete_Checked(object sender, RoutedEventArgs e) //show completed sales
        {
            RefreshSalesList(3);
        }

        private void btnNewSale_Click(object sender, RoutedEventArgs e) //Creates a new sale
        {
            YesNoDialog yesNoDialog = new YesNoDialog() //Warning message to give the user a final choice
            {
                message = "Would you like to add a new sale?" + System.Environment.NewLine + "This action cannot be undone."
            };
            yesNoDialog.ShowDialog();
            if (yesNoDialog.choice) //Did the user say yes?
            {
                long saleId = DatabaseManagementV2.CreateNewBlankSaleAndReturnId(containingWindow.userDetails);
                if (containingWindow.session != null)
                {
                    SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Created new sale (" + saleId.ToString() + ")");
                }
                SaleDetailsWindow saleDetailsWindow = new SaleDetailsWindow()
                {
                    saleId = saleId, //Add the sale to the DB and set the details window's sale id to the sales id
                    containingWindow = containingWindow
                };
                saleDetailsWindow.Show();
                RefreshSalesList(0);
            }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e) //Open checkout window
        {
            CheckoutWindow checkoutWindow = new CheckoutWindow()
            {
                parentMainWindow = containingWindow
            };
            checkoutWindow.Show();
        }

        private void btnManageCustomers_Click(object sender, RoutedEventArgs e) //Open manage customer management window
        {
            CustomerSelectionWindow customerSelectionWindow = new CustomerSelectionWindow(); //Show customer selection window
            customerSelectionWindow.ShowDialog();
            if (customerSelectionWindow.selectedCustomerId != 0)
            {
                ManageCustomerWindow manageCustomerWindow = new ManageCustomerWindow() //Open customer management window for selected customer
                {
                    customerDetails = DatabaseManagementV2.GetCustomerDetailsByCustomerId(customerSelectionWindow.selectedCustomerId)
                };
                manageCustomerWindow.ShowDialog();
            }
        }

        private void btnNewCustomer_Click(object sender, RoutedEventArgs e) //Open new customer window
        {
            NewCustomerWindow newCustomerWindow = new NewCustomerWindow();
            newCustomerWindow.ShowDialog();
        }
    }
}

