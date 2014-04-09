using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM_Lib;
using System.Collections;


namespace Master
{
    

    public class Master
    {
        TcpChannel channel = new TcpChannel(8086);
        TcpChannel channelOut;
        MasterServices ms;
        IDictionary propBag;
        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        Hashtable padIntsSortedLists = new Hashtable(); //tem sorted lists que contem padInts
        SortedList<int, PadInt> padInts = new SortedList<int, PadInt>(); // key Padint ID; Value valor.
        //############# EXISTE EM TODOS OS SERVIDORES ###############################


        SortedList<int, int> slaves = new SortedList<int, int>(); //key port, value to be decided ; o port identifica o slave.

        SortedList<int, int> padIntsLocation = new SortedList<int, int>(); //check this
        SortedList<int, SortedList<int, PadInt>> myResponsability = new SortedList<int, SortedList<int, PadInt>>();
        int port = 8087;
        int roundRobin = 0;
        int numberOfSlaves = 0;

        public Master()
        {
            ms = new MasterServices(this);
            ChannelServices.RegisterChannel(channel, false);
            //channelOut = new TcpChannel();
            //ChannelServices.RegisterChannel(channelToOut, false);
            propBag = new Hashtable();
            propBag["name"] = ""; // "Each channel must have a unique name. Set this property to an empty string ("" or String.Empty) 
            //if you want to ignore names, but avoid naming collisions."  CHECK IF WE NEED TO CARE ABOUT THE NAME

        }
        public int registSlave()
        {
            port++;
            numberOfSlaves++;
            slaves.Add(port, port); //TODO correct this
            return port;
        }
        public int getSlave()
        {
            System.Console.WriteLine(numberOfSlaves + " " + roundRobin + " resultado " + roundRobin % numberOfSlaves);
            return slaves[8088 + (roundRobin++ % numberOfSlaves)];
        }
        public MasterServices getMasterServices()
        {
            return ms;
        }
        public PadInt createPadInt(int uid)
        {
            System.Console.WriteLine("Vamos escrever");
            PadInt aux = null;
            int location = whereIsPadInt(uid);
            System.Console.WriteLine("begin location " + location + "  " + uid + " " +  isMine(uid));
            if (location == -1 && isMine(uid))
            { //Not assigned, get them!!
                System.Console.WriteLine("O Master cria: " + uid + " E fica com a parte " + uid % 101 + " da hash table");
                location = uid % 101;
                padIntsLocation[location] = port;
                myResponsability.Add(location, new SortedList<int, PadInt>());
                System.Console.WriteLine("location " + location + "key in new pie " + myResponsability.ContainsKey(location));
                aux = new PadInt(uid);
                myResponsability[location].Add(uid, aux);
                return aux;
            }
            else
            {
                if (hashPadInts(uid) && !isMine(uid))
                {
                    //create here;
                    System.Console.WriteLine("O Master cria: " + uid + " E fica com a parte " + uid % 101 + " da hash table");
                    aux = new PadInt(uid);
                    myResponsability[location].Add(uid, aux);
                    return aux;
                }
                else
                {
                    System.Console.WriteLine("Cria noutro sitio");
                    createExternalPadInt(uid, getSlave());
                    //create aboard, create TCP connection and stuff!
                }
            }
            return aux;
        }
        private bool isMine(int uid)
        {
            return (uid % 2) == 0;
        }
        public PadInt accessPadInt(int uid)
        {
            return new PadInt(uid);
        }
        public PadInt getExternalPadInt(int uid)
        {
            return new PadInt(uid); //Correct this
        }
        public bool hashPadInts(int uid)
        {
            if (myResponsability.ContainsKey(uid % 101))
                return true;
            return false;
        }
        public int whereIsPadInt(int uid)
        {
            int aux;
            int location = uid % 101;
            System.Console.WriteLine("location in Where is Pad Int " + location);
            System.Console.WriteLine("key " + myResponsability.ContainsKey(location));

            if (myResponsability.ContainsKey(location))
                return location;
            if (padIntsLocation.TryGetValue(location, out aux)) //need to connect to new server!
                return aux;
            return -1; // means that that type of uid%PrimeNumber does not exist at this moment! 
        }
        private PadInt createExternalPadInt(int uid, int location)
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
            PadInt aux = slave.createPadInt(uid);
            return aux;
        }
        private PadInt accessExternalPadInt(int uid, int location)
        {

            return new PadInt(uid);
        }
        public void createChannel(int port)
        {
            propBag["port"] = port;
            channelOut = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channelOut, false);
        }

        public bool setMine(int port,int hash){
            //for a avisar todos os slaves que os numeros com a hash <hash> pertencem ao slave com o port <port>
            //slave.setResponsability(port, hash)
            
            return true;
        }


    }



   
}
