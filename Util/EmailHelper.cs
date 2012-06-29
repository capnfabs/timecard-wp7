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

using System.Collections.Generic;
//using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.Phone.Tasks;


namespace Timecard.Util
{
    public class EmailHelper
    {
        public static String EmailFormatWorkday(Workday workday)
        {
            String str = "";
            String nl = Environment.NewLine;
            str += "Workday: " + workday.StartTime.ToString("D") + nl + nl;
            str += "Start Work: " + workday.StartTime.ToString("t") + nl + nl;
            TimeSpan breaktime = new TimeSpan();
            foreach (PunchSegment segment in workday.WorkSegments)
            {
                if (segment.SegmentCategory == Category.Break)
                {
                    breaktime += segment.Duration;
                }
                str += segment.StartLabel + ": " + segment.StartTime.ToString("t") + nl;
                if (segment.Duration.TotalMinutes >= 1)
                    str += "-- " + MiscConvert.TimeSpan2String(segment.Duration) + " = " + segment.Duration.TotalHours.ToString("F") + " hrs" + nl;
                str += segment.EndLabel + ": " + segment.EndTime.ToString("t") + nl + nl;
            }

            TimeSpan totalTime = workday.EndTime - workday.StartTime - breaktime;

            str += "End Work: " + workday.EndTime.ToString("t") + nl + nl;
            str += "Work Time (excluding breaks): " + MiscConvert.TimeSpan2String(totalTime) + " = " + totalTime.TotalHours.ToString("F") + " hrs" + nl;
            return str;
        }

        public static void EmailWorkday(Workday workday, String emailAddr)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.Body = EmailFormatWorkday(workday);
            email.Subject = "Timesheet for " + workday.StartTime.ToString("D");
            if (emailAddr != null)
                email.To = emailAddr;
            email.Show();
        }

        public static void EmailWorkday(IEnumerable<Workday> workdays, String emailAddr)
        {
            if (workdays.Count() == 0)
                return;
            if (workdays.Count() == 1)
            {
                EmailWorkday(workdays.First(), emailAddr);
                return;
            }
            EmailComposeTask email = new EmailComposeTask();
            IEnumerable<Workday> w = from wd in workdays orderby wd.StartTime select wd;
            email.Subject = "Timesheets for " +
                w.First().StartTime.ToString("D") + " to " +
                w.Last().StartTime.ToString("D");
            List<String> texts = new List<String>();
            foreach (Workday i in w)
            {
                texts.Add(EmailFormatWorkday(i));
            }
            email.Body = String.Join(Environment.NewLine + "---" +
                Environment.NewLine + Environment.NewLine, texts.ToArray());

            if (emailAddr != null)
                email.To = emailAddr;
            
            email.Show();
        }

    }
}
