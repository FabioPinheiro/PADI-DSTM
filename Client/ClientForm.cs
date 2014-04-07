﻿using System;
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
using System.Collections;

namespace Client
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();




            
 
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {


        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpChannel channel = new TcpChannel();
            int port;
            ChannelServices.RegisterChannel(channel, false);
            IMasterService obj = (IMasterService)Activator.GetObject(typeof(IMasterService), "tcp://localhost:8086/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                this.richTextBox1.Text += "HI THERE";
                this.richTextBox1.Text += "\r\n " + obj.getSlave();
                this.richTextBox1.Text += "\r\n " + obj.MetodoOla();
                port = obj.getSlave();

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            TcpChannel channel = new TcpChannel();
            ISlaveService obj = (ISlaveService)Activator.GetObject(typeof(ISlaveService), "tcp://localhost:8088/MyRemoteObjectName");
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            //System.Console.WriteLine("Could not locate server");
            else
            {
                this.richTextBox1.Text += "HI Again";
                this.richTextBox1.Text += "\r\n " + obj.MetodoOlaClient();

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
