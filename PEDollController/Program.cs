using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace PEDollController
{
    static class Program
    {
        static Task taskGUI;

        static void GuiMain()
        {
            Application.Run(new FSplash());
            Application.Run(new FMain());
        }

        [STAThread]
        static void Main()
        {
            Console.WriteLine("PEDollController InDev");
            Console.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            taskGUI = new Task(GuiMain);
            //taskGUI.Start(); // TODO: Commented until start to develop GUI

            //byte[] pkt = Puppet.Util.SerializeString("Hello");
            byte[] pkt = Puppet.Util.SerializeBinary(new byte[] { 0x12, 0x34, 0x56, 0x78 });
            //byte[] pkt = Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(0x1234));
            foreach(byte b in pkt)
            {
                Console.Write("{0:x2} ", b);
            }
            Console.WriteLine();
            

            while (true)
                Console.WriteLine(Console.ReadLine());
        }
    }
}
