using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PEDollController.Threads
{
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

        public bool isMonitor;
        public int bits;
        public int pid;
        public string clientName;
        public bool isDead => client.Connected;
        TcpClient client;
        NetworkStream stream;


        Client(TcpClient client)
        {
            Console.WriteLine("Connection from {0}", client.Client.RemoteEndPoint.ToString());
            this.client = client;
            this.stream = client.GetStream();
        }

        void TaskMain()
        {
            // Receive MSG_ONLINE
            byte[] bufOnline = Expect(Puppet.PACKET_TYPE.MSG_ONLINE);
            if (bufOnline == null)
                return;

            Puppet.PACKET_MSG_ONLINE pktOnline = Puppet.Util.Deserialize<Puppet.PACKET_MSG_ONLINE>(bufOnline);
            isMonitor = (pktOnline.isMonitor != 0);
            bits = (int)pktOnline.bits;
            pid = (int)pktOnline.pid;

            // Receive client name
            byte[] bufOnlineName = Expect(Puppet.PACKET_TYPE.STRING);
            if (bufOnlineName == null)
                return;
            clientName = Puppet.Util.DeserializeString(bufOnlineName);

            // Reply with ACK
            SendAck(0);

            Console.WriteLine("New Client: isMonitor = {0}, bits = {1}, pid = {2}, clientName = {3}",
                isMonitor, bits, pid, clientName
            );

            while(true)
            {
                byte[] buffer = Receive();
                if (buffer == null)
                    break;
                OnRecv(buffer);
            }
        }

        byte[] Receive()
        {
            byte[] bufPacketSize = new byte[4];
            Task<int> taskRecv = stream.ReadAsync(bufPacketSize, 0, 4, default);
            int idx = Task.WaitAny(CmdEngine.theInstance.stopTaskAsync, taskRecv);

            if (idx == 0 || taskRecv.Status == TaskStatus.Faulted || taskRecv.Result < 4)
            {
                // stopTaskAsync triggered, kill current client
                stream.Close();
                client.Close();
                return null;
            }
            else
            {
                int size = (int)BitConverter.ToUInt32(bufPacketSize, 0);
                byte[] bufPacket = new byte[size];
                bufPacketSize.CopyTo(bufPacket, 0);
                if (stream.Read(bufPacket, 4, size - 4) + 4 < size)
                {
                    // TODO FIXME: What should really do at this point?
                    throw new Exception("Receive(): Malformed packet");
                }

                return bufPacket;
            }
        }

        byte[] Expect(Puppet.PACKET_TYPE type)
        {
            byte[] buffer = Receive();
            if (buffer == null)
                return null;

            if (Puppet.Util.Deserialize<Puppet.PACKET>(buffer).type != type)
                throw new Exception("Expect(): Packet type mismatch");
            return buffer;
        }

        void SendAck(UInt32 status)
        {
            byte[] bufAck = Puppet.Util.Serialize(new Puppet.PACKET_ACK(status));
            stream.Write(bufAck, 0, bufAck.Length);
        }

        void OnRecv(byte[] buffer)
        {
            switch(Puppet.Util.Deserialize<Puppet.PACKET>(buffer).type)
            {
                case Puppet.PACKET_TYPE.ANY: // TODO: OnRecv()
                    break;
            }
            foreach(byte b in buffer)
            {
                Console.Write("{0:x2} ", b);
            }
            Console.WriteLine();
        }
    }
}
