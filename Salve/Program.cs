using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

namespace Salve
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            MyRemoteObject obj = (MyRemoteObject)Activator.GetObject(
                typeof(MyRemoteObject),
                "tcp://localhost:8086/MyRemoteObjectName");

            try
            {
                Console.WriteLine(obj.MetodoOla());
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate server");
            }

            Console.ReadLine();
        }
    }
}
