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
    /// Interaction logic for MaterialRequirementUserControl.xaml
    /// </summary>
    public partial class MaterialRequirementUserControl : UserControl
    {
        public MaterialRequirementUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public ProductDetailsWindow containingWindow;
        public ProductRequirement productRequirement;
        public string PictureLocation { get; set; }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManagementV2.RemoveRequirementForProduct(productRequirement); //Remove the product requirement
            containingWindow.RefreshWindow(); //Refresh the details window
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txbItemName.Text = productRequirement.materialDetails.materialName;
        }
    }
}
