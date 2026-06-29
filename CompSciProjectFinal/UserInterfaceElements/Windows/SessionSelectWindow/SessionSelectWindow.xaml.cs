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
    /// Interaction logic for SessionSelectWindow.xaml
    /// </summary>
    public partial class SessionSelectWindow : Window
    {
        public bool isAdminAccessed;
        public MainWindow containingWindow;
        List<UserDetails> users;
        
        public SessionSelectWindow()
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

        private void RefreshList(UserDetails userDetails)
        {
            stpItems.Children.Clear();
            List<Session> sessions = DatabaseManagementV2.GetAllSessionsForUser(userDetails);
            for (int i = 0; i < sessions.Count; i++)
            {
                stpItems.Children.Add(new SessionSelectionUserControl()
                {
                    session = sessions[i],
                    Height = 60,
                    Margin = new Thickness(10, 3, 3, 3),
                    isAdminAccessed = isAdminAccessed
                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isAdminAccessed)
            {
                users = DatabaseManagementV2.GetAllUserDetails();
                for (int i = 0; i < users.Count; i++)
                {
                    cboUsers.Items.Add(users[i].display_name);
                }
                cboUsers.SelectedIndex = 0;
            }
            else
            {
                grdMainGrid.RowDefinitions.RemoveAt(2);
                cboUsers.Visibility = Visibility.Hidden;
                RefreshList(containingWindow.userDetails);
            }
            
        }

        private void cboUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshList(users[cboUsers.SelectedIndex]);
        }
    }


}
