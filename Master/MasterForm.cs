using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM_Lib;

namespace Master
{
    public partial class MasterForm : Form
    {
        
        public MasterForm()
        {
            InitializeComponent();

            System.Console.WriteLine();


            Master master = new Master();
            MasterServices ms = master.getMasterServices();
            RemotingServices.Marshal(ms, "MyRemoteObjectName", typeof(MasterServices));
            this.textBox_log.Text += "\r\n" + "MASTER IS ALIVE";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void MasterForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox_log_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
