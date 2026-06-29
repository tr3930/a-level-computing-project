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
    /// Interaction logic for UserSelectionWindow.xaml
    /// </summary>
    public partial class UserSelectionWindow : Window
    {
        public int selectedUserId;
        public UserSelectionWindow()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<UserDetails> userDetails = DatabaseManagementV2.GetAllUserDetails(); //Get customers from DB
            for (int i = 0; i < userDetails.Count; i++) //Populate window
            {
                stpItems.Children.Add(new UserSelectionUserControl() //Add customer details to window
                {
                    Height = 50,
                    UserDisplayName = userDetails[i].display_name,
                    Id = userDetails[i].user_id,
                    Margin = new Thickness(10,3,3,3),
                    containingWindow = this
                }) ;
            }
        }



    }


}
