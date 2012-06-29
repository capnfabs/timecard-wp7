using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media;
using System.ComponentModel;

namespace Timecard
{
    public class PunchSegment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime _startTime;
        private DateTime _endTime;
        private static DateTime _nullTime; //used to indicate whether the other DateTimes have been set.
        private DispatcherTimer tmr;
        private Color _color;
        private String _startLabel;
        private String _endLabel;
        private Category _segmentCategory;

        public PunchSegment()
        {
            tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(60);
            tmr.Tick += OnTimerTick;
        }

        public Category SegmentCategory
        {
            get
            {
                return _segmentCategory;
            }
            set
            {
                _segmentCategory = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SegmentCategory"));
            }
        }

        public String StartLabel
        {
            get
            {
                return _startLabel;
            }
            set
            {
                _startLabel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StartLabel"));
            }
        }

        public bool Started
        {
            get
            {
                return !(_startTime == _nullTime);
            }
        }

        public bool Ended
        {
            get
            {
                return !(_endTime == _nullTime);
            }
        }

        public String EndLabel
        {
            get
            {
                return _endLabel;
            }
            set
            {
                _endLabel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EndLabel"));
            }
        }
        public Color SegmentColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SegmentColor"));
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (_startTime == _nullTime)
                {
                    return new TimeSpan(0);
                }

                if (_endTime == _nullTime)
                {
                    return DateTime.Now.Subtract(_startTime);
                }

                return _endTime.Subtract(_startTime);
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                
                tmr.Start();

                OnPropertyChanged(new PropertyChangedEventArgs("StartTime"));
                OnPropertyChanged(new PropertyChangedEventArgs("Duration"));
                OnPropertyChanged(new PropertyChangedEventArgs("Started"));
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                tmr.Stop();
                OnPropertyChanged(new PropertyChangedEventArgs("EndTime"));
                OnPropertyChanged(new PropertyChangedEventArgs("Duration"));
                OnPropertyChanged(new PropertyChangedEventArgs("Ended"));
            }
        }

        private void OnTimerTick(Object sender, EventArgs args)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Duration"));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
    }
}
