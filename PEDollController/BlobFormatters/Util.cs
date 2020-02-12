using System;
using System.Text;
using System.Collections.Generic;

namespace PEDollController.BlobFormatters
{
    static class Util
    {
        public static readonly Dictionary<string, IBlobFormatter> Formatters = new Dictionary<string, IBlobFormatter>()
        {
            { "hex", new FmtHex() },
            { "raw", new FmtRaw() },

            { "ansi", new FmtText(Encoding.Default) },
            { "unicode", new FmtText(Encoding.Unicode) },
            { "utf8", new FmtText(Encoding.UTF8) },

            { "x86", new FmtX86(false) },
            { "x64", new FmtX86(true) },
        };
    }
}
