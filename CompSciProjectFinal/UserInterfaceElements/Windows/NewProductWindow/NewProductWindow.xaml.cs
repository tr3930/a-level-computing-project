using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for NewProductWindow.xaml
    /// </summary>
    public partial class NewProductWindow : Window
    {
        ProductDetails productDetails = new ProductDetails()
        {
            productId = DataFunctions.GenerateRandomAlphaNeumericString(12)
        }; //New ProductDetails class to fill with data
        BitmapImage image;
        
        public NewProductWindow()
        {
            InitializeComponent();
        }
        

        private void btnClose_Click(object sender, RoutedEventArgs e) //these are commented in other windows
        {
            this.Close();
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

        private void btnReplaceImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "JPEG Images (*.jpeg)|*.jpeg|PNG Images (*.png)|*.png|JPG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp";
            openFileDialog.DefaultExt = "*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                image = FileProcessing.LoadBitmapImageFromLocationString(FileProcessing.LoadFile(openFileDialog.FileName));
                ImageBrush backgroundBrush = new ImageBrush(image); //Load image into border
                backgroundBrush.Stretch = Stretch.UniformToFill; //Set border background stretch
                bdrProductImage.Background = backgroundBrush; //Place the image in the border
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txbProductId.Text = "ID will be: " + productDetails.productId;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool allChecksPassed = true;
            if (txtCost.Text == "") //Prescence checks
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Price was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (txtProductName.Text == "")
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Name was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (image == null)
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "No image was selected"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (allChecksPassed)
            {
                txtCost.Text = txtCost.Text.Replace("£", "");
                productDetails.productName = txtProductName.Text;
                try
                {
                    productDetails.productCost = double.Parse(txtCost.Text); //Range check for cost
                    if (productDetails.productCost < 0)
                    {
                        throw new FormatException("Cost cannot be negative");
                    }
                    DatabaseManagementV2.AddNewProduct(productDetails); //Add product
                    FileProcessing.ExportBitmapImageAsPng(image, ProgramConfigurationManagement.GetDataPath() + "/Images/Products/" + productDetails.productId + ".png"); //Export image to folder
                    this.Close();
                }
                catch (FormatException) //Validity check using try... catch
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "Entered cost not valid"
                    };
                    errorDialog.ShowDialog();
                }
                
            }
        }
    }


}
