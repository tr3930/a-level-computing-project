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
using System.IO;
using System.Net.Mail;
using System.Net;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for EmailSettingsWindow.xaml
    /// </summary>
    public partial class EmailSettingsWindow : Window
    {
        EmailConfigurationDetails emailConfigurationDetails;
        
        public EmailSettingsWindow()
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
            if (File.Exists(ProgramConfigurationManagement.GetDataPath() + "/email_configuration.json"))
            {
                emailConfigurationDetails = EmailManagement.GetEmailConfiguration();
            }
            else
            {
                emailConfigurationDetails = new EmailConfigurationDetails()
                {
                    email_is_ssl = true,
                    email_address = "",
                    email_password = "",
                    email_server = "",
                    email_server_port = 0
                };
            }
            txtEmailAddress.Text = emailConfigurationDetails.email_address;
            txtPassword.Text = emailConfigurationDetails.email_password;
            txtPort.Text = emailConfigurationDetails.email_server_port.ToString();
            txtSmtp.Text = emailConfigurationDetails.email_server;
            chkEnabled.IsChecked = ProgramConfigurationManagement.IsEmailEnabled();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ProgramConfiguration programConfiguration = ProgramConfigurationManagement.GetAllProgramConfiguration();
            if (chkEnabled.IsChecked == false)
            {
                programConfiguration.emailEnabled = false;
                ProgramConfigurationManagement.SetProgramConfiguration(programConfiguration);
                this.Close();
            }
            else
            {
                bool allChecksPassed = true;
                if (txtEmailAddress.Text == "") //Prescence check for user id
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "Email was not entered"
                    };
                    errorDialog.ShowDialog();
                    allChecksPassed = false;
                }
                else if (!Checking.ValidateEmailAddressFormatting(txtEmailAddress.Text))
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "Email formatting not valid"
                    };
                    errorDialog.ShowDialog();
                    allChecksPassed = false;
                }
                if (txtPassword.Text == "") //Prescence check for name
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "Password was not entered"
                    };
                    errorDialog.ShowDialog();
                    allChecksPassed = false;
                }
                if (txtSmtp.Text == "") //Prescence check for name
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "SMTP Server was not entered"
                    };
                    errorDialog.ShowDialog();
                    allChecksPassed = false;
                }
                else if (!Checking.ValidateSmtpFormatting(txtSmtp.Text))
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "SMTP Server not in valid format"
                    };
                    errorDialog.ShowDialog();
                    allChecksPassed = false;
                }
                if (txtPort.Text == "") //Prescence check for name
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "SMTP Port was not entered"
                    };
                    errorDialog.ShowDialog();
                    allChecksPassed = false;
                }
                if (allChecksPassed) //If everything is ok, add to database
                {
                    
                    try
                    {
                        
                        emailConfigurationDetails.email_is_ssl = (bool)chkSsl.IsChecked;
                        emailConfigurationDetails.email_address = txtEmailAddress.Text;
                        emailConfigurationDetails.email_password = txtPassword.Text;
                        emailConfigurationDetails.email_server = txtSmtp.Text;
                        emailConfigurationDetails.email_server_port = int.Parse(txtPort.Text);
                        if (emailConfigurationDetails.email_server_port < 1 || emailConfigurationDetails.email_server_port > 65535)
                        {
                            throw new FormatException("Invalid port number");
                        }
                        MessageDialog messageDialog = new MessageDialog()
                        {
                            message = "A test email will now be sent to:" + System.Environment.NewLine + emailConfigurationDetails.email_address
                        };
                        messageDialog.ShowDialog();
                        SmtpClient smtpClient = new SmtpClient(emailConfigurationDetails.email_server)
                        {
                            Port = emailConfigurationDetails.email_server_port,
                            Credentials = new NetworkCredential(emailConfigurationDetails.email_address, emailConfigurationDetails.email_password),
                            EnableSsl = emailConfigurationDetails.email_is_ssl
                        };
                        try
                        {
                            smtpClient.Send(emailConfigurationDetails.email_address, emailConfigurationDetails.email_address, "Test email", "If you are reading this, it worked.");
                            YesNoDialog yesNoDialog = new YesNoDialog()
                            {
                                message = "Was the email recieved?"
                            };
                            yesNoDialog.ShowDialog();
                            if (yesNoDialog.choice)
                            {
                                EmailManagement.SaveEmailConfiguration(emailConfigurationDetails);
                                programConfiguration.emailEnabled = true;
                                ProgramConfigurationManagement.SetProgramConfiguration(programConfiguration);
                                this.Close();
                            }
                        }
                        catch (System.Net.Mail.SmtpException)
                        {
                            ErrorDialog errorDialog = new ErrorDialog()
                            {
                                errorMessage = "Could not send test email"
                            };
                            errorDialog.ShowDialog();
                        }
                    }
                    catch (System.FormatException)
                    {
                        ErrorDialog errorDialog = new ErrorDialog()
                        {
                            errorMessage = "Port not in correct format"
                        };
                        errorDialog.ShowDialog();
                    }
                }
            }
            
        }

        private void chkEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (chkEnabled.IsChecked == true)
            {
                txtEmailAddress.IsEnabled = true;
                txtPassword.IsEnabled = true;
                txtSmtp.IsEnabled = true;
                txtPort.IsEnabled = true;
                chkSsl.IsEnabled = true;
            }
            else
            {
                txtEmailAddress.IsEnabled = false;
                txtPassword.IsEnabled = false;
                txtSmtp.IsEnabled = false;
                txtPort.IsEnabled = false;
                chkSsl.IsEnabled = false;
            }
        }
    }


}
