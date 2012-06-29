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
    public partial class AutoTimecardExportSettings : PhoneApplicationPage
    {
        LiveConnectClient client;
        public AutoTimecardExportSettings()
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
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["AutoSubmitSettings"] = DictSettings();
            settings.Save();
            NavigationService.GoBack();

        }

        protected SetDic DictSettings()
        {
            SetDic settings = new SetDic();
            settings["EmailAddr"] = txtEmailAddr.Text;
            return settings;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected void setup()
        {
            IsolatedStorageSettings IsoSettings = IsolatedStorageSettings.ApplicationSettings;
            SetDic settings;
            if (IsoSettings.TryGetValue<SetDic>("AutoSubmitSettings", out settings))
            {
                String temp;
                if (settings.TryGetValue("Email", out temp))
                {
                    txtEmailAddr.Text = temp;
                }
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            setup();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            setup();
        }

        private void txtEmailAddr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                this.Focus();
            }
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
    }
}