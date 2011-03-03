namespace ContactManager
{
	partial class ContactDetails
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.theFirstNameTextBox = new System.Windows.Forms.TextBox();
			this.theLastNameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.theSaveButton = new System.Windows.Forms.Button();
			this.theDeleteButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.theEmailsTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.theEmailsTextBox);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.theLastNameTextBox);
			this.groupBox1.Controls.Add(this.theFirstNameTextBox);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(381, 168);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Main Info";
			// 
			// theFirstNameTextBox
			// 
			this.theFirstNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.theFirstNameTextBox.Location = new System.Drawing.Point(76, 19);
			this.theFirstNameTextBox.Name = "theFirstNameTextBox";
			this.theFirstNameTextBox.Size = new System.Drawing.Size(299, 20);
			this.theFirstNameTextBox.TabIndex = 0;
			// 
			// theLastNameTextBox
			// 
			this.theLastNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.theLastNameTextBox.Location = new System.Drawing.Point(76, 45);
			this.theLastNameTextBox.Name = "theLastNameTextBox";
			this.theLastNameTextBox.Size = new System.Drawing.Size(299, 20);
			this.theLastNameTextBox.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(63, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "First Name :";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Last Name :";
			// 
			// theSaveButton
			// 
			this.theSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.theSaveButton.Enabled = false;
			this.theSaveButton.Location = new System.Drawing.Point(309, 177);
			this.theSaveButton.Name = "theSaveButton";
			this.theSaveButton.Size = new System.Drawing.Size(75, 23);
			this.theSaveButton.TabIndex = 1;
			this.theSaveButton.Text = "Save";
			this.theSaveButton.UseVisualStyleBackColor = true;
			this.theSaveButton.Click += new System.EventHandler(this.theSaveButton_Click);
			// 
			// theDeleteButton
			// 
			this.theDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.theDeleteButton.Enabled = false;
			this.theDeleteButton.Location = new System.Drawing.Point(309-80, 177);
			this.theDeleteButton.Name = "theDeleteButton";
			this.theDeleteButton.Size = new System.Drawing.Size(75, 23);
			this.theDeleteButton.TabIndex = 1;
			this.theDeleteButton.Text = "Delete";
			this.theDeleteButton.UseVisualStyleBackColor = true;
			this.theDeleteButton.Click += new System.EventHandler(this.theDeleteButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Emails :";
			// 
			// theEmailsTextBox
			// 
			this.theEmailsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.theEmailsTextBox.Location = new System.Drawing.Point(76, 71);
			this.theEmailsTextBox.Multiline = true;
			this.theEmailsTextBox.Name = "theEmailsTextBox";
			this.theEmailsTextBox.Size = new System.Drawing.Size(299, 55);
			this.theEmailsTextBox.TabIndex = 5;
			// 
			// ContactDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.theSaveButton);
			this.Controls.Add(this.theDeleteButton);
			this.Controls.Add(this.groupBox1);
			this.Name = "ContactDetails";
			this.Size = new System.Drawing.Size(387, 203);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox theLastNameTextBox;
		private System.Windows.Forms.TextBox theFirstNameTextBox;
		private System.Windows.Forms.Button theSaveButton;
		private System.Windows.Forms.Button theDeleteButton;
		private System.Windows.Forms.TextBox theEmailsTextBox;
		private System.Windows.Forms.Label label3;
	}
}
