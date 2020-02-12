using System;
using System.Text;

namespace PEDollController.BlobFormatters
{
    class FmtHex : IBlobFormatter
    {
        string PieceToRaw(byte[] blob, int offset)
        {
            int pieceLen = Math.Min(blob.Length - offset, 16);

            // Similar to FmtRaw.ToScreen(), but convert any control characters (not just '\r') to '.'.
            StringBuilder ret = new StringBuilder();

            for(int i = 0; i < pieceLen; i++)
            {
                char c = (char)blob[offset + i];
                // Excluded characters:
                // 1. Non-ASCII characters
                // 2. Control characters
                ret.Append((blob[offset + i] > 0x7f || Char.IsControl(c)) ? '.' : c);
            }

            return ret.ToString();
        }

        string PieceToHex(byte[] blob, ref int offset)
        {
            int pieceLen = Math.Min(blob.Length - offset, 8);
            string ret = (pieceLen > 0) ? BitConverter.ToString(blob, offset, pieceLen) : String.Empty;
            ret = ret.PadRight(3 * 8 - 1).Replace('-', ' ');

            offset += pieceLen;
            return ret;
        }

        public string ToScreen(byte[] blob)
        {
            StringBuilder ret = new StringBuilder();

            int offset = 0;
            while (offset < blob.Length)
            {
                // Offset
                ret.Append(offset.ToString("x8"));
                ret.Append("  ");

                // Divide a line of 16 bytes as two pieces of 8 bytes
                int offsetNew = offset;
                ret.Append(PieceToHex(blob, ref offsetNew));
                ret.Append("  ");
                ret.Append(PieceToHex(blob, ref offsetNew));
                ret.Append("  ");

                // ASCII
                ret.Append(PieceToRaw(blob, offset));
                ret.Append(Environment.NewLine);

                offset = offsetNew;
            }

            // Blob size at the last line
            ret.Append(offset.ToString("x8"));
            ret.Append(Environment.NewLine);

            return ret.ToString();
        }

        public byte[] ToFile(byte[] blob)
        {
            return Encoding.UTF8.GetBytes(ToScreen(blob));
        }
    }
}
