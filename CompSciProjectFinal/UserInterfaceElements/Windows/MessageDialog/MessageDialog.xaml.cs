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
    /// Interaction logic for MessageDialog.xaml
    /// </summary>f
    public partial class MessageDialog : Window //A general dialog box for displaying messages
    {
        public string message { get; set; }
        public bool choice;
        
        public MessageDialog()
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
            this.Close(); //Close dialog
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Media.SystemSounds.Exclamation.Play(); //Plays exclamation sound when window is opened
        }
    }


}
