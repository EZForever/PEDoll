﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "rem": Comment. Do nothing.
    // rem [anything]...
    // #[anything]...

    class CmdDump : ICommand
    {
        public string HelpResId() => "Commands.Help.Dump";
        public string HelpShortResId() => "Commands.HelpShort.Dump";

        public Dictionary<string, object> Parse(string cmd)
        {
            int id = -1;
            string format = BlobFormatters.Util.Formatters.Keys.First();
            string save = null;

            OptionSet options = new OptionSet()
            {
                {
                    "format=",
                    x =>
                    {
                        if(!BlobFormatters.Util.Formatters.ContainsKey(x))
                            throw new ArgumentException("format");
                        format = x;
                    }
                },
                {
                    "save=",
                    x =>
                    {
                        save = Util.RemoveQuotes(x);
                    }
                },
                {
                    "<>",
                    (uint x) =>
                    {
                        if(x >= Threads.CmdEngine.theInstance.dumps.Count)
                            throw new ArgumentException("id");
                        id = (int)x;
                    }
                }
            };
            Util.ParseOptions(cmd, options);

            return new Dictionary<string, object>()
            {
                { "verb", "dump" },
                { "id", id },
                { "format", format },
                { "save", save }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            int id = (int)options["id"];
            string format = (string)options["format"];
            string save = (string)options["save"];

            if(id < 0)
            {
                Logger.I(Program.GetResourceString("Commands.Dump.Header"));

                for (int i = 0; i < Threads.CmdEngine.theInstance.dumps.Count; i++)
                {
                    Threads.DumpEntry entry = Threads.CmdEngine.theInstance.dumps[i];

                    Logger.I(Program.GetResourceString("Commands.Dump.Format",
                        i,
                        entry.Data.Length,
                        entry.Source
                    ));
                }
                return;
            }

            BlobFormatters.IBlobFormatter formatter = BlobFormatters.Util.Formatters[format];
            Threads.DumpEntry dump = Threads.CmdEngine.theInstance.dumps[id];

            if(save == null)
            {
                Logger.I(Program.GetResourceString("Commands.Dump.Title", id, dump.Source, dump.Data.Length, format));

                string formatted = formatter.ToScreen(dump.Data);
                Logger.I(formatted);
                Threads.Gui.theInstance.InvokeOn((FMain Me) =>
                {
                    Me.txtDumpContent.Text = formatted;
                });
            }
            else
            {
                BinaryWriter writer = null;
                try
                {
                    writer = new BinaryWriter(File.Open(save, FileMode.Create));
                    writer.Write(formatter.ToFile(dump.Data));
                    writer.Close();
                }
                catch (Exception e)
                {
                    throw new ArgumentException(Program.GetResourceString("Commands.IOError", e.GetType().Name, e.Message));
                }
                finally
                {
                    if (writer != null)
                        writer.Dispose();
                }
            }
        }
    }
}
