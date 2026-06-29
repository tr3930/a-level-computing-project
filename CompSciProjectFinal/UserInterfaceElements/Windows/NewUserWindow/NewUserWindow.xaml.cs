using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        UserDetails userDetails = new UserDetails(); //Customer details
        public bool forceUserAdmin = false;
        
        public NewUserWindow()
        {
            InitializeComponent();
        }
        

        private void btnClose_Click(object sender, RoutedEventArgs e) //these are commented in other windows
        {
            this.Close();
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
            if (forceUserAdmin) //Can be used to force the user to have admin rights. Will be used for first time setup
            {
                txbAdmin.Text = "User will have admin rights";
                chkAdmin.Visibility = Visibility.Hidden;
                chkAdmin.IsChecked = true;
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool allChecksPassed = true;
            if (txtUserId.Text == "") //Prescence check for user id
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "First name was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (txtDisplayName.Text == "") //Prescence check for name
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Last name was not entered"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            else if (DatabaseManagementV2.VerifyExistenceOfUserByTextId(txtUserId.Text)) //Does someonbe else have the same text id?
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "A user with this username already exists"
                };
                errorDialog.ShowDialog();
                allChecksPassed = false;
            }
            if (allChecksPassed) //If everything is ok, add to database
            {
                if (chkAdmin.IsChecked == true) //Does user have admin access?
                {
                    userDetails.admin_access = 1;
                }
                else
                {
                    userDetails.admin_access = 0;
                }
                userDetails.text_user_id = txtUserId.Text;
                userDetails.display_name = txtDisplayName.Text;
                DatabaseManagementV2.InsertNewUser(userDetails);
                this.Close();
            }
        }
    }


}
