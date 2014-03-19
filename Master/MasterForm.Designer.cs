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
            this.SuspendLayout();
            // 
            // textBox_log
            // 
            this.textBox_log.Location = new System.Drawing.Point(12, 12);
            this.textBox_log.Name = "textBox_log";
            this.textBox_log.Size = new System.Drawing.Size(260, 237);
            this.textBox_log.TabIndex = 0;
            this.textBox_log.Text = "LOG:";
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.textBox_log);
            this.Name = "MasterForm";
            this.Text = "Master";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBox_log;
    }
}

