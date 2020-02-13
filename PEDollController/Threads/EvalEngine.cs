using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Generic;

using Microsoft.CSharp;

namespace PEDollController.Threads
{
    static class EvalEngine
    {
        static readonly string contextSourceName = "PEDollController.Threads.EvalEngineContext.cs";
        static readonly string contextSourceExprMarker = "/* expr */";
        static readonly string contextSource;

        static CodeDomProvider provider;
        static CompilerParameters parameters;

        static EvalEngine()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(contextSourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                contextSource = reader.ReadToEnd();
            }

            provider = new CSharpCodeProvider();
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            // Allow using of full System namespace, and System.Linq namespace
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll"); // The LINQ extensions
            parameters.ReferencedAssemblies.Add("System.Linq.dll");
        }

        // DARK MAGIC HAPPENS AT HERE
        public static string Repr(object obj)
        {
            string typeName = obj.GetType().Name;
            string format;

            switch (typeName)
            {
                case "Byte":
                    format = "0x{0:x2}"; break;
                case "UInt16":
                    format = "0x{0:x4}"; break;
                case "UInt32":
                    format = "0x{0:x8}"; break;
                case "UInt64":
                    format = "0x{0:x16}"; break;
                case "Int16":
                case "Int32":
                case "Int64":
                    format = "{0}"; break;
                case "String":
                    format = "\"{0}\""; break;
                case "Char":
                    format = "'{0}'"; break;
                default:
                    if (typeName.EndsWith("[]")) // Array
                    {
                        // For now `obj` has a type of `xxx[]`, while `xxx` remain dynamic.
                        // However any array is a subclass of System.Array, and System.Array implements a generic IEnumerable,
                        // and generic IEnumerable can be Cast()'ed into an IEnumerable<T>,
                        // making displaying a dynamic-typed array possible.

                        // Additional "{{"s are for escaping the embracing "{", in order to make String.Format happy
                        format = $"{{{{ { String.Join(", ", ((Array)obj).Cast<object>().Select(Repr)) } }}}}";
                        break;
                    }
                    else
                    {
                        throw new ArgumentException(Program.GetResourceString("Threads.EvalEngine.InvalidType", typeName));
                    }
            }

            return String.Format(format, obj);
        }

        public static object[] Eval(Client client, IEnumerable<string> exprs)
        {
            // NOTE: Sanity checks and the removing of braces are done in EvalString()

            // Convert expressions into array initializers
            // (string[]){ "1+2", "poi(ax)+4" } => "(1+2), (poi(ax)+4)"
            string expr = $"({ String.Join("), (", exprs) })";
            string source = contextSource.Replace(contextSourceExprMarker, expr);

            // Set global build options
            parameters.CompilerOptions = "";

            // Set `#define CLIENT_X64` on a x64 client
            if (client.bits == 64)
                parameters.CompilerOptions += " -define:CLIENT_X64";

            // Compile the modified source
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);
            foreach (CompilerError error in results.Errors)
            {
                if (error.IsWarning)
                    continue; // Programmers never care about warnings

                throw new ArgumentException(Program.GetResourceString("Threads.EvalEngine.CompileError", error.ErrorText));
            }

            // Create EvalEngineScope.Context instance and fill in import delegates
            Assembly assembly = results.CompiledAssembly;
            Type contextType = assembly.GetType("EvalEngineScope.Context");
            ConstructorInfo contextCtor = contextType.GetConstructor(new Type[] { typeof(Delegate[]) });
            object context = contextCtor.Invoke(new object[] { client.Exports });

            // Call EvalEngineScope.Context.Invoke
            MethodInfo methodInvoke = contextType.GetMethod("Invoke");
            object[] ret;
            
            try
            {
                ret = (object[])methodInvoke.Invoke(context, null);
            }
            catch(TargetInvocationException e)
            {
                throw new ArgumentException(Program.GetResourceString("Threads.EvalEngine.RuntimeError", e.InnerException.GetType().Name, e.InnerException.Message));
            }

            return ret;
        }

        public static string EvalString(Client client, string str)
        {
            StringBuilder pieces = new StringBuilder();
            List<string> exprs = new List<string>();

            // FIXME: This parsing loop has loopholes. Should rethink and rewrite if necessary.
            int currentIdx = 0, prevIdx = 0;
            int formatIdx = 0;
            while ((currentIdx = str.IndexOf('{', currentIdx)) >= 0)
            {
                // Save non-expression pieces first
                pieces.Append(str.Substring(prevIdx, currentIdx - prevIdx));
                prevIdx = currentIdx;

                int stk = 0;
                bool isEscaped = false; // Escaping inside an expression must be considered
                while (currentIdx < str.Length)
                {
                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else
                    {
                        char c = str[currentIdx];
                        if (c == '\\')
                        {
                            isEscaped = true;
                        }
                        else if (c == '{')
                        {
                            stk++;
                        }
                        else if (c == '}')
                        {
                            stk--;
                            if (stk == 0)
                            {
                                break;
                            }
                            else if (stk < 0)
                            {
                                throw new ArgumentException(Program.GetResourceString("Commands.ParensMismatch"));
                            }
                        }
                    }
                    currentIdx++;
                }

                if (isEscaped || stk != 0)
                    throw new ArgumentException(Program.GetResourceString("Commands.Incomplete"));

                string expr = str.Substring(prevIdx + 1, currentIdx - prevIdx - 1);
                if (String.IsNullOrWhiteSpace(expr))
                    throw new ArgumentException(Program.GetResourceString("Threads.EvalEngine.Empty"));
                exprs.Add(expr);

                // Insert format identifier into string pieces
                pieces.Append('{');
                pieces.Append(formatIdx.ToString());
                pieces.Append('}');
                formatIdx++;

                prevIdx = currentIdx + 1;
            }

            // Add last non-expression piece
            pieces.Append(str.Substring(prevIdx));

            if (exprs.Count == 0)
                return pieces.ToString();
            else
                return String.Format(pieces.ToString(), Eval(client, exprs).Select(Repr).ToArray());
        }
    }
}
