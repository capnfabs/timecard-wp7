using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timecard
{
    public abstract class AppDataObject
    {
        public bool DefaultsOnLoadFailed { set; get; }
        public String Path { set; get; }
        public String Target { set; get; }

        public void Save()
        {
            SaveAction();
        }
        public void Load()
        {
            try
            {
                LoadAction();
            }
            catch (Exception ex)
            {
                if (DefaultsOnLoadFailed)
                {
                    Defaults();
                }
                else
                {
                    throw ex;
                }
            }
        }

        public void Defaults()
        {
            DefaultsAction();
        }

        protected abstract void SaveAction();
        protected abstract void LoadAction();
        public abstract void DefaultsAction();

    }
}
