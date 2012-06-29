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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using System.IO.IsolatedStorage;

using System.Collections.ObjectModel;


using Timecard.Util;
using DataDictionary = System.Collections.Generic.Dictionary<string, object>;

namespace Timecard
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public DataDictionary DataDictionary { get; private set; }

        public Workday Workday { set; get; }
        public ObservableCollection<PunchType> PunchList { set; get; }
        public PunchType LastPunch { set; get; }

        public RemoteEmailQueue EmailQueue { set; get; }

        public static App Inst
        {
            get
            {
                return (App)(Application.Current);
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        public void SetupDefaultState()
        {
            IsolatedStorageSettings appSet = IsolatedStorageSettings.ApplicationSettings;
            appSet["Projects"] = "Project 1,Project 2";
            appSet["Breaks"] = "Lunch";
            SetupPunchTypes();
            ResetWorkday();
            EmailQueue = new RemoteEmailQueue();
        }
        public void ResetWorkday()
        {
            App.Inst.LastPunch = new PunchType("INITIALISE!! LOLZOMG", StartEnd.End, Category.Meta);
            App.Inst.Workday = new Workday();
        }
        public void SetupPunchTypes()
        {
            IsolatedStorageSettings appSet = IsolatedStorageSettings.ApplicationSettings;
            ObservableCollection<PunchType> ptc = new ObservableCollection<PunchType>();
            AddStartPunchTypes(ptc);

            String BreakList;
            if (appSet.TryGetValue<String>("Breaks", out BreakList))
            {
                foreach (String BreakName in BreakList.Split(','))
                {
                    if (BreakName.Trim().Length != 0)
                    {
                        ptc.Add(new PunchType("Start " + BreakName, StartEnd.Start, Category.Break));
                    }

                }
            }
            String ProjList;
            if (appSet.TryGetValue<String>("Projects", out ProjList))
            {
                foreach (String ProjName in ProjList.Split(','))
                {
                    if (ProjName.Trim().Length != 0)
                    {
                        ptc.Add(new PunchType("Start " + ProjName, StartEnd.Start, Category.Project));
                    }
                }
            }
            AddEndPunchTypes(ptc);
            App.Inst.PunchList = ptc;
        }



        public static void NotifyTrialMode()
        {
            MessageBoxResult result = MessageBox.Show("This feature is only available in the full version of Timecard. Time to upgrade?", "Only in Full Version!", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                MarketplaceDetailTask market = new MarketplaceDetailTask();
                market.Show();
            }

        }

        public bool Archive(Workday wd)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (!file.DirectoryExists("/history"))
                {
                    file.CreateDirectory("/history");
                }
            }
            catch
            {
                return false;
            }
            try
            {
                String filename = Workday.StartTime.ToFileTime().ToString();
                IsolatedStorageHelper.ObjToFile(wd, typeof(Workday), "/history/" + filename + ".xml");
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void AddStartPunchTypes(ObservableCollection<PunchType> ptc)
        {

            ptc.Add(new PunchType("Start Work", StartEnd.Start, Category.Meta));
            ptc.Add(new PunchType("End Project", StartEnd.End, Category.Project));
            ptc.Add(new PunchType("End Break", StartEnd.End, Category.Break));
        }
        private void AddEndPunchTypes(ObservableCollection<PunchType> ptc)
        {
            ptc.Add(new PunchType("End Work", StartEnd.End, Category.Meta));
        }


        private void LoadState()
        {

            LicenceHelper.IsTrialMode(true);

            DataDictionary["Workday"] = 

            Workday = (Workday)IsolatedStorageHelper.FileToObj(typeof(Workday), "current/workday.xml");
            LastPunch = (PunchType)IsolatedStorageHelper.FileToObj(typeof(PunchType), "current/lastpunch.xml");
            PunchList = (ObservableCollection<PunchType>)IsolatedStorageHelper.FileToObj(typeof(ObservableCollection<PunchType>), "current/punchlist.xml");
            EmailQueue = (RemoteEmailQueue)IsolatedStorageHelper.FileToObj(typeof(RemoteEmailQueue), "current/emailqueue.xml");



        }

        private void SaveState()
        {
            IsolatedStorageSettings.ApplicationSettings.Save();
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (!file.DirectoryExists("current"))
            {
                file.CreateDirectory("current");
            }
            try
            {
                IsolatedStorageHelper.ObjToFile(Workday, typeof(Workday), "current/workday.xml");
                IsolatedStorageHelper.ObjToFile(PunchList, typeof(ObservableCollection<PunchType>), "current/punchlist.xml");
                IsolatedStorageHelper.ObjToFile(LastPunch, typeof(PunchType), "current/lastpunch.xml");
                IsolatedStorageHelper.ObjToFile(EmailQueue, typeof(RemoteEmailQueue), "current/emailqueue.xml");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            LoadState();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            LoadState();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SaveState();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            SaveState();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}