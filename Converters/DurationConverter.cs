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
using System.Windows.Data;

namespace Timecard
{
    public class DurationConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan val = new TimeSpan();
            bool setVal = false;
            if (value is Duration)
            {
                val = ((Duration)value).TimeSpan;
                setVal = true;
            }
            else if (value is TimeSpan)
            {
                val = (TimeSpan)value;
                setVal = true;
            }

            if (setVal)
            {
                String str = "";
                if (val.Hours != 0)
                {
                    str = val.Hours + ((val.Hours == 1) ? " hr " : " hrs ");
                }

                if (val.Minutes != 0 || val.Hours == 0) //i.e. if there is a minutes component, or there is no hours component
                {
                    str += val.Minutes + ((val.Minutes == 1) ? " min" : " mins");
                }
                return str;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
