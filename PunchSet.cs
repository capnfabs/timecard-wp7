using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;

namespace Timecard
{
    public enum PunchTypes
    {
        StartWork,
        EndWork,
        StartProject,
        EndProject,
        StartBreak,
        EndBreak
    }
    public class PunchSet
    {
        public ObservableCollection<Punch> Punches { get; set; }

        public PunchSet ()
        {
            Punches = new ObservableCollection<Punch>();
        }

    }
    public class Punch : INotifyPropertyChanged
    {
        private DateTime _time;
        private String _punchLabel;
        private PunchTypes _punchType;
        public event PropertyChangedEventHandler PropertyChanged;

        public PunchTypes PunchType
        {
            get
            {
                return _punchType;
            }
            set
            {
                _punchType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PunchType"));
            }
        }

        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Time"));
            }
        }
        public String PunchLabel
        {
            get
            {
                return _punchLabel;
            }
            set
            {
                _punchLabel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PunchLabel"));
            }
        }
        public Punch()
        {
            Time = new DateTime();
            PunchLabel = "";
        }
        public Punch(DateTime aTime, String aPunchLabel, PunchTypes aPunchType)
        {
            Time = aTime;
            PunchLabel = aPunchLabel;
            PunchType = aPunchType;
        }
        
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        { 
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
    }
}
