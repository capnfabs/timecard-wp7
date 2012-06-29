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

namespace Timecard.Util
{
    public class MiscConvert
    {
        public static String TimeSpan2String(TimeSpan timeSpan)
        {
            String str = "";
            if (timeSpan.Hours != 0)
            {
                str += Math.Floor(timeSpan.TotalHours) + " hrs ";
            }
            if (timeSpan.Minutes != 0)
            {
                str += timeSpan.Minutes;
                if (timeSpan.Minutes == 1)
                {
                    str += " min";
                }
                else
                {
                    str += " mins";
                }
            }
            return str;
        }
    }
}
