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
    /// Interaction logic for SessionSelectionUserControl.xaml
    /// </summary>
    public partial class SessionSelectionUserControl : UserControl
    {
        public SessionSelectionUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        
        public Session session { get; set; }
        public bool isAdminAccessed { get; set; }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            SessionEventsViewerWindow sessionEventsViewerWindow = new SessionEventsViewerWindow()
            {
                session = session
            };
            sessionEventsViewerWindow.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txbSessionId.Text = "#" + session.sessionId.ToString();
            txbSessionTime.Text = DataFunctions.FindPrimaryKeyTime(session.sessionId).ToString("dd/MM/yyyy HH:mm");
            if (isAdminAccessed)
            {
                bdrBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(236, 65, 65));

            }
            else
            {
                bdrBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(211, 88, 176));
            }
            
        }
    }
}
