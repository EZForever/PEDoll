using System;
using System.Text;

namespace PEDollController.BlobFormatters
{
    class FmtText : IBlobFormatter
    {
        Encoding encoding;

        public FmtText(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public string ToScreen(byte[] blob)
        {
            try
            {
                return encoding.GetString(blob);
            }
            catch(Exception e)
            {
                if (e is ArgumentException || e is DecoderFallbackException)
                    throw new ArgumentException(e.Message);
                else
                    throw;
            }
        }

        public byte[] ToFile(byte[] blob)
        {
            return Encoding.UTF8.GetBytes(ToScreen(blob));
        }
    }
}
