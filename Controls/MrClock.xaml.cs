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

namespace Timecard
{
    public partial class MrClock : UserControl
    {
        const int RotateDuration = 10;
        static double AnimationCycles = 0;

        DateTime animStartTime;

        private void StartAnimations()
        {
            RotateTransform rtHrs = HourHand.RenderTransform as RotateTransform;
            RotateTransform rtMins = MinuteHand.RenderTransform as RotateTransform;
            DoubleAnimation anima_hrs = new DoubleAnimation();
            DoubleAnimation anima_mins = new DoubleAnimation();
            anima_hrs.From = 360*(AnimationCycles/12);
            anima_hrs.To = 360*((AnimationCycles/12)+1);
            anima_hrs.Duration = new Duration(TimeSpan.FromSeconds(RotateDuration * 12));
            anima_hrs.RepeatBehavior = RepeatBehavior.Forever;
            anima_mins.From = 360*(AnimationCycles % 1);
            anima_mins.To = 360*((AnimationCycles % 1) + 1);
            anima_mins.Duration = new Duration(TimeSpan.FromSeconds(RotateDuration));
            anima_mins.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTarget(anima_hrs, rtHrs);
            Storyboard.SetTarget(anima_mins, rtMins);
            Storyboard.SetTargetProperty(anima_hrs, new PropertyPath(RotateTransform.AngleProperty));
            Storyboard.SetTargetProperty(anima_mins, new PropertyPath(RotateTransform.AngleProperty));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(anima_hrs);
            storyboard.Children.Add(anima_mins);
            storyboard.Begin();
            animStartTime = DateTime.Now;
        }

        public MrClock()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartAnimations();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            AnimationCycles += DateTime.Now.Subtract(animStartTime).TotalSeconds / RotateDuration;
            AnimationCycles = AnimationCycles % 12;
        }
    }
}
