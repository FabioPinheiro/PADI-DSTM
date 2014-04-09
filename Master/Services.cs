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
        public PadInt createPadInt(int uid)
        {
            return slave.createPadInt(uid);
        }
        public PadInt accessPadInt(int uid)
        {

            return slave.accessPadInt(uid);
        }
        public PadInt getPadInt(int uid)
        {
            //check this
            if (hasPadInt(uid))
            {
                return slave.accessPadInt(uid);
            }
            return getExternalPadInt(uid);
        }
        private bool hasPadInt(int uid)
        {
            return true; //correct this
        }
        private PadInt getExternalPadInt(int uid)
        {
            return new PadInt(uid);
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

        public PadInt createPadInt(int uid)
        {
            return master.createPadInt(uid);
        }
        public PadInt accessPadInt(int uid)
        {
            return new PadInt(uid);
        }

        public PadInt getPadInt(int uid)
        {
            return new PadInt(uid);
        }
        public PadInt getExternalPadInt(int uid)
        {
            return master.getExternalPadInt(uid);
        }

    }
}
