using System;
using System.Runtime.InteropServices;

namespace PEDollController.Puppet
{
    // All of this are translated from libPuppet/libPuppet.h
    // NOTE: NEVER do a "new PACKET_*" without any parameters, even if they have optional values
    //       The default constructor is NOT overridden and will set everything to zero
    //       For the packets that do not need any parameters, an extra integer is added to the parameter list

    enum PACKET_TYPE : UInt32
    {
        // Generic packets conveys plain data, the meanings of them are defined by previous packet(s)
        ANY = 0x00,
        ACK = 0x01,
        INTEGER = 0x02,
        STRING = 0x03,
        BINARY = 0x04,

        // Message packets are actively sent from a Monitor or Doll, in order to notify the Controller
        MSG_ANY = 0x10,
        MSG_ONLINE = 0x11,
        MSG_ONHOOK = 0x12,

        // Command packets are sent from Controller to a Monitor or Doll, in order to controll them
        CMD_MONITOR_ANY = 0x20,
        CMD_END = 0x21,
        CMD_DOLL = 0x22,
        CMD_PS = 0x23,
        CMD_SHELL = 0x24,
        CMD_KILL = 0x25,

        CMD_DOLL_ANY = 0x30,
        CMD_HOOK = 0x31,
        CMD_UNHOOK = 0x32,
        CMD_BREAK = 0x33,
        CMD_CONTEXT = 0x34,
        CMD_MEMORY = 0x35,
        CMD_VERDICT = 0x36,
        CMD_LOADDLL = 0x37,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET
    {
        public UInt32 size;
        public PACKET_TYPE type;

        public PACKET(UInt32 s = 0, PACKET_TYPE t = PACKET_TYPE.ANY)
        {
            size = s == 0 ? (UInt32)Marshal.SizeOf(typeof(PACKET)) : s;
            type = t;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_ACK
    {
        public PACKET header;
        public UInt32 status;

        public PACKET_ACK(UInt32 s = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_ACK)), PACKET_TYPE.ACK);
            status = s;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_INTEGER
    {
        public PACKET header;
        public UInt64 data;

        public PACKET_INTEGER(UInt64 d = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_INTEGER)), PACKET_TYPE.INTEGER);
            data = d;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_STRING
    {
        public PACKET header;
        //public char data[0]; // Use Puppet.Util.(S|Des)erializeString()

        public PACKET_STRING(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_STRING)), PACKET_TYPE.STRING);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_BINARY
    {
        public PACKET header;
        //public byte data[0]; // Use Puppet.Util.(S|Des)erializeBinary()

        public PACKET_BINARY(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_BINARY)), PACKET_TYPE.BINARY);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_MSG_ONLINE
    {
        public PACKET header;
        public UInt32 isMonitor;
        public UInt32 bits;
        public UInt32 pid;

        public PACKET_MSG_ONLINE(UInt32 s = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_MSG_ONLINE)), PACKET_TYPE.MSG_ONLINE);
            isMonitor = 0;
            bits = 64;
            pid = 0xffffffff;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_MSG_ONHOOK
    {
        public PACKET header;
        public UInt32 phase;

        public PACKET_MSG_ONHOOK(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_MSG_ONHOOK)), PACKET_TYPE.MSG_ONHOOK);
            phase = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_END
    {
        public PACKET header;

        public PACKET_CMD_END(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_END)), PACKET_TYPE.CMD_END);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_DOLL
    {
        public PACKET header;
        public UInt32 pid;

        public PACKET_CMD_DOLL(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_DOLL)), PACKET_TYPE.CMD_DOLL);
            pid = 0xffffffff;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_PS
    {
        public PACKET header;

        public PACKET_CMD_PS(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_PS)), PACKET_TYPE.CMD_PS);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_SHELL
    {
        public PACKET header;

        public PACKET_CMD_SHELL(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_SHELL)), PACKET_TYPE.CMD_SHELL);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_KILL
    {
        public PACKET header;
        public UInt32 pid;

        public PACKET_CMD_KILL(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_KILL)), PACKET_TYPE.CMD_KILL);
            pid = 0xffffffff;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_HOOK
    {
        public PACKET header;
        public UInt32 method;

        public PACKET_CMD_HOOK(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_HOOK)), PACKET_TYPE.CMD_HOOK);
            method = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_UNHOOK
    {
        public PACKET header;

        public PACKET_CMD_UNHOOK(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_UNHOOK)), PACKET_TYPE.CMD_UNHOOK);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_BREAK
    {
        public PACKET header;

        public PACKET_CMD_BREAK(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_BREAK)), PACKET_TYPE.CMD_BREAK);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_CONTEXT
    {
        public PACKET header;
        public UInt32 idx;

        public PACKET_CMD_CONTEXT(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_CONTEXT)), PACKET_TYPE.CMD_CONTEXT);
            idx = 0xffffffff;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_MEMORY
    {
        public PACKET header;
        public UInt32 len;

        public PACKET_CMD_MEMORY(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_MEMORY)), PACKET_TYPE.CMD_MEMORY);
            len = 0xffffffff;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_VERDICT
    {
        public PACKET header;
        public UInt32 verdict;

        public PACKET_CMD_VERDICT(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_VERDICT)), PACKET_TYPE.CMD_VERDICT);
            verdict = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PACKET_CMD_LOADDLL
    {
        public PACKET header;

        public PACKET_CMD_LOADDLL(int _ = 0)
        {
            header = new PACKET((UInt32)Marshal.SizeOf(typeof(PACKET_CMD_LOADDLL)), PACKET_TYPE.CMD_LOADDLL);
        }
    }

}
