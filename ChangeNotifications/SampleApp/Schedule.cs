using ChangeNotifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SampleApp
{
    public class Schedule : Changeable, INotifyPropertyChanged
    {
        private DayOfWeek weekStart;
        public DayOfWeek WeekStart { get => weekStart; set => Change(() => { weekStart = value; }); }

        private DayOfWeek weekFinish;
        public DayOfWeek WeekFinish { get => weekFinish; set => Change(() => { weekFinish = value; }); }

        private TimeSpan dayStart;
        public TimeSpan DayStart { get => dayStart; set => Change(() => { dayStart = value; }); }

        private TimeSpan dayDuration;
        public TimeSpan DayDuration { get => dayDuration; set => Change(() => { dayDuration = value; }); }
    }
}
