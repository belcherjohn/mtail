using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using Monitor;
using mtail;
using System.Diagnostics;

namespace mtail
{
    /// <summary>
    /// Summary description for ChildForm.
    /// </summary>
    public class ChildForm : System.Windows.Forms.Form
    {
        private Monitor.Monitor mon = null;
        private System.Windows.Forms.RichTextBox richTextBox;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ChildForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public void MonitorFile(string FileName)
        {
            StopMonitor();

            richTextBox.Clear();
            this.Refresh();
            this.Text = FileName;

            try
            {
                mon = new Monitor.Monitor(FileName);
                mon.OnTextAppended += new MonitorEventHandler(mon_OnTextAppended);
                mon.StartMonitoring();
            }
            catch (Exception _e)
            {
                richTextBox.ResetText();
                richTextBox.AppendText(_e.ToString());
                StopMonitor();
            }
        }

        public void StopMonitor()
        {
            if (mon != null)
            {
                mon.StopMonitoring();
                mon.OnTextAppended -= mon_OnTextAppended;
                mon.Dispose();
                mon = null;
            }
        }

        private delegate void InvokeAppend();
        private void mon_OnTextAppended(object sender, MonitorEventArgs e)
        {
            if (richTextBox.InvokeRequired)
                richTextBox.Invoke(new InvokeAppend(() => mon_OnTextAppended(e.Text)));
            else
                mon_OnTextAppended(e.Text);
        }
        private void mon_OnTextAppended(string text)
        {
            richTextBox.AppendText(text);
            richTextBox.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopMonitor();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mon != null)
                    mon.Dispose();

                if (components != null)
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
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(216, 289);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            this.richTextBox.WordWrap = false;
            // 
            // ChildForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(216, 289);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.richTextBox});
            this.Name = "ChildForm";
            this.Text = "ChildForm";
            this.ResumeLayout(false);

        }
        #endregion

    }  //class ChildForm
}  // namespace mtail
