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
using System.ComponentModel;

using Timecard.Util;

using System.Collections.ObjectModel;

namespace Timecard
{
    public class PunchTypeCollection : AppDataObject, INotifyPropertyChanged
    {
        ObservableCollection<PunchType> PunchTypes;

        public PunchTypeCollection()
        {
            PunchTypes = new ObservableCollection<PunchType>();
        }

        protected override void SaveAction()
        {
            IsolatedStorageHelper.ObjToFile(this, typeof(PunchTypeCollection), this.Path);
        }

        protected override AppDataObject LoadAction()
        {
            return (PunchTypeCollection)IsolatedStorageHelper.FileToObj(typeof(PunchTypeCollection), this.Path);
        }

        public override void DefaultsAction()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
