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
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>f
    public partial class ErrorDialog : Window //Dialog box for notifying the user of errors
    {
        public string errorMessage { get; set; }
        
        public ErrorDialog()
        {
            InitializeComponent();
            this.DataContext = this;
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

        private void btnOk_Click(object sender, RoutedEventArgs e) //OK button closes window
        {
            this.Close(); //Closes the dialog
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Media.SystemSounds.Exclamation.Play(); //Plays exclamation sound when window is opened
        }
    }


}
