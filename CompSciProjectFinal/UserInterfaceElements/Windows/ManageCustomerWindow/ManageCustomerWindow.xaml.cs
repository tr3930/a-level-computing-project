using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for ManageCustomerWindow.xaml
    /// </summary>
    public partial class ManageCustomerWindow : Window
    {
        public CustomerDetails customerDetails; //Customer details
        
        public ManageCustomerWindow()
        {
            InitializeComponent();
        }
        

        private void btnClose_Click(object sender, RoutedEventArgs e) //these are commented in other windows
        {
            this.Close();
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
            if (customerDetails != null)
            {
                txtFirstName.Text = customerDetails.customerFirstName; //Fill text boxes
                txtLastName.Text = customerDetails.customerLastName;
                txtEmail.Text = customerDetails.customerEmail;
                txtPostcode.Text = customerDetails.customerPostcode;
                txtAddress1.Text = customerDetails.customerAddress1;
                txtAddress2.Text = customerDetails.customerAddress2;
                txtAddress3.Text = customerDetails.customerAddress3;
                txtNotes.Text = customerDetails.customerNotes;
            }
            else
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "An unexpected error occurred"
                };
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool allChecksPassed = true;
            if (txtFirstName.Text == "")
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "First name was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (txtLastName.Text == "")
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Last name was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (txtEmail.Text == "")
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Email was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            else if (!Checking.ValidateEmailAddressFormatting(txtEmail.Text))
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Email not in valid format"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (txtPostcode.Text != "" && !Checking.ValidatePostcodeFormatting(txtPostcode.Text))
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Postcode not in valid format"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (allChecksPassed)
            {
                customerDetails.customerFirstName = txtFirstName.Text;
                customerDetails.customerLastName = txtLastName.Text;
                customerDetails.customerEmail = txtEmail.Text;
                customerDetails.customerPostcode = txtPostcode.Text;
                customerDetails.customerAddress1 = txtAddress1.Text;
                customerDetails.customerAddress2 = txtAddress2.Text;
                customerDetails.customerAddress3 = txtAddress3.Text;
                customerDetails.customerNotes = txtNotes.Text;
                DatabaseManagementV2.UpdateCustomerDetails(customerDetails);
                this.Close();
            }
        }
    }


}
