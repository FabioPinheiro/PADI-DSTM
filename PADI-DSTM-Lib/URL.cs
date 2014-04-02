using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM_Lib
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
    public interface IMasterService
    {
        void register(String nick, String location);
        string MetodoOla();
        string getRegisted();
    }
    public interface ISlaveService
    {
        string MetodoOlaClient();
    }



}
