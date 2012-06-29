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
using Microsoft.Phone.Controls;

namespace Timecard
{
    public partial class WorkdayViewer : UserControl
    {
        public static readonly DependencyProperty WorkdayProperty = DependencyProperty.Register("Workday", typeof(Workday), typeof(WorkdayViewer), new PropertyMetadata(null));

        public Workday Workday
        {
            get { return (Workday)GetValue(WorkdayProperty); }
            set { SetValue(WorkdayProperty, value); }
        }

        public void ScrollToEnd()
        {
            LayoutRoot.UpdateLayout();
            LayoutRoot.ScrollToVerticalOffset(double.MaxValue);
        }


        public WorkdayViewer()
        {
            InitializeComponent();
            
        }
    }
}
