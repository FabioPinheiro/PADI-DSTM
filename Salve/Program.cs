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
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086); 
            ChannelServices.RegisterChannel(channel, false); 
            URL mo = new URL(); 
            RemotingServices.Marshal(mo,"MyRemoteObjectName",typeof(URL)); 
            System.Console.WriteLine("<enter> para sair..."); 
            System.Console.ReadLine(); 
        }
    }
}
