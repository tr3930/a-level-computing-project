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
    /// Interaction logic for ProductSelectionWindow.xaml
    /// </summary>
    public partial class ProductSelectionWindow : Window
    {
        public string selectedProductId;
        public ProductSelectionWindow()
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
            List<ProductDetails> productDetails = DatabaseManagementV2.GetAllProductInformation(); //Populate list with selectable products
            for (int i = 0; i < productDetails.Count; i++)
            {
                stpItems.Children.Add(new ProductSelectionUserControl()
                {
                    Margin = new Thickness(10, 3, 3, 3),
                    ItemName = productDetails[i].productName,
                    Id = productDetails[i].productId,
                    containingWindow = this,
                    PictureLocation = FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/Images/Products/" + productDetails[i].productId + ".png")
                }) ;
            }
        }



    }


}
