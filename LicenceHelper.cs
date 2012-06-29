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
using System.ComponentModel;
using Microsoft.Phone.Marketplace;

namespace Timecard
{
    public class LicenceHelper
    {
        private static bool? cachedIsTrialMode = null;
        public static bool IsTrialMode()
        {
            return IsTrialMode(false);
        }
        public static bool IsTrialMode(bool forceRefresh)
        {
            if (DesignerProperties.IsInDesignTool)
                return false;

            if (forceRefresh || (cachedIsTrialMode == null))
            {
#if DEBUG
                   MessageBoxResult result = MessageBox.Show("Click ok to simulate paid version", "Simulate paid version?", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {   
                cachedIsTrialMode = false;   
                }   
                else   
                {   
                cachedIsTrialMode = true;
                }
#else
                LicenseInformation licenseInfo = new LicenseInformation();
                cachedIsTrialMode = licenseInfo.IsTrial();
#endif
            }
            return cachedIsTrialMode.Value;
        }
    }
}
