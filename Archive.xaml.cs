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
using System.IO.IsolatedStorage;

using Timecard.Util;

namespace Timecard
{
    public partial class Archive : PhoneApplicationPage
    {
        HistoryLister HistList;
        public Archive()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            HistList = new HistoryLister();
            WorkdayList.ItemsSource = HistList.Workdays;
            base.OnNavigatedTo(e);
        }

        private void ArchiveItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            String filename = (String)(((TextBlock)sender).Tag);
            NavigationService.Navigate(new Uri("/ArchivePage.xaml?file=" + filename, UriKind.Relative));
            e.Handled = true;
        }

        private void NotifyPleaseSelect()
        {
            MessageBox.Show("Please select one or more entries first. You can select items by tapping to the left hand side of the label.");
        }


        private void AppBarDelete_Click(object sender, EventArgs e)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (WorkdayList.SelectedItems.Count == 0)
            {
                NotifyPleaseSelect();
                return;
            }

            foreach (ArchiveFile item in WorkdayList.SelectedItems)
            {
                if (item.Filename != null)
                {
                    String filename = "/history/" + item.Filename;
                    if (file.FileExists(filename))
                    {
                        file.DeleteFile(filename);
                    }
                    HistList.Workdays.Remove(item);
                    if (WorkdayList.SelectedItems.Count == 0)
                        break;
                }
            }
        }

        private void AppBarEmail_Click(object sender, EventArgs e)
        {
            if (WorkdayList.SelectedItems.Count == 0)
            {
                NotifyPleaseSelect();
                return;
            }
            EmailHelper.EmailWorkday(GetSelectedWorkdays(), null);
        }

        private List<Workday> GetSelectedWorkdays()
        {
            List<Workday> workdays = new List<Workday>();
            foreach (ArchiveFile item in WorkdayList.SelectedItems)
            {
                workdays.Add((Workday)IsolatedStorageHelper.FileToObj(typeof(Workday), "/history/" + item.Filename));
            }
            return workdays;
        }

        private void AppBarSelect_Click(object sender, EventArgs e)
        {
            WorkdayList.IsSelectionEnabled = !WorkdayList.IsSelectionEnabled;
        }

        private void AppBarTabulate_Click(object sender, EventArgs e)
        {
            if (WorkdayList.SelectedItems.Count == 0)
            {
                NotifyPleaseSelect();
                return;
            }
            HtmlHelper.SaveTabulatedWorkdays(GetSelectedWorkdays(), "Resources/summary.html");
            NavigationService.Navigate(new Uri("/HTMLViewer.xaml?path=" + HttpUtility.UrlEncode("temp/table.html") + "&title=summary", UriKind.Relative));
        }
    }
}