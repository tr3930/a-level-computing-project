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
    /// Interaction logic for FinancialTransactionUserControl.xaml
    /// </summary>
    public partial class FinancialTransactionUserControl : UserControl
        {
            public FinancialTransactionUserControl()
            {
                InitializeComponent();
                this.DataContext = this;
            }

            public FinancialTransaction transaction { get; set; }
            string timeOfTransaction;

            private void UserControl_Loaded(object sender, RoutedEventArgs e) //Setting the text elements for the overview
            {
                txbTime.Text = transaction.time.ToString("dd/MM/yyyy HH:mm");
                txbAmount.Text = transaction.amount.ToString("C"); //The C formats it as currency
            }
        }
}
