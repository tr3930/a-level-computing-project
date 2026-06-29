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
using System.IO;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //The main window of the rpogram. contains screens for the main functions fo the program
    {
        public UserDetails userDetails;
        public Session session;
        public bool isAccessedViaAdmin = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        

        private void btnClose_Click(object sender, RoutedEventArgs e) //these are commented in other windows
        {
            this.Close();
        }

        private void btnMinimise_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Minimized;
            ProgramConfigurationManagement.CreateNewProgramDataFolder();
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

        private void btnStock_Click(object sender, RoutedEventArgs e) //Switches window content to stock management
        {
            tabSwitcher.SelectedIndex = 1; //set tab switcher index
            bdrTitleBackground.Background = new SolidColorBrush(Color.FromRgb(93, 37, 190)); //sets the background for the border around the screen title text
            txbTitleText.Text = "Stock"; //sets the screen title text
            if (session != null) //This is to prevent the window attempting to add an event when session is null. It would usually happen when the window is starting
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(session, "Accessed stock management"); //Add session event
            }
            
        }

        private void btnOverview_Click(object sender, RoutedEventArgs e) //Switches window content to overview
        {
            tabSwitcher.SelectedIndex = 0;
            bdrTitleBackground.Background = new SolidColorBrush(Color.FromRgb(20, 131, 222));
            txbTitleText.Text = "Overview";
            if (session != null)
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(session, "Accessed overview screen");
            }
            
        }

        private void btnSales_Click(object sender, RoutedEventArgs e) //Switches window content to sales
        {
            tabSwitcher.SelectedIndex = 2;
            bdrTitleBackground.Background = new SolidColorBrush(Color.FromRgb(246, 161, 94));
            txbTitleText.Text = "Sales";
            if (session != null)
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(session, "Accessed sales management");
            }
            
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e) //Switches window to account management
        {
            bool isSettingsAccessible = false;
            if (!isAccessedViaAdmin) //Verifies the user's pin before giving them access to settings. Users accessing via admin management are not required to do this
            {
                PinWindow pinWindow = new PinWindow() //show pin window 
                {
                    textId = userDetails.text_user_id
                };
                pinWindow.ShowDialog();
                if (pinWindow.pinVerified)
                {
                    isSettingsAccessible = true; //allow settings window to be accessed
                }
            }
            else
            {
                isSettingsAccessible = true;
            }
            if (isSettingsAccessible) //Show settings window if access is allowed
            {
                tabSwitcher.SelectedIndex = 3;
                bdrTitleBackground.Background = new SolidColorBrush(Color.FromRgb(211, 88, 176));
                txbTitleText.Text = "Account";
                if (session != null)
                {
                    SessionsAndAnalyticsManagement.AddNewEventToSession(session, "Accessed account management");
                }
            }
            else
            {
                btnOverview.IsChecked = true;
            }
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e) //The user's pin is verified in the same way as it is for the settings window. This will be inaccessible for users without admin rights
        {
            PinWindow pinWindow = new PinWindow()
            {
                textId = userDetails.text_user_id
            };
            pinWindow.ShowDialog();
            if (pinWindow.pinVerified)
            {
                tabSwitcher.SelectedIndex = 5;
                bdrTitleBackground.Background = new SolidColorBrush(Color.FromRgb(236, 65, 65));
                txbTitleText.Text = "Admin";
                if (session != null)
                {
                    SessionsAndAnalyticsManagement.AddNewEventToSession(session, "Accessed admin settings");
                }
            }
            else
            {
                btnOverview.IsChecked = true;
            }
            
            

        }

        private void WindowLoaded(object sender, RoutedEventArgs e) //Runs when the window is started
        {
            OverviewScreen.enclosingWindow = this; //Pass the main window through to its children UserControls so that they can access information stored in it
            OverviewScreen.FillElements();
            SettingsScreen.containingWindow = this;
            AdminScreen.containingWindow = this;
            SalesScreen.containingWindow = this;
            CheckEmailStatus();
            if (isAccessedViaAdmin) //Clear all sidebar items aside from account management if the interface is being accessed from the admin interface
            {
                txbTitleBar.Text = "Admin Session";
                stpSidebar.Children.Remove(btnAdmin);
                stpSidebar.Children.Remove(btnOverview);
                stpSidebar.Children.Remove(btnSales);
                stpSidebar.Children.Remove(btnFinance);
                stpSidebar.Children.Remove(btnStock);
                btnSettings.IsChecked = true;
            }
            else
            {
                
                if (userDetails.admin_access == 0) //Remove admin button if user is not an admin
                {
                    stpSidebar.Children.Remove(btnAdmin);
                }
            }
            txbName.Text = "Logged in as: " + userDetails.display_name; //Add user's name to the logged in text at the top right
            
            
        }

        public void CheckEmailStatus() //Checks if all files required for a functioning email system exist. If they don't, admin users will be given a prompt to configure these settings. Users without admin rights will be shown an error
        {
                //Checks to see if both email config files exist
                if ((!File.Exists(ProgramConfigurationManagement.GetDataPath() + "/email_configuration.json") | !File.Exists(ProgramConfigurationManagement.GetDataPath() + "/email_messages.json")) && ProgramConfigurationManagement.IsEmailEnabled())
                {
                    if (userDetails.admin_access == 1)
                    {
                        YesNoDialog yesNoDialog = new YesNoDialog() //Gives the user the option to either disable the email system or configure it
                        {
                            message = "Email system not fully configured" + System.Environment.NewLine + "Configure it?" + System.Environment.NewLine + "(Answering 'no' will disable it)"
                        };
                        yesNoDialog.ShowDialog();
                        if (yesNoDialog.choice) //If the user chooses yes, confine user to configuration process
                        {
                            btnRecheckEmail.Visibility = Visibility.Visible; //Shows a button allowing the user to check the email configuration
                            btnStock.IsEnabled = false; //Disables all functions aside from those required for email configuration since they could throw errors with misconfigured email settings
                            btnSales.IsEnabled = false;
                            btnFinance.IsEnabled = false;
                            btnSettings.IsEnabled = false;
                            MessageDialog messageDialog = new MessageDialog() //Instructional message 
                            {
                                message = "Please setup email settings and messages in admin settings"
                            };
                            messageDialog.ShowDialog();
                            btnAdmin.IsChecked = true; //Switch to admin settings
                        }
                        else //If user says no, disable the email system
                        {
                            ProgramConfiguration programConfiguration = ProgramConfigurationManagement.GetAllProgramConfiguration();
                            programConfiguration.emailEnabled = false;
                            ProgramConfigurationManagement.SetProgramConfiguration(programConfiguration);
                            
                        }
                    }
                    else
                    {
                        ErrorDialog errorDialog = new ErrorDialog() //Display an error message if the user doesn't have admin rights since they won't have access to tha admin interface
                        {
                            errorMessage = "Error with email system. Login as an admin user to resolve"
                        };
                        errorDialog.ShowDialog();
                        this.Close();
                    }
            }
                
            
        }

        private void btnFinance_Checked(object sender, RoutedEventArgs e) //Switch to finance screen
        {
            tabSwitcher.SelectedIndex = 4;
            bdrTitleBackground.Background = new SolidColorBrush(Color.FromRgb(32, 159, 110));
            txbTitleText.Text = "Finance";
            if (session != null)
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(session, "Accessed finance management");
            }
        }

        private void btnRecheckEmail_Click(object sender, RoutedEventArgs e) //Code for the recheck email button
        {
            //Runs the same verification process as CheckEmailStatus
            if ((!File.Exists(ProgramConfigurationManagement.GetDataPath() + "/email_configuration.json") | !File.Exists(ProgramConfigurationManagement.GetDataPath() + "/email_messages.json")) && ProgramConfigurationManagement.IsEmailEnabled())
            {
                //Email system is still not fully configured. Tell the user this and do not exit confined mode.
                MessageDialog messageDialog = new MessageDialog()
                {
                    message = "Email system still not configured"
                };
                messageDialog.ShowDialog();
            }
            else
            {
                //Email system is fully configured. Tell the user this and exit confined mode
                MessageDialog messageDialog = new MessageDialog()
                {
                    message = "Email system correctly configured"
                };
                messageDialog.ShowDialog();
                btnSales.IsEnabled = true; //Re-enable disabled functions
                btnStock.IsEnabled = true;
                btnFinance.IsEnabled = true;
                btnSettings.IsEnabled = true;
                btnRecheckEmail.Visibility = Visibility.Hidden;
            }
        }
    }


}
