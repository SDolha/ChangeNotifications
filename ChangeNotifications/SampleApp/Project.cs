using ChangeNotifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SampleApp
{
    public class Project : Changeable, INotifyPropertyChanged
    {
        private string title;
        public string Title { get => title; set => Change(() => { title = value; }); }

        private Schedule schedule;
        public Schedule Schedule { get => schedule; set => Change(() => { schedule = value; }); }

        private ObservableCollection<Task> tasks;
        public ObservableCollection<Task> Tasks { get => tasks; set => Change(() => { tasks = value; }); }

        private ObservableCollection<string> descriptionLines;
        public ObservableCollection<string> DescriptionLines { get => descriptionLines; set => Change(() => { descriptionLines = value; }); }
    }
}
