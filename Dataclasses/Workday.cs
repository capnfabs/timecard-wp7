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
using System.Windows.Threading;

namespace Timecard
{
    public class Workday : AppDataObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime _startTime;
        private DateTime _endTime;
        private static DateTime _nullTime; //used to indicate whether the other DateTimes have been set.
        private ObservableCollection<PunchSegment> _segments;

        private DispatcherTimer tmr;

        public Workday()
        {
            WorkSegments = new ObservableCollection<PunchSegment>();

            tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(60);
            tmr.Tick += OnTimerTick;
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

        public TimeSpan NetDuration
        {
            get
            {
                TimeSpan breakTime = TimeSpan.FromMilliseconds(0);
                foreach (PunchSegment s in WorkSegments)
                {
                    if (s.SegmentCategory == Category.Break)
                    {
                        breakTime += s.Duration;
                    }
                }
                if (Ended)
                {
                    tmr.Stop();
                    return _endTime - _startTime - breakTime;
                }
                else if (Started)
                {
                    tmr.Start();
                    return DateTime.Now - _startTime - breakTime;
                }
                else
                {
                    tmr.Stop();
                    return TimeSpan.FromMilliseconds(0);
                }
            }
        }

        public ObservableCollection<PunchSegment> WorkSegments
        {
            get
            {
                return _segments;
            }
            set
            {
                _segments = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkSegments"));
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
                OnPropertyChanged(new PropertyChangedEventArgs("Started"));
                OnPropertyChanged(new PropertyChangedEventArgs("NetDuration"));
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
                OnPropertyChanged(new PropertyChangedEventArgs("Ended"));
                OnPropertyChanged(new PropertyChangedEventArgs("NetDuration"));
            }
        }

        private void OnTimerTick(Object sender, EventArgs args)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("NetDuration"));
        }

        public void Punch(PunchType p)
        {
            if (p.PunchCategory == Category.Meta)
            {
                if (p.StartorEnd == StartEnd.Start)
                {
                    StartTime = DateTime.Now;
                }
                else
                {
                    EndTime = DateTime.Now;
                }
            }
            else if (p.StartorEnd == StartEnd.Start) //we've started a project or break.
            {
                //check that there aren't any unended segments.
                if (WorkSegments.Count > 0)
                {
                    PunchSegment segment = WorkSegments[WorkSegments.Count - 1];
                    if (!segment.Ended)
                    {
                        //auto-fill in the blank.
                        segment.EndTime = DateTime.Now;
                        segment.EndLabel = "End " + (p.PunchCategory == Category.Break ? "Break" : "Project");
                    }
                }
                //make a new segment and start it.
                PunchSegment newSeg = new PunchSegment();
                newSeg.StartLabel = p.Label;
                newSeg.SegmentCategory = p.PunchCategory;
                newSeg.StartTime = DateTime.Now;
                WorkSegments.Add(newSeg);
            }
            else if (p.StartorEnd == StartEnd.End)
            {
                //if the segment types don't match throw an exception.
                PunchSegment segment = WorkSegments[WorkSegments.Count - 1];
                if (segment.SegmentCategory != p.PunchCategory)
                    throw new Exception("Current Segment Type does not match the punch type.");
                //otherwise
                if (!segment.Ended)
                {
                    segment.EndLabel = p.Label;
                    segment.EndTime = DateTime.Now;
                }
            }
        }


        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }

        protected override void SaveAction()
        {
            Util.IsolatedStorageHelper.ObjToFile(this, typeof(Timecard.Workday), this.Path);
        }

        protected override void LoadAction()
        {
            Util.IsolatedStorageHelper.FileToObj(typeof(Timecard.Workday), this.Path);
        }

        public override void DefaultsAction()
        {
            //do nothing!
        }
    }
}
