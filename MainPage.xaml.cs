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
using Microsoft.Phone.Tasks;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;

using SetDic = System.Collections.Generic.Dictionary<string, string>;
using Timecard.Util;

namespace Timecard
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ObservableCollection<PunchType> ptc = new ObservableCollection<PunchType>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            PunchList.PunchTypes = App.Inst.PunchList;
            PunchList.SetLastPunch(App.Inst.LastPunch);
            WorkdayView.Workday = App.Inst.Workday;
            base.OnNavigatedTo(args);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnPunchPressed(object sender, PunchType punch)
        {
            App.Inst.Workday.Punch(punch);
            App.Inst.LastPunch = punch;

            if (punch.PunchCategory == Category.Meta && punch.StartorEnd == StartEnd.End)
            {
                EndWorkday();
            }

            WorkdayView.Workday = App.Inst.Workday;
            WorkdayView.ScrollToEnd();
        }

        private void EndWorkday()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Workday archivedWorkday = App.Inst.Workday;
            if (App.Inst.Archive(App.Inst.Workday))
            {
                App.Inst.Workday = new Workday();
                MessageBox.Show("This workday has been archived. You can access it again from the 'History' option in the menu.");
            }
            else
            {
                return;
            }
            bool autoExport;
            if (settings.TryGetValue<bool>("autoTimecardExport", out autoExport))
            {
                if (autoExport)
                {
                    AutoEmailTimecard(archivedWorkday);
                }
            }
            if (settings.TryGetValue<bool>("autoSummaryExport", out autoExport))
            {
                if (autoExport)
                {
                    AutoSummarise();
                }
            }
        }

        private void AutoSummarise()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            SetDic AutoSummariseSettings;
            if (!settings.TryGetValue<SetDic>("AutoSummariseSettings", out AutoSummariseSettings))
                return;
            String temp;
            String Frequency;
            DateTime dateNext;
            String EmailAddr;
            if (!AutoSummariseSettings.TryGetValue("Frequency", out Frequency))
                return;
            if (!AutoSummariseSettings.TryGetValue("NextDate", out temp))
                return;
            dateNext = DateTime.ParseExact(temp, "d", null);
            if (!AutoSummariseSettings.TryGetValue("EmailAddr", out EmailAddr))
                return;

            if (DateTime.Today >= dateNext)
            {

                //update the next date to summarise.
                AutoSummariseSettings["NextDate"] = String2DateTimeDecrement(AutoSummariseSettings["Frequency"], dateNext).ToString("d");
                settings["AutoSummariseSettings"] = AutoSummariseSettings;
                settings.Save();
            }

        }

        private DateTime String2DateTimeDecrement(String str, DateTime original)
        {
            switch (str)
            {
                case "Weekly":
                    return original.AddDays(-7);
                case "Fortnightly":
                    return original.AddDays(-14);
                case "Monthly":
                    return original.AddMonths(-1);
                default:
                    return original;
            }
        }

        private DateTime String2DateTimeIncrement(String str, DateTime original)
        {
            switch (str)
            {
                case "Weekly":
                    return original.AddDays(7);
                case "Fortnightly":
                    return original.AddDays(14);
                case "Monthly":
                    return original.AddMonths(1);
                default:
                    return original;
            }
        }

        private void AutoEmailTimecard(Workday archivedWorkday)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            SetDic AutoSubmitSettings;
            if (settings.TryGetValue<SetDic>("AutoSubmitSettings", out AutoSubmitSettings))
            {
                String EmailAddr;
                if (AutoSubmitSettings.TryGetValue("EmailAddr", out EmailAddr))
                {
                    RemoteEmail email = new RemoteEmail();
                    email.From = "timecard@capnfabs.net";
                    email.To = EmailAddr;
                    email.Subject = "Timecard for " + archivedWorkday.StartTime.ToString("D");
                    email.Body = HtmlHelper.ItemiseWorkday2(archivedWorkday);
                    email.EmailRequestCompleted += Email_Completed;
                    email.Email();
                }
            }
        }

        private void Email_Completed(object sender, EmailCompletedEventArgs e)
        {
            if (!e.Success)
            {
                MessageBoxResult result = MessageBox.Show("The email you just tried to send failed. Press OK to try again.", "Sending Failed", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    ((RemoteEmail)sender).Email();
                }
            }
        }

        private void MenuReset_Click(object sender, EventArgs e)
        {
            App.Inst.ResetWorkday();
            PunchList.SetLastPunch(App.Inst.LastPunch);
            WorkdayView.Workday = App.Inst.Workday;

        }

        private void MenuSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void MenuHistory_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Archive.xaml", UriKind.Relative));
        }

        private void MenuFeedback_Click(object sender, EventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "timecard@capnfabs.net";
            email.Subject = "Timecard Feedback";
            email.Show();
        }

        private void MenuReview_Click(object sender, EventArgs e)
        {
            (new MarketplaceReviewTask()).Show();
        }

    }
}