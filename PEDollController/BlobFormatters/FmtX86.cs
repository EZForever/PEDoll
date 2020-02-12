using System;
using System.Text;

using Gee.External.Capstone;
using Gee.External.Capstone.X86;

namespace PEDollController.BlobFormatters
{
    class FmtX86 : IBlobFormatter
    {
        CapstoneX86Disassembler disassembler;

        public FmtX86(bool isX64)
        {
            disassembler = CapstoneDisassembler.CreateX86Disassembler(isX64 ? X86DisassembleMode.Bit64 : X86DisassembleMode.Bit32);
            disassembler.DisassembleSyntax = DisassembleSyntax.Intel;

            // Represent invalid instructions as "db 0x.."
            disassembler.EnableSkipDataMode = true;
            disassembler.SkipDataInstructionMnemonic = "db";
        }

        ~FmtX86()
        {
            if (disassembler != null)
                disassembler.Dispose();
        }

        public string ToScreen(byte[] blob)
        {
            StringBuilder ret = new StringBuilder();
            X86Instruction[] instructions = disassembler.Disassemble(blob, 0);

            int maxbytes = 0;
            foreach (X86Instruction instruction in instructions)
            {
                maxbytes = Math.Max(maxbytes, instruction.Bytes.Length);

                // About 98.5% of IA32 instructions are shorter than 8 bytes
                // Reference: Fig.2 from https://www.strchr.com/x86_machine_code_statistics
                if (maxbytes > 8)
                {
                    maxbytes = 8;
                    break;
                }
            }

            foreach (X86Instruction instruction in instructions)
            {
                int bytesLen = instruction.Bytes.Length;
                string bytesStr;
                if (bytesLen > maxbytes)
                {
                    bytesStr = BitConverter.ToString(instruction.Bytes, 0, maxbytes - 1) + " ..";
                }
                else
                {
                    bytesStr = BitConverter.ToString(instruction.Bytes).PadRight(3 * maxbytes - 1);
                }
                bytesStr = bytesStr.Replace("-", " ");

                // if (!instruction.IsSkippedData) { ...}
                //ret.Append(String.Format("{0:x8}  {1}  {2}\t{3}", instruction.Address, bytesStr, instruction.Mnemonic, instruction.Operand));
                ret.Append(instruction.Address.ToString("x8"));
                ret.Append("  ");
                ret.Append(bytesStr);
                ret.Append("  ");
                ret.Append(instruction.Mnemonic);
                ret.Append('\t');
                ret.Append(instruction.Operand);
                ret.Append(Environment.NewLine);
            }

            return ret.ToString();
        }

        public byte[] ToFile(byte[] blob)
        {
            return Encoding.UTF8.GetBytes(ToScreen(blob));
        }
    }
}
