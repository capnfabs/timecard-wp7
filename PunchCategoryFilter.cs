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
    public class PunchCategoryFilter
    {
        public ObservableCollection<PunchCategory> Convert(ObservableCollection<PunchCategory> value, PunchSet parameter)
        {
            
                PunchSet PS = parameter as PunchSet;
                
                ObservableCollection<PunchCategory> PCS = value as ObservableCollection<PunchCategory>;
                ObservableCollection<PunchCategory> ret = new ObservableCollection<PunchCategory>(PCS);
                if (PS.Punches.Count == 0)
                {
                    foreach (PunchCategory cat in ret)
                    {
                        if (cat.PunchType != PunchTypes.StartWork)
                        {
                            ret.Remove(cat);
                        }
                    }
                    return ret;
                }
                switch (PS.Punches[PS.Punches.Count - 1].PunchType)
                {
                    case PunchTypes.StartWork:
                    case PunchTypes.EndBreak:
                        foreach (PunchCategory cat in ret)
                        {
                            if (cat.PunchType == PunchTypes.StartWork ||
                                cat.PunchType == PunchTypes.EndProject ||
                                cat.PunchType == PunchTypes.EndBreak)
                            {
                                ret.Remove(cat);
                            }
                        }
                        break;
                    case PunchTypes.EndWork:
                        foreach (PunchCategory cat in ret)
                        {
                            if (cat.PunchType != PunchTypes.StartWork)
                            {
                                ret.Remove(cat);
                            }
                        }
                        break;
                    case PunchTypes.StartBreak:
                        foreach (PunchCategory cat in ret)
                        {
                            if (cat.PunchType != PunchTypes.EndWork)
                            {
                                ret.Remove(cat);
                            }
                        }
                        break;
                    case PunchTypes.StartProject:
                    case PunchTypes.EndProject:
                        foreach (PunchCategory cat in ret)
                        {
                            if (cat.PunchType == PunchTypes.StartWork ||
                                cat.PunchType == PunchTypes.EndBreak)
                            {
                                ret.Remove(cat);
                            }
                        }
                        break;
                }
            
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
