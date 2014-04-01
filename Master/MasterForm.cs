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
        TcpChannel channel;
        public MasterForm()
        {
            InitializeComponent();

            System.Console.WriteLine("################################1");
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            System.Console.WriteLine("################################2");



        }

        private void button1_Click(object sender, EventArgs e)
        {
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");

            if (obj == null)
                this.textBox_log.Text += "\r\n" + "Could not locate server";
            //System.Console.WriteLine("Could not locate server");
            else
            {
                this.textBox_log.Text += "\r\n" +
                    obj.MetodoOla();
                obj.register("nick", "location");
            }
            //Console.WriteLine(obj.MetodoOla());
            System.Console.WriteLine("################################3");
        }

        private void MasterForm_Load(object sender, EventArgs e)
        {

        }

    }
}
