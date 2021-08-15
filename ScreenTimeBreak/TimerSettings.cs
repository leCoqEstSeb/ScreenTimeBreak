namespace ScreenTimeBreak
{
    public class TimerSettings
    {

        private int _workTimeLimit;
        private int _chillTimeLimit;
        private int _alarmRepititions;

        public int WorkTimeLimit
        {
            get { return _workTimeLimit; }
            set { _workTimeLimit = value; }
        }

        public int ChillTimeLimit
        {
            get { return _chillTimeLimit; }
            set { _chillTimeLimit = value; }
        }

        public int AlarmRepititions { get => _alarmRepititions; set => _alarmRepititions = value; }

        public TimerSettings(int workTimeLimit, int chillTimeLimit, int alarmRepititions)
        {
            WorkTimeLimit = workTimeLimit;
            ChillTimeLimit = chillTimeLimit;
            AlarmRepititions = alarmRepititions;
        }
    }
}
