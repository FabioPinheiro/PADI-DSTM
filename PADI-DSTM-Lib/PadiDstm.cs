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

            master.createPadInt(uid); //change to slave and number of args
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
            PadInt getPadInt(int uid);
            PadInt getExternalPadInt(int uid);
        }
        public interface ISlaveService
        {
            string MetodoOlaClient();
            PadInt createPadInt(int uid);
            PadInt accessPadInt(int uid);
            PadInt getPadInt(int uid);

        }
        [Serializable]
        public class PadInt
        { //read e write may throw TxException.
            private int value;
            private int id;
            public PadInt(int uid)
            {
                id = uid;
            }
            public int Read()
            {
                return value;
            }
            public void Write(int value)
            {
                this.value = value;
            }
            public String toString() {

                return "ID= "+ id + "valor= " + value;
            }

        }

        public class TxException : System.Exception
        {

        }
        public class Transaction
        {
            //guarda os objectos acedidos. aka todos
            
            //begin aka construtor
            //abort
            //commit
            //access
            //create

            


        }
 
}
