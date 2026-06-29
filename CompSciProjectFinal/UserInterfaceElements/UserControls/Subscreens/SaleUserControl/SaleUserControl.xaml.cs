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
    /// Interaction logic for SaleUserControl.xaml
    /// </summary>
    public partial class SaleUserControl : UserControl
    {
        public SaleUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public string time { get; set; }
        public long saleId { get; set; }
        public MainWindow containingWindow { get; set; }

        private void btnAdvance_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            SaleDetailsWindow saleDetailsWindow = new SaleDetailsWindow()
            {
                saleId = saleId,
                containingWindow = containingWindow
            };
            if (containingWindow.session != null)
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Viewed details for sale #" + saleId.ToString());
            }
            saleDetailsWindow.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txbSaleId.Text = "Sale #" + saleId.ToString();
        }
    }
}
