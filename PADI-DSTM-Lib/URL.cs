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
    {
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
