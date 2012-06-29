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
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Resources;


namespace Timecard.Util
{
    public class HtmlHelper
    {
        public static String TabulateWorkdays(IEnumerable<Workday> workdays)
        {
            Dictionary<DateTime, Dictionary<String, TimeSpan>> dict = new Dictionary<DateTime, Dictionary<String, TimeSpan>>();
            List<String> ColumnLabels = new List<String>();
            Dictionary<DateTime, TimeSpan> netWrkHrs = new Dictionary<DateTime, TimeSpan>();
            Dictionary<String, TimeSpan> totalForCol = new Dictionary<String, TimeSpan>();
            //for each workday:
            foreach (Workday wd in workdays)
            {
                Dictionary<String, TimeSpan> segList = new Dictionary<String, TimeSpan>();
                dict.Add(wd.StartTime, segList);
                netWrkHrs[wd.StartTime] = new TimeSpan();
                foreach (PunchSegment seg in wd.WorkSegments)
                {
                    String label = seg.StartLabel.Replace("Start ", "");
                    //if it's not a break, add it to the total hours for the day.
                    if (seg.SegmentCategory != Category.Break)
                    {
                        netWrkHrs[wd.StartTime] += seg.Duration;
                    }

                    //if it's already in the list for the day, add the hours...
                    if (segList.ContainsKey(label))
                    {
                        segList[label] += seg.Duration;
                    }
                    else //otherwise, set the hours.
                    {
                        segList[label] = seg.Duration;
                        if (!ColumnLabels.Contains(label))
                            ColumnLabels.Add(label);
                    }

                    //add it to the total for this category.
                    if (totalForCol.ContainsKey(label))
                    {
                        totalForCol[label] += seg.Duration;
                    }
                    else
                    {
                        totalForCol[label] = seg.Duration;
                    }

                }
            }
            StringBuilder output = new StringBuilder();
            //now that we've got a table of Values, along with a bunch of column names, we can start tabulating.
            output.Append("<table>");
            //setup initial row.
            output.Append("<tr class=\"firstrow\"><td>Shift Start</td>");
            for (int i = 0; i < ColumnLabels.Count; i++)
            {
                output.Append("<td>" + ColumnLabels[i] + "</td>");
            }
            output.Append("<td>Net Work Hrs</td></tr>\n");
            bool evenrow = false;
            foreach (KeyValuePair<DateTime, Dictionary<String, TimeSpan>> row in dict)
            {
                output.Append("<tr class=\"");
                output.Append(evenrow ? "evenrow" : "oddrow");
                evenrow = !evenrow;
                output.Append("\" ><td>" + row.Key.ToString("g") + "</td>");
                for (int i = 0; i < ColumnLabels.Count; i++)
                {
                    TimeSpan hrs;
                    if (row.Value.TryGetValue(ColumnLabels[i], out hrs))
                    {
                        output.Append("<td>" + hrs.TotalHours.ToString("F") + "</td>");
                    }
                    else
                    {
                        output.Append("<td></td>");
                    }
                }
                output.Append("<td>" + netWrkHrs[row.Key].TotalHours.ToString("F") + "</td>");
                output.Append("</tr>\n");
            }
            //totals row.
            output.Append("<tr class=\"totalrow\"><td>Totals</td>");
            for (int i = 0; i < ColumnLabels.Count; i++)
            {
                TimeSpan hrs;
                if (totalForCol.TryGetValue(ColumnLabels[i], out hrs))
                {
                    output.Append("<td>" + hrs.TotalHours.ToString("F") + "</td>");
                }
                else
                {
                    output.Append("<td></td>");
                }
            }

            TimeSpan netWrkHrsSum = new TimeSpan();
            foreach (KeyValuePair<DateTime, TimeSpan> i in netWrkHrs)
            {
                netWrkHrsSum += i.Value;
            }
            output.Append("<td>" + netWrkHrsSum.TotalHours.ToString("F") + "</td>");
            output.Append("</tr></table>");
            return output.ToString();
        }

        public static void SaveTabulatedWorkdays(IEnumerable<Workday> workdays, String filename)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            StreamResourceInfo tStream = Application.GetResourceStream(new Uri("/Resources/summary.html", UriKind.Relative));
            StreamReader tReader = new StreamReader(tStream.Stream);
            String html = tReader.ReadToEnd();
            tReader.Close();
            System.Diagnostics.Debug.WriteLine(html);
            html = html.Replace("PARAM_TABLE", TabulateWorkdays(workdays));
            IsolatedStorageFileStream stream = file.OpenFile(filename, System.IO.FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(html);
            writer.Close();
        }

        public static String ItemiseWorkday(Workday workday)
        {
            StringBuilder sb = new StringBuilder();
            DurationConverter dc = new DurationConverter();
            sb.Append("<div class=\"timesheet\">\n");
            sb.Append("<div class=\"time meta\">" + workday.StartTime.ToString("t") + "</div>\n");
            sb.Append("<div class=\"label\">Start Work</div>\n");
            TimeSpan TotalDuration = workday.EndTime - workday.StartTime;
            foreach (PunchSegment segment in workday.WorkSegments)
            {
                if (segment.SegmentCategory == Category.Break)
                {
                    TotalDuration -= segment.Duration;
                }
                sb.Append("<div class=\"segment " + Cat2Class(segment.SegmentCategory) + "\">\n");
                sb.Append("<div class=\"time\">" + segment.StartTime.ToString("t") + "</div>\n");
                sb.Append("<div class=\"label\">" + segment.StartLabel + "</div>\n");
                sb.Append("<div class=\"duration\">\n<div>");
                sb.Append(dc.Convert(segment.Duration, typeof(String), null, null) + " = " + segment.Duration.TotalHours.ToString("F") + " hrs");
                sb.Append("</div></div>");
                sb.Append("<div class=\"time\">" + segment.EndTime.ToString("t") + "</div>\n");
                sb.Append("<div class=\"label\">" + segment.EndLabel + "</div></div>\n");
            }
            sb.Append("<div class=\"time meta\">" + workday.EndTime.ToString("t") + "</div>\n");
            sb.Append("<div class=\"label\">End Work</div>\n");

            sb.Append("<div class=\"total\"><div class=\"label\">Total Work Time:</div>" + dc.Convert(TotalDuration, typeof(String), null, null) + " = " + TotalDuration.TotalHours.ToString("F") + " hrs"+"</div>\n</div>");
            return sb.ToString();
        }

        public static String ItemiseWorkday2(Workday workday)
        {
            StringBuilder sb = new StringBuilder();
            DurationConverter dc = new DurationConverter();
            sb.Append("<h1 style=\"font-size: 46px; font-weight: normal; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;\">" +
        "timecard for " + workday.StartTime.ToString("D") + "</h1>\n" +
    "<h2 style=\"font-weight: normal; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;\">" +
        "Generated by <a href=\"BLAH BLAH BLAH\">Timecard</a> for Windows Phone 7</h2>\n");
            sb.Append("<div style=\"width: 250px; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;\" >\n");
            sb.Append("<div style=\"font-size: 25px; font-weight: bold; color: #1BA1E2;\">" + workday.StartTime.ToString("t") + "</div>\n");
            sb.Append("<div style=\"width: 130px; font-size: small; text-align: right; color: #000000;\">Start Work</div>\n");
            TimeSpan TotalDuration = workday.EndTime - workday.StartTime;
            foreach (PunchSegment segment in workday.WorkSegments)
            {
                if (segment.SegmentCategory == Category.Break)
                {
                    TotalDuration -= segment.Duration;
                }
                sb.Append("<div style=\"" + Cat2Style(segment.SegmentCategory) + "\">\n");
                sb.Append("<div style=\"font-size: 25px; font-weight: bold;\">" + segment.StartTime.ToString("t") + "</div>\n");
                sb.Append("<div style=\"width: 130px; font-size: small; text-align: right; color: #000000;\">" + segment.StartLabel + "</div>\n");
                sb.Append("<div style=\"margin-left: 20px; padding: 15px; border-left-width: medium; border-left-style: solid; vertical-align: middle;\">\n<div style=\"color: #000000;\">");
                sb.Append(dc.Convert(segment.Duration, typeof(String), null, null) + " = " + segment.Duration.TotalHours.ToString("F") + " hrs");
                sb.Append("</div></div>");
                sb.Append("<div style=\"font-size: 25px; font-weight: bold;\">" + segment.EndTime.ToString("t") + "</div>\n");
                sb.Append("<div style=\"width: 130px; font-size: small; text-align: right; color: #000000;\">" + segment.EndLabel + "</div></div>\n");
            }
            sb.Append("<div style=\"font-size: 25px; font-weight: bold; color: #1BA1E2;\">" + workday.EndTime.ToString("t") + "</div>\n");
            sb.Append("<div style=\"width: 130px; font-size: small; text-align: right; color: #000000;\">End Work</div>\n");

            sb.Append("<div style=\"color: #000000; border-top: thin solid black; margin-top: 20px; text-align: right;\"><div style=\"width: 130px; font-size: small; text-align: right; color: #000000;\">Total Work Time:</div>" + dc.Convert(TotalDuration, typeof(String), null, null) + " = " + TotalDuration.TotalHours.ToString("F") + " hrs" + "</div>\n</div>");
            return sb.ToString();
        }

        protected static String Cat2Style(Category cat)
        {
            switch (cat)
            {
                case Category.Break:
                    return "color: #A2C139;";
                case Category.Meta:
                    return "color: #1BA1E2;";
                case Category.Project:
                    return "color: #F09609;";
                default:
                    return "";
            }
        }

        public static void SaveItemisedWorkday(Workday workday, String filename)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            StreamResourceInfo tStream = Application.GetResourceStream(new Uri("Resources/timecard.html", UriKind.Relative));
            StreamReader tReader = new StreamReader(tStream.Stream);
            String html = tReader.ReadToEnd();
            tReader.Close();
            System.Diagnostics.Debug.WriteLine(html);
            html = html.Replace("TIMECARD_DATE", workday.StartTime.ToString("D"));
            html = html.Replace("ITEMISED_WORKDAY", ItemiseWorkday(workday));
            IsolatedStorageFileStream stream = file.OpenFile(filename, System.IO.FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(html);
            writer.Close();
        }

        protected static String Cat2Class(Category cat)
        {
            switch (cat)
            {
                case Category.Break:
                    return "break";
                case Category.Meta:
                    return "meta";
                case Category.Project:
                    return "project";
                default:
                    return "";
            }
        }
    }
}
