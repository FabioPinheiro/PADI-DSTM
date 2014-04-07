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
using System.Collections;


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
        Master master;

        public MasterServices(Master aux){
            master = aux;
        }
        public MasterServices() { 
        }

        public int register(String nick, String location)
        {
            name = nick;
            url = location;
            System.Console.WriteLine(nick + " " + location);
            return master.registSlave();
        }
        public int register() {
            return master.registSlave();
        }
        public string MetodoOla()
        {
            return "ola! sou o master";
        }

        public int getSlave()
        {
            return master.getSlave(); //TODO
        }
    }
    public class Master
    {
        TcpChannel channel = new TcpChannel(8086);
        MasterServices ms;

        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        Hashtable padIntsSortedLists = new Hashtable(); //tem sorted lists que contem padInts
        SortedList slaves = new SortedList(); //key port, value to be decided ; o port identifica o slave.
        //############# EXISTE EM TODOS OS SERVIDORES ###############################


        //SortedList padInts = new SortedList(); // key Padint ID; Value valor.
        Hashtable padIntsLocation = new Hashtable(); //check this
        int port = 8087;

        public Master() {
            ms = new MasterServices(this);
            ChannelServices.RegisterChannel(channel, false);

        }
        public int registSlave() {
            port++;
            slaves.Add(port, port); //TODO correct this
            return port;
        }
        public int getSlave()
        {
            return (int) slaves.GetByIndex(0); //Correct this.
        }
        public MasterServices getMasterServices()
        {
            return ms;
        }

    }
}
