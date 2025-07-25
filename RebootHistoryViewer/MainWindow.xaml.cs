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
                logBuilder.AppendFormat(
                    "{0} ({1}){2}",
                    v.ShutdownAt.ToString(),
                    v.ShutdownEventRecordId,
                    Environment.NewLine);
            });


            this.historyLogBox.Text = logBuilder.ToString();
        }
    }
}