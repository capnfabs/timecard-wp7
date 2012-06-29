using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;

namespace Timecard
{
    public partial class PunchList : UserControl
    {
        public static readonly DependencyProperty PunchTypesProperty = DependencyProperty.Register("PunchTypes", typeof(ObservableCollection<PunchType>), typeof(PunchList), new PropertyMetadata(null));
        public static readonly DependencyProperty VisiblePunchTypesProperty = DependencyProperty.Register("VisiblePunchTypes", typeof(ObservableCollection<PunchType>), typeof(PunchList), new PropertyMetadata(null));

        public delegate void PunchPressHandler(object sender, PunchType punch);

        public event PunchPressHandler PunchPressed;

        protected void RaisePunchPressed(PunchType punch)
        {
            if (PunchPressed != null)
                PunchPressed(this, punch);
        }

        public PunchList()
        {
            PunchTypes = new ObservableCollection<PunchType>();
            InitializeComponent();
        }

        public ObservableCollection<PunchType> VisiblePunchTypes
        {
            get
            {
                return (ObservableCollection<PunchType>)GetValue(VisiblePunchTypesProperty);
            }
            set
            {
                SetValue(VisiblePunchTypesProperty, value);
            }
        }

        public ObservableCollection<PunchType> PunchTypes
        {
            get
            {
                return (ObservableCollection<PunchType>)GetValue(PunchTypesProperty);
            }
            set
            {
                SetValue(PunchTypesProperty, value);
                SetValue(VisiblePunchTypesProperty, value);
            }
        }

        public void SetLastPunch(PunchType punch)
        {
            VisiblePunchTypes = punch.FilterNext(PunchTypes);
        }

        private void LayoutRoot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
                return;
            //else
            PunchType punch = (PunchType)e.AddedItems[0];
            VisiblePunchTypes = punch.FilterNext(PunchTypes);
            RaisePunchPressed(punch);
            LayoutRoot.SelectedIndex = -1;
            LayoutRoot.ScrollIntoView(LayoutRoot.Items[0]);
        }
    }
}
