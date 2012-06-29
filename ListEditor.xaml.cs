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
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;

namespace Timecard
{
    public partial class ListEditor : PhoneApplicationPage
    {
        ObservableCollection<String> Strings;
        String Target;
        public ListEditor()
        {
            InitializeComponent();
            Strings = new ObservableCollection<string>();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            AddItemToList();
        }

        private void AddItemToList()
        {
            String str = txtNewItem.Text.Replace(",", "");
            Strings.Add(str);
            txtNewItem.Text = "";
        }

        private void RemoveSelection_Click(object sender, RoutedEventArgs e)
        {
            int idx = lstItems.SelectedIndex;
            if (idx == -1)
                return;
            Strings.RemoveAt(idx);
            try
            {
                lstItems.SelectedIndex = idx;
            }
            catch
            {

            }

        }
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("Title"))
            {
                PageTitle.Text = parameters["Title"];
            }
            if (parameters.ContainsKey("Target"))
            {
                Target = parameters["Target"];
                String readString;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<String>(Target, out readString))
                {
                    String[] gotStrings = readString.Split(',');
                    Strings.Clear();
                    foreach (String s in gotStrings)
                    {
                        Strings.Add(s);
                    }
                }
                lstItems.ItemsSource = Strings;

            }
            base.OnNavigatedTo(args);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            IsolatedStorageSettings appSet = IsolatedStorageSettings.ApplicationSettings;

            if (Target == null)
            {
                throw new Exception("No target setting was specified.");
            }
            appSet[Target] = String.Join(",", Strings);
            appSet.Save();

        }

        private void txtNewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddItemToList();
                e.Handled = true;
                AddItem.Focus();
            }
        }
    }
}