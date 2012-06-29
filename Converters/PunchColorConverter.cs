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
    public class PunchColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Category cat;
            if (value is Category)
            {
                cat = (Category)value;
            }
            else
            {
                return value;
            }
            return new SolidColorBrush(GetColorFromCategory(cat));
        }

        private Color GetColorFromCategory(Category cat)
        {
            switch (cat)
            {
                case Category.Meta:
                    return Color.FromArgb(255, 27, 161, 226);
                case Category.Break:
                    return Color.FromArgb(255, 162, 193, 57);
                case Category.Project:
                    return Color.FromArgb(255, 240, 150, 9);
                default:
                    return Color.FromArgb(255, 255, 255, 255);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
