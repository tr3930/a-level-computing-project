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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for MaterialDetailsWindow.xaml
    /// </summary>
    public partial class MaterialDetailsWindow : Window
    {
        public string materialId { get; set; } //The product id that the window will use
        private MaterialDetails MaterialDetails;
        public MaterialDetailsWindow()
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

        public void RefreshWindow(bool forceImageUpdateFromOriginalFile = false)
        {
            lbxTransactionHistory.Items.Clear();
            MaterialDetails = DatabaseManagementV2.GetMaterialDetailsByMaterialId(materialId); //Get product info from db
            txbTitle.Text = "Details for: " + MaterialDetails.materialName;
            txbId.Text = "ID: " + MaterialDetails.materialId; //Set the product id text
            txtMaterialName.Text = MaterialDetails.materialName; //set the text in the product name text box
            ImageBrush backgroundBrush = new ImageBrush(FileProcessing.LoadBitmapImageFromLocationString(FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/Images/Materials/" + materialId + ".png", forceImageUpdateFromOriginalFile))); //Load image into border
            backgroundBrush.Stretch = Stretch.UniformToFill; //Set border background stretch
            bdrProductImage.Background = backgroundBrush; //Place the image in the border
            MaterialTransactionData materialTransactionData = DatabaseManagementV2.GetMaterialTransactionsByMaterialId(materialId); //Get all material transactions
            for (int i = 0; i < materialTransactionData.materialTransactions.Count; i++)
            {
                string changeAmountString = materialTransactionData.materialTransactions[i].amount.ToString();
                if (materialTransactionData.materialTransactions[i].amount > -1)
                {
                    changeAmountString = "+" + changeAmountString;
                }
                lbxTransactionHistory.Items.Add(DataFunctions.FindPrimaryKeyTime(materialTransactionData.materialTransactions[i].transactionId).ToString("dd/MM/yyyy HH:mm") + " " + changeAmountString);
            }
            txbTotalStock.Text = "Total Stock: " + materialTransactionData.totalStock.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshWindow();
        }

        private void btnAddTake_Click(object sender, RoutedEventArgs e)
        {
            TotalChangeWindow totalChangeWindow = new TotalChangeWindow()
            {
                isProduct= false,
                itemPriKey = materialId
            };
            totalChangeWindow.ShowDialog();
            RefreshWindow();
        }

        private void btnReplaceImage_Click(object sender, RoutedEventArgs e) //Code runs when the replace image button is clicked
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "%USERPROFILE%";
            openFileDialog.Filter = "JPEG Images (*.jpeg)|*.jpeg|PNG Images (*.png)|*.png|JPG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp";
            openFileDialog.DefaultExt = "*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage image = FileProcessing.LoadBitmapImageFromLocationString(openFileDialog.FileName);
                FileProcessing.ExportBitmapImageAsPng(image, ProgramConfigurationManagement.GetDataPath() + "/Images/Materials/" + materialId + ".png");
                RefreshWindow(true);
            }
            
            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool allChecksPassed = true;
            if (txtMaterialName.Text == "")
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Name was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (allChecksPassed)
            {
                MaterialDetails.materialName = txtMaterialName.Text;
                DatabaseManagementV2.UpdateMaterialName(MaterialDetails);
                RefreshWindow();
            }
            
        }

        
    }


}
