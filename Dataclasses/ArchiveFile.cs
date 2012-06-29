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

namespace Timecard
{
    public class ArchiveFile
    {
        protected String _filename;
        public String Filename
        {
            set
            {
                _filename = value;
                String filestub = Filename.Substring(0, Filename.LastIndexOf("."));
                Filedate = DateTime.FromFileTime(long.Parse(filestub));
            }

            get
            {
                return _filename;
            }
        }
        public DateTime Filedate { protected set; get; }

        public ArchiveFile()
        {
        }

        public ArchiveFile(String filename)
        {
            Filename = filename;
        }
    }
}
