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
    /// Interaction logic for StockUserControl.xaml
    /// </summary>
    public partial class StockUserControl : UserControl
    {
        public StockUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public StockScreen containingScreen;
        public MainWindow containingWindow;
        public string ItemName { get; set; }
        public string Amount { get; set; }
        public string Cost { get; set; }
        public string Id { get; set; }
        public string PictureLocation { get; set; }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            containingScreen.ShowProperties(Id, ItemName);
        }

        private void btnAmount_Click(object sender, RoutedEventArgs e)
        {
            containingScreen.AddTake(Id);
        }
    }
}
