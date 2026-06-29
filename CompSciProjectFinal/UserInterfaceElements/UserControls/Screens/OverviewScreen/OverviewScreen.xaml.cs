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
    /// Interaction logic for OverviewScreen.xaml
    /// </summary>
    public partial class OverviewScreen : UserControl //Overview screen displaying key statistics. Displayed in main window
    {
        public MainWindow enclosingWindow; //reference to main window
        public OverviewScreen()
        {
            InitializeComponent();
        }

        public void FillElements() //Fill the text fields with values from the database
        {
            OverviewScreenStats overviewScreenStats = DatabaseManagementV2.GetStatsForOverviewPage(); //get stats as an "OverviewScreenStats" object
            txbAllTimeEarnings.Text = overviewScreenStats.incomeAllTime.ToString("C"); //"C" formats as price
            txbWeeklyIncome.Text = overviewScreenStats.incomePastWeek.ToString("C");
            txbWeeklyOutgoing.Text = overviewScreenStats.outgoingsPastWeek.ToString("C");
            txbAllTimeSales.Text = overviewScreenStats.salesAllTime.ToString();
            txbWeeklySales.Text = overviewScreenStats.salesPastWeek.ToString();
        }
    }
}
