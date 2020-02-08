using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{
    // Command "listen": Listen for clients.
    // listen [--ipv6] [PORT]

    class CmdListen : ICommand
    {
        public string HelpResId() => "Commands.Help.Listen";
        public string HelpShortResId() => "Commands.HelpShort.Listen";

        public Dictionary<string, object> Parse(string cmd)
        {
            bool ipv6 = false;
            int port;
            List<string> extras;

            OptionSet options = new OptionSet()
            {
                { "ipv6", x => ipv6 = (x != null) }
            };

            try
            {
                extras = options.Parse(CommandLine.ToArgs(cmd));
            }
            catch(Exception e)
            {
                if (e is ArgumentException || e is OptionException)
                    throw new ArgumentException(e.Message);
                else
                    throw;
            }

            if(extras.Count == 0)
            {
                port = Puppet.Util.DEFAULT_PORT;
            }
            else if(extras.Count == 1)
            {
                try
                {
                    port = Int32.Parse(extras[0]);
                }
                catch(Exception e)
                {
                    if (e is FormatException || e is OverflowException)
                        throw new ArgumentException(e.Message);
                    else
                        throw;
                }
            }
            else
            {
                throw new ArgumentException();
            }

            return new Dictionary<string, object>()
            {
                { "verb", "listen" },
                { "ipv6", ipv6 },
                { "port", port }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            // TODO: "Commands.Listen.AlreadyStarted"
            if (Threads.Listener.theInstance != null)
                throw new ArgumentException(Program.GetResourceString("Commands.Listen.AlreadyStarted"));

            bool ipv6 = (bool)options["ipv6"];
            int port = (int)options["port"];

            try
            {
                Threads.Listener.CreateInstance(ipv6, port);
            }
            catch(Exception e)
            {
                throw new ArgumentException(e.ToString());
            }

            // TODO: "Commands.Listen.AvailableAddresses"
            Logger.I(Program.GetResourceString("Commands.Listen.AvailableAddresses"));

            List<IPAddress> addresses = new List<IPAddress>(Dns.GetHostAddresses(Dns.GetHostName()));
            addresses.Add(IPAddress.Loopback);
            addresses.Add(IPAddress.IPv6Loopback);
            foreach(IPAddress address in addresses)
            {
                if (address.AddressFamily != (ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork))
                    continue;

                if(port == Puppet.Util.DEFAULT_PORT)
                    Logger.I(address.ToString());
                else
                    Logger.I(ipv6 ? "[{0}]:{1}" : "{0}:{1}", address.ToString(), port);
            }
        }
    }
}
