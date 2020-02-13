using System;

namespace PEDollController
{
    static class Logger
    {
        // Info, Notice, Highlight, Warning, Error
        static readonly ConsoleColor colorI = ConsoleColor.Gray;
        static readonly ConsoleColor colorN = ConsoleColor.Green;
        static readonly ConsoleColor colorH = ConsoleColor.White;
        static readonly ConsoleColor colorW = ConsoleColor.Yellow;
        static readonly ConsoleColor colorE = ConsoleColor.Red;

        public static void Write(ConsoleColor color, string msg, object[] args = null)
        {
            Console.ForegroundColor = color;
            if (args == null)
                Console.WriteLine(msg);
            else
                Console.WriteLine(msg, args);
            Console.ResetColor();
        }

        public static void I(string msg) => Write(colorI, msg);
        public static void I(string msg, params object[] args) => Write(colorI, msg, args);

        public static void N(string msg) => Write(colorN, msg);
        public static void N(string msg, params object[] args) => Write(colorN, msg, args);

        public static void H(string msg) => Write(colorH, msg);
        public static void H(string msg, params object[] args) => Write(colorH, msg, args);

        public static void W(string msg) => Write(colorW, msg);
        public static void W(string msg, params object[] args) => Write(colorW, msg, args);

        public static void E(string msg) => Write(colorE, msg);
        public static void E(string msg, params object[] args) => Write(colorE, msg, args);
    }
}
