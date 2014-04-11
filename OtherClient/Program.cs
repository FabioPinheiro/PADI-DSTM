using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM_Lib;

namespace OtherClient
{
    class Program
    {
        static void Main()
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());*/
            Console.WriteLine("PRESS ENTER TO START THE CLIENT");
            Console.ReadLine();

            bool res;

            PadiDstm.Init();
            Console.WriteLine("Before recover");
            //res = PadiDstm.Recover("tcp://localhost:8087/MyRemoteObjectName");
            Console.WriteLine("After recover");
            res = PadiDstm.TxBegin();
            PadInt pi_a = PadiDstm.CreatePadInt(3);
            PadInt pi_b = PadiDstm.CreatePadInt(4);
            res = PadiDstm.TxCommit();

            PadiDstm.Status();

            res = PadiDstm.TxBegin();
            pi_a = PadiDstm.AccessPadInt(0);
            pi_b = PadiDstm.AccessPadInt(1);
            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            /*PadiDstm.Status();
            // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
            res = PadiDstm.Freeze("tcp://localhost:2001/Server");
            res = PadiDstm.Recover("tcp://localhost:2001/Server");
            res = PadiDstm.Fail("tcp://localhost:2002/Server");
            res = PadiDstm.TxCommit();*/
            Console.ReadLine();
        }
    }
}
