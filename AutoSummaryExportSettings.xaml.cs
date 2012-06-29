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
using Microsoft.Live;
using Microsoft.Live.Controls;

using System.IO;
using System.IO.IsolatedStorage;

using System.Xml.Serialization;
using System.Xml;

using SetDic = System.Collections.Generic.Dictionary<string, string>;

namespace Timecard
{
    public partial class AutoSummaryExportSettings : PhoneApplicationPage
    {
        private String[] lstFrequencyItems;
        LiveConnectClient client;
        public AutoSummaryExportSettings()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            //Save options
            saveState();
            NavigationService.GoBack();

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.OriginalString.Contains("Microsoft.Phone.Controls.Toolkit"))
            {
                IsolatedStorageSettings.ApplicationSettings["tmpSel"] = lstFrequency.SelectedItem;
                IsolatedStorageSettings.ApplicationSettings["tmpEmail"] = txtEmailAddr.Text;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            base.OnNavigatedFrom(e);
        }

        protected void selectText(String text)
        {
            for (int i = 0; i < lstFrequencyItems.Length; i++)
            {
                if (lstFrequencyItems[i] == text)
                {
                    lstFrequency.SelectedIndex = i;
                    return;
                }
            }
        }

        protected void setup()
        {
            String[] items = { "Weekly", "Fortnightly", "Monthly" };
            lstFrequency.ItemsSource = items;
           
            IsolatedStorageSettings IsoSettings = IsolatedStorageSettings.ApplicationSettings;
            String temp;
            bool lstFreqSet = false;

            if (IsoSettings.TryGetValue<String>("tmpSel", out temp))
            {
                txtEmailAddr.Text = (String)IsoSettings["tmpEmail"];
                selectText(temp);
                lstFreqSet = true;
                IsoSettings.Remove("tmpSel");
                IsoSettings.Remove("tmpEmail");
                IsoSettings.Save();
            }

            if (lstFreqSet)
                return;

            SetDic settings;
            lstFrequencyItems = items;
            if (IsoSettings.TryGetValue<SetDic>("AutoSummariseSettings", out settings))
            {
                
                if (settings.TryGetValue("Frequency", out temp))
                {
                    selectText(temp);
                }
                if (settings.TryGetValue("NextDate", out temp))
                {
                    dateNext.Value = DateTime.ParseExact(temp, "d", null);
                }
                if (settings.TryGetValue("EmailAddr", out temp))
                {
                    txtEmailAddr.Text = temp;
                }
            }
        }

        protected void saveState()
        {
            IsolatedStorageSettings IsoSettings = IsolatedStorageSettings.ApplicationSettings;
            SetDic settings = new SetDic();
            settings["Frequency"] = (String)lstFrequency.SelectedItem;
            settings["NextDate"] = dateNext.Value.Value.ToString("d");
            settings["EmailAddr"] = txtEmailAddr.Text;
            IsoSettings["AutoSummariseSettings"] = settings;
            IsoSettings.Save();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
                setup();
        }

        private void dateNext_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            UpdateDateRange();
        }

        private void UpdateDateRange()
        {
            DateConverter dc = new DateConverter();
            String nextDate = (String)dc.Convert(dateNext.Value, typeof(String), ":sd", null);
            DateTime pastVal;
            switch (lstFrequency.SelectedIndex)
            {
                case 0:
                    pastVal = dateNext.Value.Value.AddDays(-6);
                    break;
                case 1:
                    pastVal = dateNext.Value.Value.AddDays(-13);
                    break;
                case 2:
                    pastVal = dateNext.Value.Value.AddMonths(-1).AddDays(1);
                    break;
                default:
                    pastVal = dateNext.Value.Value;
                    break;
            }
            String pastDate = (String)dc.Convert(pastVal, typeof(String), ":sd", null);
            lblRecursiveDayClarifier.Text = String.Format("i.e. the next summary will be for all shifts between {0} and {1} inclusive.", pastDate, nextDate);
        }

        private void lstFrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDateRange();
        }

        private void txtEmailAddr_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEmailAddr.Text == "me@example.com")
                txtEmailAddr.Style = (System.Windows.Style)this.Resources["DefTextStyle"];
            else
                txtEmailAddr.Style = null;
        }

        private void txtEmailAddr_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmailAddr.Text == "me@example.com")
                txtEmailAddr.Text = "";
        }

        private void txtEmailAddr_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmailAddr.Text == "")
                txtEmailAddr.Text = "me@example.com";
        }

        private void txtEmailAddr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                this.Focus();
            }
        }
    }
}