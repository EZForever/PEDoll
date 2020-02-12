using System;
using System.Text;

namespace PEDollController.BlobFormatters
{
    class FmtRaw : IBlobFormatter
    {
        public string ToScreen(byte[] blob)
        {
            StringBuilder ret = new StringBuilder();

            foreach (byte b in blob)
            {
                char c = (char)b;
                // Excluded characters:
                // 1. Non-ASCII characters
                // 2. '\r' (Will cause characters being overwritten if displayed on console)
                // 3. Other non-whitespace control characters
                if (b > 0x7f || c == '\r' || (Char.IsControl(c) && !Char.IsWhiteSpace(c)))
                    ret.Append('.');
                else
                    ret.Append(c);
            }

            return ret.ToString();
        }

        public byte[] ToFile(byte[] blob)
        {
            return blob;
        }
    }
}
