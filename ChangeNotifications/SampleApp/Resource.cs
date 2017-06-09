using ChangeNotifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SampleApp
{
    public class Resource : Changeable, INotifyPropertyChanged
    {
        private string firstName;
        public string FirstName { get => firstName; set => Change(() => { firstName = value; }); }

        private string lastName;
        public string LastName { get => lastName; set => Change(() => { lastName = value; }); }
    }
}
