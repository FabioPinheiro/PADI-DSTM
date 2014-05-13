using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM;
using System.Collections;
using System.Runtime.Serialization.Formatters;

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

        public int matei = 0;
        public int abortou = 0;
        public TcpChannel channelToOut; //change to a list or something of tcpChannel
        public TcpChannel channelListening;
        SlaveServices cs; //for this slave
        IMasterService master;
        IDictionary propBag;
        private int port;
        private int myReplication;//where this Slave is replicated
        private Counter counter;//adicionar a Version da transacçao.
        //############# EXISTE EM TODOS OS SERVIDORES ###############################
        SortedList<int, int> padIntsLocation = new SortedList<int, int>(); //key is the hash, value is the port of the slave that is responsable for that hash
        SortedList<int, SortedList<int, PadIntStored>> myResponsability = new SortedList<int, SortedList<int, PadIntStored>>(); //key is the hash, value is a list of PadiInt's stored in this master
        //$$$$$$$$ COORDENADOR
        List<TransactionWrapper> transacções_state = new List<TransactionWrapper>(); //key: The transaction, value: state (true if live, false is deth ou diyng)
        //############# EXISTE EM TODOS OS SERVIDORES ###############################

        //%%%%%%%%%%%% REPLICAÇÃO %%%%%%%%%%%%%
        private History history;
        public void monitor()
        {
            while (true)
            { //quando fizer freeze isto "morre"
                while (currentStatus == LIVE)
                {
                    System.Threading.Thread.Sleep(1000);
                    //Console.WriteLine("olá");
                    master.ping(port);
                }
                System.Threading.Thread.Sleep(500); //menos tempo para rever se o status nao mudou, pode morrer

            }
        }

        //%%%%%%%%%%%% REPLICAÇÃO %%%%%%%%%%%%%%%%
        public Slave()
        {
            channelToOut = new TcpChannel();
            //ChannelServices.RegisterChannel(channelToOut, false);
            propBag = new Hashtable();
            propBag["name"] = ""; // "Each channel must have a unique name. Set this property to an empty string ("" or String.Empty) 
            //if you want to ignore names, but avoid naming collisions."  CHECK IF WE NEED TO CARE ABOUT THE NAME
            counter = new Counter();
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
            myReplication = master.whereIsMyReplica(port);
            int serverBefore = master.whichReplicaDoIHave(port);
            Console.WriteLine("Registei-me sou o " + port + " e a minha replica é " + myReplication + "e eu replico o " + serverBefore);
            history = new History(serverBefore);
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + serverBefore + "/MyRemoteObjectName");
            try
            {
                slave.reorganizeGrid();
            }
            catch (SocketException)
            {
            //replic is dead, warn master :D
            
            }
            reorganizeGrid();
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
                System.Threading.Thread.Sleep(100);
            }
        }
        public bool fail()
        {
            currentStatus = DETH;
            return true;
        }
        public int getSlave()
        {
            return master.getSlave();
        }
        public void createChannel(int port)
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            propBag["port"] = port;
            channelListening = new TcpChannel(propBag, null, provider);
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
            ISlaveService slave;
            if (location == MINE)
            {
                if (myResponsability[hashUid(uid)].ContainsKey(uid))
                {
                    return myResponsability[hashUid(uid)][uid].getVersion() == "none:0" ? myResponsability[hashUid(uid)][uid] : null;
                }
                aux = new PadIntStored(uid);
                aux.setVersion(DateTime.Now.ToString("s") + ":" + port + ":" + counter.update());

                myResponsability[hashUid(uid)].Add(uid, aux);
                slave = connectToReplic();
                bool rep = false;
                try {
                   rep =  slave.createInReplica(aux, location, false);
                }
                catch(SocketException){
                //slave is dead, warn master, replicate in the new server
                
                }
                if (rep)
                    return aux;
                else {

                    //throw new TxException();
                }
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
                    aux.setVersion( DateTime.Now.ToString("s") + ":" + port + ":" + counter.update());
                    myResponsability[location].Add(uid, aux);
                    slave = connectToReplic();

                    bool rep = false;

                    try {
                    rep = slave.createInReplica(aux, location, true);
                    }
                    catch (SocketException) {
                        //slave is dead, warn master, replicate in the new server

                    }
                    if (rep)
                        return aux;
                    else
                    {

                       // throw new TxException();
                    }
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
                ISlaveService slave = connectToReplic();
                myResponsability[hashUid(uid)][uid].setVaule(value, newVersion, oldVersion);
                bool rep = false;
                try { 
                rep = slave.setValueInReplica(uid, value, newVersion, oldVersion);
                }
                catch (SocketException) {
                    //slave is dead, warn master, replicate in the new server

                
                }

                if (rep)
                {
                    Console.WriteLine("SET VALUE!! " + value + "  version  " + newVersion);
                    return true;
                }
                throw new TxException("failed to set value in replica"); //TODO CHANGE THIS
               
                //return true;
            }
            else
            {
                int location = whereIsPadInt(uid);
                if (location == NONE)
                    return false;
                else
                {
                    ISlaveService slaveAUX = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    try
                    {
                        return slaveAUX.setVaule(uid, value, newVersion, oldVersion);
                    }
                    catch (SocketException) {
                        //slave is dead, warn master, replicate in the new server
                        return true;
                    }
                }


            }
        }
        public bool unlockPadInt(int uid, String lockby)
        {
            if (isMine(uid))
            {
                ISlaveService slave = connectToReplic();
                myResponsability[hashUid(uid)][uid].unlockPadInt(lockby);
                bool myrep = false;
                try { 
                    myrep = slave.unlockInReplica(uid, lockby);
                }
                catch (SocketException) {
                    //slave is dead, warn master, replicate in the new server

                
                }
                if (myrep)
                    return true;
                throw new TxException("failed to unlock in replica"); //TODO CHANGE THIS
            }
            else
            {
                int location = whereIsPadInt(uid);
                if (location == NONE)
                    return false;
                else
                {
                    ISlaveService slaveAUX = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    try
                    {
                        return slaveAUX.unlockPadInt(uid, lockby);
                    }
                    catch (SocketException) {
                        //slave is dead, warn master, replicate in the new server
                        return true;
                    }
                }
            }
        }
        public bool lockPadInt(int uid, String lockby)
        {

            if (isMine(uid))
            {
                ISlaveService slave = connectToReplic();
                if (!myResponsability[hashUid(uid)][uid].lockPadInt(lockby)) {
                    //get the Transaction ID of the lock, in the ID has the port of the slave.
                   /*String beenLockedBy = myResponsability[hashUid(uid)][uid].getLockby(); 
                    //Decide who to kill
                   bool suicide = false;
                   bool toKill = TransactionWrapper.txCompareTo(lockby,beenLockedBy);
                   if (!toKill)
                   {
                       return suicide;
                   }
                   else
                   {
                       //I will try to kill
                       if (tryToKill(beenLockedBy)) //se eu matei
                       {
                           //TODO make kill function! muahahahah
                           Console.WriteLine("##################### I Killed: " + beenLockedBy + " $$$ ");
                           matei++;
                           return !suicide;
                       }
                       else
                       {
                           return suicide;
                       }
                   } */
                   return false;
                }
                bool rep = false;
                try {
                    rep = slave.lockInReplica(uid, lockby);
                }
                catch(SocketException){
                    //slave is dead, warn master, replicate in the new server
                }
                if (rep)
                    return true;
                throw new TxException("failed to lock in replica"); //TODO CHANGE THIS
            }
            else
            {
                int location = whereIsPadInt(uid);
                if (location == NONE)
                    return false;
                else
                {
                    ISlaveService slaveAUX = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    try
                    {
                        return slaveAUX.lockPadInt(uid, lockby);
                    }
                    catch (SocketException)
                    {
                        //slave is dead, warn master, replicate in the new server
                        return true;
                    }
                }
            }
        }
        private bool tryToKill(String tId) 
        { 
            //Find the transaction
            TransactionWrapper t = findTransaction(tId);
            if (t == null)
            {
                return false;
            }
            //AbortTransaction()
            bool willAbort =  t.AbortTransaction();
            //Inform that it will die.
                //Call the function
            //Get the result
            return willAbort; //return false if not.
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
            PadIntStored aux = null;
            try {
                aux = slave.createPadInt(uid);
            }
            catch (SocketException) { 
            //slave is dead, warn master, replicate in the new server
            }
            
            return aux;
        }
        private PadIntStored accessExternalPadInt(int uid, int location)
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
            PadIntStored aux = null;
             try {
             aux = slave.accessPadInt(uid);
             }
             catch (SocketException)
             {
                 //slave is dead, warn master, replicate in the new server
             }

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
            ISlaveService replic = connectToReplic();
            TransactionWrapper newTx = new PADI_DSTM.TransactionWrapper(cs, t, this.port, counter.update(), replic);
            transacções_state.Add(newTx);
            try
            {
                replic.addTransaction(newTx);
            }
            catch (SocketException) {
                //this guy is dead, warn master, replicate in the new server

            }
            Console.WriteLine("CommitTransaction no SLAVE!");
            bool aux= newTx.CommitTransaction(); 
            abortou += newTx.abortou;
            return aux;
        }
        private Int64 incCounter() {
            lock (counter)
            {
                return counter.update();
            }

        }
        private TransactionWrapper findTransaction(String tId) {
            foreach (TransactionWrapper t in transacções_state) {
                if (String.Equals(t.getTransactionWrapperID(), tId))
                    return t;
            }
                return null;
            }
        public int getSlaveId(){
            return port;
        }



        //%%%%%%%%%%%% REPLICAÇÃO %%%%%%%%%%%%%%%%
        public int getReplic() {
            return myReplication;
        }
        private ISlaveService connectToReplic()
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + myReplication + "/MyRemoteObjectName");
            return slave;
        }
        public void slaveIsDead(int slaveId)
        {
            //I have the replica of the dead server. Must add to my data and replicate somewhere
            if (slaveId == history.getId())
            {
                //Add to data
                movePassiveToPrimary();

                myReplication = master.whichReplicaDoIHave(port);
                //add to where is replicated


            }
            //my replica died, change that :D
            if (slaveId == myReplication)
            {
                int replicaId = master.whereIsMyReplica(port);
                changeReplica(replicaId);
            
            }
        }
        private void movePassiveToPrimary() {
            SortedList<int, SortedList<int, PadIntStored>> aux = history.moveToPrimary();
            List<TransactionWrapper> aux_transactions = history.moveCoordinator();
            List<TransactionWrapper> finish_transactions = new List<TransactionWrapper>();
            //move the history to primary!!
            foreach(KeyValuePair<int, SortedList<int, PadIntStored>> kvp in aux){
                myResponsability.Add(kvp.Key,kvp.Value);
            }
            foreach (TransactionWrapper transaction in aux_transactions) {
                //check if this works!!
                if (transaction.isImpossibleToAbort(transaction)) {
                    finish_transactions.Add(transaction);
                }
            }
            //call Function to finish the transactions!!
            updatePassive(myReplication, aux, finish_transactions); 
        }
        private void updatePassive(int myReplication, SortedList<int, SortedList<int, PadIntStored>> auxPadInts, List<TransactionWrapper> finish_transactions)
        { 
        //send the new information to the passive of this server.
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + myReplication + "/MyRemoteObjectName");
            //call the slave
            //send the info! using this addToReplic
            try
            {
                slave.mergePassive(auxPadInts, finish_transactions);
            }
            catch (SocketException)
            {
                //slave is dead, warn master, replicate in the new server
            }
        }
        public void reorganizeGrid() {
            int replicaId = master.whereIsMyReplica(port);
            if(replicaId != port)
                changeReplica(replicaId);
        }
        //change the replica because the replica died OR a new server was added.
        private void changeReplica(int replicaId) {
            Console.WriteLine("muda o sitio onde esta replicado o server: " + port + " para a" + replicaId);
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + replicaId + "/MyRemoteObjectName");
            try
            {
                slave.modifyHistory(myResponsability, transacções_state, port);
            }
            catch (SocketException)
            {
                //slave is dead, warn master, replicate in the new server
            }
        }
        //merge the data with the data of the replication.
        public void mergePassive(SortedList<int, SortedList<int, PadIntStored>> rep, List<TransactionWrapper> finish_transactions)
        {
            history.addToReplic(rep, finish_transactions);
        }
        
        public void modifyHistory(SortedList<int, SortedList<int, PadIntStored>> myResponsability, List<TransactionWrapper> transacções_state, int newSlaveId)
        {

            Console.WriteLine("MODIFIQUEI A REPLICA");
            history.changeReplic(myResponsability, transacções_state, newSlaveId);
        }



        public bool createInReplica(PadIntStored padInt, int hash, bool newhash)
        {
            Console.WriteLine("Vai criar na Replica weeee");
            history.createInReplic(hash, padInt, newhash);
            return true;
        }


        public bool commitInReplica() {
            //call the history
            return true;
        }
        public bool lockInReplica(int uid, String lockby)
        {
            //call the history
            //history.myReplication
            return true;
        }
        public bool unlockInReplica(int uid, String lockby)
        {
            //call the history
            history.myReplication[hashUid(uid)][uid].unlockPadInt(lockby);

            return true;
        }

        public bool setValueInReplica(int uid, int value, String newVersion, String oldVersion) {
            history.myReplication[hashUid(uid)][uid].setVaule(value, newVersion, oldVersion);
            return true;
        }

        public void printSlave()
        {
            Console.WriteLine("O servidor " + port);
            foreach (KeyValuePair<int, SortedList<int, PadIntStored>> kvp in myResponsability)
            {
                Console.WriteLine("Hash number " + kvp.Key);
                foreach (KeyValuePair<int, PadIntStored> k in kvp.Value)
                {
                    Console.WriteLine("PadInt number " + kvp.Key);

                }

            }
            history.printHistory();
        
        }

        public void addTransaction(TransactionWrapper newTx) {
            history.transacções_state_Replication.Add(newTx);
        }
        public TransactionWrapper findTransaction(int port, long counter) {
            TransactionWrapper tx = null;
            foreach(TransactionWrapper t in history.transacções_state_Replication){
                if (t.getPort() == port && t.getCounter() == counter) {

                    tx = t;
                }
            
            }

            return tx;
        
        }
    }

    public class Counter {
        private Int64 counter;

        public Counter()
        {
            counter = 0;
        }
        public Int64 get() {
            return counter;
        }
        public Int64 update() {
            return counter++;
        } 
    }
    public class History {
        int slaveId; //o slave que está a replicar
        public SortedList<int, SortedList<int, PadIntStored>> myReplication = new SortedList<int, SortedList<int, PadIntStored>>(); //key is the hash, value is a list of PadiInt's stored in this master
        //$$$$$$$$ COORDENADOR
        public List<TransactionWrapper> transacções_state_Replication = new List<TransactionWrapper>(); //key: The transaction, value: state (true if live, false is deth ou diyng)
        public int getId() {
            return slaveId;
        }
        public History(int port) {
             slaveId = port;
        }
        public void changeReplic(SortedList<int, SortedList<int, PadIntStored>> myNewReplication, List<TransactionWrapper> transacções_state_Replication_new, int slaveIdnew)
        {

            Console.WriteLine("I am the new replic of " + slaveIdnew);
            myReplication = myNewReplication;
            transacções_state_Replication = transacções_state_Replication_new;
            slaveId = slaveIdnew;
        }

        public void compare(SortedList<int, SortedList<int, PadIntStored>> l1, SortedList<int, SortedList<int, PadIntStored>> l2) {
        
        }
        public void compare(List<TransactionWrapper> l1, List<TransactionWrapper> l2) { 
        
        }
        //Recupera um servidor
        public void restore() { }

        //Trata como sendo um novo.
        public void reCreate() { }

        //Recebe uma lista e adiciona a replicação
        public void addToReplic(SortedList<int, SortedList<int, PadIntStored>> rep, List<TransactionWrapper> finish_transactions)
        {
            foreach (KeyValuePair<int, SortedList<int, PadIntStored>> kvp in rep)
            {
                myReplication.Add(kvp.Key, kvp.Value);
            }
            foreach (TransactionWrapper t in finish_transactions) { 
                //DO Stuff
            }
        }
        //adiciona uma parte da hash a replicação
        public void addToReplic(int hasnumber, SortedList<int, PadIntStored> list)
        {

        }
        public void createInReplic(int hasnumber, PadIntStored padInt, bool newHash) {
            if (newHash)
            {
                myReplication.Add(hasnumber,new SortedList<int, PadIntStored>());
                //cria a lista na hash :D
            }
            myReplication[hasnumber].Add(padInt.getID(), padInt);
            //adiciona o PadInt
        }
        public SortedList<int, SortedList<int, PadIntStored>> moveToPrimary() {
            return myReplication;
        }

        public List<TransactionWrapper> moveCoordinator()
        {
            return transacções_state_Replication;
        }

        public bool finishTransactions() { 
        //the slave is dead and we need to finish the transactions that were stored in the replic
            List<TransactionWrapper> possibleToAbort = new List<TransactionWrapper>();
            List<TransactionWrapper> impossibleToAbort = new List<TransactionWrapper>();
            foreach (TransactionWrapper t in transacções_state_Replication) {
                if (t.isImpossibleToAbort(t)) {
                    impossibleToAbort.Add(t);
                }
                if (t.isPossibleToAbort(t)) {
                    possibleToAbort.Add(t);
                }
            
            }
            foreach (TransactionWrapper t in impossibleToAbort)
            {
                t.CommitTransaction();
            }
            foreach (TransactionWrapper t in possibleToAbort)
            {
                t.CommitTransaction();
            }




            return true;
        }

        public void printHistory() {

            Console.WriteLine("Replicação do server " + slaveId);
            foreach (KeyValuePair<int, SortedList<int, PadIntStored>> kvp in myReplication)
            {
                Console.WriteLine("Hash number " + kvp.Key);
                foreach (KeyValuePair<int, PadIntStored> k in kvp.Value)
                {
                    Console.WriteLine("PadInt number " + kvp.Key);

                }
            
            }
        
        }


    }
}
