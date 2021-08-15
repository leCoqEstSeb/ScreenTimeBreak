using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ScreenTimeBreak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Stopwatch workWatch = new Stopwatch();
        Stopwatch chillWatch = new Stopwatch();
        TimerSettings timer;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        bool allowAlarm = true;
        int alarmRepititionsCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(timer_tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            if (File.Exists(".\\TimerSettings.json"))
            {
                TimerSettings lastTimerSettings = JsonSerializer.Deserialize<TimerSettings>(File.ReadAllText(".\\TimerSettings.json"));
                timer = new(lastTimerSettings.WorkTimeLimit, lastTimerSettings.ChillTimeLimit, lastTimerSettings.AlarmRepititions);
            }
            else
            {
                timer = new(50, 10, 1);
            }
            this.DataContext = timer;
            workWatch.Start();
            mediaPlayer.Open(new Uri("C:\\Windows\\Media\\Alarm05.wav"));
            mediaPlayer.MediaEnded += OnMediaEnded;
            Deactivated += OnWindowDeactivated;
            Closing += OnWindowClosing;
        }
        #region "Event handling"
        void timer_tick(object sender, EventArgs e)
        {
            TimeSpan wts = workWatch.Elapsed;
            lblWorkTime.Content = String.Format("{0:00}:{1:00}:{2:00}", wts.Hours, wts.Minutes, wts.Seconds);
            TimeSpan cts = chillWatch.Elapsed;
            lblChillTime.Content = String.Format("{0:00}:{1:00}:{2:00}", cts.Hours, cts.Minutes, cts.Seconds);

            if (IsTimeLimitReached(workWatch, timer.WorkTimeLimit))
            {
                lblWorkTime.Foreground = new SolidColorBrush(System.Windows.Media.Colors.OrangeRed);
                this.WindowState = WindowState.Normal;
                this.Activate();
            }
            if (IsTimeLimitReached(chillWatch, timer.ChillTimeLimit))
            {
                this.WindowState = WindowState.Normal;
                this.Activate();

                if(allowAlarm & alarmRepititionsCount < timer.AlarmRepititions)
                {
                    lblChillTime.Foreground = new SolidColorBrush(System.Windows.Media.Colors.OrangeRed);
                    mediaPlayer.Play();
                    allowAlarm = false;
                    alarmRepititionsCount++;
                }
            }
        }
        private void btnChill_Click(object sender, RoutedEventArgs e)
        {
            lblWorkTime.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Black);
            workWatch.Stop();
            chillWatch.Start();
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            lblChillTime.Foreground = new SolidColorBrush(Colors.Black);
            workWatch.Restart();
            chillWatch.Reset();
            mediaPlayer.Stop();
            alarmRepititionsCount = 0;
        }
        void OnWindowClosing(object sender, CancelEventArgs e)
        {
            string serializeTimerSettings = JsonSerializer.Serialize(timer);
            File.WriteAllText(".\\TimerSettings.json", serializeTimerSettings);
        }
        void OnWindowDeactivated(object sender, EventArgs e)
        {
            tgbPopup.IsChecked = false;
        }
        private void OnMediaEnded(object sender, EventArgs e)
        {
            allowAlarm = true;
            mediaPlayer.Stop();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process p = new Process();
            p.StartInfo.FileName = e.Uri.AbsoluteUri;
            p.StartInfo.UseShellExecute = true;
            p.Start();
            e.Handled = true;
        }
        #endregion
        #region "Methods"
        static bool IsTimeLimitReached(Stopwatch stopWatch, int limit)
        {
            if (stopWatch.IsRunning == false) { return false; }
            int result = TimeSpan.Compare(stopWatch.Elapsed, new TimeSpan(0, limit, 0));
            return result == 1;
        }
        #endregion
    }
}
