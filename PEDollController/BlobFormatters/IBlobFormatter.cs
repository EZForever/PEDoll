using System;

namespace PEDollController.BlobFormatters
{
    interface IBlobFormatter
    {
        string ToScreen(byte[] blob);

        byte[] ToFile(byte[] blob);
    }
}
