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
    /// Interaction logic for CustomerSelectionUserControl.xaml
    /// </summary>
    public partial class CustomerSelectionUserControl : UserControl
    {
        public CustomerSelectionUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public CustomerSelectionWindow containingWindow;
        public string CustomerName { get; set; }
        public int Id { get; set; }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            containingWindow.selectedCustomerId = Id;
            containingWindow.Close();
        }

        
    }
}
