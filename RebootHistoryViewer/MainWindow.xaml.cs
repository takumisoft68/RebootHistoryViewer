using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RebootHistoryViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now - TimeSpan.FromDays(31);

            var service = new RebootHistoryService();
            var history = service.QueryHistory(date);

            var logBuilder = new StringBuilder();
            history.ForEach(v =>
            {
                if(v.HistoryType == HistoryType.Reboot)
                {
                    logBuilder.AppendFormat(
                        "{0} ({1}) -> {2} ({3}) ... (reboot){4}",
                        v.ShutdownAt.ToString(),
                        v.ShutdownEventRecordId,
                        v.BootAt.ToString(),
                        v.BootEventRecordId,
                        Environment.NewLine);
                }
                else if(v.HistoryType == HistoryType.Shutdown)
                {
                    logBuilder.AppendFormat(
                        "{0} ({1}) -> {2} ({3}){4}",
                        v.ShutdownAt.ToString(),
                        v.ShutdownEventRecordId,
                        v.BootAt.ToString(),
                        v.BootEventRecordId,
                        Environment.NewLine);
                }
            });


            this.historyLogBox.Text = logBuilder.ToString();
        }
    }
}