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

namespace Client
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
            System.Console.WriteLine();
            TcpChannel channel = new TcpChannel();

            ChannelServices.RegisterChannel(channel, false);
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                obj.MetodoOla();
                System.Console.WriteLine(obj.getRegisted());
                this.richTextBox1.Text += "HI THERE";
                this.richTextBox1.Text += "\r\n "+  obj.getRegisted();
                this.richTextBox1.Text += "\r\n " + obj.MetodoOla();

            }
            
 
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            ISlaveService obj = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:8087/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                this.richTextBox1.Text += "HI Again";
                this.richTextBox1.Text += "\r\n " + obj.MetodoOlaClient();

            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
