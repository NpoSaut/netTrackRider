using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

// Delete all unrelevant to conversion — is have to placed in gps datum itself.
namespace Simulator.GPS
{
    public class MNPBinary : IGPSProtocol
    {
        public struct MnpPacketHeader
        {
            public UInt16 sync;
            public UInt16 frame_id;
            public UInt16 data_len;
            public UInt16 reserved;
            public UInt16 header_checksum;
        }

        public struct Frame3000Packet
        {
            public MnpPacketHeader header;
            public UInt32 lat;      // radian
            public UInt32 lon;      // radian
            public UInt32 alt;   // meters
            public UInt32 speed;    // m/s
            public UInt32 azimuth;   // radian
            public UInt32 v_speed;  // m/s
            public UInt16 channels_in_sol;
            public UInt16 diff_asserts;
        }

        private bool gpsValid = true;
        private double currentSpeed = 0;
        private GPSDatum gpsDatum;

        public bool IsValid
        {
            set { this.gpsValid = value; }
            //get { return this.gpsValid; }
        }

        public double CurrentSpeed
        {
            set { this.currentSpeed = value; }
            //get { return this.currentSpeed; }
        }

        public byte[] GetPacket(GPSDatum d)
        {
            gpsDatum = d; // To return in GetPacketString()
            
            Frame3000Packet packet = new Frame3000Packet();
            MnpPacketHeader h = new MnpPacketHeader();

            h.sync = 0x81ff;
            h.frame_id = 3000;
            h.data_len = (ushort)(Marshal.SizeOf(packet) - Marshal.SizeOf(h));
            h.reserved = 0;
            h.header_checksum = 0;

            packet.header = h;
            packet.lat = d.i32Latitude;
            packet.lon = d.i32Longitude;
            packet.alt = 0;
            packet.speed = 0;
            packet.azimuth = 0;
            packet.v_speed = 0;
            packet.channels_in_sol = 0;
            packet.diff_asserts = 0;
            return GetBytes(packet);
        }

        private byte[] GetBytes(object packet)
        {
            int size = Marshal.SizeOf(packet);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public string GetPacketString()
        {
            return "MNPbinary packet: LAT = " + gpsDatum.Latitude + " LON = " + gpsDatum.Longitude;
        }
    }
}
