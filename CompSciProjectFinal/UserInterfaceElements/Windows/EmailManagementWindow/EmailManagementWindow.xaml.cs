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
    /// Interaction logic for EmailManagementWindow.xaml
    /// </summary>
    public partial class EmailManagementWindow : Window
    {
        EmailMessages emailMessages;
        int currentEdit = -1;
        public EmailManagementWindow()
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
            if (ProgramConfigurationManagement.DoesEmailMessagesFileExist())
            {
                emailMessages = EmailManagement.GetEmailMessages();
            }
            else
            {
                emailMessages = new EmailMessages()
                {
                    order_posted_email_text = "",
                    order_paid_email_text = "",
                    order_confirm_email_text = ""
                };
            }
            cboMessages.Items.Add("Request Payment");
            cboMessages.Items.Add("Recieved Payment");
            cboMessages.Items.Add("Order Posted");
            cboMessages.SelectedIndex = 0;
        }

        private void cboMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentEdit == 0)
            {
                emailMessages.order_confirm_email_text = txtMessage.Text;
            }
            else if (currentEdit == 1)
            {
                emailMessages.order_paid_email_text = txtMessage.Text;
            }
            else if (currentEdit == 2)
            {
                emailMessages.order_posted_email_text= txtMessage.Text;
            }
            if (cboMessages.SelectedIndex == 0)
            {
                txtMessage.Text = emailMessages.order_confirm_email_text;
            }
            else if (cboMessages.SelectedIndex == 1)
            {
                txtMessage.Text = emailMessages.order_paid_email_text;
            }
            else if (cboMessages.SelectedIndex == 2)
            {
                txtMessage.Text = emailMessages.order_posted_email_text;
            }
            currentEdit = cboMessages.SelectedIndex;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentEdit == 0)
            {
                emailMessages.order_confirm_email_text = txtMessage.Text;
            }
            else if (currentEdit == 1)
            {
                emailMessages.order_paid_email_text = txtMessage.Text;
            }
            else if (currentEdit == 2)
            {
                emailMessages.order_posted_email_text = txtMessage.Text;
            }
            if (emailMessages.order_paid_email_text != "" && emailMessages.order_confirm_email_text != "" && emailMessages.order_posted_email_text != "")
            {
                EmailManagement.SaveEmailMessages(emailMessages);
                MessageDialog messageDialog = new MessageDialog()
                {
                    message = "Email messages saved"
                };
                messageDialog.ShowDialog();
                this.Close();
            }
            else
            {
                ErrorDialog errorDialog = new ErrorDialog()
                {
                    errorMessage = "Please enter a value for every field"
                };
                errorDialog.ShowDialog();
            }
        }
    }


}
