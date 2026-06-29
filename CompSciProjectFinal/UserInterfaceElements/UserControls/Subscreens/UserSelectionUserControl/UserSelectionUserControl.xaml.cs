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
    /// Interaction logic for UserSelectionUserControl.xaml
    /// </summary>
    public partial class UserSelectionUserControl : UserControl
    {
        public UserSelectionUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public UserSelectionWindow containingWindow;
        public string UserDisplayName { get; set; }
        public int Id { get; set; }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            containingWindow.selectedUserId = Id;
            containingWindow.Close();
        }

        
    }
}
