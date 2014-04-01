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

        static void Main(string[] args)
        {
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");

            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                obj.MetodoOla();
                obj.register("nick", "location");
            }
            //Console.WriteLine(obj.MetodoOla());
            System.Console.WriteLine("#END SLAVE# PEACE OUT");
        }
    }
   


}
