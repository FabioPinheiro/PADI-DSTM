using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class URL : MarshalByRefObject
    {
        private string username;
        private string port;

        public string MetodoOla()
        {
            return "ola!";
        }

        public void register(String name, String location)
        {
            username = name;
            port = location;
        }
        public string getNick()
        {
            return username;
        }
        public string getPort()
        {
            return port;
        }
    }



}
