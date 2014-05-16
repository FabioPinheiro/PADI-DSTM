using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PADI_DSTM;


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
        public bool fail()
        {
            return slave.fail();
        }
        public bool setVaule(int uid, int value, String newVersion, String oldVersion)
        {
            Console.WriteLine("SET VALUE!! " + value + "  version  " + newVersion);
            return slave.setVaule(uid, value, newVersion, oldVersion);
        }
        public bool unlockPadInt(int uid, String lockby)
        {
            return slave.unlockPadInt(uid,lockby);
        }
        public bool lockPadInt(int uid, String lockby)
        {
            return slave.lockPadInt(uid, lockby);
        }
        public String accessPadiIntVersion(int uid) {
            return slave.accessPadiIntVersion(uid);
        }
        public bool CommitTransaction(Transaction t) {
            Console.WriteLine("slave.CommitTransaction(t) SERVICES");
            return slave.CommitTransaction(t);
        }
        public int getSlaveId()
        {
            return slave.getSlaveId();
        }
        public void slaveIsDead(int slaveId) {
            slave.slaveIsDead(slaveId);
        }
        public void reorganizeGrid() {
            slave.reorganizeGrid();
        }
        public void modifyHistory(SortedList<int, SortedList<int, PadIntStored>> myResponsability, List<TransactionWrapper> transacções_state, int newSlaveId)
        {
            Console.WriteLine(" Modifying the History!!!");
            slave.modifyHistory(myResponsability,transacções_state, newSlaveId);
        }
        public void mergePassive(SortedList<int, SortedList<int, PadIntStored>> auxPadInts, List<TransactionWrapper> finish_transactions)
        {
            slave.mergePassive(auxPadInts, finish_transactions);
        }

        public bool createInReplica(PadIntStored padInt, int hash, bool newhash) {
            return slave.createInReplica(padInt, hash, newhash);
        }
        public bool lockInReplica(int uid, String lockby)
        {
            return slave.lockInReplica(uid, lockby);
        }
        public bool unlockInReplica(int uid, String lockby)
        {
            return slave.unlockInReplica(uid, lockby);
        }
        public bool commitInReplica(TransactionWrapper t) {
            return slave.commitInReplica(t);
        }
        public bool setValueInReplica(int uid, int value, String newVersion, String oldVersion)
        {
            return slave.setValueInReplica(uid, value, newVersion, oldVersion);
        }
        public void addTransaction(TransactionWrapper newTx) {

            slave.addTransaction(newTx);
        }
        public TransactionWrapper findTransaction(int port, long counter) {

            return slave.findTransaction(port, counter);
        }
        public int getReplic() {
            return slave.getReplic();
        }
        public long updateCounter() {
            return slave.updateCounter();
        }
        public long updateCounterInReplica()
        {
            return slave.updateCounterInReplica();
        }

        public String accessPadiIntVersionInReplica(int uid) {
            return slave.accessPadiIntVersionInReplica(uid);
        }
        public PadIntStored createPadIntInReplica(int uid) {
            return slave.createPadIntInReplica(uid);
        }
        public PadIntStored acessPadIntInReplica(int uid) {
            return slave.acessPadIntInReplica(uid);
        }
        public int whereIsPadInt(int uid) {
            return slave.whereIsPadInt(uid);
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
        public bool fail(String url) {
            return master.fail(url);
        }
        public bool status()
        {
            return master.status();
        }
        public bool ping(int slaveId) {
            return master.ping(slaveId);
        }

        public int whereIsMyReplica(int slaveId){
            return master.whereIsMyReplica(slaveId);
        }
        public int whichReplicaDoIHave(int slaveId){
            return master.whichReplicaDoIHave(slaveId);
        }
        public bool updateHash(int hash, int slaveId) {

            return master.updateHash(hash, slaveId);
        }
        public void slaveIsDead(int slaveId) {
            master.slaveIsDead(slaveId);
        }



    }
}
