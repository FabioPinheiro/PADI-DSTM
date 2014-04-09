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
    class Slave
    {
        //CONSTANTES
        private static const int MINE = 0;
        private static const int MYPRIME = 101;
        private static const int NOONE = -1;

        public TcpChannel channelToOut; //change to a list or something of tcpChannel
        public TcpChannel channelListening;
        SlaveServices cs;
        IMasterService master;
        IDictionary propBag;
        private int port;
        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        // Hashtable padIntsSortedLists = new Hashtable(); //tem sorted lists que contem padInts
        //SortedList padInts = new SortedList(); // key Padint ID; Value valor.
        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        SortedList<int, List<int>> slaves = new SortedList<int, List<int>>(); //key port, value list of the hashes that the slave is responsible ; o port identifica o slave.

        SortedList<int, int> padIntsLocation = new SortedList<int, int>(); //check this
        SortedList<int, SortedList<int, PadInt>> myResponsability = new SortedList<int, SortedList<int, PadInt>>();


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
        public int getSlave()
        {
            //            System.Console.WriteLine(numberOfSlaves + " " + roundRobin + " resultado " + roundRobin % numberOfSlaves);
            return master.getSlave();//slaves[8088 + (roundRobin++ % numberOfSlaves)]; //FIXME
        }
        public void createChannel(int port)
        {
            propBag["port"] = port;
            channelListening = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channelListening, false);
            RemotingServices.Marshal(cs, "MyRemoteObjectName", typeof(SlaveServices));
        }
        public PadInt createPadInt(int uid)
        {
            System.Console.WriteLine("Vamos escrever");
            PadInt aux = null;
            int location = whereIsPadInt(uid);
            System.Console.WriteLine("begin location " + location + " uid  " + uid + " isMine: " + isMine(uid) + " port: " + port);

            if (location == MINE)
            {
                aux = new PadInt(uid);
                myResponsability[hashUid(uid)].Add(uid, aux);
                return aux;
            }
            else
            {
                if (location == NOONE)
                {
                    master.setMine(port, hashUid(uid));
                    location = hashUid(uid);
                    padIntsLocation[location] = port;
                    myResponsability.Add(location, new SortedList<int, PadInt>());
                    aux = new PadInt(uid);
                    myResponsability[location].Add(uid, aux);
                    return aux;
                }
                else
                {
                    createExternalPadInt(uid, getSlave());
                }
            }
            return aux;
        }
        //create access padInt
        public PadInt accessPadInt(int uid)
        {
            PadInt aux = myResponsability[hashUid(uid)][uid];
            return aux;
        }
        public PadInt getExternalPadInt(int uid)
        {
            return master.getExternalPadInt(uid);//REVER!! NAO FUNCIONA!!
        }
        private bool isMine(int uid)
        {
            return (port % 2) == ((hashUid(uid)) % 2);
        }


        public bool hashPadInts(int uid)
        {
            if (myResponsability.ContainsKey(hashUid(uid)))
                return true;
            return false;
        }
        public int whereIsPadInt(int uid)
        {
            int aux;
            int location = hashUid(uid);
            System.Console.WriteLine("location in Where is Pad Int " + location);
            System.Console.WriteLine("key " + myResponsability.ContainsKey(location));

            if (myResponsability.ContainsKey(location))
                return 0;
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
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
            PadInt aux = slave.accessPadInt(uid);
            return aux;
        }

        public bool setResponsability(int port, int hash)
        {
            if (padIntsLocation.ContainsKey(hash))
            {
                return false;
            }
            else
            {
                padIntsLocation[hash] = port;
                return true;
            }
        }

        private int hashUid(int uid)
        {
            return uid % MYPRIME;
        }
    }
}
