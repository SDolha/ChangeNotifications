using ChangeNotifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SampleApp
{
    public class Task : Changeable, INotifyPropertyChanged
    {
        private string title;
        public string Title { get => title; set => Change(() => { title = value; }); }

        private DateTime start;
        public DateTime Start { get => start; set => Change(() => { start = value; }); }

        private TimeSpan duration;
        public TimeSpan Duration { get => duration; set => Change(() => { duration = value; }); }

        private ObservableCollection<Resource> assignments;
        public ObservableCollection<Resource> Assignments { get => assignments; set => Change(() => { assignments = value; }); }
    }
}
