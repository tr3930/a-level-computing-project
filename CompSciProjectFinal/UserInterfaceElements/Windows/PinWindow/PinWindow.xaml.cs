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
    /// Interaction logic for PinWindow.xaml
    /// </summary>
    public partial class PinWindow : Window
    {
        public PinWindow()
        {
            InitializeComponent();
            txbPin.Focus();
        }
        public string textId;
        public bool pinVerified = false;
        public bool isWindowBeingUsedForPinChange = false;
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

        private void updatePinDots(object sender, RoutedEventArgs e) //updates the dots on the pin screen to match the textbox length
        {
            if (txbPin.Text.Length <= 4)
            {
                switch (txbPin.Text.Length)
                {
                    case 0:
                        pinDotA.IsChecked = false;
                        pinDotB.IsChecked = false;
                        pinDotC.IsChecked = false;
                        pinDotD.IsChecked = false;
                        break;
                    case 1:
                        pinDotA.IsChecked = true;
                        pinDotB.IsChecked = false;
                        pinDotC.IsChecked = false;
                        pinDotD.IsChecked = false;
                        break;
                    case 2:
                        pinDotA.IsChecked = true;
                        pinDotB.IsChecked = true;
                        pinDotC.IsChecked = false;
                        pinDotD.IsChecked = false;
                        break;
                    case 3:
                        pinDotA.IsChecked = true;
                        pinDotB.IsChecked = true;
                        pinDotC.IsChecked = true;
                        pinDotD.IsChecked = false;
                        break;
                    case 4:
                        pinDotA.IsChecked = true;
                        pinDotB.IsChecked = true;
                        pinDotC.IsChecked = true;
                        pinDotD.IsChecked = true;
                        break;
                }
            }
            if (txbPin.Text.Length == 4)
            {
                if (!isWindowBeingUsedForPinChange) //Window being used for verification
                {
                    if (Security.VerifyPassword(txbPin.Text, textId)) //Check if pin is correct and show status
                    {
                        bdrPinStatus.Visibility = Visibility.Visible;
                        bdrPinStatus.Background = Brushes.MediumSeaGreen;
                        txbPinStatus.Text = "PIN Correct";
                        pinVerified = true;
                        this.Close();
                    }
                    else
                    {
                        bdrPinStatus.Visibility = Visibility.Visible;
                        bdrPinStatus.Background = Brushes.Tomato;
                        txbPinStatus.Text = "Try Again";
                        System.Media.SystemSounds.Exclamation.Play();
                        txbPin.IsEnabled = true;
                        txbPin.Text = "";
                        txbPin.Focus();
                    }
                }
                else
                {
                    pinVerified = true; //close the window, allowing the parent window to access the pin entered
                    this.Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //Runs on window load, changes text if window is being used for a pin change
        {
            if (isWindowBeingUsedForPinChange)
            {
                txbInstructions.Text = "Enter your new pin";
            }
        }
    }


}
