using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM_Lib
{
    class PadiDstm
    {
        public bool Init()
        { //so é feito uma vez aka por o Master up
            return true;
        }
        public bool TxBegin()
        { //vai ao master e pede um slave, liga-se ao slave.
            return true;
        }
        public bool TxCommit()
        {
            return true;
        }
        public bool TxAbort()
        {
            return true;
        }
        public bool Status()
        {
            return true;
        }
        public bool Fail(string URL)
        {
            return true;
        }
        public bool Freeze(string URL)
        {
            return true;
        }
        public bool Recover(string URL)
        {
            return true;
        }

        PadInt CreatePadInt(int uid)
        {
            return new PadInt(uid);
        }

        PadInt AccessPadInt(int uid)
        {
            return new PadInt(uid);
        }
    }


        public interface IMasterService
        {
            int register();
            string MetodoOla();
            int getSlave();
        }
        public interface ISlaveService
        {
            string MetodoOlaClient();
            void createPadInt();

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

    
}
