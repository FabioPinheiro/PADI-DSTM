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
        String nick;
        String location;

        public void printToScreen(URL teste)
        {
            System.Console.WriteLine(teste.getPort());
        }
        public void register(String username, String url)
        {
            nick = username;
            location = url;
        }
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086); 
            ChannelServices.RegisterChannel(channel, false);
            MasterServices ms = new MasterServices();
            RemotingServices.Marshal(ms, "MyRemoteObjectName", typeof(MasterServices));
            System.Console.WriteLine("<enter> para sair..."); 
            System.Console.ReadLine(); 
        }
    }
    public class MasterServices : MarshalByRefObject,IMasterService 
    {
        public void register(String nick, String location)
        {
            System.Console.WriteLine(nick + " " + location);
        }
        public string MetodoOla()
        {
            return "ola!";
        }
    }


}
