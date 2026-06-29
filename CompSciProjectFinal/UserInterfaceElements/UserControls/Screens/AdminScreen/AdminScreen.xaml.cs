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
    /// Interaction logic for AdminScreen.xaml
    /// </summary>
    public partial class AdminScreen : UserControl //Admin screen user control. contains a simple list of buttons that allow the user to access admin functions
    {
        public MainWindow containingWindow;
        public AdminScreen()
        {
            InitializeComponent();
        }

        private void btnSessions_Click(object sender, RoutedEventArgs e) //Open sessions viewer window
        {
            SessionSelectWindow sessionSelectWindow = new SessionSelectWindow()
            {
                isAdminAccessed = true,
                containingWindow = containingWindow
            };
            sessionSelectWindow.Show();
            if (containingWindow.session != null) //This is to prevent the window attempting to add an event when session is null. It would usually happen when the window is starting
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Opened session viewer"); //add session log
            }
        }

        private void btnViewDB_Click(object sender, RoutedEventArgs e) //Launches database viewer window
        {
            DatabaseViewerWindow databaseViewerWindow = new DatabaseViewerWindow();
            databaseViewerWindow.Show();
            if (containingWindow.session != null) //This is to prevent the window attempting to add an event when session is null. It would usually happen when the window is starting
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Opened DB viewer"); //add session log
            }
        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e) //launces new user window
        {
            NewUserWindow newUserWindow = new NewUserWindow();
            newUserWindow.ShowDialog();
            if (containingWindow.session != null) //This is to prevent the window attempting to add an event when session is null. It would usually happen when the window is starting
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Opened new user window"); //add session log
            }
        }

        private void btnLoginAsUser_Click(object sender, RoutedEventArgs e) //Allows the user to access the account settings screen from the perspective of another user
        {
            UserSelectionWindow userSelectionWindow = new UserSelectionWindow(); //Opens a window prompting the user tpo select a user to view
            userSelectionWindow.ShowDialog();
            if (userSelectionWindow.selectedUserId != 0) 
            {
                UserDetails userDetails = DatabaseManagementV2.GetUserDetailsByUserId(userSelectionWindow.selectedUserId);
                Session session = SessionsAndAnalyticsManagement.StartAndReturnNewSession(userDetails);
                MainWindow mainWindow = new MainWindow()  //since the settings screen depends upon a main window, launch a mainwindow instance and disable the inncecessary functions
                {
                    userDetails = userDetails,//passes required details to window
                    session = session,
                    isAccessedViaAdmin = true
                }; //main window
                SessionsAndAnalyticsManagement.AddNewEventToSession(session, "Accessed via admin login"); //add to log
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Accessed user:" + userDetails.display_name + "(ID: " + userDetails.user_id.ToString() + ")");
                mainWindow.ShowDialog();//open main window
            }
        }

        private void btnEmailMessages_Click(object sender, RoutedEventArgs e) //opens email messages window
        {
            EmailManagementWindow emailManagementWindow = new EmailManagementWindow();
            emailManagementWindow.ShowDialog();
        }

        private void btnEmailSettings_Click(object sender, RoutedEventArgs e) //opens email settings window
        {
            EmailSettingsWindow emailSettingsWindow = new EmailSettingsWindow();
            emailSettingsWindow.ShowDialog();
            if (containingWindow.btnRecheckEmail.Visibility == Visibility.Hidden) //Checks to see if window is in email configuration mode
            {
                containingWindow.CheckEmailStatus(); //checks email configuration
            }
            
        }
    }
}
