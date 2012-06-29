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
    public class DateConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime && targetType == typeof(string) && parameter is string)
            {
                String p = (String)parameter;
                DateTime val = (DateTime)value;
                if (p.StartsWith(":"))
                {
                    switch (p.Substring(1).ToLower())
                    {
                        case "sd": //short date
                            return val.ToString("ddd ") + val.ToString("d");
                    }
                }
                return ((DateTime)value).ToString((string)parameter);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
