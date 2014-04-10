using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PADI_DSTM_Lib;


namespace Master
{
    class SlaveServices : MarshalByRefObject, ISlaveService
    {
        Slave slave;
        public SlaveServices(Slave aux)
        {
            slave = aux;
        }
        public string MetodoOlaClient()
        {
            return "ola cliente :D";
        }
        public PadIntStored createPadInt(int uid)
        {
            return slave.createPadInt(uid);
        }
        public PadIntStored accessPadInt(int uid)
        {

            return slave.accessPadInt(uid);
        }

        private bool hasPadInt(int uid)
        {
            return true; //correct this
        }

        public bool setResponsability(int port, int hash)
        {
            return slave.setResponsability(port, hash);
        }

        public bool freeze()
        {
            return slave.freeze();
        }

        public bool recover()
        {
            return slave.recover();
        }
        public bool status()
        {
            return slave.status();
        }
    }
    public class MasterServices : MarshalByRefObject, IMasterService
    {
        String name;
        String url;
        Master master;

        public MasterServices(Master aux)
        {
            master = aux;
        }
        public MasterServices()
        {
        }

        public int register(String nick, String location)
        {
            name = nick;
            url = location;
            System.Console.WriteLine(nick + " " + location);
            return master.registSlave();
        }
        public int register()
        {
            return master.registSlave();
        }
        public string MetodoOla()
        {
            return "ola! sou o master";
        }

        public int getSlave()
        {
            return master.getSlave(); //TODO
        }

        public PadIntStored createPadInt(int uid)
        {
            return master.createPadInt(uid);
        }
        public PadIntStored accessPadInt(int uid)
        {
            return new PadIntStored(uid);
        }
        public bool setMine(int port, int hash)
        {
            return master.setMine(port, hash);
        }
        public bool freeze(String url)
        {
            return master.freeze(url);
        }

        public bool recover(String url)
        {
            return master.recover(url);
        }
        public bool status()
        {
            return master.status();
        }

    }
}
