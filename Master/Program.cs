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


namespace Master
{
    static class Program
    {

     /*   String nick;
        String location;

        public void register(String username, String url)
        {
            nick = username;
            location = url; //Check this :D
        }*/
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            System.Console.WriteLine("MASTER IN THE HOUSE");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MasterForm());


        
        }

     
    }
    public class MasterServices : MarshalByRefObject, IMasterService
    {
        String name;
        String url;


        public void register(String nick, String location)
        {
            name = nick;
            url = location;
            System.Console.WriteLine(nick + " " + location);
        }
        public string MetodoOla()
        {
            return "ola!";
        }
        public string getRegisted() {
            return "nome: " + name + " localização " + url; 
        }
    }
}
