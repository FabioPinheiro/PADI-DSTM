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
        static void Main(string[] args)
        {
            if (args[0] == "0")
            {
                System.Console.WriteLine("MASTER IN THE HOUSE");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MasterForm());
            }
            if (/*args[0] == "1" ||*/ true)
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

        public bool createPadInt(int uid)
        {
            return true;
        }
        public bool accessPadInt(int uid)
        {
            return true;
        }

        public bool getPadInt(int uid)
        {
            return true;
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
        public bool createPadInt() {
            return true;
        }
        public bool accessPadInt()
        {
            return true;
        }

    }

    class SlaveServices : MarshalByRefObject, ISlaveService
    {
        public string MetodoOlaClient()
        {
            return "ola cliente :D";
        }
        public void createPadInt()
        {
        }
        public bool accessPadInt(int uid)
        {
            return true;
        }
        public bool getPadInt(int uid)
        {
            return true;
        }


    }

    class Slave
    {
        public TcpChannel channelToOut; //change to a list or something of tcpChannel
        public TcpChannel channelListening;
        //SlaveServices cs;
        IDictionary propBag;
        private int port;

        public Slave()
        {
            channelToOut = new TcpChannel();
            //ChannelServices.RegisterChannel(channelToOut, false);
            propBag = new Hashtable();
            propBag["name"] = ""; // "Each channel must have a unique name. Set this property to an empty string ("" or String.Empty) 
            //if you want to ignore names, but avoid naming collisions."  CHECK IF WE NEED TO CARE ABOUT THE NAME

        }
        public void registSlave()
        {
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                System.Console.WriteLine(obj.MetodoOla());
                port = obj.register();
                System.Console.WriteLine("porto " + port);
                createChannel(port);

            }
        }
        public void createChannel(int port)
        {
            propBag["port"] = port;
            channelListening = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channelListening, false);
            SlaveServices cs = new SlaveServices();
            RemotingServices.Marshal(cs, "MyRemoteObjectName", typeof(SlaveServices));
        }

    }
}
