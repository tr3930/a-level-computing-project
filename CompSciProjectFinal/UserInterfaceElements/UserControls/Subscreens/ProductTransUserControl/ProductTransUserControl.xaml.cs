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
    /// Interaction logic for ProductTransUserControl.xaml
    /// </summary>
    public partial class ProductTransUserControl : UserControl
    {
        
        public ProductTransUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public SaleDetailsWindow containingWindow; //Reference to the containing window
        public int Amount { get; set; } //Amount of product
        public double Cost { get; set; } //Cost of the product within the sale
        public ProductDetails productDetails { get; set; }
        public string PictureLocation { get; set; } //file path to the image
        public bool isEditable { get; set; } = false;
        public int indexInList { get; set; }
        public long productTransactionId { get; set; } //product transaction id

        private void btnP1_Click(object sender, RoutedEventArgs e)
        {
            UpdateAmount(1);
        }

        private void btnP10_Click(object sender, RoutedEventArgs e)
        {
            UpdateAmount(10);
        }

        private void btnM1_Click(object sender, RoutedEventArgs e)
        {
            UpdateAmount(-1);
        }

        private void btnM10_Click(object sender, RoutedEventArgs e)
        {
            UpdateAmount(-10);
        }

        private void UpdateAmount(int givenAmount)
        {
            if (Checking.WillProductTransactionStayAboveZero(new ProductTransaction()
            {
                productId = productDetails.productId,
                confirmed = false,
                changeAmount = -givenAmount
            }))
            {
                Amount += givenAmount;
                RefreshTotal();
            }
            else
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Not enough stock"
                };
                errorDialog.ShowDialog();
            }
                
        }

        private void RefreshTotal()
        {
            if (Amount < 0) //Can't sell a negative amount of products so set it to zero
            {
                Amount = 0;
            }
            txbName.Text = productDetails.productName;
            containingWindow.saleDetails.prodTransSaleAsscs[indexInList].financialTransaction.amount = productDetails.productCost * Amount;
            containingWindow.saleDetails.prodTransSaleAsscs[indexInList].productTransaction.changeAmount = -Amount;
            DatabaseManagementV2.UpdateProductTransactionForSale(productTransactionId, -Amount);
            txbAmount.Text = Amount.ToString(); //Update text field
            txbCost.Text = containingWindow.saleDetails.prodTransSaleAsscs[indexInList].financialTransaction.amount.ToString("C");
            if (Amount == 0) //Database management class will remove item if it gets to zero the window is refreshed to reflect this change
            {
                containingWindow.RefreshWindow();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isEditable) //Disable the ibcrementing buttons if the window is not editable
            {
                btnM1.IsEnabled = false;
                btnM10.IsEnabled = false;
                btnP1.IsEnabled = false; 
                btnP10.IsEnabled = false;
            }
            RefreshTotal();
        }
    }
}
