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
            return master.createPadInt(uid);
        }
        public PadInt accessPadInt(int uid)
        {
            return new PadInt(uid);
        }

        public PadInt getPadInt(int uid)
        {
            return new PadInt(uid);
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
        SortedList<int,PadInt> padInts = new SortedList<int,PadInt>(); // key Padint ID; Value valor.
        //############# EXISTE EM TODOS OS SERVIDORES ###############################


        SortedList<int, int> slaves = new SortedList<int, int>(); //key port, value to be decided ; o port identifica o slave.

        SortedList<int, int> padIntsLocation = new SortedList<int, int>(); //check this
        int port = 8087;
        int roundRobin = 0;
        int numberOfSlaves = 0;

        public Master() {
            ms = new MasterServices(this);
            ChannelServices.RegisterChannel(channel, false);

        }
        public int registSlave() {
            port++;
            numberOfSlaves++;
            slaves.Add(port, port); //TODO correct this
            return port;
        }
        public int getSlave()
        {
            return slaves[roundRobin++ % numberOfSlaves];
        }
        public MasterServices getMasterServices()
        {
            return ms;
        }
        public PadInt createPadInt(int uid) {
            PadInt aux= null;
            int location = whereIsPadInt(uid);
            if (location != -1)
            { //Not assigned, get them!!
                System.Console.WriteLine("O Master cria: " + uid + "E fica com a parte " + uid%101 + " da hash table");
                padIntsLocation[location] = port;
                aux= new PadInt(uid);
                padInts.Add(uid, aux);
                return aux;
            }
            else
            {
                if (hashPadInts(uid)) { 
                //create here;
                    System.Console.WriteLine("O Master cria: " + uid + "E fica com a parte " + uid % 101 + " da hash table");

                    aux = new PadInt(uid);
                    padInts.Add(uid, aux);
                    return aux;
                }
                //create aboard
            }
            return aux;
        }
        public PadInt accessPadInt(int uid)
        {
            return new PadInt(uid);
        }
        public PadInt getExternalPadInt(int uid){
            return new PadInt(uid); //Correct this
        }
        public bool hashPadInts(int uid) {
            return true;
        }
        public int whereIsPadInt(int uid) {
            int aux;
            if(padIntsLocation.TryGetValue(uid%101, out aux))
                return aux;
            return -1; // means that that type of uid%PrimeNumber does not exist at this moment! 
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

            return slave.accessPadInt(uid);
        }
        public PadInt getPadInt(int uid)
        {
            //check this
            if(hasPadInt(uid)){
                return slave.accessPadInt(uid);
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
            PadInt aux = new PadInt(uid);
            padInts.Add(uid, aux);
            return aux;
        }
        //create access padInt
        public PadInt accessPadInt(int uid)
        {
            PadInt aux = (PadInt) padInts[uid];
            return aux;
        }
        public PadInt getExternalPadInt(int uid){
            return master.getExternalPadInt(uid);
        }

    }
}
