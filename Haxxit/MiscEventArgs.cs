using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit
{
    public class SilicoinEventArgs : EventArgs
    {
        public ushort Silicoins { get; private set; }

        public SilicoinEventArgs(ushort num_silicoins) :
            base()
        {
            Silicoins = num_silicoins;
        }
    }
}