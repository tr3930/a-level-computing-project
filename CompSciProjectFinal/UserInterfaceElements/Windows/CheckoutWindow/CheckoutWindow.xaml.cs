using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Common;
namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for CheckoutWindow.xaml
    /// </summary>
    public partial class CheckoutWindow : System.Windows.Window
    {
        public CheckoutWindow()
        {
            InitializeComponent();
        }
        SaleDetailsV2 sale = new SaleDetailsV2() //new sale object to store all information on the sale
        {
            price = 0,
            prodTransSaleAsscs = new List<ProdTransSaleAssc>()
        };
        public MainWindow parentMainWindow; //Main window that the sales user control that opened this was part of
        List<ProductDetails> productDetails = DatabaseManagementV2.GetAllProductInformation(); //Get all product information
        FilterInfoCollection filterInfoCollection; //
        VideoCaptureDevice captureDevice; //AForge capture device. (the webcam)
        Bitmap webcamSrc; //bitmap that the webcam feed is loaded into
        Bitmap alternateWebcamSrc; //duplicate of webcamSrc because I had errors during the development of the prototype where the webcamSrc object was in use when the QR was being read
        ImageBrush webcamDisplayBrush = new ImageBrush(); //Imagebrush for the border's background
        System.Timers.Timer timer = new System.Timers.Timer() //Timer which will attempt to scan the qr code every second
        {
            Interval = 1000,
            AutoReset = true,
            Enabled = true
        };

        private void NewFrame(object sender, NewFrameEventArgs eventArgs) //Gets the frame from the webcam and adds it to the border bg
        {
            webcamSrc = (Bitmap)eventArgs.Frame.Clone(); //Get webcam frame
            alternateWebcamSrc = (Bitmap)webcamSrc.Clone(); //Duplicate frame
            eventArgs = null;
            this.Dispatcher.Invoke(() =>
            {
                webcamDisplayBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(webcamSrc.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); //set imagesource image
            });
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

        private void btnStart_Click(object sender, RoutedEventArgs e) //Start the webcam feed
        {
            btnStart.IsEnabled = false; //Disable the elements used to select the webcam and start the feed
            cboWebcam.IsEnabled = false;
            btnDone.IsEnabled = true;
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboWebcam.SelectedIndex].MonikerString); //Set the capture device
            captureDevice.NewFrame += NewFrame; //Set the webcam to run NewFrame every time it generates a new frame
            captureDevice.Start(); //Start the webcam capture
        }


        private void ScanQR(object sender, EventArgs e) //This is used for scannign the QR code
        {
            if (webcamSrc != null) //Does the webcam actually exist?
            {
                LuminanceSource source = new BitmapLuminanceSource(alternateWebcamSrc); //This wasn't included with the ZXing binary from nuget so I downloaded the class from the github repo and placed it within this solution.
                BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                Result result = new MultiFormatReader().decode(bitmap); //Decoding QR
                if (result != null)
                {
                    bool isResultAValidItem = false;
                    int index = 0;
                    for (int i = 0; i < productDetails.Count; i++) //iterate through product details list and check to see if the code scanned matches a product id
                    {
                        if (productDetails[i].productId == result.Text)
                        {
                            isResultAValidItem = true;
                            index = i;
                            break;
                        }
                    }
                    if (isResultAValidItem) //Runs if the QR code is a valid product
                    {
                        sale.price += productDetails[index].productCost;
                        this.Dispatcher.Invoke(() =>
                        {
                            stpItems.Children.Add(new CheckoutItemUserControl());
                            txbCost.Text = sale.price.ToString("C");
                            int indexToIterate = -1;
                            for (int i = 0; i < sale.prodTransSaleAsscs.Count; i++)
                            {
                                if (sale.prodTransSaleAsscs[i].productTransaction.productId == result.Text)
                                {
                                    indexToIterate = i; 
                                    break;
                                }
                            }
                            if (indexToIterate == -1) //Add new item to sale data
                            {
                                sale.prodTransSaleAsscs.Add(new ProdTransSaleAssc()
                                {
                                    productTransaction = new ProductTransaction()
                                    {
                                        productId = result.Text,
                                        changeAmount = 1,
                                    },
                                    financialTransaction = new FinancialTransaction()
                                    {
                                        amount = productDetails[index].productCost
                                    }
                                }) ;
                                
                            }
                            else //Update existing item
                            {
                                sale.prodTransSaleAsscs[indexToIterate].productTransaction.changeAmount++;
                                sale.prodTransSaleAsscs[indexToIterate].financialTransaction.amount += productDetails[index].productCost;
                            }
                        });
                        RefreshList();
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(FileProcessing.LoadFile("checkoutping.wav"));
                        player.Play();
                    }
                    else
                    {
                        System.Media.SystemSounds.Exclamation.Play();
                    }
                    
                }

                //BarcodeReader barcodeReader = new BarcodeReader();
                //Result result = barcodeReader.Decode(pictureBox1.Image);
            }


        }

        private void RefreshList()
        {
            this.Dispatcher.Invoke(() =>
            {
                stpItems.Children.Clear();
                for (int i = 0; i < sale.prodTransSaleAsscs.Count; i++)
                {
                    int itemIndex = 0;
                    for (int e = 0; e < productDetails.Count; e++)
                    {
                        if (productDetails[e].productId == sale.prodTransSaleAsscs[i].productTransaction.productId)
                        {
                            itemIndex = e;
                        }
                    }
                    stpItems.Children.Add(new CheckoutItemUserControl()
                    {
                        ItemName = productDetails[itemIndex].productName,
                        PictureLocation = FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/Images/Products/" + productDetails[itemIndex].productId.ToString() + ".png"),
                        Amount = sale.prodTransSaleAsscs[i].productTransaction.changeAmount.ToString(),
                        Price = sale.prodTransSaleAsscs[i].financialTransaction.amount.ToString("C"),
                        Margin = new Thickness(3, 3, 3, 3)
                    });
                }
            });
            
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            if (sale.prodTransSaleAsscs.Count == 0)
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Item list is empty"
                };
                errorDialog.ShowDialog();
            }
            else
            {
                DatabaseManagementV2.InsertCheckoutSale(sale, parentMainWindow.userDetails);
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (captureDevice != null)
            {
                captureDevice.SignalToStop(); //Stop webcam feed
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bdrWebcamPreview.Background = webcamDisplayBrush; //set the border background to the webcam feed
            btnDone.IsEnabled = false; //Disable the done button
            timer.Elapsed += ScanQR; //Sets the timer increment function to be ScanQR
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice); //Get webcam list
            txbCost.Text = sale.price.ToString("C"); //Set total cost text
            foreach (FilterInfo filter in filterInfoCollection) //iterate through each entry and add it to the combobox
            {
                cboWebcam.Items.Add(filter.Name);
            }
            if (cboWebcam.Items.Count == 0) //Checks to see if no webcams are connected
            {
                ErrorDialog errorDialog = new ErrorDialog() //Opens an error message to alert the user
                {
                    errorMessage = "A webcam is required to use the checkout function"
                };
                errorDialog.ShowDialog();
                this.Close(); //Close window since it won't be useable
            }
            else //If the program does detect a webcam
            {
                cboWebcam.SelectedIndex = 0; //Sets the index to zero
            }
            
        }
    }


}
