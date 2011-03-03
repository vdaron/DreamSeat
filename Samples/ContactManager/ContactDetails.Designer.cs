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
			this.theEmailsTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.theLastNameTextBox = new System.Windows.Forms.TextBox();
			this.theFirstNameTextBox = new System.Windows.Forms.TextBox();
			this.theSaveButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.theCreationDateLabel = new System.Windows.Forms.Label();
			this.theLastUpdateDateLabel = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.theLastUpdateDateLabel);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.theCreationDateLabel);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.theEmailsTextBox);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.theLastNameTextBox);
			this.groupBox1.Controls.Add(this.theFirstNameTextBox);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(381, 200);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Main Info";
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
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Emails :";
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(63, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "First Name :";
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
			// theFirstNameTextBox
			// 
			this.theFirstNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.theFirstNameTextBox.Location = new System.Drawing.Point(76, 19);
			this.theFirstNameTextBox.Name = "theFirstNameTextBox";
			this.theFirstNameTextBox.Size = new System.Drawing.Size(299, 20);
			this.theFirstNameTextBox.TabIndex = 0;
			// 
			// theSaveButton
			// 
			this.theSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.theSaveButton.Enabled = false;
			this.theSaveButton.Location = new System.Drawing.Point(309, 209);
			this.theSaveButton.Name = "theSaveButton";
			this.theSaveButton.Size = new System.Drawing.Size(75, 23);
			this.theSaveButton.TabIndex = 1;
			this.theSaveButton.Text = "Save";
			this.theSaveButton.UseVisualStyleBackColor = true;
			this.theSaveButton.Click += new System.EventHandler(this.theSaveButton_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 143);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Creation Date :";
			// 
			// theCreationDateLabel
			// 
			this.theCreationDateLabel.AutoSize = true;
			this.theCreationDateLabel.Location = new System.Drawing.Point(90, 143);
			this.theCreationDateLabel.Name = "theCreationDateLabel";
			this.theCreationDateLabel.Size = new System.Drawing.Size(63, 13);
			this.theCreationDateLabel.TabIndex = 7;
			this.theCreationDateLabel.Text = "Unspecified";
			// 
			// theLastUpdateDateLabel
			// 
			this.theLastUpdateDateLabel.AutoSize = true;
			this.theLastUpdateDateLabel.Location = new System.Drawing.Point(109, 167);
			this.theLastUpdateDateLabel.Name = "theLastUpdateDateLabel";
			this.theLastUpdateDateLabel.Size = new System.Drawing.Size(63, 13);
			this.theLastUpdateDateLabel.TabIndex = 9;
			this.theLastUpdateDateLabel.Text = "Unspecified";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 167);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(97, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "Last Update Date :";
			// 
			// ContactDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.theSaveButton);
			this.Controls.Add(this.groupBox1);
			this.Name = "ContactDetails";
			this.Size = new System.Drawing.Size(387, 235);
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
		private System.Windows.Forms.TextBox theEmailsTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label theCreationDateLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label theLastUpdateDateLabel;
		private System.Windows.Forms.Label label6;
	}
}
