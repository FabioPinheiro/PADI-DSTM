namespace Client
{
    partial class FormClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonTxCommit = new System.Windows.Forms.Button();
            this.buttonTxAbort = new System.Windows.Forms.Button();
            this.buttonCreatePadInt = new System.Windows.Forms.Button();
            this.buttonAccessPadInt = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.labelUID = new System.Windows.Forms.Label();
            this.labelVaule = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // buttonTxCommit
            // 
            this.buttonTxCommit.Location = new System.Drawing.Point(12, 226);
            this.buttonTxCommit.Name = "buttonTxCommit";
            this.buttonTxCommit.Size = new System.Drawing.Size(75, 23);
            this.buttonTxCommit.TabIndex = 0;
            this.buttonTxCommit.Text = "TxCommit";
            this.buttonTxCommit.UseVisualStyleBackColor = true;
            this.buttonTxCommit.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonTxAbort
            // 
            this.buttonTxAbort.Location = new System.Drawing.Point(94, 226);
            this.buttonTxAbort.Name = "buttonTxAbort";
            this.buttonTxAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonTxAbort.TabIndex = 1;
            this.buttonTxAbort.Text = "TxAbort";
            this.buttonTxAbort.UseVisualStyleBackColor = true;
            // 
            // buttonCreatePadInt
            // 
            this.buttonCreatePadInt.Location = new System.Drawing.Point(12, 109);
            this.buttonCreatePadInt.Name = "buttonCreatePadInt";
            this.buttonCreatePadInt.Size = new System.Drawing.Size(100, 23);
            this.buttonCreatePadInt.TabIndex = 2;
            this.buttonCreatePadInt.Text = "CreatePadInt";
            this.buttonCreatePadInt.UseVisualStyleBackColor = true;
            // 
            // buttonAccessPadInt
            // 
            this.buttonAccessPadInt.Location = new System.Drawing.Point(118, 109);
            this.buttonAccessPadInt.Name = "buttonAccessPadInt";
            this.buttonAccessPadInt.Size = new System.Drawing.Size(100, 23);
            this.buttonAccessPadInt.TabIndex = 3;
            this.buttonAccessPadInt.Text = "AccessPadInt";
            this.buttonAccessPadInt.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 63);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(118, 63);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 5;
            // 
            // labelUID
            // 
            this.labelUID.AutoSize = true;
            this.labelUID.Location = new System.Drawing.Point(13, 44);
            this.labelUID.Name = "labelUID";
            this.labelUID.Size = new System.Drawing.Size(26, 13);
            this.labelUID.TabIndex = 6;
            this.labelUID.Text = "UID";
            this.labelUID.Click += new System.EventHandler(this.label1_Click);
            // 
            // labelVaule
            // 
            this.labelVaule.AutoSize = true;
            this.labelVaule.Location = new System.Drawing.Point(118, 44);
            this.labelVaule.Name = "labelVaule";
            this.labelVaule.Size = new System.Drawing.Size(34, 13);
            this.labelVaule.TabIndex = 7;
            this.labelVaule.Text = "Vaule";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FormClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.labelVaule);
            this.Controls.Add(this.labelUID);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonAccessPadInt);
            this.Controls.Add(this.buttonCreatePadInt);
            this.Controls.Add(this.buttonTxAbort);
            this.Controls.Add(this.buttonTxCommit);
            this.Name = "FormClient";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTxCommit;
        private System.Windows.Forms.Button buttonTxAbort;
        private System.Windows.Forms.Button buttonCreatePadInt;
        private System.Windows.Forms.Button buttonAccessPadInt;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label labelUID;
        private System.Windows.Forms.Label labelVaule;
        private System.Windows.Forms.ImageList imageList1;
    }
}

