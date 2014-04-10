﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PADI_DSTM_Lib
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
            slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:"+port+"/MyRemoteObjectName");
            tx = new Transaction(port,DateTime.Now.ToString("s"),slave);
            if (slave == null)
                return false;
            else
            {
                return true;

            }
        }

        public static bool TxCommit()
        {
            return tx.TxCommit();
        }
        public static bool TxAbort() //SOMOS CONTRA O ABORTO!! PRO VIDA!!
        {
            return true;
        }
        public static bool Status()
        {
            //fala com o master
            return master.status(); ;
        }
        public static bool Fail(string URL)
        {
            return true;
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
            return tx.CreatePadInt(uid);
        }

        public static PadInt AccessPadInt(int uid)
        {
            return tx.AccessPadInt(uid);
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
        bool status();

    }
    public interface ISlaveService
    {
        string MetodoOlaClient();
        PadIntStored createPadInt(int uid);
        PadIntStored accessPadInt(int uid);
        bool setResponsability(int port, int hash);
        bool freeze();
        bool recover();
        bool status();
    }

    [Serializable] //FIXME passar por referencia
    public class PadIntStored
    {
        private String version = "none:0";
        private int id;
        private int value;
        private String lockby = "none";

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
        { //FIXME LOCK
            if (oldVersion == this.version &/*&&*/ lockby == newVersion)
            {
                version = newVersion;
                this.value = value;
                return true;
            }
            else return false;
        }
        public int getID() { return id; }
        public int getValue() { return value; }
        public String getVersion() { return version; }
        public String toString() { return ">ID=" + id + " valor=" + value + " version=" + version + " lockby=" + lockby + "; "; }
    }

    public class PadInt // só existe no salve
    { //read e write may throw TxException.

        private PadIntStored padInt;
        private String accessVersion;
        private bool readedAux = false;/*for client*/
        private bool writedAux = false;/*for client*/
        private int valueAux;/*for client*/

        public PadInt(PadIntStored padInt) {
            this.padInt = padInt;
            this.accessVersion = padInt.getVersion();
            this.valueAux = padInt.getValue();
        }

        public int Read()/*for client*/{ readedAux = true; return padInt.getValue(); }
        public void Write(int value)/*for client*/ { writedAux = true; this.valueAux = value; }
        public String toString() { return ">PadIntStored:" + padInt.toString() + " >PadInt: valueAux=" + valueAux + " readedAux=" + readedAux + " writedAux=" + writedAux + ";"; }

        public bool setLock(Transaction transaction) { return padInt.lockPadInt(transaction.getTransactionID()); } //FIXME
        public bool setUnlock(Transaction transaction)
        {
            if (padInt.unlockPadInt(transaction.getTransactionID()))
                return true;
            else throw new NotImplementedException(); //FIXME!!!!!!!!!!!!!!!!!!
        } //FIXME
        public bool commitVaule(Transaction t)
        {
            if (readedAux || writedAux)
                return padInt.setVaule(this.valueAux, t.getTransactionID(), this.accessVersion);
            else
                return true;//FIXME padInt o PadInt deve ver informado disto ou não? ()alterar a verção
        }

    }

    public class TxException : System.Exception
    {
        //TODO
    }

    public class Transaction
    {
        private String transactionID = null;
        private ISlaveService slave;
        private SortedList<int, PadInt> poolPadInt = new SortedList<int, PadInt>();
        private int Status = 0; // 1 - in commit!!;

        public String getTransactionID() { return this.transactionID; }

        public Transaction(int idServer, String timeStramp, ISlaveService slave) {
            transactionID = Convert.ToString(idServer) + ":" + timeStramp;
            this.slave = slave;
        }


        private bool lockAllPadInt()
        {
            //bool locking = true; //FIXME
            foreach (KeyValuePair<int, PadInt> pair in poolPadInt)
            {
                if (!pair.Value.setLock(this))
                    return false;
            }
            return true; // consegui fazer look a todo
        }
        private bool unlockAllPadInt()
        {
            //bool locking = true; //FIXME
            foreach (KeyValuePair<int, PadInt> pair in poolPadInt)
            {
                if (!pair.Value.setUnlock(this))
                    return false;
            }
            return true; // consegui fazer look a todo
        }
        public bool TxCommitAUX()//FIXME muitos problemas de consistencia
        {

            try
            {
                lockAllPadInt();

                foreach (KeyValuePair<int, PadInt> pair in poolPadInt)
                {
                    if (!pair.Value.commitVaule(this))
                        throw new TxException();
                }
            }
            finally { unlockAllPadInt(); }

            return true;
        }

        public bool TxCommit()
        {
            Task taskA = new Task(() => TxCommitAUX());
            //ThreadStart ts = new ThreadStart(this.TxCommitAUX()); 
            Task<bool>[] taskArray = { Task<bool>.Factory.StartNew(() => this.TxCommitAUX()) };

            Console.WriteLine("WAITNG !!!!");
            taskA.Wait();
            return taskArray[0].Result;
            //Task[] taskArray = new Task[poolPadInt.Count]; //SEE http://msdn.microsoft.com/en-us/library/dd537609(v=vs.110).aspx
        }
        private PadIntStored remotingAccessPadIntStored(int uid, bool toCreate)
        {
            if (toCreate)
                return slave.createPadInt(uid);
            else return slave.accessPadInt(uid);
        }
        public PadInt remotingAccessPadInt(int uid, bool toCreate)
        {
            //se toCreate == true
            //devolve null se já existir OU SE A VERSÂO != "none:0"; caso contrario devolve o PadInt
            //se toCreate == false
            //delvolve o PadInt se existir E se a versão  for diferente de "none:0"
            PadIntStored padIntStored = remotingAccessPadIntStored(uid, toCreate);
            if (padIntStored != null)
                return new PadInt(padIntStored);
            else return null;

            /*if (toCreate)
            {
                padIntStored = slave.createPadInt(uid); //FIXME se o servidor tiver morto isto devolve null .... ERROR
                if (padIntStored != null)
                    return padIntStored;
                else {
                    padIntStored = slave.accessPadInt(uid); //FIXME se o servidor tiver morto isto devolve null .... ERROR
                    if (padIntStored.getVersion() == "none:0")
                        return padIntStored;
                    else return null;
                }
            }
            else {
                padInt = slave.accessPadInt(uid); //FIXME se o servidor tiver morto isto devolve null .... ERROR
                if (padInt.getVersion() != "none:0")
                    return padInt;
                else return null;
            }*/
        }


        public PadInt CreatePadInt(int uid)
        {
            PadInt aux = remotingAccessPadInt(uid, true);
            if (aux != null)
            {
                poolPadInt.Add(uid, aux);
                return aux;
            }
            else return null; //!!Confirmado (Fabio: segundo o rafael)
        }

        public PadInt AccessPadInt(int uid)
        {
            if (poolPadInt.ContainsKey(uid))
                return poolPadInt[uid];
            else
            {
                PadInt aux = remotingAccessPadInt(uid, false);
                if (aux != null)
                {
                    poolPadInt.Add(uid, aux);
                    return aux;
                }
                else return null; //!!Confirmado (Fabio: segundo o rafael)
            }
        }
                   
        //guarda os objectos acedidos. aka todos

        //begin aka construtor
        //abort
        //commit




    }

}
