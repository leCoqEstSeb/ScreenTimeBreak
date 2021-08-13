namespace ScreenTimeBreak
{
    public class TimerSettings
    {

        private string _workTimeLimit;
        private string _chillTimeLimit;

        public string WorkTimeLimit
        {
            get { return _workTimeLimit; }
            set { _workTimeLimit = value; }
        }

        public string ChillTimeLimit
        {
            get { return _chillTimeLimit; }
            set { _chillTimeLimit = value; }
        }

        public TimerSettings(string workTimeLimit, string chillTimeLimit)
        {
            WorkTimeLimit = workTimeLimit;
            ChillTimeLimit = chillTimeLimit;
        }
    }
}
