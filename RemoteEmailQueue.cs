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
using System.Collections.Generic;

using RemoteEmailList = System.Collections.Generic.List<Timecard.RemoteEmail>;

namespace Timecard
{
    public class RemoteEmailQueue
    {
        RemoteEmailList Emails;
        

        public RemoteEmailQueue()
        {
            Emails = new RemoteEmailList();
        }

        public void Add(RemoteEmail email)
        {

        }

        public void StartProcessing()
        {
            if (Emails.Count == 0)
                return;
        }
    }
}
