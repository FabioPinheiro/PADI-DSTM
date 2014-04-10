using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM_Lib;
using System.Collections;


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
                System.Console.WriteLine("Estou vivo");
                System.Console.ReadLine();

            }
            if (args1 == "1")
            {
                System.Console.WriteLine("#BEGIN SLAVE# YELLOW");
                Slave slave = new Slave();
                slave.registSlave();

                System.Console.WriteLine("#SLAVE# Wainting press enter to leave");

                System.Console.Read();
                System.Console.WriteLine("#END SLAVE# PEACE OUT");
            }



            System.Console.WriteLine("EEEEEEEEEEEEEEEEEEEEEEEEENNNNNNNNNNNNNNNNNNNNNNDDDDDDDDDDDDDDDDDDDDDDddd");


        }


    }
}
