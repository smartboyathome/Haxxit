using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.MonoGame
{
    public class BoolWrapper
    {
        private bool status;
        public bool Status { get { return status; } set { status = value; } }

        public BoolWrapper(bool newStatus)
        {
            status = newStatus;
        }

        public bool Equals(bool check)
        {
            return status == check;
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            BoolWrapper bw = obj as BoolWrapper;
            if ((System.Object)bw == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (status == bw.Status);
        }

        public override int GetHashCode()
        {
            if (status)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static implicit operator bool(BoolWrapper check)
        {
            return check.Status;
        }

        public static implicit operator BoolWrapper(bool newStatus)
        {
            return new BoolWrapper(newStatus);
        }

        public static bool operator ==(BoolWrapper checkWrapper, bool checkStatus)
        {
            bool checkWrapperStatus = checkWrapper.Status;
            return checkWrapperStatus == checkStatus;
        }

        public static bool operator !=(BoolWrapper checkWrapper, bool checkStatus)
        {
            bool checkWrapperStatus = checkWrapper.Status;
            return checkWrapperStatus != checkStatus;
        }
    }
}
