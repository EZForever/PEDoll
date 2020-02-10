using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PEDollController.Threads
{
    struct HookEntry
    {
        public string name;
        public UInt64 oep;

        public string addrMode; // {null|"symbol"|"addr"|"pattern"}
        public string symbol;
        public UInt64 addr;
        public byte[] pattern;

        public string convention; // {"stdcall"|"cdecl"|"fastcall"|"msvc"|"gcc"}, default value set on Invoke()
        public UInt64 stack;
        public UInt64 ret;

        public List<Dictionary<string, object>> beforeActions;
        public string beforeVerdict; // {null|"approve"|"reject"|"terminate"}

        public List<Dictionary<string, object>> afterActions;
        public string afterVerdict; // {null|"approve"|"terminate"}
    }

    class Client
    {
        // The Client instances will be created by Listener
        public static List<Client> theInstances = new List<Client>();
        public static List<Task> theTasks = new List<Task>();

        public static void CreateInstance(TcpClient client)
        {
            Client instance = new Client(client);
            Task task = new Task(instance.TaskMain);
            theInstances.Add(instance);
            theTasks.Add(task);
            task.Start();
        }

        // ----------

        TcpClient client;
        NetworkStream stream;
        BlockingQueue<byte[]> rxQueue;

        public bool isMonitor;
        public bool isDead => !client.Connected;
        public int bits;
        public int pid;
        public string clientName;

        public UInt64 hookOep;
        public UInt32 hookPhase;
        public List<HookEntry> hooks;
        public Dictionary<string, string> context;

        Client(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();

            try
            {
                // Receive MSG_ONLINE
                byte[] bufOnline = Expect(Puppet.PACKET_TYPE.MSG_ONLINE, true);
                if (bufOnline == null)
                    return;

                Puppet.PACKET_MSG_ONLINE pktOnline = Puppet.Util.Deserialize<Puppet.PACKET_MSG_ONLINE>(bufOnline);
                isMonitor = (pktOnline.isMonitor != 0);
                bits = (int)pktOnline.bits;
                pid = (int)pktOnline.pid;

                // Receive client name
                byte[] bufOnlineName = Expect(Puppet.PACKET_TYPE.STRING, true);
                if (bufOnlineName == null)
                    return;
                clientName = Puppet.Util.DeserializeString(bufOnlineName);

                // Reply with ACK
                SendAck(0);
            }
            catch (IOException e)
            {
                // TODO: "Threads.Client.Disconnected"
                // "Client '{0}' disconnected, reason: {1}"
                Logger.W(Program.GetResourceString("Threads.Client.Disconnect", clientName, e.Message));
                stream.Close();
                client.Close();
                // TODO: Refresh targets' list
            }

            // Initialize packet receive queue
            rxQueue = new BlockingQueue<byte[]>();

            // Initialize hook list and context dictionary
            hookOep = 0; // A Monitor will never enter the Hooked state
            if (!isMonitor)
            {
                hooks = new List<HookEntry>();
                context = new Dictionary<string, string>();
            }

            int idx = theInstances.Count; // For now the instance is not included into theInstances

            // If no current target, set current target to self
            if (CmdEngine.theInstance.target < 0)
                CmdEngine.theInstance.target = idx;

            // If last Monitor or Doll is not set, set them
            if(isMonitor)
            {
                if (CmdEngine.theInstance.targetLastMonitor < 0)
                    CmdEngine.theInstance.targetLastMonitor = idx;
            }
            else
            {
                if (CmdEngine.theInstance.targetLastDoll < 0)
                    CmdEngine.theInstance.targetLastDoll = idx;
            }

            // TODO: "Threads.Client.Connected"
            Logger.N("New Client #{0} from {1}: isMonitor = {2}, bits = {3}, pid = {4}, clientName = {5}",
                idx,
                client.Client.RemoteEndPoint.ToString(),
                isMonitor, bits, pid, clientName
            );

            // TODO: Refresh targets' list
        }

        void TaskMain()
        {
            try
            {
                while(true)
                {
                    byte[] buffer = ReceiveDirect();
                    if (buffer == null)
                        break;
                    OnRecv(buffer);
                }
            }
            catch(IOException e)
            {
                // TODO: "Threads.Client.Disconnected"
                // "Client '{0}' disconnected, reason: {1}"
                Logger.W(Program.GetResourceString("Threads.Client.Disconnect", clientName, e.Message));
            }
            finally
            {
                stream.Close();
                client.Close();
                // TODO: Refresh targets' list, refresh target state to DEAD
            }
        }

        void SendAck(UInt32 status)
        {
            byte[] bufAck = Puppet.Util.Serialize(new Puppet.PACKET_ACK(status));
            stream.Write(bufAck, 0, bufAck.Length);
        }

        byte[] ReceiveDirect()
        {
            byte[] bufPacketSize = new byte[sizeof(UInt32)];
            Task<int> taskRecv = stream.ReadAsync(bufPacketSize, 0, sizeof(UInt32), default);
            int idx = Task.WaitAny(CmdEngine.theInstance.stopTaskAsync, taskRecv);

            if (idx == 0)
            {
                // stopTaskAsync(from `this` or CmdEngine) triggered
                return null;
            }
            else if (taskRecv.Status == TaskStatus.Faulted || taskRecv.Result < sizeof(UInt32))
            {
                // TODO: "Threads.Client.MalformedPacket"
                throw (taskRecv.Exception == null) ? 
                    new IOException(Program.GetResourceString("Threads.Client.MalformedPacket"))
                    : taskRecv.Exception.InnerException;
            }
            else
            {
                int size = (int)BitConverter.ToUInt32(bufPacketSize, 0);
                byte[] bufPacket = new byte[size];
                bufPacketSize.CopyTo(bufPacket, 0);
                if (stream.Read(bufPacket, sizeof(UInt32), size - sizeof(UInt32)) + sizeof(UInt32) < size)
                    throw new IOException(Program.GetResourceString("Threads.Client.MalformedPacket"));

                return bufPacket;
            }
        }

        void OnRecv(byte[] buffer)
        {
            switch (Puppet.Util.Deserialize<Puppet.PACKET>(buffer).type)
            {
                case Puppet.PACKET_TYPE.MSG_ONHOOK:
                    // Set hookPhase
                    hookPhase = Puppet.Util.Deserialize<Puppet.PACKET_MSG_ONHOOK>(buffer).phase;

                    // Receive hookOep
                    Puppet.PACKET_INTEGER pktOep;
                    pktOep = Puppet.Util.Deserialize<Puppet.PACKET_INTEGER>(Expect(Puppet.PACKET_TYPE.INTEGER, true));
                    hookOep = pktOep.data;

                    // Reply with ACK
                    SendAck(0);

                    OnHook();
                    break;
                default:
                    // Save packet for command usage
                    rxQueue.BlockingEnqueue(buffer);
                    break;
            }
        }

        void OnHook()
        {
            HookEntry entry = new HookEntry();
            foreach (HookEntry hook in hooks)
            {
                if(hook.oep == hookOep)
                {
                    entry = hook;
                    break;
                }
            }
            if (entry.name == null) // Uninitialized
                throw new IOException(Program.GetResourceString("Threads.Client.UnknownHook"));

            // TODO: "Threads.Client.Hooked"
            // "Client \"{0}\" hooked on \"{1}\" - phase \"{2}\""
            // FIXME: Fit description above
            Logger.N(Program.GetResourceString("Threads.Client.Hooked"));

            List<Dictionary<string, object>> actions = (hookPhase == 0) ? entry.beforeActions : entry.afterActions;
            string verdict = (hookPhase == 0) ? entry.beforeVerdict : entry.afterVerdict;

            foreach(Dictionary<string, object> action in actions)
            {
                // TODO: Implement actions (Req. EvalEngine)
                switch((string)action["verb"])
                {
                    case "echo":
                        break;
                    case "dump":
                        break;
                    case "ctx":
                        break;
                    default:
                        // Unknown action. Should not happen.
                        throw new IOException();
                }
            }

            if(verdict == null)
            {
                // TODO: Refresh hooked state
            }
            else
            {
                // TODO: `verdict` but must be command-line independant, due to target changing
            }
        }

        public void Send(byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        public byte[] Receive()
        {
            return rxQueue.BlockingDequeue();
        }

        public byte[] Expect(Puppet.PACKET_TYPE type, bool direct = false)
        {
            byte[] buffer = direct ? ReceiveDirect() : Receive();
            if (buffer == null)
                return null;

            if (Puppet.Util.Deserialize<Puppet.PACKET>(buffer).type != type)
            {
                if(direct)
                {
                    // IOException can be handled by .ctor() / TaskMain()
                    throw new IOException(Program.GetResourceString("Threads.Client.MalformedPacket"));
                }
                else
                {
                    // Kill client, and return an error for command execution
                    stream.Close();
                    client.Close();
                    throw new ArgumentException(Program.GetResourceString("Threads.Client.MalformedPacket"));
                }
            }
            return buffer;
        }
    }
}
