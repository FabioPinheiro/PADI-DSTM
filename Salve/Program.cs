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

namespace Salve
{
    class Program
    {

        public static TcpChannel channel;
        public static TcpChannel channel1;
        static void Main(string[] args)
        {
            System.Console.WriteLine("#BEGIN SLAVE# YELLOW");
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");

            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                obj.MetodoOla();
                obj.register("aovelhanegra", "tcp://localhost:8087/MyRemoteObjectName");

                //CREATE THE NEW CHANNEL
                channel1 = new TcpChannel(8087);
                ChannelServices.RegisterChannel(channel1, false);
                SlaveServices cs = new SlaveServices();
                RemotingServices.Marshal(cs, "MyRemoteObjectName", typeof(SlaveServices));
                //END CHANNEL CREATION
               System.Console.WriteLine(obj.getRegisted());
                
            }
            //Console.WriteLine(obj.MetodoOla());
            System.Console.WriteLine("#END SLAVE# PEACE OUT");
            System.Console.Read();
        }
    }
    class SlaveServices : MarshalByRefObject, ISlaveService
    {
        public string MetodoOlaClient()
        {
            return "ola cliente :D";
        }

    }
   


}
