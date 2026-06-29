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
    /// Interaction logic for EditNameAndTxtIdWindow.xaml
    /// </summary>
    public partial class EditNameAndTxtIdWindow : Window
    {
        public EditNameAndTxtIdWindow()
        {
            InitializeComponent();
        }
        public UserDetails userDetails;
        public bool isEditingTextId = false;
        public string enteredValue;

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Cannot be empty"
                };
                errorDialog.ShowDialog();
            }
            else
            {
                enteredValue = txtName.Text;
                this.Close();
            }
            
        }

        private void txtProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isEditingTextId)
            {
                txbTitlebar.Text = "Edit Username";
                txbInstruction.Text = "New Username:";
                txtName.Text = userDetails.text_user_id;
            }
            else
            {
                txtName.Text = userDetails.display_name;
            }
        }
    }


}
