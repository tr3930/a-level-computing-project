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
    /// Interaction logic for SettingsScreen.xaml
    /// </summary>
    public partial class SettingsScreen : UserControl
    {
        public SettingsScreen()
        {
            InitializeComponent();
        }
        public MainWindow containingWindow;

        public void RefreshDetails()
        {
            txbUsername.Text = "Username: " + containingWindow.userDetails.text_user_id; //Set username
            txbDisplayName.Text = "Name: " + containingWindow.userDetails.display_name; //Set display name
            txbCurrentSesison.Text = "Current Session: " + containingWindow.session.sessionId.ToString(); //Set session id
            txbLoginTime.Text = "Login Time: " + DataFunctions.FindPrimaryKeyTime(containingWindow.session.sessionId).ToString("dd/MM/yyyy HH:mm");
        }

        private void btnChangePin_Click(object sender, RoutedEventArgs e) //Changes the user's pin. Doesn't ask for pin unlike prototype becuase a pin is required to view the screen
        {
            PinWindow pinWindow = new PinWindow() //launch pin window in change mode
            {
                isWindowBeingUsedForPinChange = true
            };
            pinWindow.ShowDialog();
            Security.UpdateUserPassword(containingWindow.userDetails.text_user_id, pinWindow.txbPin.Text); //update pin
            MessageDialog messageDialog = new MessageDialog() //notify user of pin update
            {
                message = "PIN Changed"
            };
            messageDialog.ShowDialog();
        }

        private void btnChangeName_Click(object sender, RoutedEventArgs e)
        {
            EditNameAndTxtIdWindow editNameAndTxtIdWindow = new EditNameAndTxtIdWindow() //launch details change window for name
            {
                userDetails = containingWindow.userDetails
            };
            editNameAndTxtIdWindow.ShowDialog();
            if (editNameAndTxtIdWindow.enteredValue != null) //has save button been pressed?
            {
                UserDetails userDetails = containingWindow.userDetails; //get userdetails from mainwindow
                userDetails.display_name = editNameAndTxtIdWindow.enteredValue; //update display name
                DatabaseManagementV2.UpdateUserDisplayName(userDetails); //reflect this change in the db
                containingWindow.userDetails = userDetails; //update the userdetails on the main window
            };
        }

        private void btnChangeId_Click(object sender, RoutedEventArgs e)
        {
            EditNameAndTxtIdWindow editNameAndTxtIdWindow = new EditNameAndTxtIdWindow() //launch details change window for id
            {
                userDetails = containingWindow.userDetails,
                isEditingTextId = true
            };
            editNameAndTxtIdWindow.ShowDialog();
            if (editNameAndTxtIdWindow.enteredValue != null) //has save button been pressed?
            {
                if (!DatabaseManagementV2.VerifyExistenceOfUserByTextId(editNameAndTxtIdWindow.enteredValue))
                {
                    UserDetails userDetails = containingWindow.userDetails; //get userdetails from mainwindow
                    userDetails.text_user_id = editNameAndTxtIdWindow.enteredValue; //update text id
                    DatabaseManagementV2.UpdateTextUserId(userDetails); //reflect this change in the db
                    containingWindow.userDetails = userDetails; //update the userdetails on the main window
                }
                else
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "Another user with this id already exists"
                    };
                    errorDialog.ShowDialog();
                }
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDetails(); //Refresh the user details on load
        }

        private void btnViewEvents_Click(object sender, RoutedEventArgs e) //opens evnet viewer window for current session
        {
            if (containingWindow.session != null)
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Viewed events for this session");
            }
            SessionEventsViewerWindow sessionEventsViewerWindow = new SessionEventsViewerWindow()
            {
                session = containingWindow.session, //passing session details and mainwindow to the sesion window
                containingWindow = containingWindow
            };
            sessionEventsViewerWindow.Show();
        }

        private void btnPastSessions_Click(object sender, RoutedEventArgs e)
        {
            SessionSelectWindow sessionSelectWindow = new SessionSelectWindow() //Launch a session select window containing only sessions for the current user
            {
                isAdminAccessed = false,
                containingWindow = containingWindow
            };
            sessionSelectWindow.Show();
            if (containingWindow.session != null) //This is to prevent the window attempting to add an event when session is null. It would usually happen when the window is starting
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Opened session viewer");
            }
        }
    }
}
