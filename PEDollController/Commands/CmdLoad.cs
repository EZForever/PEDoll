using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PEDollController.Commands
{

    // Command "load": Load commands from a script.
    // load <script[.txt]>
    // load "<script.txt>"

    class CmdLoad : ICommand
    {
        public string HelpResId() => "Commands.Help.Load";
        public string HelpShortResId() => "Commands.HelpShort.Load";

        public Dictionary<string, object> Parse(string cmd)
        {
            List<string> args = Commandline.ToArgs(cmd);
            string script = args[0];
            string parameters = String.Join(" ", args.Skip(1));

            if (script.StartsWith("\"") && script.EndsWith("\""))
            {
                // If embraced with double-quotes, treat the path as a qualified path and no more guessing
                script = script.Trim('"');
                try
                {
                    script = Path.GetFullPath(script);
                }
                catch(Exception e)
                {
                    if (e is ArgumentException || e is NotSupportedException || e is PathTooLongException)
                        throw new ArgumentException("script");
                    else
                        throw; // Not expected to be `catch`ed
                }
            }
            else
            {
                // Otherwise the input is treated as a plain file name
                foreach(char c in Path.GetInvalidFileNameChars())
                {
                    if (script.Contains(c))
                        throw new ArgumentException("script");
                }

                if (!Path.HasExtension(script))
                    script = Path.ChangeExtension(script, "txt");

                string selfPath = Path.GetDirectoryName(Application.ExecutablePath);
                string possiblePath = Path.Combine(selfPath, "Scripts", script);
                if (File.Exists(possiblePath))
                    script = possiblePath; // $self\Scripts\$script
                else
                    script = Path.Combine(selfPath, "Scripts", "API", script); // $self\Scripts\API\$script
            }

            return new Dictionary<string, object>()
            {
                { "verb", "load" },
                { "script", script },
                { "parameters", parameters }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            string script = (string)options["script"];
            string parameters = (string)options["parameters"];

            StreamReader reader = null;
            try
            {
                reader = new StreamReader(script);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.EndsWith("*"))
                        line = line.TrimEnd('*') + parameters;
                    Threads.CmdEngine.theInstance.AddCommand(line);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                throw new ArgumentException(Program.GetResourceString("Commands.IOError", e.GetType().Name, e.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }
    }

}
