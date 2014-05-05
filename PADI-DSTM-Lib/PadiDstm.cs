using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PADI_DSTM
{

    public class PadiDstm
    {
        private static int port = 0;
        private static IMasterService master;
        private static ISlaveService slave; // some slave
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
                return true;

            }
        }
        public static bool TxBegin()
        { //Liga-se ao slave e começa uma transacçºao. falta começar uma transacção.
            TcpChannel channel = new TcpChannel();
            slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + port + "/MyRemoteObjectName");
            tx = new Transaction();
            if (slave == null)
                return false;
            else return true;
        }

        public static bool TxCommit()
        {
            if (tx != null)
            {
                Console.WriteLine("TxCommit!!! SIM OUTRA VEZ!!");
                bool ret = slave.CommitTransaction(tx);
                tx = null;
                return ret;
            }
            else { throw new TxException("Não existe nenhuma transação");  return false; }
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
            //fala com o master
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
            return tx.CreatePadInt(uid, slave);
        }

        public static PadInt AccessPadInt(int uid)
        {
            return tx.AccessPadInt(uid, slave);
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

        public bool setLock(String transactionID, ISlaveService slave)
        {
            lockedAux = slave.lockPadInt(padInt.getID(), transactionID);
            return lockedAux;
        }
        public bool setUnlock(String transactionID, ISlaveService slave)
        {
            lockedAux = !slave.unlockPadInt(padInt.getID(), transactionID);
            if (!lockedAux)
                return true;
            else throw new TxException("setUnlock FAIL nuca devia chegarr aqui"); //FIXME!!!!!!!!!!!!!!!!!!
        }

        public bool confirmVersion(ISlaveService slave)
        {
            Console.WriteLine("confirmVersion TEM PROBLEMAS!! accessVersion " + accessVersion + "  ####  padint Version " + slave.accessPadiIntVersion(padInt.getID()) + "  RESULT  " + (accessVersion == slave.accessPadiIntVersion(padInt.getID())));
            if (accessVersion == slave.accessPadiIntVersion(padInt.getID()))
                return true;
            else return false;
        }

        public bool commitVaule(String transactionID, ISlaveService slave)
        {
            Console.WriteLine("  TRANSACTION COMMITVALUE  " + this.toString());

            if (readedAux || writedAux)
            {
                return slave.setVaule(padInt.getID(), this.valueAux, transactionID, this.accessVersion);
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

        /*LIXO
        private bool lockAllPadInt()
        {
            //bool locking = true; //FIXM
            foreach (KeyValuePair<int, PadInt> pair in poolPadInt)
            {
                if (!pair.Value.setLock(this, slave))
                    return false;
            }
            return true; // consegui fazer look a todo
        }
        private bool unlockAllPadInt()
        {
            //bool locking = true; //FIXM
            foreach (KeyValuePair<int, PadInt> pair in poolPadInt)
            {
                if (!pair.Value.setUnlock(this, slave))
                    return false;
            }
            return true; // consegui fazer look a todo
        }
        public bool TxCommitAUX()//FIXM muitos problemas de consistencia
        {
            Console.WriteLine("TxCommitAUX()");
            try
            {
                lockAllPadInt(); //FIXM return ...
                Console.WriteLine("lockAllPadInt()");
                foreach (KeyValuePair<int, PadInt> pair in poolPadInt)
                {
                    if (!pair.Value.commitVaule(this, slave))
                    {
                        Console.WriteLine("throw new TxException();");
                        throw new TxException("TxCommitAUX->commitVaule Fail");
                    }
                }

            }
            finally { unlockAllPadInt(); } ///FIXM return ...
            Console.WriteLine("DONE-TxCommitAUX()");
            return true;
        }

        public bool TxCommit()
        {
            //Task taskA = new Task(() => TxCommitAUX());
            //ThreadStart ts = new ThreadStart(this.TxCommitAUX()); 
            Task<bool>[] taskArray = { Task<bool>.Factory.StartNew(() => this.TxCommitAUX()) };

            Console.WriteLine("WAITNG !!!!");
            taskArray[0].Wait();
            //taskA.Wait();
            return taskArray[0].Result;
            //Task[] taskArray = new Task[poolPadInt.Count]; //SEE http://msdn.microsoft.com/en-us/library/dd537609(v=vs.110).aspx
        }*/

        private PadIntStored remotingAccessPadIntStored(int uid, bool toCreate, ISlaveService slave)
        {
            if (toCreate)
                return slave.createPadInt(uid);
            else return slave.accessPadInt(uid);
        }
        public PadInt remotingAccessPadInt(int uid, bool toCreate, ISlaveService slave)
        {
            //se toCreate == true
            //devolve null se já existir OU SE A VERSÂO != "none:0"; caso contrario devolve o PadInt
            //se toCreate == false
            //delvolve o PadInt se existir E se a versão  for diferente de "none:0"
            PadIntStored padIntStored = remotingAccessPadIntStored(uid, toCreate, slave);
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


        public PadInt CreatePadInt(int uid, ISlaveService slave)
        {
            PadInt aux = remotingAccessPadInt(uid, true, slave);
            if (aux != null)
            {
                poolPadInt.Add(uid, aux);
                return aux;
            }
            else return null; //!!Confirmado (Fabio: segundo o rafael) FIXME isto não devia devolver exection?
        }

        public PadInt AccessPadInt(int uid, ISlaveService slave)
        {
            if (poolPadInt.ContainsKey(uid))
                return poolPadInt[uid];
            else
            {
                PadInt aux = remotingAccessPadInt(uid, false, slave);
                if (aux != null)
                {
                    poolPadInt.Add(uid, aux);
                    return aux;
                }
                else return null; //!!Confirmado (Fabio: segundo o rafael) FIXME isto não devia devolver exection?
            }
        }

        //guarda os objectos acedidos. aka todos

        //begin aka construtor
        //abort
        //commit






    }

    public class TransactionWrapper
    {
        enum State { possibleToAbort=1, impossibleToAbort=2, Abort=3 };
        private String timeStramp;
        private ISlaveService slave;
        private State state;
        public String getTransactionWrapperID() { return timeStramp + ":" + Convert.ToString(port) + ":" + Convert.ToString(counter); } //FIXME EEEEEEEE
        private Transaction transaction;
        private int port;
        private Int64 counter;
        private Object lockState = new Object();
        public int abortou = 0;

        public TransactionWrapper(ISlaveService slave, Transaction transaction, int port, Int64 counter) //FIXME remove port
        {
            this.port = port; //FIXME é para remover!!!
            this.slave = slave;
            this.transaction = transaction;
            timeStramp = DateTime.Now.ToString("s");
            state = State.possibleToAbort;
            this.counter = counter;
        }

        //###########################################################

        private bool lockAllPadIntAndCheckVersion()
        {
            foreach (KeyValuePair<int, PadInt> pair in transaction.getPoolPadInt())
            {
                if (!pair.Value.setLock(getTransactionWrapperID(), slave))
                    return false;
                else {
                    if (!pair.Value.confirmVersion(slave))
                        return false;
                }
                if (readState() == State.Abort)
                {
                   //PAssa por aqui!!! Wich is good :D
                    abortou++;
                    return false;
                }
            }
            return true; // consegui fazer look a tudo
        }
        private bool unlockAllPadIntLocked()
        {
            foreach (KeyValuePair<int, PadInt> pair in transaction.getPoolPadInt())
            {
                if (pair.Value.islockedAux() /*evita fazer unlock as variavei que não estão lock*/ && !pair.Value.setUnlock(getTransactionWrapperID(), slave))
                    return false;
            }
            return true; // consegui fazer unlook a todo
        }
        private bool reasonsForSuicide()
        {
            foreach (KeyValuePair<int, PadInt> pair in transaction.getPoolPadInt())
            {
                if (!pair.Value.confirmVersion(slave))
                    return false;
            }
            return true;
        }

        private bool TxCommitAUX()//FIXME muitos problemas de consistencia
        {
            Console.WriteLine("TxCommitAUX()");
            if (!reasonsForSuicide()) //tem motivos para isso !!
            {
                Console.WriteLine("TxCommitAUX -> reasonsForSuicide!!");
                return false;
            }
            else
            {
                try
                {
                    if (!lockAllPadIntAndCheckVersion())
                        return false;
                    //check this!!
                    else
                    {

                        //Em teoria nunca vai chegar aqui... pois aborta antes
                        if (readState() == State.Abort)
                        {

                            abortou++;
                            throw new Exception(); //Remove this after debug
                            //return false;

                        }
                        else
                        {
                            changeState(State.impossibleToAbort); //FIXME não é atomico parte2; é facil de resolver mas tb é preciso de ter azar!!
                            foreach (KeyValuePair<int, PadInt> pair in this.transaction.getPoolPadInt())
                            {
                                if (!pair.Value.commitVaule(getTransactionWrapperID(), slave))
                                {
                                    Console.WriteLine("########################### throw new TxException();");
                                    throw new TxException("TxCommitAUX->commitVaule Fail (possivel inconcistencia)");
                                }
                            }
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
                changeState(State.Abort);
                Console.WriteLine("### I will abort " + getTransactionWrapperID());
                if (readState() == State.Abort)
                    return true;
                else
                    throw new Exception();
            }
            else return false;

        }


        private void changeState(State status) {
           
            lock (lockState)
            {
                if (status == State.impossibleToAbort && state == State.Abort)
                    throw new TxException("ChangeState, ImpossibleToAbort when it was to Abort");
                state=status;
            }
        }

        private State readState() {

            State aux;
            lock (lockState) {

                aux = state;
            }

            return aux;
        }
        //guarda os objectos acedidos. aka todos

        //begin aka construtor
        //abort
        //commit




    }

}
