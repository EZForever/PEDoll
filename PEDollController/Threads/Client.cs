using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PEDollController.Threads
{
    struct HookEntry
    {
        public string name; // Removed hook if name == null
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

        public readonly Delegate[] Exports;

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
                this.Send(Puppet.Util.Serialize(new Puppet.PACKET_ACK(0)));
            }
            catch (IOException e)
            {
                Logger.W(Program.GetResourceString("Threads.Client.Disconnected", clientName, e.Message));
                stream.Close();
                client.Close();
                // TODO: Refresh targets' list
            }

            // Initialize packet receive queue
            rxQueue = new BlockingQueue<byte[]>();

            // Initialize EvalEngine exports
            Exports = new Delegate[]
            {
                new Func<uint, UInt64>(eval_getContext),
                new Func<UInt64, string>(eval_str),
                new Func<UInt64, string>(eval_wstr),
                new Func<string, string>(eval_ctx),
                new Func<UInt64, uint, byte[]>(eval_mem),
                new Func<UInt64, UInt64>(eval_poi),
                new Func<uint, UInt64>(eval_arg),
            };

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

            Logger.N(Program.GetResourceString("Threads.Client.Connected",
                client.Client.RemoteEndPoint.ToString(),
                idx,
                clientName,
                this.GetTypeString()
            ));

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
                Logger.W(Program.GetResourceString("Threads.Client.Disconnected", clientName, e.Message));
            }
            finally
            {
                stream.Close();
                client.Close();
                // TODO: Refresh targets' list, refresh target state to DEAD
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
                throw (taskRecv.Exception == null) ? 
                    new IOException(Program.GetResourceString("Threads.Client.MalformedPacket"))
                    : taskRecv.Exception.InnerException;
            }
            else
            {
                int size = (int)BitConverter.ToUInt32(bufPacketSize, 0);
                if(size < bufPacketSize.Length)
                    throw new IOException(Program.GetResourceString("Threads.Client.MalformedPacket"));

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
                    this.Send(Puppet.Util.Serialize(new Puppet.PACKET_ACK(0)));

                    // NOTE: OnHook() Must run in another thread, since it uses EvalEngine, which indirectly wait for packets.
                    // Make EvalEngine directly wait for packets will cause trouble in command `eval`.
                    new Task(OnHook).Start();
                    break;
                default:
                    // Save packet for command usage
                    rxQueue.BlockingEnqueue(buffer);
                    break;
            }
        }

        void OnHook()
        {
            HookEntry entry = hooks.Where(x => x.oep == hookOep).FirstOrDefault();
            if (entry.name == null) // Uninitialized
                throw new IOException(Program.GetResourceString("Threads.Client.UnknownHook"));

            Logger.N(Program.GetResourceString("Threads.Client.Hooked",
                this.clientName,
                hooks.IndexOf(entry),
                entry.name,
                (hookPhase == 0) ? "before" : "after"
            ));

            List<Dictionary<string, object>> actions = (hookPhase == 0) ? entry.beforeActions : entry.afterActions;
            string verdict = (hookPhase == 0) ? entry.beforeVerdict : entry.afterVerdict;

            foreach(Dictionary<string, object> action in actions)
            {
                try
                {
                    switch((string)action["verb"])
                    {
                        case "echo":
                        {
                            string evalEcho = EvalEngine.EvalString(this, (string)action["echo"]);
                            Logger.N(Program.GetResourceString("Threads.Client.Echo", evalEcho));
                            break;
                        }
                        case "dump":
                        {

                            object[] evalResults = EvalEngine.Eval(this, new string[] { (string)action["addr"], (string)action["size"] });

                            UInt64 ptr;
                            if(bits == 64 ? evalResults[0] is UInt64 : evalResults[0] is UInt32)
                            {
                                ptr = Convert.ToUInt64(evalResults[0]);
                            }
                            else
                            {
                                throw new ArgumentException(Program.GetResourceString("Threads.Client.TypeMismatch"));
                            }

                            uint len;
                            try
                            {
                                len = Convert.ToUInt32(evalResults[1]);
                            }
                            catch(InvalidCastException)
                            {
                                throw new ArgumentException(Program.GetResourceString("Threads.Client.TypeMismatch"));
                            }

                            int idx = CmdEngine.theInstance.dumps.Count;

                            DumpEntry dumpEntry = new DumpEntry();
                            dumpEntry.Source = this.clientName;
                            dumpEntry.Data = eval_mem(ptr, len);
                            CmdEngine.theInstance.dumps.Add(dumpEntry);

                            Logger.N(Program.GetResourceString("Threads.Client.Dump", idx, dumpEntry.Data.Length));
                            // TODO: Update dump list
                            break;
                        }
                        case "ctx":
                        {
                            string evalKey = EvalEngine.EvalString(this, (string)action["key"]);
                            string evalValue = EvalEngine.EvalString(this, (string)action["value"]);
                            this.context.Add(evalKey, evalValue);
                            Logger.N(Program.GetResourceString("Threads.Client.Ctx", evalKey, evalValue));
                            break;
                        }
                    }
                }
                catch(ArgumentException e)
                {
                    Logger.E(Program.GetResourceString("Threads.Client.ActionError", (string)action["verb"], e.Message));
                }
            }

            if(verdict == null)
            {
                Logger.N(Program.GetResourceString("Threads.Client.VerdictWait"));
                // TODO: Update target list
            }
            else
            {
                SendVerdict(verdict);
            }
        }

        byte[] eval_readString(UInt64 ptr, int charSize, int maxSize = 256)
        {
            List<byte> strBuffer = new List<byte>();

            // Prepare CMD_MEMORY
            Puppet.PACKET_CMD_MEMORY pktMem = new Puppet.PACKET_CMD_MEMORY(0);
            pktMem.len = (UInt32)charSize;

            UInt64 ptrCurrent = ptr;
            while (true)
            {
                // Send packets
                this.Send(Puppet.Util.Serialize(pktMem));
                this.Send(Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(ptrCurrent)));

                // Expent ACK
                Puppet.PACKET_ACK pktAck;
                pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(this.Expect(Puppet.PACKET_TYPE.ACK));

                if (pktAck.status == 0 || pktAck.status < (UInt32)charSize)
                {
                    // Do not allow MemoryReadWarning
                    if (pktAck.status != 0)
                        this.Expect(Puppet.PACKET_TYPE.BINARY); // Dispose BINARY packet

                    Logger.W(Program.GetResourceString("Threads.Client.StringIncompleteWarning"));
                    break;
                }

                // Expect blob
                byte[] blob = Puppet.Util.DeserializeBinary(this.Expect(Puppet.PACKET_TYPE.BINARY));

                if (Array.TrueForAll(blob, x => x == 0))
                {
                    // Discard & end reading if got zero terminator (C-style string)
                    break;
                }
                else if(strBuffer.Count / charSize >= maxSize)
                {
                    Logger.W(Program.GetResourceString("Threads.Client.StringTooLongWarning"));
                    break;
                }

                strBuffer.AddRange(blob);
                ptrCurrent += (UInt64)charSize;
            }

            return strBuffer.ToArray();
        }

        UInt64 eval_getContext(uint index)
        {
            // XXX: Caching?

            // Prepare CMD_CONTEXT
            Puppet.PACKET_CMD_CONTEXT pktCtx = new Puppet.PACKET_CMD_CONTEXT(0);
            pktCtx.idx = index; // libDoll will check this

            // Send CMD_CONTEXT
            this.Send(Puppet.Util.Serialize(pktCtx));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(this.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Program.GetResourceString("Threads.Client.ContextReadError", index));

            // Expect register value
            Puppet.PACKET_INTEGER pktVal;
            pktVal = Puppet.Util.Deserialize<Puppet.PACKET_INTEGER>(this.Expect(Puppet.PACKET_TYPE.INTEGER));
            return pktVal.data;
        }

        string eval_str(UInt64 ptr)
        {
            byte[] strBuffer = eval_readString(ptr, sizeof(byte));
            return Encoding.Default.GetString(strBuffer);
        }

        string eval_wstr(UInt64 ptr)
        {
            byte[] strBuffer = eval_readString(ptr, sizeof(byte));
            return Encoding.Unicode.GetString(strBuffer);
        }

        string eval_ctx(string key)
        {
            if(!this.context.ContainsKey(key))
                throw new ArgumentException(Program.GetResourceString("Threads.Client.DictionaryReadError", key));
            return this.context[key];
        }

        byte[] eval_mem(UInt64 ptr, uint len)
        {
            // Prepare CMD_MEMORY
            Puppet.PACKET_CMD_MEMORY pktMem = new Puppet.PACKET_CMD_MEMORY(0);
            pktMem.len = len;

            // Send packets
            this.Send(Puppet.Util.Serialize(pktMem));
            this.Send(Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(ptr)));

            // Expent ACK
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(this.Expect(Puppet.PACKET_TYPE.ACK));

            if (pktAck.status == 0)
            {
                throw new ArgumentException(Program.GetResourceString("Threads.Client.MemoryReadError"));
            }

            if (pktAck.status < len)
                Logger.W(Program.GetResourceString("Threads.Client.MemoryReadWarning", len, pktAck.status));

            // Expect blob
            return Puppet.Util.DeserializeBinary(this.Expect(Puppet.PACKET_TYPE.BINARY));
        }

        UInt64 eval_poi(UInt64 ptr)
        {
            HookEntry entry = hooks.Where(x => x.oep == hookOep).First();
            int wordsize = this.bits / 8;

            // Prepare CMD_MEMORY
            Puppet.PACKET_CMD_MEMORY pktMem = new Puppet.PACKET_CMD_MEMORY(0);
            pktMem.len = (UInt32)wordsize;

            // Send packets
            this.Send(Puppet.Util.Serialize(pktMem));
            this.Send(Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(ptr)));

            // Expent ACK
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(this.Expect(Puppet.PACKET_TYPE.ACK));

            if (pktAck.status == 0 || pktAck.status < (UInt32)wordsize)
            {
                // Do not allow MemoryReadWarning
                if (pktAck.status != 0)
                    this.Expect(Puppet.PACKET_TYPE.BINARY); // Dispose BINARY packet

                throw new ArgumentException(Program.GetResourceString("Threads.Client.MemoryReadError"));
            }

            // Expect blob
            byte[] blob = Puppet.Util.DeserializeBinary(this.Expect(Puppet.PACKET_TYPE.BINARY));

            if (this.bits == 64)
                return BitConverter.ToUInt64(blob, 0);
            else
                return BitConverter.ToUInt32(blob, 0);
        }

        UInt64 eval_arg(uint index)
        {
            HookEntry entry = hooks.Where(x => x.oep == hookOep).First();
            int wordsize = this.bits / 8;

            UInt64 val = 0;
            long ptrOffset = -1; // int * int == long
            switch(entry.convention)
            {
                case "stdcall":
                case "cdecl":
                {
                    // stack
                    ptrOffset = (wordsize * index);
                    break;
                }
                case "fastcall":
                {
                    // cx, dx, stack
                    switch(index)
                    {
                        case 0: val = eval_getContext(1); break;
                        case 1: val = eval_getContext(2); break;
                        default: ptrOffset = (wordsize * (index - 2)); break;
                    }
                    break;
                }
                case "msvc":
                {
                    // cx, dx, r8, r9, stack (4 * wordsize offset)
                    switch (index)
                    {
                        case 0: val = eval_getContext(1); break;
                        case 1: val = eval_getContext(2); break;
                        case 2: val = eval_getContext(8); break;
                        case 3: val = eval_getContext(9); break;
                        default: ptrOffset = (wordsize * (index - 4 + 4)); break;
                    }
                    break;
                }
                case "gcc":
                {
                    // di, si, dx, cx, r8, r9, stack
                    switch (index)
                    {
                        case 0: val = eval_getContext(7); break;
                        case 1: val = eval_getContext(6); break;
                        case 2: val = eval_getContext(2); break;
                        case 3: val = eval_getContext(1); break;
                        case 4: val = eval_getContext(8); break;
                        case 5: val = eval_getContext(9); break;
                        default: ptrOffset = (wordsize * (index - 6)); break;
                    }
                    break;
                }
            }

            if(ptrOffset >= 0)
            {
                // Value is from stack

                // Calculate stack ptr
                UInt64 ptr = eval_getContext(4) + (UInt64)wordsize; // Argument stack base
                ptr += (UInt64)ptrOffset;

                val = eval_poi(ptr);
            }

            return val;
        }

        public string OepToString(UInt64 oep)
        {
            return "0x" + oep.ToString(this.bits == 64 ? "x16" : "x8");
        }

        public string GetTypeString()
        {
            return Program.GetResourceString(this.isMonitor ? "Threads.Client.Type.Monitor" : "Threads.Client.Type.Doll");
        }

        public string GetStatusString()
        {
            string resId;

            if (this.isDead)
                resId = "Threads.Client.Status.Dead";
            else if(this.hookOep != 0)
                resId = "Threads.Client.Status.Hooked";
            else
                resId = "Threads.Client.Status.Alive";

            return Program.GetResourceString(resId);
        }

        public void Send(byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        public void SendVerdict(string verdict)
        {
            HookEntry entry = hooks.Where(x => x.oep == hookOep).First();

            Logger.N(Program.GetResourceString("Threads.Client.Verdict", verdict));

            // Verdict string to ID
            UInt32 verdictId = 0;
            switch(verdict)
            {
                case "approve":
                    verdictId = 0; break;
                case "reject":
                    verdictId = 1; break;
                case "terminate":
                    verdictId = 2; break;
            }

            // Prepare & send CMD_VERDICT
            Puppet.PACKET_CMD_VERDICT pktVerdict = new Puppet.PACKET_CMD_VERDICT(0);
            pktVerdict.verdict = verdictId;
            this.Send(Puppet.Util.Serialize(pktVerdict));

            // If verdict == "reject", send SP offset and AX
            if(verdictId == 1)
            {
                this.Send(Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(entry.stack)));
                this.Send(Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(entry.ret)));
            }

            // Expect ACK
            this.Expect(Puppet.PACKET_TYPE.ACK);

            // Clear hook status
            hookOep = 0;
            hookPhase = 0;
            // TODO: Refresh targets' list
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
