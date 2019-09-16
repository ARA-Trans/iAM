namespace RoadCare3
{
	partial class FormSecurityActionGroups
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnRemoveActionGroup = new System.Windows.Forms.Button();
			this.btnChangeActionGroup = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtActionGroupName = new System.Windows.Forms.TextBox();
			this.btnAddActionGroup = new System.Windows.Forms.Button();
			this.cblActions = new System.Windows.Forms.CheckedListBox();
			this.lstActionGroups = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// btnRemoveActionGroup
			// 
			this.btnRemoveActionGroup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnRemoveActionGroup.Location = new System.Drawing.Point( 712, 808 );
			this.btnRemoveActionGroup.Name = "btnRemoveActionGroup";
			this.btnRemoveActionGroup.Size = new System.Drawing.Size( 133, 20 );
			this.btnRemoveActionGroup.TabIndex = 54;
			this.btnRemoveActionGroup.Text = "Remove Actiongroup";
			this.btnRemoveActionGroup.UseVisualStyleBackColor = true;
			this.btnRemoveActionGroup.Click += new System.EventHandler( this.btnRemoveActionGroup_Click );
			// 
			// btnChangeActionGroup
			// 
			this.btnChangeActionGroup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnChangeActionGroup.Location = new System.Drawing.Point( 14, 808 );
			this.btnChangeActionGroup.Name = "btnChangeActionGroup";
			this.btnChangeActionGroup.Size = new System.Drawing.Size( 120, 20 );
			this.btnChangeActionGroup.TabIndex = 53;
			this.btnChangeActionGroup.Text = "Update Actiongroup";
			this.btnChangeActionGroup.UseVisualStyleBackColor = true;
			this.btnChangeActionGroup.Click += new System.EventHandler( this.btnChangeActionGroup_Click );
			// 
			// label1
			// 
			this.label1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 12, 785 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 67, 13 );
			this.label1.TabIndex = 52;
			this.label1.Text = "Group Name";
			// 
			// txtActionGroupName
			// 
			this.txtActionGroupName.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtActionGroupName.Location = new System.Drawing.Point( 85, 782 );
			this.txtActionGroupName.Name = "txtActionGroupName";
			this.txtActionGroupName.Size = new System.Drawing.Size( 129, 20 );
			this.txtActionGroupName.TabIndex = 50;
			// 
			// btnAddActionGroup
			// 
			this.btnAddActionGroup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnAddActionGroup.Location = new System.Drawing.Point( 186, 808 );
			this.btnAddActionGroup.Name = "btnAddActionGroup";
			this.btnAddActionGroup.Size = new System.Drawing.Size( 108, 20 );
			this.btnAddActionGroup.TabIndex = 51;
			this.btnAddActionGroup.Text = "Add Actiongroup";
			this.btnAddActionGroup.UseVisualStyleBackColor = true;
			this.btnAddActionGroup.Click += new System.EventHandler( this.btnAddActionGroup_Click );
			// 
			// cblActions
			// 
			this.cblActions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.cblActions.CheckOnClick = true;
			this.cblActions.FormattingEnabled = true;
			this.cblActions.Location = new System.Drawing.Point( 387, 12 );
			this.cblActions.Name = "cblActions";
			this.cblActions.Size = new System.Drawing.Size( 458, 754 );
			this.cblActions.TabIndex = 49;
			this.cblActions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler( this.cblActions_ItemCheck );
			// 
			// lstActionGroups
			// 
			this.lstActionGroups.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.lstActionGroups.FormattingEnabled = true;
			this.lstActionGroups.Location = new System.Drawing.Point( 12, 12 );
			this.lstActionGroups.Name = "lstActionGroups";
			this.lstActionGroups.Size = new System.Drawing.Size( 369, 758 );
			this.lstActionGroups.TabIndex = 48;
			this.lstActionGroups.SelectedIndexChanged += new System.EventHandler( this.lstActionGroups_SelectedIndexChanged );
			// 
			// FormSecurityActionGroups
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 857, 840 );
			this.Controls.Add( this.btnRemoveActionGroup );
			this.Controls.Add( this.btnChangeActionGroup );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.txtActionGroupName );
			this.Controls.Add( this.btnAddActionGroup );
			this.Controls.Add( this.cblActions );
			this.Controls.Add( this.lstActionGroups );
			this.Name = "FormSecurityActionGroups";
			this.TabText = "FormSecurityActionGroups";
			this.Text = "FormSecurityActionGroups";
			this.Load += new System.EventHandler( this.FormSecurityActionGroups_Load );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormSecurityActionGroups_FormClosed );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnRemoveActionGroup;
		private System.Windows.Forms.Button btnChangeActionGroup;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtActionGroupName;
		private System.Windows.Forms.Button btnAddActionGroup;
		private System.Windows.Forms.CheckedListBox cblActions;
		private System.Windows.Forms.ListBox lstActionGroups;
	}
}