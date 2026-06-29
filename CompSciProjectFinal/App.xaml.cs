using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CompSciProjectFinal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (System.Diagnostics.Debugger.IsAttached) //If the program is running in debug mode don't use the error dialog window
            {
                StartupOperations();
            }
            else
            {
                try
                {
                    StartupOperations();
                }
                catch (Exception ex)
                {
                    ErrorDialog errorDialog = new ErrorDialog()
                    {
                        errorMessage = "Unexpected error:" + System.Environment.NewLine + ex.Message,
                    };
                    errorDialog.ShowDialog();
                    this.Shutdown();
                }
            }

        }

        private void StartupOperations()
        {
            if (ProgramConfigurationManagement.CheckIfProgramDataFolderExists())
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                this.Shutdown();
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog()
                {
                    message = "Select the folder you want program data to be stored in"
                };
                messageDialog.ShowDialog();
                ProgramConfigurationManagement.CreateNewProgramDataFolder();
                NewUserWindow newUserWindow = new NewUserWindow()
                {
                    forceUserAdmin = true //No users will be in the database so force the new user to have admin rights
                };
                newUserWindow.ShowDialog();
                StartupOperations();
                this.Shutdown();
            }
        }
    }
}
