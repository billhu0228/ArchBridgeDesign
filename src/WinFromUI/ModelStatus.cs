using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFromUI
{
    public class ModelStatus
    {
        private bool status;

        public delegate void StatusHandler();
        public event StatusHandler ChangeStatus;

        protected virtual void OnStatusChanged()
        {
            if (ChangeStatus!=null)
            {
                ChangeStatus();
            }
         
        }

        public ModelStatus()
        {
            status = false;
        }

        public void SetStatus(bool v)
        {
            status = v;
            OnStatusChanged();
        }

        public bool GetStatus()
        {
            return status;
        }

    }
}
