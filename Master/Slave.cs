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
        static int currentStatus;

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
        List<TransactionWrapper> transacções_state = new List<TransactionWrapper>(); //key: The transaction, value: state (true if live, false is deth ou diyng)
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
            currentStatus = LIVE;
        }
        private String getStatus()
        {
            switch (currentStatus)
            {
                case LIVE:
                    return "LIVE";
                case FROZEN:
                    return "FROZEN";
                case DETH:
                    return "DETH";
                default:
                    return "DETH";
            }
        }
        public bool recover()
        {
            currentStatus = LIVE;
            return true;
        }
        public bool status()
        {
            //

            printStatus();
            return true;
        }
        private void printStatus()
        {
            System.Console.WriteLine("Slave" + port + "current status: " + getStatus());
            System.Console.WriteLine(myResponsability.Count() + " Registed slaves: ");
            foreach (KeyValuePair<int, SortedList<int, PadIntStored>> kvp in myResponsability)
            {
                Console.WriteLine(" - hash " + kvp.Key + " - Includes:");
                foreach (KeyValuePair<int, PadIntStored> locationPair in kvp.Value)
                {
                    Console.WriteLine("     uid " + locationPair.Key + " - value " + locationPair.Value.getValue() + " version: " + locationPair.Value.getVersion());
                }
                Console.WriteLine();
            }

        }
        public bool freeze()
        {
            currentStatus = FROZEN;
            return true;
        }
        public void frozenHandler()
        {
            while (currentStatus == FROZEN)
            {
                System.Threading.Thread.Sleep(3000);
            }
        }
        public bool fail()
        {
            currentStatus = DETH;
            return true;
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
            if (currentStatus == DETH)
                return null;
            frozenHandler();
            // System.Console.WriteLine("Vamos escrever");
            PadIntStored aux = null;
            int location = whereIsPadInt(uid);
            //System.Console.WriteLine("begin location " + location + " uid  " + uid + " port: " + port);

            if (location == MINE)
            {
                if (myResponsability[hashUid(uid)].ContainsKey(uid))
                {
                    return myResponsability[hashUid(uid)][uid].getVersion() == "none:0" ? myResponsability[hashUid(uid)][uid] : null;
                }
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
                        return null;
                    }
                    location = hashUid(uid);
                    padIntsLocation[location] = port;
                    myResponsability.Add(location, new SortedList<int, PadIntStored>());
                    aux = new PadIntStored(uid);
                    aux.setVersion(port + ":" + DateTime.Now.ToString("s"));
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
            if (currentStatus == DETH)
                //Environment.Exit(DETH);
                return null;
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
                return accessExternalPadInt(uid, location);
            }
            return aux.getVersion() == "none:0" ? null : aux;
        }
        public bool setVaule(int uid, int value, String newVersion, String oldVersion)
        {
            Console.WriteLine("SET VALUE!! " + value + " isMine? " + isMine(uid));
            if (isMine(uid))
            {
                myResponsability[hashUid(uid)][uid].setVaule(value, newVersion, oldVersion);
                Console.WriteLine("SET VALUE!! " + value + "  version  " + newVersion);
                return true;
            }
            else
            {
                int location = whereIsPadInt(uid);
                if (location == NONE)
                    return false;
                else
                {
                    ISlaveService slaveAUX = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    return slaveAUX.setVaule(uid, value, newVersion, oldVersion);
                }


            }
        }
        public bool unlockPadInt(int uid, String lockby)
        {
            if (isMine(uid))
            {
                myResponsability[hashUid(uid)][uid].unlockPadInt(lockby);
                return true;
            }
            else
            {
                int location = whereIsPadInt(uid);
                if (location == NONE)
                    return false;
                else
                {
                    ISlaveService slaveAUX = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    return slaveAUX.unlockPadInt(uid, lockby);
                }
            }
        }
        public bool lockPadInt(int uid, String lockby)
        {
            if (isMine(uid))
            {
                if (!myResponsability[hashUid(uid)][uid].lockPadInt(lockby)) {
                    String beenLockedBy = myResponsability[hashUid(uid)][uid].getLockby();
                    String killed = TransactionWrapper.txCompareTo(lockby,beenLockedBy);

                }
                return true;
            }
            else
            {
                int location = whereIsPadInt(uid);
                if (location == NONE)
                    return false;
                else
                {
                    ISlaveService slaveAUX = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    return slaveAUX.lockPadInt(uid, lockby);
                }
            }
        }
        public String accessPadiIntVersion(int uid) {
            PadIntStored  padint = accessPadInt(uid);
            if (padint != null) {
                return padint.getVersion();
            }
            return "ERROR:ERROR";
        }
        public bool isMine(int uid)
        {
            return (whereIsPadInt(uid) == MINE && myResponsability[hashUid(uid)].ContainsKey(uid));
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


        //###########################################################
        public bool CommitTransaction(Transaction t){
            TransactionWrapper newTx = new PADI_DSTM_Lib.TransactionWrapper(cs, t, this.port);
            transacções_state.Add(newTx);
            Console.WriteLine("CommitTransaction no SLAVE!");
            return newTx.CommitTransaction(); //FIXME!!
        }

    }
}
