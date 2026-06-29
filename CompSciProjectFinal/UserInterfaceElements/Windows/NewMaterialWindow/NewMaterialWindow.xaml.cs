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
    /// Interaction logic for NewMaterialWindow.xaml
    /// </summary>
    public partial class NewMaterialWindow : Window
    {
        MaterialDetails materialDetails = new MaterialDetails()
        {
            materialId = DataFunctions.GenerateRandomAlphaNeumericString(12)
        };
        BitmapImage image;
        
        public NewMaterialWindow()
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
            txbMaterialId.Text = "ID will be: " + materialDetails.materialId;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool allChecksPassed = true;
            if (txtMaterialName.Text == "") //Prescence checks
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
                materialDetails.materialName = txtMaterialName.Text;
                DatabaseManagementV2.InserNewMaterial(materialDetails); //Add material to database
                FileProcessing.ExportBitmapImageAsPng(image, ProgramConfigurationManagement.GetDataPath() + "/Images/Materials/" + materialDetails.materialId + ".png");//Export image
                this.Close(); //Close window
            }
        }
    }


}
