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
using System.Text;

using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Timecard
{
    public class EmailCompletedEventArgs
    {
        public bool Success {set; get;}
        public String ErrorMessage {set; get;}
        
        public EmailCompletedEventArgs(bool success, String errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }


    public class RemoteEmail
    {
        public static String Key { set; get; }

        static RemoteEmail()
        {
            Key = "sks017w71oVgiaoHMC8L7AH6QnGhF2TpoPqwCKUctkSq6o0";
        }

        public delegate void EmailRequestCompletedHandler(object sender, EmailCompletedEventArgs e);

        public event EmailRequestCompletedHandler EmailRequestCompleted;

        public String From { set; get; }
        public String To { set; get; }
        public String Body { set; get; }
        public String Subject { set; get; }
        
        public void Email()
        {
            StringBuilder data = new StringBuilder();
            data.Append("key="+HttpUtility.UrlEncode(Key));
            if (From != null)
            {
                data.Append("&from="+HttpUtility.UrlEncode(From));
            }
            if (To != null)
            {
                data.Append("&to="+HttpUtility.UrlEncode(To));
            }
            if (Subject != null)
            {
                data.Append("&subject="+HttpUtility.UrlEncode(Subject));
            }
            if (Body != null)
            {
                data.Append("&body="+HttpUtility.UrlEncode(Body));
            }
            WebClient client = new WebClient();
            client.UploadStringCompleted += Email_Response;
            try
            {
                client.UploadStringAsync(new Uri("http://timecard.capnfabs.net/email.php"), "POST", data.ToString());
            }
            catch
            {
                EmailRequestCompleted(this, new EmailCompletedEventArgs(false, "The Email Request Failed"));
            }
            
        }

        private void Email_Response(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Descendants("status").FirstOrDefault().Value == "fail")
                {
                    EmailRequestCompleted(this, new EmailCompletedEventArgs(false, doc.Descendants("message").FirstOrDefault().Value));
                }
                EmailRequestCompleted(this, new EmailCompletedEventArgs(true, null));
            }
            catch (Exception ex)
            {
                EmailRequestCompleted(this, new EmailCompletedEventArgs(false, "The Email Request Failed"));
            }
            
        }
    }
}
