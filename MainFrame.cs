using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Monitor;
using System.Diagnostics;

namespace mtail
{

    /// <summary>
    /// Summary description for MainFrame.
    /// </summary>
    public class MainFrame : System.Windows.Forms.Form
    {
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Splitter splitter;
        private System.Windows.Forms.MenuItem newMonitor;
        private System.Windows.Forms.MenuItem exitMenu;
        private System.Windows.Forms.MenuItem menuItem;
        private IContainer components;

        public MainFrame()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            treeView.Nodes.Add("Monitoring");  // Give the tree a name
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem = new System.Windows.Forms.MenuItem();
            this.newMonitor = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.exitMenu = new System.Windows.Forms.MenuItem();
            this.treeView = new System.Windows.Forms.TreeView();
            this.splitter = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem});
            // 
            // menuItem
            // 
            this.menuItem.Index = 0;
            this.menuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.newMonitor,
            this.menuItem3,
            this.exitMenu});
            this.menuItem.Text = "File";
            // 
            // newMonitor
            // 
            this.newMonitor.Index = 0;
            this.newMonitor.Text = "New Monitor";
            this.newMonitor.Click += new System.EventHandler(this.OnClick_New);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "-";
            // 
            // exitMenu
            // 
            this.exitMenu.Index = 2;
            this.exitMenu.Text = "Exit";
            this.exitMenu.Click += new System.EventHandler(this.OnClick_Exit);
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(96, 553);
            this.treeView.TabIndex = 1;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeViewAfterSelect);
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point(96, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(3, 553);
            this.splitter.TabIndex = 2;
            this.splitter.TabStop = false;
            // 
            // MainFrame
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(424, 553);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.treeView);
            this.IsMdiContainer = true;
            this.Menu = this.mainMenu;
            this.Name = "MainFrame";
            this.Text = "mtail";
            this.ResumeLayout(false);

        }
        #endregion


        #region Event Handlers
        private void OnClick_New(object sender, System.EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Files For Monitoring";
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //openFileDialog.FileName = @"Build.log";
            openFileDialog.Filter = "Log files|*.log;*.txt|Text files|*.txt|All files|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    ChildForm fChild = new ChildForm();
                    fChild.MdiParent = this;
                    fChild.Show();
                    fChild.MonitorFile(filename);

                    var tn = new TreeNode(filename);
                    tn.Tag = fChild;
                    treeView.Nodes.Add(tn);
                    fChild.Tag = tn;
                    fChild.FormClosed += new FormClosedEventHandler(fChild_FormClosed);
                    fChild.TextChanged += new EventHandler(fChild_TextChanged);
                }
            }
        }

        void fChild_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var fChild = (ChildForm)sender;
                var tn = (TreeNode)fChild.Tag;
                treeView.Nodes.Remove(tn);
                tn.Text = fChild.Text;
                treeView.Nodes.Add(tn);
            }
            catch
            {
            }
        }

        private void fChild_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                var fChild = (ChildForm)sender;
                treeView.Nodes.Remove((TreeNode)fChild.Tag);
            }
            catch
            {
            }
        }

        private void OnTreeViewAfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            var form = e.Node.Tag as ChildForm;
            if (form == null)
                return;
            try
            {
                form.Activate();
            }
            catch (Exception _e)
            {
                System.Windows.Forms.MessageBox.Show(_e.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    treeView.Nodes.Remove(e.Node);
                }
                catch
                {
                }
            }
        }

        private void OnClick_Exit(object sender, System.EventArgs e)
        {
            this.Close();
        }


        #endregion Event Handlers

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] _args)
        {
            Application.Run(new MainFrame());
        }

    }  // class MainFrame
}  // namespace mtail
