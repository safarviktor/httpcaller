using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Interop;

namespace HttpCaller
{
    public partial class MainWindow : Window
    {
        private HttpClient _httpClient = new HttpClient();
        private bool _shouldRun = false;
        private List<long> _timesTaken;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void gobutton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(inputurl.Text))
            {
                return;
            }
            try
            {
                var uri = new Uri(inputurl.Text);
            }
            catch
            {
                MessageBox.Show("Not a valid URI");
                return;
            }
            

            logoutput.Text = $"Calling{Environment.NewLine}{inputurl.Text}";
            _shouldRun = true;

            _ = Go();
        }

        private async Task Go()
        {
            var timer = new Stopwatch();
            _timesTaken = new List<long>();

            while (_shouldRun) 
            {
                timer.Restart();
                var response = await _httpClient.GetAsync(inputurl.Text);
                timer.Stop();
                var msg = $"{response.StatusCode}\t{timer.ElapsedMilliseconds} ms";
                _timesTaken.Add(timer.ElapsedMilliseconds);
                await Dispatcher.BeginInvoke(() =>
                {
                    logoutput.Text += $"{Environment.NewLine}{msg}";
                    logoutput.ScrollToEnd();
                });                
            }

            await Dispatcher.BeginInvoke(() =>
            {
                var msg = $"Average: {_timesTaken.Average()}{Environment.NewLine}";
                msg += $"Min: {_timesTaken.Min()}{Environment.NewLine}";
                msg += $"Max: {_timesTaken.Max()}{Environment.NewLine}";
                msg += $"Attempts: {_timesTaken.Count}";
                logoutput.Text += $"{Environment.NewLine}{msg}";
            });
        }

        private void stopbutton_Click(object sender, RoutedEventArgs e)
        {
            _shouldRun = false;
            logoutput.Text += $"{Environment.NewLine}Requested stop";
        }
    }
}