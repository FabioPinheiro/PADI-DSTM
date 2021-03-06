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
using PADI_DSTM;
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
           PadiDstm.Init();
           this.richTextBox1.Text += "HI";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            PadiDstm.TxBegin();
            this.richTextBox1.Text += "HI Again";


        }

        private void button4_Click(object sender, EventArgs e)
        {
            //create
            String s = textBox1.Text;
            int aux = Convert.ToInt32(s);
            PadiDstm.CreatePadInt(aux);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String s = textBox1.Text;
            int aux = Convert.ToInt32(s);
            PadInt coisa = PadiDstm.AccessPadInt(aux);
             this.richTextBox1.Text += "\r\n" + coisa.toString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String Suid = textBox1.Text;
            int uid = Convert.ToInt32(Suid);
            String Svalue = textBox2.Text;
            int value = Convert.ToInt32(Svalue);
            PadInt coisa = PadiDstm.AccessPadInt(uid);
            coisa.Write(value);


        }
    }
}
