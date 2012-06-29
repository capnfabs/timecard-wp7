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

namespace Timecard
{
    public partial class HTMLViewer : PhoneApplicationPage
    {
        public HTMLViewer()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> query = NavigationContext.QueryString;
            String path;
            if (query.TryGetValue("path", out path))
            {
                Web.Navigate(new Uri(path, UriKind.Relative));
            }
            String title;
            if (query.TryGetValue("title", out title))
            {
                PageTitle.Text = title;
            }
            base.OnNavigatedTo(e);
        }
    }
}