using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PEDollController.Threads
{
    class Listener
    {
        // The Listener instance will be created by CmdEngine
        public static Listener theInstance;
        public static Task theTask;

        public static void CreateInstance(bool ipv6, int port)
        {
            try
            {
                theInstance = new Listener(ipv6, port);
                theTask = new Task(theInstance.TaskMain);
                theTask.Start();
            }
            catch
            {
                theInstance = null;
                theTask = null;
                throw;
            }
        }

        TcpListener listener;

        Listener(bool ipv6, int port)
        {
            // IPv6Any without IPv6Only will listen on both v4 & v6 interfaces
            listener = new TcpListener(ipv6 ? IPAddress.IPv6Any : IPAddress.Any, port);
            listener.Start();
        }

        void TaskMain()
        {
            while(true)
            {
                Task<TcpClient> taskTcp = listener.AcceptTcpClientAsync();
                int idx = Task.WaitAny(CmdEngine.theInstance.stopTaskAsync, taskTcp);

                if(idx == 0)
                {
                    // stopTaskAsync triggered
                    listener.Stop();
                    break;
                }
                else
                {
                    Client.CreateInstance(taskTcp.Result);
                }
            }
        }
    }
}
