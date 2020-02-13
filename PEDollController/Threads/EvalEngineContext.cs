// NOTE: This file will not get compiled into the assembly, but embedded into the resources.
// It is meant to be compiled by the EvalEngine at runtime.

// NOTE: The default C# compiler .NET Framework provides (Microsoft.CSharp.CSharpCodeProvider) only support up to C# 5.
// Any newer features will cause compile errors in this file.

using System;
using System.Linq;

#if CLIENT_X64

// x64 client
using word = System.Int64;
using uword = System.UInt64;

#else

// x86 client or in IDE
using word = System.Int32;
using uword = System.UInt32;

#endif // CLIENT_X64

namespace EvalEngineScope
{
    public class Context
    {
        // Import delegates
        readonly Func<uint, UInt64> _getContext;
        readonly Func<UInt64, string> _str;
        readonly Func<UInt64, string> _wstr;
        readonly Func<string, string> _ctx;
        readonly Func<UInt64, uint, byte[]> _mem;
        readonly Func<UInt64, UInt64> _poi;
        readonly Func<uint, UInt64> _arg;
        readonly Func<byte[], int> _dump;

        // Methods
        public string str(uword ptr) { return _str(ptr); }
        public string wstr(uword ptr) { return _wstr(ptr); }
        public string ctx(string key) { return _ctx(key); }
        public byte[] mem(uword ptr, uint len) { return _mem(ptr, len); }
        public uword poi(uword ptr) { return (uword)_poi(ptr); }
        public uword arg(uint index) { return (uword)_arg(index); }
        public int dump(byte[] blob) { return _dump(blob); }

        // Readonly register fields
        public uword ax { get { return (uword)_getContext(0); } }
        public uword cx { get { return (uword)_getContext(1); } }
        public uword dx { get { return (uword)_getContext(2); } }
        public uword bx { get { return (uword)_getContext(3); } }
        public uword sp { get { return (uword)_getContext(4); } }
        public uword bp { get { return (uword)_getContext(5); } }
        public uword si { get { return (uword)_getContext(6); } }
        public uword di { get { return (uword)_getContext(7); } }

#       if CLIENT_X64
        public uword r8 { get { return (uword)_getContext(8); } }
        public uword r9 { get { return (uword)_getContext(9); } }
#       endif

        public Context(Delegate[] imports)
        {
            _getContext = (Func<uint, UInt64>)imports[0];
            _str = (Func<UInt64, string>)imports[1];
            _wstr = (Func<UInt64, string>)imports[2];
            _ctx = (Func<string, string>)imports[3];
            _mem = (Func<UInt64, uint, byte[]>)imports[4];
            _poi = (Func<UInt64, UInt64>)imports[5];
            _arg = (Func<uint, UInt64>)imports[6];
            _dump = (Func<byte[], int>)imports[7];
        }

        public object[] Invoke()
        {
            return new object[] { /* expr */ };
        }
    }
}
