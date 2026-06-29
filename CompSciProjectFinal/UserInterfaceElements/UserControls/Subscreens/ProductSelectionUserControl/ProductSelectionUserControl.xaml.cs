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
    /// Interaction logic for ProductSelectionUserControl.xaml
    /// </summary>
    public partial class ProductSelectionUserControl : UserControl
    {
        public ProductSelectionUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public ProductSelectionWindow containingWindow;
        public string ItemName { get; set; }
        public string Amount { get; set; }
        public string Cost { get; set; }
        public string Id { get; set; }
        public string PictureLocation { get; set; }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            containingWindow.selectedProductId = Id;
            containingWindow.Close();
        }

        
    }
}
