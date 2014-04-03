using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM_Lib;
using System.Collections;

namespace Salve
{
    class Program
    {


        static void Main(string[] args)
        {
            System.Console.WriteLine("#BEGIN SLAVE# YELLOW");
            Slave slave = new Slave();
            slave.registSlave();

            System.Console.WriteLine("#SLAVE# Wainting press enter to leave");

            System.Console.Read();
            System.Console.WriteLine("#END SLAVE# PEACE OUT");

        }
    }
    class SlaveServices : MarshalByRefObject, ISlaveService
    {
        public string MetodoOlaClient()
        {
            return "ola cliente :D";
        }

    }

    class Slave
    {
        public TcpChannel channelToOut; //change to a list or something of tcpChannel
        public TcpChannel channelListening;
        SlaveServices cs;
        IDictionary propBag;

        public Slave()
        {
            channelToOut = new TcpChannel();
            ChannelServices.RegisterChannel(channelToOut, false);
            propBag = new Hashtable();
            propBag["name"] = ""; // "Each channel must have a unique name. Set this property to an empty string ("" or String.Empty) 
            //if you want to ignore names, but avoid naming collisions."  CHECK IF WE NEED TO CARE ABOUT THE NAME

        }
        public void registSlave(){
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                System.Console.WriteLine(obj.MetodoOla());
                obj.register("aovelhanegra", "tcp://localhost:8087/MyRemoteObjectName");
                System.Console.WriteLine(obj.getRegisted());
                createChannel(8087);

            }
        }
        public void createChannel(int port)
        {
            propBag["port"] = port;
            TcpChannel channelListening = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channelListening, false);
            SlaveServices cs = new SlaveServices();
            RemotingServices.Marshal(cs, "MyRemoteObjectName", typeof(SlaveServices));
        }

    }
   


}
