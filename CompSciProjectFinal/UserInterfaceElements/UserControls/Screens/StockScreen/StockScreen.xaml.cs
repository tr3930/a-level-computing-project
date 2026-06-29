using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for StockScreen.xaml
    /// </summary>
    public partial class StockScreen : UserControl
    {
        
        string dataPath = ProgramConfigurationManagement.GetDataPath();
        public StockScreen()
        {
            InitializeComponent();
            refreshProducts();
        }

        private void refreshProducts()
        {
            stpItems.Children.Clear(); //Clear out list of stock items
            List<ProductDetails> data = DatabaseManagementV2.GetAllProductInformation(); //Get information on all products
            List<int> amounts = new List<int>(); //Add a new list to store the amount for each item. This is because totals are calculated dynamically
            List<long> lastTransactionIds = new List<long>();
            for (int i = 0; i < data.Count; i++)
            {
                amounts.Add(0); //Add a zero that can be incremented to each row of the datatable
                lastTransactionIds.Add(0); //Adds a value to be set to the last transaction id
            }
            List<ProductTransaction> transactionData = DatabaseManagementV2.GetAllProductTransactions(); //Get all product transactions. These will be ran through to calculate a total
            for (int i = 0; i < transactionData.Count; i++) //go through each product transaction and add it to the total
            {
                for (int e = 0; e < data.Count; e++) //
                {
                    if (data[e].productId == transactionData[i].productId) //Works out which item the transaction refers to and increments that item's value
                    {
                        amounts[e] += transactionData[i].changeAmount; //Add to the existing value
                        lastTransactionIds[e] = transactionData[i].transactionId; //Set last transaction id to transaction id
                        break; //Break out of the loop
                    }
                }
            }
            long timeThereshold = DataFunctions.GenerateIdForPrimaryKey() - 604800000;
            for (int i = 0; i < data.Count; i++) //This goes through each item in the datatable and adds it to the user interface
            {
                if (amounts[i] < 5 && lastTransactionIds[i] > timeThereshold) //Show a stock low warning if there was a transaction in the last week and the stock level is less than 5
                {
                    MessageDialog messageDialog = new MessageDialog()
                    {
                        message = "Stock low for:" + System.Environment.NewLine + data[i].productName
                    };
                    messageDialog.ShowDialog();
                }
                stpItems.Children.Add(new StockUserControl() //New usercontrol added to stackpanel
                {
                    ItemName = data[i].productName, //Item name
                    PictureLocation = FileProcessing.LoadFile(dataPath + "/Images/Products/" + data[i].productId + ".png"), //Path for item picture
                    Amount = "x" + amounts[i].ToString(), //Stock level
                    Margin = new Thickness(3, 3, 3, 3), //Spacing out the ui elements
                    Id= data[i].productId, //Item id
                    containingScreen = this //So the usercontol can reference this window
                }) ;
            }
        }

        private void refreshMaterials() //Very similar to refreshproducts seen above but for materials
        {
            stpItems.Children.Clear();
            List<MaterialDetails> materialDetails = DatabaseManagementV2.GetAllMaterialInformation(); 
            List<int> amounts = new List<int>();
            List<long> lastTransactionIds = new List<long>();
            for (int i = 0; i < materialDetails.Count; i++)
            {
                lastTransactionIds.Add(0); //Adds a value to be set to the last transaction id
                amounts.Add(0);
            }
            List<MaterialTransaction> transactionData = DatabaseManagementV2.GetAllMaterialTransactions();
            for (int i = 0; i < transactionData.Count; i++)
            {
                for (int e = 0; e < materialDetails.Count; e++)
                {
                    if (materialDetails[e].materialId == transactionData[i].materialId)
                    {
                        amounts[e] += transactionData[i].amount;
                        lastTransactionIds[e] = transactionData[i].transactionId; //Set last transaction id to transaction id
                        break;
                    }
                }
            }
            long timeThereshold = DataFunctions.GenerateIdForPrimaryKey() - 604800000;
            for (int i = 0; i < materialDetails.Count; i++)
            {
                if (amounts[i] < 5 && lastTransactionIds[i] > timeThereshold) //Show a stock low warning if there was a transaction in the last week and the stock level is less than 5
                {
                    MessageDialog messageDialog = new MessageDialog()
                    {
                        message = "Stock low for:" + System.Environment.NewLine + materialDetails[i].materialName
                    };
                    messageDialog.ShowDialog();
                }
                stpItems.Children.Add(new StockUserControl()
                {
                    ItemName = materialDetails[i].materialName,
                    PictureLocation = FileProcessing.LoadFile(dataPath + "/Images/Materials/" + materialDetails[i].materialId.ToString() + ".png"),
                    Amount = "x" + amounts[i].ToString(),
                    Margin = new Thickness(3, 3, 3, 3),
                    Id = materialDetails[i].materialId.ToString(),
                    containingScreen = this
                });
            }
        }

        public void AddTake(string itemId) //Launch stock level changer
        {
            TotalChangeWindow totalChangeWindow = new TotalChangeWindow();
            if (chkProductsMaterialsSwitch.IsChecked == true) //materials
            {
                totalChangeWindow.itemPriKey = itemId;
                totalChangeWindow.isProduct = false;
                totalChangeWindow.ShowDialog();
                refreshMaterials();
            }
            else //products
            {
                totalChangeWindow.itemPriKey = itemId;
                totalChangeWindow.isProduct = true;
                totalChangeWindow.ShowDialog();
                refreshProducts();
            }


        }

        private void btnCreate_Click(object sender, RoutedEventArgs e) //Addnew product or material
        {
            if (chkProductsMaterialsSwitch.IsChecked == true) //check if viewing products or materials
            {
                NewMaterialWindow newMaterialWindow = new NewMaterialWindow(); //launch new material window
                newMaterialWindow.ShowDialog();
                refreshMaterials();
            }
            else
            {
                NewProductWindow newProductWindow = new NewProductWindow(); //launch new product window
                newProductWindow.ShowDialog();
                refreshProducts();
            }

        }

        public void ShowProperties(string itemId, string itemName) //Launch details window foe either materials or products
        {
            if (chkProductsMaterialsSwitch.IsChecked == true) //Is user viewing products or materials
            {
                MaterialDetailsWindow materialDetailsWindow = new MaterialDetailsWindow() //show material details window for specified material
                {
                    materialId = itemId,
                };
                materialDetailsWindow.ShowDialog();
                refreshMaterials(); //refresh when window is closed
            }
            else
            {
                ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow() //show product detials window for specified product
                {
                    productId = itemId
                };
                productDetailsWindow.ShowDialog();
                refreshProducts(); //refresh when window is closed
            }

        }

        private void chkProductsMaterialsSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (chkProductsMaterialsSwitch.IsChecked == true) //switch between products and materials.
            {
                btnCreate.Content = "Add New Material"; //changes button text
                refreshMaterials(); //refreshes list
            }
            else
            {
                btnCreate.Content = "Add New Product";
                refreshProducts();
            }
        }
    }
}
