using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM_Lib
{
    class PadiDstm
    {
        public bool Init() { //so é feito uma vez
            return true;
        }
        public bool TxBegin()
        {
            return true;
        }
        public bool TxCommit() { 
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
}
