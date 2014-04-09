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
        private const int MINE = 0;
        private const int MYPRIME = 101;
        private const int NONE = -1;
        private const int LIVE = 10;
        private const int FROZEN = 0;
        private const int DETH = -1;
        static int status;

        public TcpChannel channelToOut; //change to a list or something of tcpChannel
        public TcpChannel channelListening;
        SlaveServices cs;
        IMasterService master;
        IDictionary propBag;
        private int port;
        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        SortedList<int, int> padIntsLocation = new SortedList<int, int>(); //key is the hash, value is the port of the slave that is responsable for that hash
        SortedList<int, SortedList<int, PadIntStored>> myResponsability = new SortedList<int, SortedList<int, PadIntStored>>(); //key is the hash, value is a list of PadiInt's stored in this master
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
            status = LIVE;
        }

        public bool recover()
        {
            status = LIVE;
            return true;
        }

        public bool freeze()
        {
            status = FROZEN;
            return true;
        }
        public void frozenHandler()
        {
            while (status == FROZEN)
            {
                System.Threading.Thread.Sleep(500);
                System.Console.WriteLine(status);
            }
        }

        public int getSlave()
        {
            return master.getSlave();//FIXME
        }
        public void createChannel(int port)
        {
            propBag["port"] = port;
            channelListening = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channelListening, false);
            RemotingServices.Marshal(cs, "MyRemoteObjectName", typeof(SlaveServices));
        }
        public PadIntStored createPadInt(int uid)
        {
            frozenHandler();
            System.Console.WriteLine("Vamos escrever");
            PadIntStored aux = null;
            int location = whereIsPadInt(uid);
            System.Console.WriteLine("begin location " + location + " uid  " + uid + " port: " + port);

            if (location == MINE)
            {
                if (myResponsability[hashUid(uid)].ContainsKey(uid))
                    return null;
                aux = new PadIntStored(uid);
                myResponsability[hashUid(uid)].Add(uid, aux);
                return aux;
            }
            else
            {
                if (location == NONE)
                {
                    if (!master.setMine(port, hashUid(uid)))
                    {
                        return null; //pedir ao master onde está
                    }
                    location = hashUid(uid);
                    padIntsLocation[location] = port;
                    myResponsability.Add(location, new SortedList<int, PadIntStored>());
                    aux = new PadIntStored(uid);
                    myResponsability[location].Add(uid, aux);
                    return aux;
                }
                else
                {
                    createExternalPadInt(uid, location);
                }
            }
            return aux;
        }
        //create access padInt
        public PadIntStored accessPadInt(int uid)
        {
            frozenHandler();
            PadIntStored aux = null;
            int location = whereIsPadInt(uid);
            if (location == NONE)
            {
                return null;
            }
            if (location == MINE)
            {
                aux = myResponsability[hashUid(uid)][uid];
            }
            else
            {
                accessExternalPadInt(uid, location);
            }
            return aux;
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
                return MINE;
            if (padIntsLocation.TryGetValue(location, out aux)) //need to connect to new server!
                return aux;
            return NONE; // means that that type of uid%PrimeNumber does not exist at this moment! 
        }
        private PadIntStored createExternalPadInt(int uid, int location)
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
            PadIntStored aux = slave.createPadInt(uid);
            return aux;
        }
        private PadIntStored accessExternalPadInt(int uid, int location)
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
            PadIntStored aux = slave.accessPadInt(uid);
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
