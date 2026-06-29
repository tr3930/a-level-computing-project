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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
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

        private void btnLogin_Click(object sender, RoutedEventArgs e) //Login button clicked
        {
            bool pinVerified = false; //Boolean to determine verification
            if (txtUsername.Text != "") //Prescence check for username
            {
                if (DatabaseManagementV2.VerifyExistenceOfUserByTextId(txtUsername.Text)) //Does user exist in db?
                {
                    if (DatabaseManagementV2.CheckIfUserHasPinByTextId(txtUsername.Text))
                    {
                        PinWindow pinWindow = new PinWindow(); //pin window
                        pinWindow.textId = txtUsername.Text; //Give pin window user info
                        pinWindow.ShowDialog(); //open pin window
                        pinVerified = pinWindow.pinVerified; //update pinverified variable
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog() //Mesage dialog notifying the user of a required pin change
                        {
                            message = "You need to set a new pin"
                        };
                        messageDialog.ShowDialog();
                        PinWindow pinWindow = new PinWindow() //show pin window
                        {
                            isWindowBeingUsedForPinChange = true
                        };
                        pinWindow.ShowDialog();
                        Security.UpdateUserPassword(txtUsername.Text, pinWindow.txbPin.Text); //update password details
                    }
                    
                }
                else
                {
                    ErrorDialog errorDialog = new ErrorDialog() //Freindly error message if user doesn't exist
                    {
                        errorMessage = "User could not be found"
                    };
                    errorDialog.Show();
                }
                
                if (pinVerified)
                {
                    UserDetails userDetails = DatabaseManagementV2.GetUserDetailsByTextId(txtUsername.Text); //get extra user details
                    Session session = SessionsAndAnalyticsManagement.StartAndReturnNewSession(userDetails);
                    this.Hide(); //hide login window
                    txtUsername.Text = ""; //clear textbox
                    MainWindow mainWindow = new MainWindow() 
                    { 
                        userDetails = userDetails,
                        session = session
                    }; //main window
                    SessionsAndAnalyticsManagement.AddNewEventToSession(session, "User has logged in");
                    mainWindow.ShowDialog();//open main window
                    this.Show(); //show this window again after main window is closed 
                }
            }
            else
            {
                ErrorDialog errorDialog = new ErrorDialog() //Error message
                {
                    errorMessage = "Please enter a username"
                };
                errorDialog.Show();
            }
            
        }
    }


}
