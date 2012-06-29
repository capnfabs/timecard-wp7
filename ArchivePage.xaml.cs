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
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;

using Timecard.Util;

namespace Timecard
{
    public partial class ArchivePage : PhoneApplicationPage
    {
        protected String filename;
        public ArchivePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IDictionary<string,string> parameters = NavigationContext.QueryString;
            if (parameters.ContainsKey("file"))
            {
                filename = "/history/"+parameters["file"];
                Workday workday = (Workday)IsolatedStorageHelper.FileToObj(typeof(Workday), filename);
                WorkdayView.Workday = workday;

                PageTitle.Text = workday.StartTime.ToString("ddd ") + workday.StartTime.ToString("d");
            }

            base.OnNavigatedTo(e);
        }

        private void AppBarEmail_Click(object sender, EventArgs e)
        {
            if (LicenceHelper.IsTrialMode())
            {
                App.NotifyTrialMode();
                return;
            }
            EmailHelper.EmailWorkday(WorkdayView.Workday, null);
        }

        private void AppBarDelete_Click(object sender, EventArgs e)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (filename != null)
            if (file.FileExists(filename))
            {
                file.DeleteFile(filename);
            }
            NavigationService.GoBack();
        }
    }
}