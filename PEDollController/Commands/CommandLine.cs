using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace PEDollController.Commands
{
    public static class CommandLine
    {

        // CommandLine.ToArgv() provides an simple implemention of CommandLineToArgvW()
        // https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-commandlinetoargvw
        // However, this implemention allows parens outside of quotes

        // CommandLine.ToArgs() Removes argv[0] from result, make it easlier for Mono.Options to parse

        static Dictionary<char, char> PairingCharacters = new Dictionary<char, char>()
        {
            { '"', '"' },
            { '\'', '\'' },
            { '(', ')' },
            { '[', ']' },
            { '{', '}' }
        };

        public static List<string> ToArgv(string cmd, bool skipArgv0 = false)
        {
            List<string> ret = new List<string>();
            StringBuilder builder = new StringBuilder();

            Stack<char> pairingStack = new Stack<char>();
            bool isEscaped = false;
            foreach (char c in cmd)
            {
                char stackTop = (pairingStack.Count == 0) ? (char)0 : pairingStack.Peek();

                if (stackTop == '"' || stackTop == '\'')
                {
                    // If inside a pair of quotes, consider following rules:
                    // 1. (Escape rule) Character after '\\' is ignored (i.e. has no effect on rule appliance);
                    // 2. No match rule except quotes may apply
                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else
                    {
                        if (c == stackTop)
                            pairingStack.Pop();
                        else if (c == '\\')
                            isEscaped = true;
                    }
                }
                else
                {
                    // Otherwise, consider standard rules:
                    // 1. (Match rule) If `c` is a left-hand pairing character, push it onto the stack;
                    // 2. (Match rule) If `c` is a right-hand pairing character,
                    //    pop the stack if `c` matches the stack top, fail otherwise
                    // 3. (Separate rule) If `c` is a whitespace character and stack is empty,
                    //    ignore `c` and cut the string to be a piece of argv[]
                    if (PairingCharacters.ContainsKey(c))
                    {
                        pairingStack.Push(c);
                    }
                    else if (PairingCharacters.ContainsValue(c))
                    {
                        if (c == PairingCharacters[stackTop])
                            pairingStack.Pop();
                        else
                            throw new ArgumentException("Parens mismatch");
                    }
                    else if (Char.IsWhiteSpace(c) && pairingStack.Count == 0)
                    {
                        if (builder.Length > 0)
                        {
                            // Ignore argv[0] if required
                            if (skipArgv0)
                                skipArgv0 = false;
                            else
                                ret.Add(builder.ToString());

                            builder.Clear();
                        }
                        continue; // Ignore `c`
                    }
                }
                builder.Append(c); // Add `c` to current piece
            }

            // Add the leftover piece in builder
            if (builder.Length > 0 && !(skipArgv0 && ret.Count == 0))
                ret.Add(builder.ToString());

            // For now, if still in paren-matching state or escaped state, the string is incomplete and should fail
            if (isEscaped || pairingStack.Count != 0)
                throw new ArgumentException("Incomplete string");

            return ret;
        }
        
        public static List<string> ToArgs(string cmd)
        {
            return ToArgv(cmd, true);
        }
    }
}
