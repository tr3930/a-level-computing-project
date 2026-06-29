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
using System.Data;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for DatabaseViewerWindow.xaml
    /// </summary>
    public partial class DatabaseViewerWindow : Window
    {
        public DatabaseViewerWindow()
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
            cboTables.Items.Add("Users"); //Add tables tgo combobox
            cboTables.Items.Add("Products");
            cboTables.Items.Add("Materials");
            cboTables.Items.Add("Customers");
            cboTables.Items.Add("Sales");
            cboTables.Items.Add("ProductTransactions");
            cboTables.Items.Add("MaterialTransactions");
            cboTables.Items.Add("FinancialTransactions");
            cboTables.Items.Add("ProductTransactionSalesAssociations");
            cboTables.Items.Add("Sessions");
            cboTables.Items.Add("SessionEvents");
            cboTables.Items.Add("ProductRequirements");
        }

        private void TableGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            
        }

        private void cboTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTable dbData = DatabaseManagementV2.GetEntireTable(cboTables.SelectedIndex); //Get the table and fill the datagrid with the data
            TableGrid.DataContext = dbData;
        }
    }


}
