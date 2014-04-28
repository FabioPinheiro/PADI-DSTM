using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class Class1
    {
    }

    public class PADI_DSTM
    {
        bool Init() /* this method is called only once by the application and initializes the PADI-DSTM*/
        {

            return true;
        }
        //bool TxBegin();

        //bool TxCommit();

        //bool TxAbort();

        //bool Status();

        //bool Fail(string URL);      /*this method makes the server at the URL stop responding to external calls except for a Recover call (see below).*/

        //bool Freeze(string URL);

        //bool Recover(string URL);   /*this method makes the server at URL recover from a previous Fail or Freeze call. */

    }
}
