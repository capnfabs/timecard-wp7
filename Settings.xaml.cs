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
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace Timecard
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void EditProjects_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ListEditor.xaml?Target=Projects&Title=Edit+Projects", UriKind.Relative));
        }

        private void EditBreaks_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ListEditor.xaml?Target=Breaks&Title=Edit+Breaks", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            IsolatedStorageSettings appSet = IsolatedStorageSettings.ApplicationSettings;
            bool val;
            if (appSet.TryGetValue<bool>("autoTimecardExport", out val))
            {
                chkAutoTimecardExport.IsChecked = val;
            }
            if (appSet.TryGetValue<bool>("autoSummaryExport", out val))
            {
                chkAutoSummaryExport.IsChecked = val;
            }
            base.OnNavigatedTo(args);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            //set the default email address, and generate the appropriate punchtypes.
            IsolatedStorageSettings appSet = IsolatedStorageSettings.ApplicationSettings;
            appSet["autoTimecardExport"] = chkAutoTimecardExport.IsChecked;
            appSet["autoSummaryExport"] = chkAutoSummaryExport.IsChecked;
            appSet.Save();
            App.Inst.SetupPunchTypes();
        }

        private void btnAutoTimecardExport_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AutoTimecardExportSettings.xaml", UriKind.Relative));
        }

        private void btnAutoSummaryExport_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AutoSummaryExportSettings.xaml", UriKind.Relative));
        }

        private void chk_Timecard(object sender, RoutedEventArgs e)
        {
        }

        private void chk_Summaries(object sender, RoutedEventArgs e)
        {

        }
    }
}