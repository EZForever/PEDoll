using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PEDollController.Threads
{
    struct HookEntry
    {
        // TODO
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
            if (!isMonitor)
            {
                hooks = new List<HookEntry>();
                context = new Dictionary<string, string>();
            }

            int idx = theInstances.Count; // For now the instance is not included into theInstances

            // TODO: "Threads.Client.Connect"?
            Logger.N("New Client #{0} from {1}: isMonitor = {2}, bits = {3}, pid = {4}, clientName = {5}",
                idx,
                client.Client.RemoteEndPoint.ToString(),
                isMonitor, bits, pid, clientName
            ); ;

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

        void OnRecv(byte[] buffer)
        {
            switch (Puppet.Util.Deserialize<Puppet.PACKET>(buffer).type)
            {
                case Puppet.PACKET_TYPE.MSG_ONHOOK:
                    // TODO: Change self hooking state
                    SendAck(0);
                    break;
                default:
                    // Save packet for command usage
                    rxQueue.BlockingEnqueue(buffer);
                    break;
            }
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
