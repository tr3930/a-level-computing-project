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
using System.Drawing;
using QRCoder;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for ProductDetailsWindow.xaml
    /// </summary>
    public partial class ProductDetailsWindow : Window
    {
        public string productId { get; set; } //The product id that the window will use
        ProductDetails productDetails;
        List<ProductRequirement> productRequirements;
        public ProductDetailsWindow()
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
            productDetails = DatabaseManagementV2.GetProductDetailsByProductId(productId); //Get product info from db
            stpItems.Children.Clear();
            txbTitle.Text = "Details for: " + productDetails.productName;
            txbId.Text = "ID: " + productDetails.productId; //Set the product id text
            txtProductName.Text = productDetails.productName; //set the text in the product name text box
            txtCost.Text = productDetails.productCost.ToString("C").Replace("£",""); //Format and set the text in the price text box
            ImageBrush backgroundBrush = new ImageBrush(FileProcessing.LoadBitmapImageFromLocationString(FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/Images/Products/" + productId + ".png", forceImageUpdateFromOriginalFile))); //Load image into border
            backgroundBrush.Stretch = Stretch.UniformToFill; //Set border background stretch
            bdrProductImage.Background = backgroundBrush; //Place the image in the border
            ProductTransactionData productTransactionData = DatabaseManagementV2.GetTransactionHistoryForProductById(productId); //Get all product transactions
            txbTotalStock.Text = "Total Stock: " + productTransactionData.totalStock.ToString() + ", Total Sellable Stock: " + productTransactionData.totalSellableStock.ToString();
            for (int i = 0; i < productTransactionData.productTransactions.Count; i++) //Add product transactions to list
            {
                string changeAmountString = productTransactionData.productTransactions[i].changeAmount.ToString();
                if (productTransactionData.productTransactions[i].changeAmount > -1)
                {
                    changeAmountString = "+" + changeAmountString;
                }
                if (productTransactionData.productTransactions[i].confirmed)
                {
                    lbxTransactionHistory.Items.Add(productTransactionData.productTransactions[i].time.ToString("dd/MM/yyyy HH:mm") + " " + changeAmountString);
                }
                else
                {
                    lbxTransactionHistory.Items.Add(productTransactionData.productTransactions[i].time.ToString("dd/MM/yyyy HH:mm") + " " + changeAmountString + " (Unconfirmed)");
                }

            }
            productRequirements = DatabaseManagementV2.GetAllRequirementsForProduct(productDetails); //Get all requirememnts for product
            for (int i = 0; i < productRequirements.Count;i++) //Add requiremenmts to interface
            {
                stpItems.Children.Add(new MaterialRequirementUserControl()
                {
                    containingWindow = this,
                    Margin = new Thickness(10, 3, 3, 3),
                    productRequirement = productRequirements[i],
                    PictureLocation = FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/Images/Materials/" + productRequirements[i].materialDetails.materialId.ToString() + ".png")
                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshWindow();
        }

        private void btnAddTake_Click(object sender, RoutedEventArgs e)
        {
            TotalChangeWindow totalChangeWindow = new TotalChangeWindow()
            {
                isProduct= true,
                itemPriKey = productId
            };
            totalChangeWindow.ShowDialog();
            RefreshWindow();
        }

        private void btnReplaceImage_Click(object sender, RoutedEventArgs e) //Code runs when the replace image button is clicked
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); //Openfiledialof to select image
            openFileDialog.InitialDirectory = "%USERPROFILE%";
            openFileDialog.Filter = "JPEG Images (*.jpeg)|*.jpeg|PNG Images (*.png)|*.png|JPG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp";
            openFileDialog.DefaultExt = "*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage image = FileProcessing.LoadBitmapImageFromLocationString(openFileDialog.FileName); //Open image
                FileProcessing.ExportBitmapImageAsPng(image, ProgramConfigurationManagement.GetDataPath() + "/Images/Products/" + productId + ".png"); //Export the image as a png
                RefreshWindow(true); //Refresh the details
            }
            
            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e) //Updates the details for the product in the database
        {
            bool allChecksPassed = true;
            if (txtCost.Text == "") //Validation
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
            if (allChecksPassed)
            {
                txtCost.Text = txtCost.Text.Replace("£", "");
                productDetails.productName = txtProductName.Text;
                try
                {

                    productDetails.productCost = double.Parse(txtCost.Text); //Range check
                    if (productDetails.productCost < 0)
                    {
                        throw new FormatException("Cost cannot be negative");
                    }
                    DatabaseManagementV2.UpdateProductDetails(productDetails); //Update details in DB
                    RefreshWindow();
                }
                catch (FormatException)
                {
                    ErrorDialog errorDialog = new ErrorDialog() //Format check for price
                    {
                        errorMessage = "Entered cost not valid"
                    };
                    errorDialog.ShowDialog();
                }
                
            }
        }

        private void btnAddRequirement_Click(object sender, RoutedEventArgs e) //Adds a new require ment for the product
        {
            MaterialSelectionWindow materialSelectionWindow = new MaterialSelectionWindow();
            materialSelectionWindow.ShowDialog();
            if (materialSelectionWindow.selectedMaterialId != null) //Has user selected something?
            {
                bool addable = true;
                for (int i = 0; i < productRequirements.Count; i++) //Check if item is alreeady within requirements
                {
                    if (productRequirements[i].materialDetails.materialId == materialSelectionWindow.selectedMaterialId)
                    {
                        ErrorDialog errorDialog = new ErrorDialog()
                        {
                            errorMessage = "Requirement already exists"
                        };
                        errorDialog.ShowDialog();
                        addable = false;
                    }
                }
                if (addable)
                {
                    DatabaseManagementV2.InsertRequirementForProduct(productDetails, materialSelectionWindow.selectedMaterialId); //Add material to requirements
                }
                
            }
            RefreshWindow();
        }

        private void btnQr_Click(object sender, RoutedEventArgs e) //Usually I would place this in another class but it's small and only used here so there isn't much point
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image (*.png)|*.png";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                Bitmap qrCode = new QRCode(new QRCodeGenerator().CreateQrCode(productId, QRCodeGenerator.ECCLevel.Q)).GetGraphic(20); //Generate and export qr code image
                qrCode.Save(saveFileDialog.FileName); //Save using this rather than the FileProcessing procedure because the it is a Bitmap rather than a BitmapImage
                MessageDialog messageDialog = new MessageDialog()
                {
                    message = "Image exported to:" + System.Environment.NewLine + saveFileDialog.FileName
                };
                messageDialog.ShowDialog();
            }
        }
    }


}
