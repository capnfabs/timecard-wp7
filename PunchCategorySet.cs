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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Timecard
{
    public class PunchCategorySet
    {
        public ObservableCollection<PunchCategory> PunchCategories { set; get; }
        public PunchCategorySet()
        {
            PunchCategories = new ObservableCollection<PunchCategory>();
        }
        public void Defaults()
        {
            PunchCategory startWork = new PunchCategory(1, Color.FromArgb(255, 27, 161, 226), "Start Work", PunchTypes.StartWork);
            startWork.Followers.Add(2);
            startWork.Followers.Add(4);
            startWork.Followers.Add(6);
            PunchCategory lunch = new PunchCategory(2, Color.FromArgb(255, 140, 191, 38), "Lunch", PunchTypes.StartBreak);
            lunch.Followers.Add(3);
            PunchCategory endlunch = new PunchCategory(3, Color.FromArgb(255, 140, 191, 38), "End Lunch", PunchTypes.EndBreak);
            endlunch.Followers.Add(2);
            endlunch.Followers.Add(4);
            endlunch.Followers.Add(6);
            PunchCategory startProject = new PunchCategory(4, Color.FromArgb(255, 191, 140, 38), "BeAwesome", PunchTypes.StartProject);
            endlunch.Followers.Add(5);
            PunchCategory endProject = new PunchCategory(5, Color.FromArgb(255, 191, 140, 38), "End BeAwesome", PunchTypes.EndProject);
            endlunch.Followers.Add(2);
            endlunch.Followers.Add(4);
            endlunch.Followers.Add(6);
            PunchCategory endWork = new PunchCategory(6, Color.FromArgb(255, 27, 161, 226), "End Work", PunchTypes.EndWork);
            PunchCategories.Add(startWork);
            PunchCategories.Add(lunch);
            PunchCategories.Add(endlunch);
            PunchCategories.Add(startProject);
            PunchCategories.Add(endProject);
            PunchCategories.Add(endWork);

        }
    }
    public class PunchCategory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Color _punchColor;
        String _punchLabel;
        PunchTypes _punchType;
        int _punchCatID;
        ObservableCollection<int> _followers;

        public ObservableCollection<int> Followers
        {
            set
            {
                _followers = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Followers"));
            }
            get
            {
                return _followers;
            }
        }

        public int PunchCatID
        {
            set
            {
                _punchCatID = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PunchCatID"));
            }
            get
            {
                return _punchCatID;
            }
        }
        public Color PunchColor
        {
            set
            {
                _punchColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PunchColor"));
            }
            get
            {
                return _punchColor;
            }
        }
        public String PunchLabel
        {
            set
            {
                _punchLabel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PunchLabel"));
            }
            get
            {
                return _punchLabel;
            }
        }

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
        
        public PunchCategory(int ID, Color color, String label, PunchTypes type)
        {
            PunchCatID = ID;
            PunchColor = color;
            PunchLabel = label;
            PunchType = type;
            Followers = new ObservableCollection<int>;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
    }
}
