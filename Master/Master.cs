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


        static void Main(string[] args)
        {

            System.Console.WriteLine(args[0]);
            System.Console.ReadLine();
            if (args[0] == "0")
            {
                System.Console.WriteLine("MASTER IN THE HOUSE");

                Master master = new Master();
                MasterServices ms = master.getMasterServices();
                RemotingServices.Marshal(ms, "MyRemoteObjectName", typeof(MasterServices));
                System.Console.WriteLine("Estou vivo");
                System.Console.ReadLine();
                
            }
            if (args[0] == "1")
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

        public PadInt createPadInt(int uid)
        {
            System.Console.WriteLine("estamos a criar isto: "+ uid);
            return true;
        }
        public PadInt accessPadInt(int uid)
        {
            return true;
        }

        public PadInt getPadInt(int uid)
        {
            return true;
        }
        public PadInt getExternalPadInt(int uid){
            return master.getExternalPadInt(uid);
        }
    }
    public class Master
    {
        TcpChannel channel = new TcpChannel(8086);
        MasterServices ms;

        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        Hashtable padIntsSortedLists = new Hashtable(); //tem sorted lists que contem padInts
        SortedList padInts = new SortedList(); // key Padint ID; Value valor.
        //############# EXISTE EM TODOS OS SERVIDORES ###############################


        SortedList slaves = new SortedList(); //key port, value to be decided ; o port identifica o slave.

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
        public PadInt createPadInt(int uid) {
            return new PadInt(uid);
        }
        public PadInt accessPadInt(int uid)
        {
            return new PadInt(uid);
        }
        public PadInt getExternalPadInt(int uid){
            return new PadInt(uid); //Correct this
        }

    }

    class SlaveServices : MarshalByRefObject, ISlaveService
    {
        Slave slave;
        public SlaveServices(Slave aux) {
            slave = aux;
        }
        public string MetodoOlaClient()
        {
            return "ola cliente :D";
        }
        public PadInt createPadInt(int uid)
        {
            return slave.createPadInt(uid);
        }
        public PadInt accessPadInt(int uid)
        {
            
            return slave.getPadInt(uid);
        }
        public PadInt getPadInt(int uid)
        {
            //check this
            if(hasPadInt(uid)){
                return slave.getPadInt(uid);
            }
            return getExternalPadInt(uid);
        }
        private bool hasPadInt(int uid){
            return true; //correct this
        }
        private PadInt getExternalPadInt(int uid){
            return new PadInt(uid);
        }


    }

    class Slave
    {
        public TcpChannel channelToOut; //change to a list or something of tcpChannel
        public TcpChannel channelListening;
        SlaveServices cs;
        IMasterService master;
        IDictionary propBag;
        private int port;
        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        Hashtable padIntsSortedLists = new Hashtable(); //tem sorted lists que contem padInts
        SortedList padInts = new SortedList(); // key Padint ID; Value valor.
        //############# EXISTE EM TODOS OS SERVIDORES ###############################

        public Slave()
        {
            channelToOut = new TcpChannel();
            //ChannelServices.RegisterChannel(channelToOut, false);
            propBag = new Hashtable();
            propBag["name"] = ""; // "Each channel must have a unique name. Set this property to an empty string ("" or String.Empty) 
            //if you want to ignore names, but avoid naming collisions."  CHECK IF WE NEED TO CARE ABOUT THE NAME
            cs = new SlaveServices(this);
        }
        public void registSlave()
        {
            master = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");
            if (master == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                System.Console.WriteLine(master.MetodoOla());
                port = master.register();
                System.Console.WriteLine("porto " + port);
                createChannel(port);

            }
        }
        public void createChannel(int port)
        {
            propBag["port"] = port;
            channelListening = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channelListening, false);
            RemotingServices.Marshal(cs, "MyRemoteObjectName", typeof(SlaveServices));
        }
        public PadInt createPadInt(int uid) {
            System.Console.WriteLine("O slave cria: " + uid);
            PadInt aux = new PadInt(uid)
            padInts.Add(uid, aux);
            return aux;
        }
        //create access padInt
        public PadInt accessPadInt(int uid)
        {
            PadInt aux = padInts[uid];
            return aux;
        }
        public PadInt getExternalPadInt(int uid){
            return master.getExternalPadInt(uid);
        }

    }
}
