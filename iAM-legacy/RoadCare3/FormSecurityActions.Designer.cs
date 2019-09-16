namespace RoadCare3
{
	partial class FormSecurityActions
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
			this.btnRemoveAction = new System.Windows.Forms.Button();
			this.btnChangeAction = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtActionType = new System.Windows.Forms.TextBox();
			this.btnAddAction = new System.Windows.Forms.Button();
			this.lblDescription = new System.Windows.Forms.Label();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtNetwork = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtSimulation = new System.Windows.Forms.TextBox();
			this.lstActions = new System.Windows.Forms.ListBox();
			this.cblActionGroups = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// btnRemoveAction
			// 
			this.btnRemoveAction.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.btnRemoveAction.Location = new System.Drawing.Point( 702, 665 );
			this.btnRemoveAction.Name = "btnRemoveAction";
			this.btnRemoveAction.Size = new System.Drawing.Size( 110, 20 );
			this.btnRemoveAction.TabIndex = 41;
			this.btnRemoveAction.Text = "Remove Action";
			this.btnRemoveAction.UseVisualStyleBackColor = true;
			this.btnRemoveAction.Click += new System.EventHandler( this.btnRemoveAction_Click );
			// 
			// btnChangeAction
			// 
			this.btnChangeAction.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnChangeAction.Location = new System.Drawing.Point( 15, 665 );
			this.btnChangeAction.Name = "btnChangeAction";
			this.btnChangeAction.Size = new System.Drawing.Size( 101, 20 );
			this.btnChangeAction.TabIndex = 40;
			this.btnChangeAction.Text = "Update Action";
			this.btnChangeAction.UseVisualStyleBackColor = true;
			this.btnChangeAction.Click += new System.EventHandler( this.btnChangeAction_Click );
			// 
			// label1
			// 
			this.label1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 12, 564 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 31, 13 );
			this.label1.TabIndex = 39;
			this.label1.Text = "Type";
			// 
			// txtActionType
			// 
			this.txtActionType.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtActionType.Location = new System.Drawing.Point( 86, 561 );
			this.txtActionType.Name = "txtActionType";
			this.txtActionType.Size = new System.Drawing.Size( 208, 20 );
			this.txtActionType.TabIndex = 37;
			// 
			// btnAddAction
			// 
			this.btnAddAction.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnAddAction.Location = new System.Drawing.Point( 184, 665 );
			this.btnAddAction.Name = "btnAddAction";
			this.btnAddAction.Size = new System.Drawing.Size( 110, 20 );
			this.btnAddAction.TabIndex = 38;
			this.btnAddAction.Text = "Add Action";
			this.btnAddAction.UseVisualStyleBackColor = true;
			this.btnAddAction.Click += new System.EventHandler( this.btnAddAction_Click );
			// 
			// lblDescription
			// 
			this.lblDescription.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.lblDescription.AutoSize = true;
			this.lblDescription.Location = new System.Drawing.Point( 12, 590 );
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size( 60, 13 );
			this.lblDescription.TabIndex = 43;
			this.lblDescription.Text = "Description";
			// 
			// txtDescription
			// 
			this.txtDescription.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtDescription.Location = new System.Drawing.Point( 86, 587 );
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.Size = new System.Drawing.Size( 208, 20 );
			this.txtDescription.TabIndex = 42;
			// 
			// label3
			// 
			this.label3.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point( 12, 616 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 47, 13 );
			this.label3.TabIndex = 45;
			this.label3.Text = "Network";
			// 
			// txtNetwork
			// 
			this.txtNetwork.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtNetwork.Location = new System.Drawing.Point( 86, 613 );
			this.txtNetwork.Name = "txtNetwork";
			this.txtNetwork.Size = new System.Drawing.Size( 208, 20 );
			this.txtNetwork.TabIndex = 44;
			// 
			// label4
			// 
			this.label4.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point( 12, 642 );
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size( 55, 13 );
			this.label4.TabIndex = 47;
			this.label4.Text = "Simulation";
			// 
			// txtSimulation
			// 
			this.txtSimulation.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtSimulation.Location = new System.Drawing.Point( 86, 639 );
			this.txtSimulation.Name = "txtSimulation";
			this.txtSimulation.Size = new System.Drawing.Size( 208, 20 );
			this.txtSimulation.TabIndex = 46;
			// 
			// lstActions
			// 
			this.lstActions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.lstActions.FormattingEnabled = true;
			this.lstActions.Location = new System.Drawing.Point( 12, 12 );
			this.lstActions.Name = "lstActions";
			this.lstActions.Size = new System.Drawing.Size( 398, 511 );
			this.lstActions.TabIndex = 35;
			this.lstActions.SelectedIndexChanged += new System.EventHandler( this.lstActions_SelectedIndexChanged );
			// 
			// cblActionGroups
			// 
			this.cblActionGroups.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.cblActionGroups.CheckOnClick = true;
			this.cblActionGroups.FormattingEnabled = true;
			this.cblActionGroups.Location = new System.Drawing.Point( 417, 12 );
			this.cblActionGroups.Name = "cblActionGroups";
			this.cblActionGroups.Size = new System.Drawing.Size( 395, 514 );
			this.cblActionGroups.TabIndex = 36;
			this.cblActionGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler( this.cblActionGroups_ItemCheck );
			// 
			// FormSecurityActions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 824, 697 );
			this.Controls.Add( this.lstActions );
			this.Controls.Add( this.cblActionGroups );
			this.Controls.Add( this.label4 );
			this.Controls.Add( this.txtSimulation );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.txtNetwork );
			this.Controls.Add( this.lblDescription );
			this.Controls.Add( this.txtDescription );
			this.Controls.Add( this.btnRemoveAction );
			this.Controls.Add( this.btnChangeAction );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.txtActionType );
			this.Controls.Add( this.btnAddAction );
			this.Name = "FormSecurityActions";
			this.TabText = "SecurityAction";
			this.Text = "SecurityAction";
			this.Load += new System.EventHandler( this.FormSecurityActions_Load );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormSecurityAction_FormClosed );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnRemoveAction;
		private System.Windows.Forms.Button btnChangeAction;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtActionType;
		private System.Windows.Forms.Button btnAddAction;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtNetwork;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtSimulation;
		private System.Windows.Forms.ListBox lstActions;
		private System.Windows.Forms.CheckedListBox cblActionGroups;
	}
}