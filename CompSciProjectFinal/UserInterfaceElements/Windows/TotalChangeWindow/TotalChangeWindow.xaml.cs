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
    /// Interaction logic for TotalChangeWindow.xaml
    /// </summary>
    public partial class TotalChangeWindow : Window
    {
        public TotalChangeWindow()
        {
            InitializeComponent();
        }
        int change = 0;
        public object itemPriKey;
        public bool isProduct;
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
        private void RefreshTotal()
        {
            if (change >= 0) //Checks if total is positive or negitive, if positive, add a plus to the amount
            {
                txbChange.Text = "+" + change.ToString();
            }
            else
            {
                txbChange.Text = change.ToString();
            }
        }

        //These buttons change the total for the transaction

        private void btnP1_Click(object sender, RoutedEventArgs e)
        {
          change++;
          RefreshTotal();
        }

        private void btnP10_Click(object sender, RoutedEventArgs e)
        {
            change += 10;
            RefreshTotal();
        }

        private void btnM1_Click(object sender, RoutedEventArgs e)
        {
            change--;
            RefreshTotal();
        }

        private void btnM10_Click(object sender, RoutedEventArgs e)
        {
            change -= 10;
            RefreshTotal();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) //Save the stock transaction
        {
            if (change != 0)
            {
                bool isTransactionViable = true; //Used for checking if transaction will put stock levels into negative, which should be impossible
                
                if (isProduct)
                {
                    ProductTransaction productTransaction = new ProductTransaction
                    {
                        productId = itemPriKey.ToString(),
                        changeAmount = change,
                        confirmed = true
                    };
                    if (Checking.WillProductTransactionStayAboveZero(productTransaction)) //Check if transaction will push value below zero
                    {
                        DatabaseManagementV2.InsertNewProductTransaction(productTransaction);
                        this.Close();
                    }
                    else
                    {
                        ErrorDialog errorDialog = new ErrorDialog()
                        {
                            errorMessage = "Total cannot go below zero"
                        };
                        errorDialog.ShowDialog();
                        isTransactionViable = false;
                    }
                    
                }
                else
                {
                    MaterialTransaction materialTransaction = new MaterialTransaction
                    {
                        materialId = itemPriKey.ToString(),
                        amount = change
                    };
                    if (Checking.WillMaterialTransactionStayAboveZero(materialTransaction)) //Check if transaction will push value below zero
                    {
                        DatabaseManagementV2.InsertNewMaterialTransaction(materialTransaction);
                        this.Close();
                    }
                    else
                    {
                        ErrorDialog errorDialog = new ErrorDialog()
                        {
                            errorMessage = "Total cannot go below zero"
                        };
                        errorDialog.ShowDialog();
                        isTransactionViable = false;
                    }
                }
                if (isTransactionViable)
                {
                    YesNoDialog yesNoDialog = new YesNoDialog() //Give the user the oprion to add a financial transaction 
                    {
                        message = "Add outgoing for stock change?"
                    };
                    yesNoDialog.ShowDialog();
                    if (yesNoDialog.choice)
                    {
                        NewFinancialTransactionWindow newFinancialTransactionWindow = new NewFinancialTransactionWindow()
                        {
                            isOutgoing = true
                        };
                        newFinancialTransactionWindow.ShowDialog();
                    }
                }
                
            }
            else
            {
                ErrorDialog errorDialog = new ErrorDialog() //Non-zero value error
                {
                    errorMessage = "Please enter a non-zero amount"
                };
                errorDialog.Show();
            }
            
        }
    }


}
