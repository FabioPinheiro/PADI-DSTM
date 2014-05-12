using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM;
using System.Collections;
using System.Threading;


namespace Master
{
    static class Server
    {

        
        static void Main(string[] args)
        {

            System.Console.WriteLine("Insert 0 for Master or 1 for Slave:");
            String args1 = System.Console.ReadLine();
            if (args1 == "0")
            {
                System.Console.WriteLine("MASTER IN THE HOUSE");

                Master master = new Master();
                MasterServices ms = master.getMasterServices();
                RemotingServices.Marshal(ms, "MyRemoteObjectName", typeof(MasterServices));

                Thread checkMonitor = new Thread(new ThreadStart(master.checkMonitor));
                checkMonitor.Start();
                System.Console.ReadLine();
                checkMonitor.Abort();
            }
            if (args1 == "1")
            {
                System.Console.WriteLine("#BEGIN SLAVE");
                Slave slave = new Slave();
                slave.registSlave();

                Console.WriteLine("Vai chamar");
                Thread monitor = new Thread(new ThreadStart(slave.monitor));
                monitor.Start();
                Console.WriteLine("já chamou?");
                Console.Read();
                Console.WriteLine("e agora?");

                Console.WriteLine("Foram mortos: " + slave.matei);
                Console.WriteLine("foram abortados: " + slave.abortou);
                monitor.Abort();
                slave.printSlave();
            }



            System.Console.WriteLine("THE END");


        }


    }
}
