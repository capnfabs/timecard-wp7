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
using System.Collections.ObjectModel;

namespace Timecard
{
    public enum StartEnd
    {
        Start,
        End
    }

    public enum Category
    {
        Meta,
        Break,
        Project
    }

    public class PunchType
    {
        public String Label { set; get; }
        public StartEnd StartorEnd { set; get; }
        public Category PunchCategory { set; get; }

        public PunchType(String label, StartEnd startOrEnd, Category punchCategory)
        {
            Label = label;
            StartorEnd = startOrEnd;
            PunchCategory = punchCategory;
        }

        public PunchType()
        {

        }

        public ObservableCollection<PunchType> FilterNext(ObservableCollection<PunchType> PunchSet)
        {
            ObservableCollection<PunchType> newlist = new ObservableCollection<PunchType>();
            if (
                    (StartorEnd == StartEnd.Start && PunchCategory == Category.Meta) || //started work OR 
                    (StartorEnd == StartEnd.End && PunchCategory != Category.Meta) //Ended a Break or Project, i.e. Ended something but NOT 'Work'
                )
            {
                foreach (PunchType p in PunchSet)
                {
                    if ((p.StartorEnd == StartEnd.End && p.PunchCategory == Category.Meta) ||
                        (p.StartorEnd == StartEnd.Start && p.PunchCategory != Category.Meta))
                    {
                        newlist.Add(p);
                    }
                }

            }
            else if (StartorEnd == StartEnd.End && PunchCategory == Category.Meta) //Ended Work
            {
                foreach (PunchType p in PunchSet)
                {
                    if (p.StartorEnd == StartEnd.Start && p.PunchCategory == Category.Meta)
                    {
                        newlist.Add(p);
                    }
                }
            }
            else if (StartorEnd == StartEnd.Start && PunchCategory == Category.Break) // Started a Break
            {
                foreach (PunchType p in PunchSet)
                {
                    if (p.StartorEnd == StartEnd.End && p.PunchCategory == Category.Break)
                    {
                        newlist.Add(p);
                    }
                }
            }
            else if (StartorEnd == StartEnd.Start && PunchCategory == Category.Project) //Started a Project
            {
                foreach (PunchType p in PunchSet)
                {
                    //add all starts (except start work) and add end project.
                    if ((p.StartorEnd == StartEnd.Start && p.PunchCategory != Category.Meta) ||
                        (p.StartorEnd == StartEnd.End && p.PunchCategory == Category.Project)
                        )
                    {
                        newlist.Add(p);
                    }
                }
                //loop through. If we find an identical project then remove it from the new list.
                foreach (PunchType p in newlist)
                {
                    if (p.PunchCategory == Category.Project && p.StartorEnd == StartEnd.Start && p.Label == this.Label)
                    {
                        newlist.Remove(p);
                        break;
                    }
                }


            }
            return newlist;
        }
    }

}
