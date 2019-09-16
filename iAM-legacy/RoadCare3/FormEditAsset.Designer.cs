namespace RoadCare3
{
	partial class FormEditAsset
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditAsset));
			this.pgProperties = new PropertyGridEx.PropertyGridEx();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbPropertyType = new System.Windows.Forms.ComboBox();
			this.txtName = new System.Windows.Forms.TextBox();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.chkReadOnly = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cbRemoveProperty = new System.Windows.Forms.ComboBox();
			this.cmdRemove = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.cbIsNative = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// pgProperties
			// 
			this.pgProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			// 
			// 
			// 
			this.pgProperties.DocCommentDescription.AccessibleName = "";
			this.pgProperties.DocCommentDescription.AutoEllipsis = true;
			this.pgProperties.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
			this.pgProperties.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
			this.pgProperties.DocCommentDescription.Name = "";
			this.pgProperties.DocCommentDescription.Size = new System.Drawing.Size(260, 37);
			this.pgProperties.DocCommentDescription.TabIndex = 1;
			this.pgProperties.DocCommentImage = null;
			// 
			// 
			// 
			this.pgProperties.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
			this.pgProperties.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.pgProperties.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
			this.pgProperties.DocCommentTitle.Name = "";
			this.pgProperties.DocCommentTitle.Size = new System.Drawing.Size(260, 15);
			this.pgProperties.DocCommentTitle.TabIndex = 0;
			this.pgProperties.DocCommentTitle.Text = "FACILITY";
			this.pgProperties.DocCommentTitle.UseMnemonic = false;
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item1"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item2"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item3"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item4"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item5"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item6"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item7"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item8"))));
			this.pgProperties.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("pgProperties.Item9"))));
			this.pgProperties.Location = new System.Drawing.Point(226, 12);
			this.pgProperties.Name = "pgProperties";
			this.pgProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.pgProperties.SelectedObject = ((object)(resources.GetObject("pgProperties.SelectedObject")));
			this.pgProperties.ShowCustomProperties = true;
			this.pgProperties.Size = new System.Drawing.Size(266, 529);
			this.pgProperties.TabIndex = 1;
			// 
			// 
			// 
			this.pgProperties.ToolStrip.AccessibleName = "ToolBar";
			this.pgProperties.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
			this.pgProperties.ToolStrip.AllowMerge = false;
			this.pgProperties.ToolStrip.AutoSize = false;
			this.pgProperties.ToolStrip.CanOverflow = false;
			this.pgProperties.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.pgProperties.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.pgProperties.ToolStrip.Location = new System.Drawing.Point(0, 1);
			this.pgProperties.ToolStrip.Name = "";
			this.pgProperties.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
			this.pgProperties.ToolStrip.Size = new System.Drawing.Size(266, 25);
			this.pgProperties.ToolStrip.TabIndex = 1;
			this.pgProperties.ToolStrip.TabStop = true;
			this.pgProperties.ToolStrip.Text = "PropertyGridToolBar";
			this.pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProperties_PropertyValueChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbPropertyType);
			this.groupBox1.Controls.Add(this.txtName);
			this.groupBox1.Controls.Add(this.cmdAdd);
			this.groupBox1.Controls.Add(this.chkReadOnly);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(13, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(208, 134);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Add Property";
			// 
			// cbPropertyType
			// 
			this.cbPropertyType.FormattingEnabled = true;
			this.cbPropertyType.Items.AddRange(new object[] {
            "bigint",
            "binary(50)",
            "bit",
            "char(10)",
            "datetime",
            "decimal(18, 0)",
            "float",
            "image",
            "int",
            "money",
            "nchar(10)",
            "ntext",
            "numeric(18, 0)",
            "nvarchar(50)",
            "nvarchar(MAX)",
            "real",
            "smalldatetime",
            "smallint",
            "smallmoney",
            "text",
            "timestamp",
            "tinyint",
            "uniqueidentifier",
            "varbinary(50)",
            "varbinary(MAX)",
            "varchar(50)",
            "varchar(MAX)"});
			this.cbPropertyType.Location = new System.Drawing.Point(64, 76);
			this.cbPropertyType.Name = "cbPropertyType";
			this.cbPropertyType.Size = new System.Drawing.Size(136, 21);
			this.cbPropertyType.TabIndex = 4;
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(64, 28);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(136, 20);
			this.txtName.TabIndex = 2;
			// 
			// cmdAdd
			// 
			this.cmdAdd.Location = new System.Drawing.Point(64, 103);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Size = new System.Drawing.Size(96, 24);
			this.cmdAdd.TabIndex = 5;
			this.cmdAdd.Text = "Add";
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// chkReadOnly
			// 
			this.chkReadOnly.Location = new System.Drawing.Point(64, 54);
			this.chkReadOnly.Name = "chkReadOnly";
			this.chkReadOnly.Size = new System.Drawing.Size(120, 16);
			this.chkReadOnly.TabIndex = 3;
			this.chkReadOnly.Text = "ReadOnly";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 81);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Value";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Property";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cbRemoveProperty);
			this.groupBox2.Controls.Add(this.cmdRemove);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Location = new System.Drawing.Point(12, 152);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(208, 86);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Remove Property";
			// 
			// cbRemoveProperty
			// 
			this.cbRemoveProperty.FormattingEnabled = true;
			this.cbRemoveProperty.Location = new System.Drawing.Point(65, 24);
			this.cbRemoveProperty.Name = "cbRemoveProperty";
			this.cbRemoveProperty.Size = new System.Drawing.Size(136, 21);
			this.cbRemoveProperty.TabIndex = 6;
			// 
			// cmdRemove
			// 
			this.cmdRemove.Location = new System.Drawing.Point(64, 50);
			this.cmdRemove.Name = "cmdRemove";
			this.cmdRemove.Size = new System.Drawing.Size(96, 24);
			this.cmdRemove.TabIndex = 7;
			this.cmdRemove.Text = "Remove";
			this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 27);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "Name";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(129, 510);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(91, 31);
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(12, 510);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(86, 31);
			this.btnOk.TabIndex = 9;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// cbIsNative
			// 
			this.cbIsNative.AutoSize = true;
			this.cbIsNative.Checked = true;
			this.cbIsNative.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbIsNative.Location = new System.Drawing.Point(13, 244);
			this.cbIsNative.Name = "cbIsNative";
			this.cbIsNative.Size = new System.Drawing.Size(85, 17);
			this.cbIsNative.TabIndex = 8;
			this.cbIsNative.Text = "Native asset";
			this.cbIsNative.UseVisualStyleBackColor = true;
			this.cbIsNative.CheckedChanged += new System.EventHandler(this.cbIsNative_CheckedChanged);
			// 
			// FormEditAsset
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(494, 543);
			this.Controls.Add(this.cbIsNative);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pgProperties);
			this.Name = "FormEditAsset";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormAddNewAsset";
			this.Load += new System.EventHandler(this.FormEditAsset_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private PropertyGridEx.PropertyGridEx pgProperties;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cbPropertyType;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.CheckBox chkReadOnly;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.CheckBox cbIsNative;
		private System.Windows.Forms.ComboBox cbRemoveProperty;
	}
}