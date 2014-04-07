namespace Master
{
    partial class MasterForm
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
            this.textBox_log = new System.Windows.Forms.RichTextBox();
            this.souOMester = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_log
            // 
            this.textBox_log.Location = new System.Drawing.Point(12, 12);
            this.textBox_log.Name = "textBox_log";
            this.textBox_log.Size = new System.Drawing.Size(159, 237);
            this.textBox_log.TabIndex = 0;
            this.textBox_log.Text = "LOG:";
            this.textBox_log.TextChanged += new System.EventHandler(this.textBox_log_TextChanged);
            // 
            // souOMester
            // 
            this.souOMester.Location = new System.Drawing.Point(177, 12);
            this.souOMester.Name = "souOMester";
            this.souOMester.Size = new System.Drawing.Size(75, 23);
            this.souOMester.TabIndex = 1;
            this.souOMester.Text = "SouOMester!!";
            this.souOMester.UseVisualStyleBackColor = true;
            this.souOMester.Click += new System.EventHandler(this.button1_Click);
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.souOMester);
            this.Controls.Add(this.textBox_log);
            this.Name = "MasterForm";
            this.Text = "Master";
            this.Load += new System.EventHandler(this.MasterForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBox_log;
        private System.Windows.Forms.Button souOMester;
    }
}

