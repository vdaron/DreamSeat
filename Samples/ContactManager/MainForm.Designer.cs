namespace ContactManager
{
	partial class MainForm
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.compactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.theContactsListBox = new ContactManager.ContactsListBox();
			this.theContactDetails = new ContactManager.ContactDetails();
			this.theChangesListBox = new ContactManager.ChangesListBox();
			this.menuStrip1.SuspendLayout();
			//((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			//((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(574, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
			this.compactToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(66, 20);
			this.toolStripMenuItem1.Text = "Contacts";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newToolStripMenuItem.Text = "New...";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// compactToolStripMenuItem
			// 
			this.compactToolStripMenuItem.Name = "compactToolStripMenuItem";
			this.compactToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.compactToolStripMenuItem.Text = "Compact DB";
			this.compactToolStripMenuItem.Click += new System.EventHandler(this.compactToolStripMenuItem_Click);
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.theChangesListBox);
			this.splitContainer1.Size = new System.Drawing.Size(574, 332);
			this.splitContainer1.SplitterDistance = 235;
			this.splitContainer1.TabIndex = 2;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.theContactsListBox);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.theContactDetails);
			this.splitContainer2.Size = new System.Drawing.Size(574, 235);
			this.splitContainer2.SplitterDistance = 191;
			this.splitContainer2.TabIndex = 0;
			// 
			// theContactsListBox
			// 
			this.theContactsListBox.Database = null;
			this.theContactsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.theContactsListBox.FormattingEnabled = true;
			this.theContactsListBox.Items.AddRange(new object[] {
            "Loading...",
            "Loading...",
            "Loading..."});
			this.theContactsListBox.Location = new System.Drawing.Point(0, 0);
			this.theContactsListBox.Name = "theContactsListBox";
			this.theContactsListBox.Size = new System.Drawing.Size(191, 235);
			this.theContactsListBox.TabIndex = 0;
			this.theContactsListBox.SelectedContactChanged += new ContactManager.SelectedContactChangedDelegate(this.theContactsListBox_SelectedContactChanged);
			// 
			// theContactDetails
			// 
			this.theContactDetails.CurrentContact = null;
			this.theContactDetails.Database = null;
			this.theContactDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.theContactDetails.Location = new System.Drawing.Point(0, 0);
			this.theContactDetails.Name = "theContactDetails";
			this.theContactDetails.Size = new System.Drawing.Size(379, 235);
			this.theContactDetails.TabIndex = 0;
			// 
			// theChangesListBox
			// 
			this.theChangesListBox.Database = null;
			this.theChangesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.theChangesListBox.FormattingEnabled = true;
			this.theChangesListBox.Items.AddRange(new object[] {
            "Loading...",
            "Loading...",
            "Loading..."});
			this.theChangesListBox.Location = new System.Drawing.Point(0, 0);
			this.theChangesListBox.Name = "theChangesListBox";
			this.theChangesListBox.Size = new System.Drawing.Size(574, 93);
			this.theChangesListBox.TabIndex = 0;
			this.theChangesListBox.ContactChanged += new ContactManager.ContactChangedDelegate(this.theChangesListBox_ContactChanged);
			this.theChangesListBox.ContactDeleted += new ContactManager.ContactDeletedDelegate(this.theChangesListBox_ContactDeleted);

			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(574, 356);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "Contacts Manager";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
		//	((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
		//	((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private ContactsListBox theContactsListBox;
		private ContactDetails theContactDetails;
		private ChangesListBox theChangesListBox;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem compactToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;

	}
}

