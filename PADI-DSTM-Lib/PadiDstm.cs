﻿using System;
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
        private IMasterService master;
        private ISlaveService slave; // some slave
        public static bool Init()
        { //so é feito uma vez aka por o Master up
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");
            if (obj == null)
                return false;
            //System.Console.WriteLine("Could not locate server");
            else
            {
                port = obj.getSlave();
                return true;

            }
        }
        public static bool TxBegin()
        { //Liga-se ao slave e começa uma transacçºao. falta começar uma transacção.
            TcpChannel channel = new TcpChannel();
            ISlaveService obj = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:8088/MyRemoteObjectName");
            if (obj == null)
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


        PadInt CreatePadInt(int uid)
        {

            master.createPadInt(uid); //change to slave and number of args
            return new PadInt(uid);
        }

        PadInt AccessPadInt(int uid)
        {
            slave.getPadInt(uid);
            return new PadInt(uid);
        }
    }


        public interface IMasterService
        {
            int register();
            string MetodoOla();
            int getSlave();
            bool createPadInt(int uid);
            bool accessPadInt(int uid);
            bool getPadInt(int uid);
        }
        public interface ISlaveService
        {
            string MetodoOlaClient();
            void createPadInt();
            bool accessPadInt(int uid);
            bool getPadInt(int uid);

        }

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
