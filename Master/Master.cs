using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM;
using System.Collections;


namespace Master
{


    public class Master
    {
        //CONSTANTES
        private const int MINE = 0;
        private const int MYPRIME = 101;
        private const int NONE = -1;
        private const int LIVE = 10;
        private const int FROZEN = 0;
        private const int DETH = -1;
        static int currentStatus;

        TcpChannel channel = new TcpChannel(8086);
        MasterServices ms;
        SortedList<int, int> slaves = new SortedList<int, int>(); //key port, value Live(1) or Dead(-1); o port identifica o slave.
        //SortedList<int, int> slavesMonitor = new SortedList<int, int>(); //key port, value 1 or 0. 1 had received the ping, 0 haven't received the ping
        Dictionary<int, int> slavesMonitor = new Dictionary<int, int>();
        SortedList<int, int> padIntsLocation = new SortedList<int, int>(); //key hash pie; value Port
        SortedList<int, SortedList<int, PadIntStored>> myResponsability = new SortedList<int, SortedList<int, PadIntStored>>(); //key rest: value: key port, value PadInt
        int port = 8087;
        int roundRobin = 0;
        int numberOfSlaves = 0;

        public Master()
        {
            ms = new MasterServices(this);
            ChannelServices.RegisterChannel(channel, false);
            currentStatus = LIVE;
        }
        public int registSlave()
        {
            int serverBefore = port - 1;
            lock (slaves)
            {
                numberOfSlaves++;
                slaves.Add(port, 1);
                lock (slavesMonitor)
                {
                    slavesMonitor.Add(port, 1);
                }

            }
            return port++;
        }

        public int getSlave()
        {
            Console.WriteLine("get slave");
            lock (slaves)
            {
                //System.Console.WriteLine(numberOfSlaves + " " + roundRobin + " resultado " + roundRobin % numberOfSlaves);
                if (numberOfSlaves == 0)
                    return 0;
                Console.WriteLine("Entrou no lock");
                int aux = slaves[8087 + (roundRobin++ % numberOfSlaves)];
                int i= 0;
                while (i < numberOfSlaves) {
                    Console.WriteLine("Ciclo  valor i: " + i +" aux value: " + aux+ "  aux e I: " + aux+i);
                    if (aux != -1) //check if it's alive :D
                    {
                        Console.WriteLine("o slave é: " + aux);
                        return aux;
                    }
                    else {
                        roundRobin++;
                        i++;
                    }
                }
                Console.WriteLine("No slaves found");
                return 0; //no slaves found
            }
        }
        public MasterServices getMasterServices()
        {
            return ms;
        }
        public PadIntStored createPadInt(int uid)
        {
            System.Console.WriteLine("Vamos escrever");
            PadIntStored aux = null;
            int location = whereIsPadInt(uid);
            System.Console.WriteLine("begin location " + location + "  " + uid);
            if (location == -1)
            { //Not assigned, get them!!
                System.Console.WriteLine("O Master cria: " + uid + " E fica com a parte " + hashUid(uid) + " da hash table");
                location = hashUid(uid);
                padIntsLocation[location] = port;
                myResponsability.Add(location, new SortedList<int, PadIntStored>());
                System.Console.WriteLine("location " + location + "key in new pie " + myResponsability.ContainsKey(location));
                aux = new PadIntStored(uid);
                myResponsability[location].Add(uid, aux);
                return aux;
            }
            else
            {
                if (hashPadInts(uid))
                {
                    //create here;
                    System.Console.WriteLine("O Master cria: " + uid + " E fica com a parte " + hashUid(uid) + " da hash table");
                    aux = new PadIntStored(uid);
                    myResponsability[location].Add(uid, aux);
                    return aux;
                }
                else
                {
                    System.Console.WriteLine("Cria noutro sitio");
                    createExternalPadInt(uid, getSlave());
                    //create aboard, create TCP connection and stuff!
                }
            }
            return aux;
        }
        public PadIntStored accessPadInt(int uid)
        {
            return new PadIntStored(uid);
        }
        public bool hashPadInts(int uid)
        {
            if (myResponsability.ContainsKey(hashUid(uid)))
                return true;
            return false;
        }
        public int whereIsPadInt(int uid)
        {
            int aux;
            int location = hashUid(uid);
            System.Console.WriteLine("location in Where is Pad Int " + location);
            System.Console.WriteLine("key " + myResponsability.ContainsKey(location));

            if (myResponsability.ContainsKey(location))
                return MINE;
            if (padIntsLocation.TryGetValue(location, out aux))
            { //need to connect to new server!
                return aux;
            }
            return NONE; // means that that type of uid%PrimeNumber does not exist at this moment! 
        }
        private PadIntStored createExternalPadInt(int uid, int location)
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
            PadIntStored aux = slave.createPadInt(uid);
            return aux;
        }
        private PadIntStored accessExternalPadInt(int uid, int location)
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + location + "/MyRemoteObjectName");
            PadIntStored aux = slave.accessPadInt(uid);
            return aux;
        }
        public bool setMine(int port, int hash)
        {
            //for a avisar todos os slaves que os numeros com a hash <hash> pertencem ao slave com o port <port>
            //slave.setResponsability(port, hash)
            /*foreach (KeyValuePair<int, int> kvp in slaves)
            {
                Console.WriteLine(kvp.Value);
                Console.WriteLine(kvp.Key);
            }*/

            //ve se está na lista de responsabilidades
            if (padIntsLocation.ContainsKey(hash))
            {
                return false;
            }
            else
            {
                //add a lista
                padIntsLocation.Add(hash, port);
                lock (slaves)
                {
                    //envia a info para todos: Melhorar se houver tempo
                    foreach (KeyValuePair<int, int> kvp in slaves)
                    {
                        Console.WriteLine("comunica com " + kvp.Key);
                        ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + kvp.Key + "/MyRemoteObjectName");
                        slave.setResponsability(port, hash);
                    }
                }
                return true;
            }
        }
        public bool freeze(String url)
        {
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), url);
            slaves.Remove(getPortFromUrl(url));
            slave.freeze();
            return true;
        }
        public bool recover(String url)
        {
            Console.WriteLine("Faz recover " + url);
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), url);
            slaves.Add(getPortFromUrl(url), getPortFromUrl(url));
            addToActives(getPortFromUrl(url));
            slave.recover();
            return true;
        }
        public bool fail(String url)
        {
            Console.WriteLine("Faz fail " + url);

            slaves.Remove(getPortFromUrl(url));
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), url);
            if (slave.fail())
            {
               removeFromActives(slave.getSlaveId());
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool status()
        {
            try
            {

                printStatus();
                //envia a info para todos: Melhorar se houver tempo
                foreach (KeyValuePair<int, int> kvp in slaves)
                {
                    ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + kvp.Key + "/MyRemoteObjectName");
                    slave.status();
                }
                return true;
            }
            catch (RemotingException e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        private String getStatus()
        {
            switch (currentStatus)
            {
                case LIVE:
                    return "LIVE";
                case FROZEN:
                    return "FROZEN";
                case DETH:
                    return "DETH";
                default:
                    return "DETH";
            }
        }
        private void printStatus()
        {
            System.Console.WriteLine("Master current status: " + getStatus());
            System.Console.WriteLine(slaves.Count() + " Registed slaves: ");
            lock (slaves)
            {
                foreach (KeyValuePair<int, int> kvp in slaves)
                {
                    Console.WriteLine(" - " + kvp.Key + " - Responsible for:");
                    foreach (KeyValuePair<int, int> locationPair in padIntsLocation)
                    {
                        if (locationPair.Value == kvp.Key)
                            Console.Write("      " + locationPair.Key + " - ");
                    }
                    Console.WriteLine();
                }
            }

        }
        private int hashUid(int uid)
        {
            return uid % MYPRIME;
        }

        private int getPortFromUrl(String url) {
            char[] delimiterChars = { ':', '/' };
            string[] words = url.Split(delimiterChars);
            foreach (string s in words)
            {
                System.Console.WriteLine(s);
            }
            return Convert.ToInt32(words[4]);
        }

        //monitor part
        public bool ping(int slaveId)
        {
            lock (slavesMonitor)
            {
               // Console.WriteLine("O " + slaveId + " I'm alive recebido");
                slavesMonitor[slaveId] = 1;
                return true;
            }
        }

        public void checkMonitor() {
            while(true){
                System.Threading.Thread.Sleep(1500);
                lock(slavesMonitor){
                    foreach (KeyValuePair<int, int> kvp in slavesMonitor.ToArray())
                    {
                       // Console.WriteLine("estou dentro do for");
                        if (kvp.Value == 1)
                        {
                            slavesMonitor[kvp.Key] = 0;
                           // Console.WriteLine(slavesMonitor[kvp.Key]);
                           // Console.WriteLine(kvp.Key + " IS alive :D :D");
                        }
                        else
                        {
                            //check if it's "kvp.Key" is dead
                            removeFromActives(kvp.Key);

                            //call Replication methods.
                            Console.WriteLine("Removed from Actives "+ kvp.Key);
                        }
                       // Console.WriteLine("sai");
                         
                    }
                }
                System.Threading.Thread.Sleep(2000);
            }
        }
        private void removeFromActives(int slaveId) {
            lock (slaves)
            {
                slaves[slaveId] = -1;
            }
            lock (slavesMonitor) {
                //Console.WriteLine("removendo o" + slaveId);
                slavesMonitor.Remove(slaveId);
            
            }
        }
        private void addToActives(int slaveId) {
            lock (slaves)
            {
                slaves[slaveId] = 1;
            }
            lock (slavesMonitor)
            {
                //Console.WriteLine("removendo o" + slaveId);
                slavesMonitor.Add(slaveId, 1);

            }
            
        }

        public void slaveIsDead(int slaveId) {

                //envia a info para todos: Melhorar se houver tempo
            int replication = whereIsMyReplica(slaveId);
            int hasreplic = whichReplicaDoIHave(slaveId);
            Console.WriteLine("Informa a " + replication + " que este " + slaveId + " está morto Para por como Activo");
            ISlaveService slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + replication + "/MyRemoteObjectName");
            slave.slaveIsDead(slaveId);
            Console.WriteLine("Informa a " + hasreplic + " que este " + slaveId + " está morto Para replicar noutro sitio");
            slave = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:" + hasreplic + "/MyRemoteObjectName");
            slave.slaveIsDead(slaveId);



        }


        //usado para criar a grid e para actualizar a grid.
        //Devolve o Id do slave que replica o slaveId
        public int whereIsMyReplica(int slaveId) {
            int replicId = 0;
            lock (slaves)
            {
                bool foundme = false;
                Console.WriteLine("procura a replica para " + slaveId);
                foreach (KeyValuePair<int, int> kvp in slaves)
                {
                    if (kvp.Key == slaveId)
                    {
                        foundme = true;
                        continue;
                    }
                    if (foundme)
                    {
                        if (kvp.Value == 1)
                        { //is alive
                            Console.WriteLine("A replica é o " + kvp.Key);

                            return kvp.Key;
                        }
                    }

                    
                }
                //segunda ronda, ja me encontrei (da a volta)
                //procura a replica da outra ponta da lista (do inicio)
                foreach (KeyValuePair<int, int> kvp in slaves)
                {
                    if (kvp.Value == 1)
                    { //is alive
                        Console.WriteLine("A replica é o " + kvp.Key);

                        return kvp.Key;
                    }
                }
                //NAO ENCONTREI NENHUM PANIC
                replicId = slaveId;

            }
            Console.WriteLine("A replica é o Proprio");
            return replicId;
        
        }
        public int whichReplicaDoIHave(int slaveId) {
            int replicId = 0;

            bool found = false;
            lock (slaves)
            {
                Console.WriteLine("procura quem é replica que " + slaveId + " tem.");   
                foreach (KeyValuePair<int, int> kvp in slaves)
                {
                    if (kvp.Value == 1)
                    { //is alive
                        Console.WriteLine("A replica que tem é o " + kvp.Key);
                        found = true;
                        replicId = kvp.Key;
                    }

                    if (kvp.Key == slaveId)
                    {
                        if (found)
                            return replicId;
                        else {
                            //a replica vai ser a ultima.
                            continue;
                        }
                    } 
                }

            }
            if (found)
                return replicId;
            //NAO ENCONTREI NENHUM PANIC
            replicId = slaveId;
            Console.WriteLine("A replica é o Master");
            return replicId;
        
        }        
    

        



    }

      




}
