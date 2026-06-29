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
    /// Interaction logic for CheckoutItemUserControl.xaml
    /// </summary>
    public partial class CheckoutItemUserControl : UserControl
    {
        public CheckoutItemUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public StockScreen containingScreen;
        public string ItemName { get; set; }
        public string Amount { get; set; }
        public string Price { get; set; }
        public string PictureLocation { get; set; }

        
    }
}
