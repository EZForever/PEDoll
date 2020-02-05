using System;
using System.Text;
using System.Runtime.InteropServices;

namespace PEDollController.Puppet
{

    static class Util
    {
        // Credit of (S|Des)erialize<T>: https://stackoverflow.com/a/3278956

        public static readonly int DEFAULT_PORT = 31415;

        public static byte[] Serialize<T>(T obj)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = new byte[size];

            IntPtr pData = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, pData, true);
            Marshal.Copy(pData, data, 0, size);
            Marshal.FreeHGlobal(pData);

            return data;
        }

        public static byte[] SerializeString(string str)
        {
            PACKET_STRING obj = new PACKET_STRING(0);
            int strSize = sizeof(char) * (str.Length + 1);
            obj.header.size += (UInt32)strSize;

            byte[] pktHeader = Serialize(obj);
            byte[] strBytes = Encoding.Unicode.GetBytes(str);

            byte[] data = new byte[obj.header.size];
            pktHeader.CopyTo(data, 0);
            strBytes.CopyTo(data, pktHeader.Length);
            return data;
        }

        public static byte[] SerializeBinary(byte[] bin)
        {
            PACKET_BINARY obj = new PACKET_BINARY(0);
            obj.header.size += (UInt32)bin.Length;
            byte[] pktHeader = Serialize(obj);
            byte[] data = new byte[obj.header.size];

            pktHeader.CopyTo(data, 0);
            bin.CopyTo(data, pktHeader.Length);
            return data;
        }

        public static T Deserialize<T>(byte[] data)
        {
            int size = Marshal.SizeOf(typeof(T));
            T obj;

            IntPtr pData = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, pData, size);
            obj = (T)Marshal.PtrToStructure(pData, typeof(T));
            Marshal.FreeHGlobal(pData);

            return obj;
        }

        public static string DeserializeString(byte[] data)
        {
            int offset = Marshal.SizeOf(typeof(PACKET_STRING));
            return Encoding.Unicode.GetString(data, offset, data.Length - offset);
        }

        public static byte[] DeserializeBinary(byte[] data)
        {
            int size = data.Length - Marshal.SizeOf(typeof(PACKET_BINARY));
            byte[] bin = new byte[size];
            data.CopyTo(bin, Marshal.SizeOf(typeof(PACKET_BINARY)));
            return bin;
        }

    }

}
