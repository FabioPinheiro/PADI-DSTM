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

            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);
            MasterServices ms = new MasterServices();
            RemotingServices.Marshal(ms, "MyRemoteObjectName", typeof(MasterServices));
            this.textBox_log.Text += "\r\n" + " MASTER Entering THE HOUSE";
            this.textBox_log.Text += "\r\n" + "MASTER LEAVING THE HOUSE";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void MasterForm_Load(object sender, EventArgs e)
        {

        }

    }
}
