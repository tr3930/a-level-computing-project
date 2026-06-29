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
    /// Interaction logic for NewFinancialTransactionWindow.xaml
    /// </summary>
    public partial class NewFinancialTransactionWindow : Window
    {
        public NewFinancialTransactionWindow()
        {
            InitializeComponent();
        }
        public bool isOutgoing;

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAmount.Text != "")
            {
                try
                {
                    double amount = double.Parse(txtAmount.Text);
                    if (amount >= 0)
                    {
                        if (this.isOutgoing)
                        {
                            amount = -amount;
                        }
                        DatabaseManagementV2.InsertNewFinancialTransaction(new FinancialTransaction()
                        {
                            amount = amount
                        });
                        this.Close();
                    }
                    else
                    {
                        throw new System.FormatException("Amount was zero or negative");
                    }
                }
                catch (System.FormatException)
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "Not a valid amount"
                    };
                    errorDialog.ShowDialog();
                }
                
            }
            else
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Please enter an amount"
                };
                errorDialog.ShowDialog();
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.isOutgoing)
            {
                txbTitlebar.Text = "New outgoing";
            }
            else
            {
                txbTitlebar.Text = "New incoming";
            }
        }
    }


}
