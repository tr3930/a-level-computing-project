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
    /// Interaction logic for FinanceScreen.xaml
    /// </summary>
    public partial class FinanceScreen : UserControl //The screen that displays the financial overview
    {
        public FinanceScreen()
        {
            InitializeComponent();
        }

        private void refreshTransactionList()
        {
            stpItems.Children.Clear();
            List<FinancialTransaction> financialTransactions = DatabaseManagementV2.GetAllFinancialTransactions();
            for (int i = 0; i < financialTransactions.Count; i++)
            {
                stpItems.Children.Add(new FinancialTransactionUserControl()
                {
                    transaction = financialTransactions[i],
                    Margin = new Thickness(3, 3, 3, 3),
                    Height = 60
                });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            refreshTransactionList();
        }

        private void LoadTransactionWindow(bool isOutgoing)
        {
            NewFinancialTransactionWindow newFinancialTransactionWindow = new NewFinancialTransactionWindow()
            {
                isOutgoing = isOutgoing
            };
            newFinancialTransactionWindow.ShowDialog();
            refreshTransactionList();
        }

        private void btnOutgoing_Click(object sender, RoutedEventArgs e)
        {
            LoadTransactionWindow(true);
        }

        private void btnIncoming_Click(object sender, RoutedEventArgs e)
        {
            LoadTransactionWindow(false);
        }
    }
}
