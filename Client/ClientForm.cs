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
using System.Collections;

namespace Client
{
    public partial class ClientForm : Form
    {
        private int port= 0;
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

        }
    }
}
