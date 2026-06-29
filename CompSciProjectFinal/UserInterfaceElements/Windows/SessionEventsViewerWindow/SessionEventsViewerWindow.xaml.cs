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
    /// Interaction logic for SessionEventsViewerWindow.xaml
    /// </summary>
    public partial class SessionEventsViewerWindow : Window //Window to view session events
    {
        public Session session;
        public MainWindow containingWindow;
        
        public SessionEventsViewerWindow()
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

        private void RefreshList() //Refreshes the list of events from the database
        {
            lstEvents.Items.Clear(); //Clear events list
            List<SessionEvent> events = DatabaseManagementV2.GetAllEventsForSession(session); //get events from database
            
            for (int i = 0; i < events.Count; i++) //Add events to list
            {
                lstEvents.Items.Add(DataFunctions.FindPrimaryKeyTime(events[i].eventId).ToString("dd/MM/yyyy HH:mm.ss") + " " + events[i].comment); //format string with the time and the comment
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txbTitlebar.Text = "Details for session #" + session.sessionId.ToString(); //Set text on titlebar
            RefreshList(); //fill the list
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (containingWindow.session != null) //Add a session log
            {
                SessionsAndAnalyticsManagement.AddNewEventToSession(containingWindow.session, "Refreshed events for session #" + session.sessionId.ToString());
            }
            RefreshList(); //refresh the list of details
        }
    }


}
