using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM_Lib;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
 
        //[STAThread]
        static void Main()
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());*/
            Console.WriteLine("PRESS ENTER TO START THE CLIENT");
            Console.ReadLine();
            bool res;

            PadiDstm.Init();
            Console.WriteLine("PadiDstm.Init();");
            res = PadiDstm.TxBegin();
            Console.WriteLine("res = PadiDstm.TxBegin();");
            PadInt pi_a = PadiDstm.CreatePadInt(0);
            Console.WriteLine("PadInt pi_a = PadiDstm.CreatePadInt(0);");
            PadInt pi_b = PadiDstm.CreatePadInt(1);
            Console.WriteLine("PadInt pi_b = PadiDstm.CreatePadInt(1);");
            pi_a.Write(36);
            Console.WriteLine("pi_a.Write(36)");
            pi_b.Write(37);
            Console.WriteLine("pi_b.Write(37);");
            res = PadiDstm.TxCommit();
            Console.WriteLine("res = PadiDstm.TxCommit();");
            //res = PadiDstm.Freeze("tcp://localhost:8087/MyRemoteObjectName");
            Console.WriteLine("res = PadiDstm.Freeze(...)");

            PadiDstm.Status();
            Console.WriteLine("after status call");
            res = PadiDstm.TxBegin();
            Console.WriteLine("PadiDstm.TxBegin()");
            pi_a = PadiDstm.AccessPadInt(0);
            Console.WriteLine("PadiDstm.AccessPadInt(0)");
            pi_b = PadiDstm.AccessPadInt(1);
            Console.WriteLine("PadiDstm.AccessPadInt(1)");

            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            /*
            // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
            res = PadiDstm.Freeze("tcp://localhost:2001/Server");
            res = PadiDstm.Recover("tcp://localhost:2001/Server");
            res = PadiDstm.Fail("tcp://localhost:2002/Server");
            res = PadiDstm.TxCommit();*/
            Console.ReadLine();
        }
    }
}
