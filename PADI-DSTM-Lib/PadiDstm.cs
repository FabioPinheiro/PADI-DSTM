﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace PADI_DSTM
{

    public class PadiDstm
    {
        private static int port = 0;
        private static int replic = 0;
        private static IMasterService master;
        private static ISlaveService slave; // some slave
        private static ISlaveService slave_replic;
        private static Transaction tx;
        public static bool Init()
        { //so é feito uma vez aka por o Master up
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            master = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");
            if (master == null)
                return false;
            //System.Console.WriteLine("Could not locate server");
            else
            {
                port = master.getSlave();
                if (port != 0)
                    return true;
                else {
                    Console.WriteLine("nao ha port");
                    return false;
                }

            }
        }
        public static bool TxBegin()
        { //Liga-se ao slave e começa uma transacçºao. falta começar uma transacção.
            if (port == 0) {
                Console.WriteLine("No Slaves Found");
                throw new TxException("No Slaves Found");
               
            }
            TcpChannel channel = new TcpChannel();
            slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + port + "/MyRemoteObjectName");
            try
            {
             replic = slave.getReplic();
               

            }
            catch (SocketException) {
                //this guy is dead, warn master, replicate in the new server
                master.slaveIsDead(port);
                Console.WriteLine("TxBegin catch: port before: " + port);
                port = master.getSlave();
                Console.WriteLine("TxBegin catch: port after: "+ port);

                slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + port + "/MyRemoteObjectName");
                try {
                    replic = slave.getReplic();
                }
                catch (SocketException) { 
                    Console.WriteLine("TxBegin: error so toleramos uma falta");
                }
                
            }

            slave_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + replic + "/MyRemoteObjectName");
            if (slave == null)
            {
                Console.WriteLine("Slave not Found");
                throw new TxException("Slave not Found"); //fix me
            }
            else
            {
                tx = new Transaction();
                return true;
            }
        }

        public static bool TxCommit()
        {
            if (tx != null)
            {
                //Console.WriteLine("Entrou no TxCommit!!! SIM OUTRA VEZ!!");

                bool ret = false;
                long counter;
                try
                {
                    counter = slave.updateCounter();
                    slave_replic.updateCounterInReplica();
                }
                catch (SocketException) {
                    master.slaveIsDead(port);
                    counter = slave_replic.updateCounterInReplica();
                
                }
                TransactionWrapper newTx = new TransactionWrapper(slave, tx, port, counter ,slave_replic, master);

                try
                {
                    ret = slave.CommitTransaction(tx);
                }
                catch (SocketException) {
                    master.slaveIsDead(port);
                }
                try
                {
                    slave_replic.commitInReplica(newTx);
                }
                catch (SocketException) {
                    master.slaveIsDead(replic);
                
                }
                tx = null;
                return ret;
            }
            else { throw new TxException("Não existe nenhuma transação"); }
        }
        public static bool TxAbort() //SOMOS CONTRA O ABORTO!! PRO VIDA!!
        {
            if (tx != null)
            {
                tx = null;
                return true;
            }
            else return false;
        }
        public static bool Status()
        {
            //fala com o master of puppets
            return master.status(); ;
        }
        public static bool Fail(string URL)
        {
            return master.fail(URL);
        }
        public static bool Freeze(string URL)
        {
            return master.freeze(URL);
        }
        public static bool Recover(string URL)
        {
            return master.recover(URL);
        }

        public static PadInt CreatePadInt(int uid)
        {
            //master.createPadInt(uid); //change to slave and number of args
            // PadIntStored pds = slave.createPadInt(uid);
            // return pds == null ? null : new PadInt(pds);
            return tx.CreatePadInt(uid, slave, master, port);
        }

        public static PadInt AccessPadInt(int uid)
        {
            return tx.AccessPadInt(uid, slave, master, port);
        }
    }


    public interface IMasterService
    {
        int register();
        string MetodoOla();
        int getSlave();
        PadIntStored createPadInt(int uid);
        PadIntStored accessPadInt(int uid);
        bool setMine(int port, int hash);
        bool freeze(String url);
        bool recover(String url);
        bool fail(String url);
        bool status();
        bool ping(int slaveId);
        int whereIsMyReplica(int slaveId);
        int whichReplicaDoIHave(int slaveId);
        bool updateHash(int hash, int slaveId);
        void slaveIsDead(int slaveId);
        SortedList<int, int> getPadIntsLocation();
    }
    public interface ISlaveService
    {
        string MetodoOlaClient();
        PadIntStored createPadInt(int uid);
        PadIntStored accessPadInt(int uid);
        String accessPadiIntVersion(int uid);
        bool setResponsability(int port, int hash);
        bool freeze();
        bool recover();
        bool fail();
        bool status();
        bool setVaule(int uid, int value, String newVersion, String oldVersion);
        bool unlockPadInt(int uid, String lockby);
        bool lockPadInt(int uid, String lockby);
        bool CommitTransaction(Transaction Trtnsaction);
        int getSlaveId();
        int getReplic();
        void slaveIsDead(int slaveId);
        void reorganizeGrid();
        void modifyHistory(SortedList<int, SortedList<int, PadIntStored>> myResponsability, List<TransactionWrapper> transacções_state, int newSlaveId);
        
        void mergePassive(SortedList<int, SortedList<int, PadIntStored>> auxPadInts, List<TransactionWrapper> finish_transactions);

        bool createInReplica(PadIntStored padInt, int hash, bool newhash);
        bool lockInReplica(int uid, String lockby);
        bool unlockInReplica(int uid, String lockby);
        bool commitInReplica(TransactionWrapper t);
        bool setValueInReplica(int uid, int value, String newVersion, String oldVersion);
        void addTransaction(TransactionWrapper newTx);
        TransactionWrapper findTransaction(int port, long counter);
        long updateCounter();
        long updateCounterInReplica();
        String accessPadiIntVersionInReplica(int uid);
        PadIntStored createPadIntInReplica(int uid);
        PadIntStored acessPadIntInReplica(int uid);
        int whereIsPadInt(int uid);

    }

    [Serializable] // passar por referencia; já não nessecario
    public class PadIntStored
    {
        private String version = "none:0";
        private int id;
        private int value;
        private String lockby = "none"; //Transaction ID que tem time/port/counter 

        public String getLockby() { return lockby; }
        public PadIntStored(int uid)
        {
            id = uid;
        }
        public bool lockPadInt(String lockby)
        {
            if (this.lockby == "none")
            {
                this.lockby = lockby;
                return true;
            }
            else return false;
        }
        public bool unlockPadInt(String lockby)
        {
            Console.WriteLine("unlockPadInt");
            if (this.lockby == lockby)
            {
                this.lockby = "none";
                return true;
            }
            else return false;
        }

        public bool setVaule(int value, String newVersion, String oldVersion)
        {
            Console.WriteLine("setVaule!!" + " lockby:" + lockby + " newVersion: " + newVersion + " oldVersion:" + oldVersion + "  this.version:" + this.version);
            if (oldVersion == this.version && lockby == newVersion)
            {
                Console.WriteLine("setVaule!! newVersion: " + newVersion + " oldVersion:" + oldVersion);
                version = newVersion;
                this.value = value;
                return true;
            }
            else return false;
        }
        public void setVersion(String newVersion) {
            version = newVersion;
        }
        public int getID() { return id; }
        public int getValue() { return value; }
        public String getVersion() { return version; }
        public String toString() { return ">ID=" + id + " valor=" + value + " version=" + version + " lockby=" + lockby + "; "; }
    }
     [Serializable] 
    public class PadInt // só existe no salve
    { //read e write may throw TxException.

        private PadIntStored padInt;
        private String accessVersion;
        private bool readedAux = false;/*for client*/
        private bool writedAux = false;/*for client*/
        private int valueAux;/*for client*/
        private bool lockedAux = false;/*for salve*/

        public bool islockedAux() { return lockedAux; }

        public PadInt(PadIntStored padInt)
        {
            this.padInt = padInt;
            this.accessVersion = padInt.getVersion();
            if (this.accessVersion == "none:0") { readedAux = true; }
            this.valueAux = padInt.getValue();
        }

        public int Read()/*for client*/{ readedAux = true; return this.valueAux; }
        public void Write(int value)/*for client*/ { writedAux = true; this.valueAux = value; }
        public String toString() { return ">PadIntStored:" + padInt.toString() + " >PadInt: valueAux=" + valueAux + " readedAux=" + readedAux + " writedAux=" + writedAux + ";"; }

        public bool setLock(String transactionID, ISlaveService slave, IMasterService master, int slaveId)
        {
            ISlaveService slave_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + master.whereIsMyReplica(slaveId) + "/MyRemoteObjectName");
            try
            {
                lockedAux = slave.lockPadInt(padInt.getID(), transactionID);
                
            }
            catch (SocketException) {
                lockedAux = slave_replic.lockPadInt(padInt.getID(), transactionID);
                master.slaveIsDead(slaveId);

            }
            /*try
            {
                if (repeat)
                {
                   // Console.WriteLine("unlockInReplica Begin in " + slave_replic.getSlaveId());
                    int location = slave_replic.whereIsPadInt(padInt.getID());
                    ISlaveService aux_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    location = aux_replic.getReplic();
                    aux_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
                    aux_replic.lockInReplica(padInt.getID(), transactionID);
                   // Console.WriteLine("unlockInReplica End");
                }
            }
            catch (SocketException) {
                master.slaveIsDead(slaveId);
            
            }*/
            return lockedAux;
        }
        public bool setUnlock(String transactionID, ISlaveService slave, IMasterService master, int slaveId)
        {
            ISlaveService slave_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + master.whereIsMyReplica(slaveId) + "/MyRemoteObjectName");
            try
            {
                lockedAux = !slave.unlockPadInt(padInt.getID(), transactionID);

            }
            catch (SocketException) {
                lockedAux = !slave_replic.unlockPadInt(padInt.getID(), transactionID);
                master.slaveIsDead(slaveId);
            }

            if (!lockedAux)
                return true;
            else throw new TxException("setUnlock FAIL nuca devia chegarr aqui"); //FIXME!!!!!!!!!!!!!!!!!!
        }

        public bool confirmVersion(ISlaveService slave, IMasterService master, int slaveId)
        {
            ISlaveService slave_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + master.whereIsMyReplica(slaveId) + "/MyRemoteObjectName");
            String padiVersion = "";
            try
            {
                Console.WriteLine("accessVersion " + accessVersion + "  ####  padint Version " + slave.accessPadiIntVersion(padInt.getID()) + "  RESULT  " + (accessVersion == slave.accessPadiIntVersion(padInt.getID())));
                padiVersion = slave.accessPadiIntVersion(padInt.getID());
            }
            catch (SocketException) {
                padiVersion = slave_replic.accessPadiIntVersionInReplica(padInt.getID());
                master.slaveIsDead(slaveId);
                //call the function
            }

            if (accessVersion == padiVersion )
                return true;
            else return false;
        }

        public bool commitVaule(String transactionID, ISlaveService slave, IMasterService master, int slaveId)
        {
            Console.WriteLine("  TRANSACTION COMMITVALUE  " + this.toString());
            bool ret;
            ISlaveService slave_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + master.whereIsMyReplica(slaveId) + "/MyRemoteObjectName");

            if (readedAux || writedAux)
            {
                try
                {
                    ret = slave.setVaule(padInt.getID(), this.valueAux, transactionID, this.accessVersion);
                    //slave_replic.setValueInReplica(padInt.getID(), this.valueAux, transactionID, this.accessVersion);
                    return ret;
                }
                catch (SocketException) {
                    ret = slave_replic.setVaule(padInt.getID(), this.valueAux, transactionID, this.accessVersion);
                    master.slaveIsDead(slaveId);
                    return ret;
                }
            }
            else
                return true;//FIXME padInt o PadInt deve ser informado disto ou não? ()alterar a verção
        }

    }

    public class TxException : System.Exception
    {
        private String error;
        public TxException(String errorInf) { this.error = errorInf; }

        public TxException()
        {
            // TODO: Complete member initialization
        }
        public String toString() { return error; }
    }

    [Serializable]
    public class Transaction
    {
        private SortedList<int, PadInt> poolPadInt = new SortedList<int, PadInt>();
        public SortedList<int, PadInt> getPoolPadInt() { return poolPadInt; }

        public Transaction() {}


        private PadIntStored remotingAccessPadIntStored(int uid, bool toCreate, ISlaveService slave, IMasterService master, int slaveId)
        {

            ISlaveService slave_replic = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + master.whereIsMyReplica(slaveId) + "/MyRemoteObjectName");
            PadIntStored ret = null;
            if (toCreate)
            {
                try
                {
                    ret = slave.createPadInt(uid);
                    //slave_replic.createPadIntInReplica(uid);
                    return ret;
                }
                catch (SocketException)
                {
                    //this guy is dead, warn master, replicate in the new server
                    ret = slave_replic.createPadIntInReplica(uid);
                    master.slaveIsDead(slaveId);
                    //call the function
                    return ret;
                }
            }
            else
            {
                try
                {
                    return slave.accessPadInt(uid);
                }
                catch (SocketException) {
                    ret = slave_replic.accessPadInt(uid);
                    master.slaveIsDead(slaveId);
                    return ret;
                }
            }
        }
        public PadInt remotingAccessPadInt(int uid, bool toCreate, ISlaveService slave, IMasterService master, int slaveId )
        {
            //se toCreate == true
            //devolve null se já existir OU SE A VERSÂO != "none:0"; caso contrario devolve o PadInt
            //se toCreate == false
            //delvolve o PadInt se existir E se a versão  for diferente de "none:0"
            PadIntStored padIntStored = remotingAccessPadIntStored(uid, toCreate, slave, master, slaveId);
            if (padIntStored != null)
                return new PadInt(padIntStored);
            else return null;

            /*if (toCreate)
            {
                padIntStored = slave.createPadInt(uid); //FIXM se o servidor tiver morto isto devolve null .... ERROR
                if (padIntStored != null)
                    return padIntStored;
                else {
                    padIntStored = slave.accessPadInt(uid); //FIXM se o servidor tiver morto isto devolve null .... ERROR
                    if (padIntStored.getVersion() == "none:0")
                        return padIntStored;
                    else return null;
                }
            }
            else {
                padInt = slave.accessPadInt(uid); //FIXM se o servidor tiver morto isto devolve null .... ERROR
                if (padInt.getVersion() != "none:0")
                    return padInt;
                else return null;
            }*/
        }
         

        public PadInt CreatePadInt(int uid, ISlaveService slave, IMasterService master, int slaveId)
        {
            PadInt aux = remotingAccessPadInt(uid, true, slave, master, slaveId);
            if (aux != null)
            {
                poolPadInt.Add(uid, aux);
                return aux;
            }
            else return null; //!!Confirmado (Fabio: segundo o rafael) FIXME isto não devia devolver exection?
        }

        public PadInt AccessPadInt(int uid, ISlaveService slave, IMasterService master, int port)
        {
            if (poolPadInt.ContainsKey(uid))
                return poolPadInt[uid];
            else
            {
                PadInt aux = remotingAccessPadInt(uid, false, slave, master, port);
                if (aux != null)
                {
                    poolPadInt.Add(uid, aux);
                    return aux;
                }
                else return null;
            }
        }

    }


    [Serializable]
    public class TransactionWrapper
    {
        enum State { possibleToAbort=1, impossibleToAbort=2, Abort=3, Commited = 4 };
        private String timeStramp;
        private ISlaveService slave;
        private State state;
        public String getTransactionWrapperID() { return timeStramp + ":" + Convert.ToString(port) + ":" + Convert.ToString(counter); } //FIXME EEEEEEEE
        private Transaction transaction;
        private int port;
        private Int64 counter;
        private Object lockState = new Object();
        public int abortou = 0;
        private ISlaveService replic;
        private IMasterService master;

        public TransactionWrapper(ISlaveService slave, Transaction transaction, int port, Int64 counter, ISlaveService replic, IMasterService master) //FIXME remove port
        {
            this.port = port; //FIXME é para remover!!!
            this.slave = slave;
            this.transaction = transaction;
            timeStramp = DateTime.Now.ToString("s");
            state = State.possibleToAbort;
            this.counter = counter;
            this.replic = replic;
            this.master = master;
        }

        //###########################################################

        private bool lockAllPadIntAndCheckVersion()
        {
            foreach (KeyValuePair<int, PadInt> pair in transaction.getPoolPadInt())
            {
                if (!pair.Value.setLock(getTransactionWrapperID(), slave, master, port))
                {
                    //manda nack
                    return false;
                }
                else
                {
                    if (!pair.Value.confirmVersion(slave,master,port))
                    {
                        //manda nack
                        unlockAllPadIntLocked();
                        return false;
                    }
                }
                if (readState() == State.Abort)
                {
                   //PAssa por aqui!!! Wich is good :D
                    abortou++;
                    //manda nack
                    unlockAllPadIntLocked();
                    return false;
                }
                //manda ack ao coordenador.
            }

            return true; // consegui fazer look a tudo
        }
        private bool unlockAllPadIntLocked()
        {
            foreach (KeyValuePair<int, PadInt> pair in transaction.getPoolPadInt())
            {
                if (pair.Value.islockedAux() /*evita fazer unlock as variavei que não estão lock*/ && !pair.Value.setUnlock(getTransactionWrapperID(), slave, master, port))
                {
                    throw new TxException("UnlockAllPadIntLocked");
                }
            }
            return true; // consegui fazer unlook a todo
        }
        private bool reasonsForSuicide()
        {
            foreach (KeyValuePair<int, PadInt> pair in transaction.getPoolPadInt())
            {
                if (!pair.Value.confirmVersion(slave, master, port))
                    return false;
            }
            return true;
        }
        private bool TxCommitAUX()
        {
            Console.WriteLine("TxCommitAUX()");
            if (!reasonsForSuicide()) 
            {
                Console.WriteLine("TxCommitAUX -> reasonsForSuicide!!");
                return false;
            }
            else
            {
                try
                {
                    Console.WriteLine("CommitAux -> Try");
                    if (!lockAllPadIntAndCheckVersion())
                        return false;
     
                    else
                    {
                        //Em teoria nunca vai chegar aqui... pois aborta antes
                        if (readState() == State.Abort){
                            abortou++;
                            throw new Exception(); //Remove this after debug
                            //return false;
                        }
                        else
                        {
                            Console.WriteLine("CommitAux -> antes do changeState");

                            changeState(State.impossibleToAbort, master, port); 
                            foreach (KeyValuePair<int, PadInt> pair in this.transaction.getPoolPadInt())
                            {
                                if (!pair.Value.commitVaule(getTransactionWrapperID(), slave, master, port))
                                {
                                    Console.WriteLine("########################### throw new TxException();");
                                    throw new TxException("TxCommitAUX->commitVaule Fail (possivel inconcistencia)");
                                }
                            }
                            Console.WriteLine("CommitAux -> antes do changeState -> Commit");
                            changeState(State.Commited, master, port);
                        }
                    }


                }
                finally { unlockAllPadIntLocked(); } ///FIXME return ... talvez não seja um problema REVER
            }
            Console.WriteLine("DONE-TxCommitAUX()");
            return true;
        }
        public bool CommitTransaction()
        {
            //Task taskA = new Task(() => TxCommitAUX());
            //ThreadStart ts = new ThreadStart(this.TxCommitAUX()); 
            Task<bool>[] taskArray = { Task<bool>.Factory.StartNew(() => this.TxCommitAUX()) };

            Console.WriteLine("WAITNG !!!!");
            taskArray[0].Wait();
            //taskA.Wait();
            Console.WriteLine("AFTER WAIT !!!!");
            return taskArray[0].Result;
            //Task[] taskArray = new Task[poolPadInt.Count]; //SEE http://msdn.microsoft.com/en-us/library/dd537609(v=vs.110).aspx
        }
        private static string timeFromId(String ts)
        {
            String[] words = ts.Split(':');

            return words[0] + ":" + words[1] + ":" + words[2];
        }
        private static string portFromId(String tId) {
            String[] words = tId.Split(':');

            return words[3];
        }
        private static string counterFromId(String tId)
        {
            String[] words = tId.Split(':');

            return words[4];
        }
        private static long compareIntsInStrings(String str1, String str2) {
            return Convert.ToInt64(str1) - Convert.ToInt64(str2);
        }
        public static bool txCompareTo(String transactionID1, String transactionID2)
        {
            Console.WriteLine("Comparing Transactions => transactionID1: " + transactionID1 + "   transactionID2: " + transactionID2);
            String str1 = timeFromId(transactionID1);
            String str2 = timeFromId(transactionID2);
            Console.WriteLine("str1: " + str1 + "   str2: " + str2);
            bool suicide = false;
            bool kill = true;
            int comp = DateTime.Compare(DateTime.Parse(str1), DateTime.Parse(str2));
            if (comp < 0)
                return kill;
            else if(comp > 0){
                return suicide;
                }
            else
            {
                str1 = portFromId(transactionID1);
                str2 = portFromId(transactionID2);
                //Port
                long aux = compareIntsInStrings(str1, str2);
                if(aux < 0){
                    return kill;
                    //port > 0
                   
                }
                else if (aux > 0) {
                    return suicide;
                }
                    //Counter
                else {
                    str1 = counterFromId(transactionID1);
                    str2 = counterFromId(transactionID2);
                    aux = compareIntsInStrings(str1, str2);
                    if (aux < 0)
                    {
                        return kill;
                        //port > 0

                    }
                    else if (aux >0) {
                        return suicide;
                    }
                    else
                    {
                        //HELL FROZEN :| :| DANGER DANGER
                        return suicide; //
                    }
                }

            }
        }
        public bool AbortTransaction()
        {
            if (readState() == State.possibleToAbort)
            {
                changeState(State.Abort, master, port);
                Console.WriteLine("### I will abort " + getTransactionWrapperID());
                if (readState() == State.Abort)
                    return true;
                else
                    throw new Exception();
            }
            else return false;

        }
        private void changeState(State status, IMasterService master, int slaveId)
        {
            //Console.WriteLine("Entra no changeState");
            lock (lockState)
            {
                if (status == State.impossibleToAbort && state == State.Abort && state == State.Commited)
                    throw new TxException("ChangeState, ImpossibleToAbort when it was to Abort");
                state=status;
            }
            try
            {
                replic.findTransaction(port, counter).changeStateInReplic(status, master, slaveId);
            }
            catch (SocketException) {
                master.slaveIsDead(slaveId);
            }
           // Console.WriteLine("Sai no changeState");

        }
        private void changeStateInReplic(State status, IMasterService master, int slaveId) { 
            lock(lockState){
                state = status;
            }
        
        }
        private State readState() {

            State aux;
            lock (lockState) {

                aux = state;
            }

            return aux;
        }
        public int getPort() {
            return port;
        }
        public long getCounter() {
            return counter;
        }

        public bool isImpossibleToAbort(TransactionWrapper t) {
            return t.state == State.impossibleToAbort;
        }
        public bool isPossibleToAbort(TransactionWrapper t)
        {
            return t.state == State.possibleToAbort;
        }



    }

}
