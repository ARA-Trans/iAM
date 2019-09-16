using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using DatabaseManager;

namespace RoadCare3
{

	public class FormDynamicProperty : Form
	{
		private System.Windows.Forms.PropertyGrid pgDynamic;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkReadOnly;
		private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNameToRemove;

        private List<TableParameters> m_listTP = new List<TableParameters>();
        private List<TableParameters> m_listTPHistory = new List<TableParameters>();
		private List<TableParameters> m_listActivityTable = new List<TableParameters>();
		private List<TableParameters> m_listActivityChangelogTable = new List<TableParameters>();

        CustomClass myProperties = new CustomClass();
        private ComboBox cbPropertyType;
        private Button btnOk;
        private Button btnCancel;
		

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormDynamicProperty()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
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
            this.pgDynamic = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbPropertyType = new System.Windows.Forms.ComboBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.chkReadOnly = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtNameToRemove = new System.Windows.Forms.TextBox();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgDynamic
            // 
            this.pgDynamic.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pgDynamic.Location = new System.Drawing.Point(226, -2);
            this.pgDynamic.Name = "pgDynamic";
            this.pgDynamic.Size = new System.Drawing.Size(178, 344);
            this.pgDynamic.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbPropertyType);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.cmdAdd);
            this.groupBox1.Controls.Add(this.chkReadOnly);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 160);
            this.groupBox1.TabIndex = 2;
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
            this.cbPropertyType.Location = new System.Drawing.Point(64, 58);
            this.cbPropertyType.Name = "cbPropertyType";
            this.cbPropertyType.Size = new System.Drawing.Size(136, 21);
            this.cbPropertyType.TabIndex = 5;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(64, 28);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(136, 20);
            this.txtName.TabIndex = 4;
            // 
            // cmdAdd
            // 
            this.cmdAdd.Location = new System.Drawing.Point(64, 128);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(96, 24);
            this.cmdAdd.TabIndex = 3;
            this.cmdAdd.Text = "Add";
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // chkReadOnly
            // 
            this.chkReadOnly.Location = new System.Drawing.Point(8, 96);
            this.chkReadOnly.Name = "chkReadOnly";
            this.chkReadOnly.Size = new System.Drawing.Size(120, 16);
            this.chkReadOnly.TabIndex = 2;
            this.chkReadOnly.Text = "ReadOnly";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 64);
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
            this.groupBox2.Controls.Add(this.txtNameToRemove);
            this.groupBox2.Controls.Add(this.cmdRemove);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 174);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 96);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remove Property";
            // 
            // txtNameToRemove
            // 
            this.txtNameToRemove.Location = new System.Drawing.Point(64, 24);
            this.txtNameToRemove.Name = "txtNameToRemove";
            this.txtNameToRemove.Size = new System.Drawing.Size(136, 20);
            this.txtNameToRemove.TabIndex = 4;
            // 
            // cmdRemove
            // 
            this.cmdRemove.Location = new System.Drawing.Point(64, 64);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(96, 24);
            this.cmdRemove.TabIndex = 3;
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
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(12, 285);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(86, 57);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(129, 285);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 57);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormDynamicProperty
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(410, 349);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pgDynamic);
            this.Name = "FormDynamicProperty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Dynamic Property";
            this.Load += new System.EventHandler(this.FormDynamicProperty_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void cmdAdd_Click(object sender, System.EventArgs e)
		{
			CustomProperty myProp = new CustomProperty(txtName.Text,cbPropertyType.Text,chkReadOnly.Checked,true);
			myProperties.Add(myProp); 
			pgDynamic.Refresh();
		}

		private void cmdRemove_Click(object sender, System.EventArgs e)
		{
            if (txtNameToRemove.Text != "Name")
            {
                myProperties.Remove(txtNameToRemove.Text);
                pgDynamic.Refresh();
            }
		}

        private void FormDynamicProperty_Load(object sender, System.EventArgs e)
		{
			pgDynamic.SelectedObject = myProperties;

            // Add Name as a required field for every material.
            CustomProperty myProp;
            myProp = new CustomProperty("ASSET", "", false, true);
            
            myProperties.Add(myProp);
            pgDynamic.Refresh();
		}

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (myProperties[0].Value.ToString() != "")
            {
                String strProperty;
                String strDataType;

                this.DialogResult = DialogResult.OK;

                // Now add several default rows to the asset being created in the database as table "ASSET NAME"
                m_listTP.Add(new TableParameters("ID", DataType.Int, false, true, true));
				m_listTP.Add(new TableParameters("ROUTE", DataType.VarChar(-1), true, false, false));
                m_listTP.Add(new TableParameters("BEGIN_STATION", DataType.Float, true, false, false));
                m_listTP.Add(new TableParameters("END_STATION", DataType.Float, true, false, false));
                m_listTP.Add(new TableParameters("DIRECTION", DataType.VarChar(50), true, false, false));
				m_listTP.Add(new TableParameters("FACILITY", DataType.VarChar(-1), true, false, false));
				m_listTP.Add(new TableParameters("SECTION", DataType.VarChar(-1), true, false, false));
                m_listTP.Add(new TableParameters("ENTRY_DATE", DataType.DateTime, true, false, false));
                m_listTP.Add(new TableParameters("LATITUDE", DataType.Float, true, false, false));
                m_listTP.Add(new TableParameters("LONGITUDE", DataType.Float, true, false, false));
				m_listTP.Add(new TableParameters("GEOMETRY", DataType.VarChar(-1), true, false, false));

                m_listTPHistory.Add(new TableParameters("ID", DataType.Int, false, true, true));
                m_listTPHistory.Add(new TableParameters("ATTRIBUTE_ID", DataType.Int, false, false, false));
                m_listTPHistory.Add(new TableParameters("FIELD", DataType.VarChar(50), false, false, false));
                m_listTPHistory.Add(new TableParameters("VALUE", DataType.VarChar(50), true, false, false));
                m_listTPHistory.Add(new TableParameters("USER_ID", DataType.VarChar(200), true, false, false));
				m_listTPHistory.Add(new TableParameters("WORKACTIVITY", DataType.VarChar(-1), true, false, false));
				m_listTPHistory.Add(new TableParameters("WORKACTIVITY_ID", DataType.VarChar(-1), true, false, false));
                m_listTPHistory.Add(new TableParameters("DATE_MODIFIED", DataType.DateTime, false, false, false));

                for (int i = 1; i < myProperties.Count; i++)
                {
                    strProperty = myProperties[i].Name;
                    strDataType = myProperties[i].Value.ToString();

                    DataType dataTypeToInsert = Global.ConvertStringToDataType(strDataType);

                    // Create the table params list to pass into the DBMgr.CreateTable function.
                    m_listTP.Add(new TableParameters(strProperty, dataTypeToInsert, false, false));
                }
                String strInsert;

                // Now create the row in the ASSETS tables.
                strInsert = "Insert INTO ASSETS (ASSET, DATE_CREATED, CREATOR_ID, LAST_MODIFIED) " +
                                       "Values ('" + myProperties[0].Value
                                       + "', '" + DateTime.Now
                                       + "', '" + DBMgr.NativeConnectionParameters.UserName
                                       + "', '" + DateTime.Now
                                       + "')";
                try
                {
                    DBMgr.ExecuteNonQuery(strInsert);
                }
                catch (Exception sqlE)
                {
                    Global.WriteOutput("Error: Insert new asset failed. " + sqlE.Message);
                    return;
                }

                // Now create the table in the database <asset name>
                try
                {
                    DBMgr.CreateTable(myProperties[0].Value.ToString(), m_listTP);
                }
                catch (FailedOperationException sqlE)
                {
                    Global.WriteOutput("Error: Create asset data table failed. " + sqlE.Message);
                    return;
                }

                // Now create the <asset name>_CHANGELOG table for this asset in the database.
                try
                {
                    DBMgr.CreateTable(myProperties[0].Value.ToString() + "_" + "CHANGELOG", m_listTPHistory);
                }
                catch (FailedOperationException sqlE)
                {
                    Global.WriteOutput("Error: Create asset Changelog failed. " + sqlE.Message);
                    return;
                }
            }
        }
        
        public String GetDynamicPropertyName()
        {
            return myProperties[0].Value.ToString();
        }

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
    }
}
