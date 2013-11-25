using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator.GPS
{
    public interface IGPSProtocol
    {
        //string Printable(GPSDatum d);
        double CurrentSpeed { set; /*get;*/ }
        bool IsValid { set; /*get;*/ }
        byte[] GetPacket(GPSDatum d);
        string GetPacketString();
    }
}
