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

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for CustomerSelectionWindow.xaml
    /// </summary>
    public partial class CustomerSelectionWindow : Window
    {
        public int selectedCustomerId;
        public CustomerSelectionWindow()
        {
            InitializeComponent();
        }

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<CustomerDetails> customerDetails = DatabaseManagementV2.GetAllCustomerDetails(); //Get customers from DB
            for (int i = 0; i < customerDetails.Count; i++) //Populate window
            {
                stpItems.Children.Add(new CustomerSelectionUserControl() //Add customer details to window
                {
                    Height = 50,
                    CustomerName = customerDetails[i].customerFirstName + " " + customerDetails[i].customerLastName,
                    Id = customerDetails[i].customerId,
                    Margin = new Thickness(10,3,3,3),
                    containingWindow = this
                }) ;
            }
        }



    }


}
