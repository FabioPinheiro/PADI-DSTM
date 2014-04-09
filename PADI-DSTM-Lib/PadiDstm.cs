using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM_Lib
{

    public class PadiDstm
    {
        private static int port = 0;
        private static IMasterService master;
        private static ISlaveService slave; // some slave
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
            slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:8088/MyRemoteObjectName");
            if (slave == null)
                return false;
            else
            {
                return true;

            }
        }

        public static bool TxCommit()
        {
            return true;
        }
        public static bool TxAbort()
        {
            return true;
        }
        public static bool Status()
        {
            //fala com o master
            return true;
        }
        public static bool Fail(string URL)
        {
            return true;
        }
        public static bool Freeze(string URL)
        {
            return true;
        }
        public static bool Recover(string URL)
        {
            return true;
        }


        public static PadInt CreatePadInt(int uid)
        {

            //master.createPadInt(uid); //change to slave and number of args
            slave.createPadInt(uid);
            return new PadInt(uid);
        }

        public static PadInt AccessPadInt(int uid)
        {
            PadInt teste;
            teste = slave.accessPadInt(uid);
            return teste;
        }
    }


        public interface IMasterService
        {
            int register();
            string MetodoOla();
            int getSlave();
            PadInt createPadInt(int uid);
            PadInt accessPadInt(int uid);
        }
        public interface ISlaveService
        {
            string MetodoOlaClient();
            PadInt createPadInt(int uid);
            PadInt accessPadInt(int uid);
        }
        [Serializable]
        public class PadInt
        { //read e write may throw TxException.
            private String version = "none:0";
            private int value;
            private int id;
            private bool readedAux = false;/*for client*/
            private bool writedAux = false;/*for client*/
            private int valueAux;/*for client*/
            
            public bool setVaule(int value, String newVersion, String oldVersion){ //FIXME LOCK
                if (oldVersion == this.version)
                {
                    version = newVersion;
                    this.value = value;
                    return true;
                }
                else return false;
            }
            public int getID() { return id; }
            public PadInt(int uid)
            {
                id = uid;
            }
            public int Read()/*for client*/
            {
                if (writedAux == false)
                {
                    readedAux = true;
                    return value;
                }
                else return valueAux;
            }
            public void Write(int value)/*for client*/
            {
                writedAux = true;
                this.value = value;
            }
            public String toString() {
                //return "PadIntTransaction: value=" + value + " readed=" + readed + " writed=" + writed + " ; padInt:" + padInt.toString();
                return "ID= "+ id + "valor= " + value;
            }

        }

        public class TxException : System.Exception
        {
            //TODO
        }

        public class Transaction
        {
            public static PadInt remotingAccessPadInt(int uid, bool toCreate)
            {
                //TODO
                //se toCreate == true
                //devolve null se já existir OU SE A VERSÂO != "none:0"; caso contrario devolve o PadInt
                
                //se toCreate == false
                //delvolve o PadInt se existir E se a versão  for diferente de "none:0"
                return null;
            }
            private String id = null;
            private SortedList<int, PadInt> poolPadInt = new SortedList<int, PadInt>();

            Transaction(int idServer, int timeStramp)
            {
                id = Convert.ToString(idServer) + ":" + Convert.ToString(timeStramp);
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
                else {
                    PadInt aux = remotingAccessPadInt(uid,false);
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
