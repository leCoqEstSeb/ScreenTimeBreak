using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
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
        string currentWorkTime = string.Empty;
        string currentChillTime = string.Empty;
        TimerSettings timer;
        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(timer_tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            if (File.Exists(".\\TimerSettings.json"))
            {
                TimerSettings lastTimerSettings = JsonSerializer.Deserialize<TimerSettings>(File.ReadAllText(".\\TimerSettings.json"));
                timer = new(lastTimerSettings.WorkTimeLimit, lastTimerSettings.ChillTimeLimit);
            }
            else
            {
                timer = new("50", "10");
            }
            this.DataContext = timer;
            workWatch.Start();

            Closing += OnWindowClosing;
        }
        void timer_tick(object sender, EventArgs e)
        {
            TimeSpan wts = workWatch.Elapsed;
            currentWorkTime = String.Format("{0:00}:{1:00}:{2:00}", wts.Hours, wts.Minutes, wts.Seconds);
            lblWorkTime.Content = currentWorkTime;

            TimeSpan cts = chillWatch.Elapsed;
            currentChillTime = String.Format("{0:00}:{1:00}:{2:00}", cts.Hours, cts.Minutes, cts.Seconds);
            lblChillTime.Content = currentChillTime;

            if (TimeSpan.Compare(workWatch.Elapsed, new TimeSpan(0, int.Parse(timer.WorkTimeLimit), 0)) == 1 && workWatch.IsRunning == true)
            {
                this.WindowState = WindowState.Normal;
                this.Activate();
            }
        }

        private void btnChill_Click(object sender, RoutedEventArgs e)
        {
            workWatch.Stop();
            chillWatch.Start();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            workWatch.Restart();
            chillWatch.Reset();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            string serializeTimerSettings = JsonSerializer.Serialize(timer);
            File.WriteAllText(".\\TimerSettings.json", serializeTimerSettings);
        }
    }
}
