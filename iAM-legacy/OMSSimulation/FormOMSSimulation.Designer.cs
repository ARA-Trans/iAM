namespace OMSSimulation
{
    partial class FormOMSSimulation
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSimulationDescription = new System.Windows.Forms.ComboBox();
            this.buttonRunSimulation = new System.Windows.Forms.Button();
            this.labelConnection = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBoxSimulation1 = new System.Windows.Forms.TextBox();
            this.tabControlOutput = new System.Windows.Forms.TabControl();
            this.contextMenuStripClose = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerSimulation = new System.Windows.Forms.Timer(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSectionID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxTreatment = new System.Windows.Forms.ComboBox();
            this.comboBoxAction = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxYear = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.tabPage1.SuspendLayout();
            this.tabControlOutput.SuspendLayout();
            this.contextMenuStripClose.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(741, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome to the OMS Simulation Test Platform.   By enterinng a connection string a" +
    "nd selecting a simulation it is possible to run and follow a OMS simulation.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Connection string:";
            // 
            // textBoxConnectionString
            // 
            this.textBoxConnectionString.Location = new System.Drawing.Point(113, 40);
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            this.textBoxConnectionString.Size = new System.Drawing.Size(745, 20);
            this.textBoxConnectionString.TabIndex = 2;
            this.textBoxConnectionString.Validated += new System.EventHandler(this.textBoxConnectionString_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Select simulation:";
            // 
            // comboBoxSimulationDescription
            // 
            this.comboBoxSimulationDescription.FormattingEnabled = true;
            this.comboBoxSimulationDescription.Location = new System.Drawing.Point(113, 78);
            this.comboBoxSimulationDescription.Name = "comboBoxSimulationDescription";
            this.comboBoxSimulationDescription.Size = new System.Drawing.Size(656, 21);
            this.comboBoxSimulationDescription.TabIndex = 4;
            // 
            // buttonRunSimulation
            // 
            this.buttonRunSimulation.Location = new System.Drawing.Point(775, 78);
            this.buttonRunSimulation.Name = "buttonRunSimulation";
            this.buttonRunSimulation.Size = new System.Drawing.Size(100, 23);
            this.buttonRunSimulation.TabIndex = 5;
            this.buttonRunSimulation.Text = "Run Simulation";
            this.buttonRunSimulation.UseVisualStyleBackColor = true;
            this.buttonRunSimulation.Click += new System.EventHandler(this.buttonRunSimulation_Click);
            // 
            // labelConnection
            // 
            this.labelConnection.AutoEllipsis = true;
            this.labelConnection.Location = new System.Drawing.Point(15, 62);
            this.labelConnection.Name = "labelConnection";
            this.labelConnection.Size = new System.Drawing.Size(840, 14);
            this.labelConnection.TabIndex = 6;
            this.labelConnection.Text = "labelConnection";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBoxSimulation1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(864, 328);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Simulation Output";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBoxSimulation1
            // 
            this.textBoxSimulation1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSimulation1.Location = new System.Drawing.Point(3, 3);
            this.textBoxSimulation1.Multiline = true;
            this.textBoxSimulation1.Name = "textBoxSimulation1";
            this.textBoxSimulation1.Size = new System.Drawing.Size(858, 322);
            this.textBoxSimulation1.TabIndex = 0;
            // 
            // tabControlOutput
            // 
            this.tabControlOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlOutput.ContextMenuStrip = this.contextMenuStripClose;
            this.tabControlOutput.Controls.Add(this.tabPage1);
            this.tabControlOutput.Location = new System.Drawing.Point(13, 134);
            this.tabControlOutput.Name = "tabControlOutput";
            this.tabControlOutput.SelectedIndex = 0;
            this.tabControlOutput.Size = new System.Drawing.Size(872, 354);
            this.tabControlOutput.TabIndex = 7;
            // 
            // contextMenuStripClose
            // 
            this.contextMenuStripClose.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.endThreadToolStripMenuItem});
            this.contextMenuStripClose.Name = "contextMenuStripClose";
            this.contextMenuStripClose.Size = new System.Drawing.Size(132, 48);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // endThreadToolStripMenuItem
            // 
            this.endThreadToolStripMenuItem.Name = "endThreadToolStripMenuItem";
            this.endThreadToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.endThreadToolStripMenuItem.Text = "End thread";
            // 
            // timerSimulation
            // 
            this.timerSimulation.Tick += new System.EventHandler(this.timerSimulation_Tick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "SectionID:";
            // 
            // textBoxSectionID
            // 
            this.textBoxSectionID.Location = new System.Drawing.Point(74, 104);
            this.textBoxSectionID.Name = "textBoxSectionID";
            this.textBoxSectionID.Size = new System.Drawing.Size(81, 20);
            this.textBoxSectionID.TabIndex = 9;
            this.textBoxSectionID.TextChanged += new System.EventHandler(this.textBoxSectionID_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(156, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Treatment:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(351, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Action:";
            // 
            // comboBoxTreatment
            // 
            this.comboBoxTreatment.FormattingEnabled = true;
            this.comboBoxTreatment.Location = new System.Drawing.Point(215, 105);
            this.comboBoxTreatment.Name = "comboBoxTreatment";
            this.comboBoxTreatment.Size = new System.Drawing.Size(132, 21);
            this.comboBoxTreatment.TabIndex = 13;
            // 
            // comboBoxAction
            // 
            this.comboBoxAction.FormattingEnabled = true;
            this.comboBoxAction.Items.AddRange(new object[] {
            "Delete",
            "Commit",
            "Add",
            "AddDoNotAllow",
            "Move",
            "DoNotAllow",
            "ChangeBudget"});
            this.comboBoxAction.Location = new System.Drawing.Point(391, 105);
            this.comboBoxAction.Name = "comboBoxAction";
            this.comboBoxAction.Size = new System.Drawing.Size(103, 21);
            this.comboBoxAction.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(500, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Year:";
            // 
            // comboBoxYear
            // 
            this.comboBoxYear.FormattingEnabled = true;
            this.comboBoxYear.Location = new System.Drawing.Point(534, 106);
            this.comboBoxYear.Name = "comboBoxYear";
            this.comboBoxYear.Size = new System.Drawing.Size(62, 21);
            this.comboBoxYear.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(602, 109);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Value:";
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(642, 106);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(100, 20);
            this.textBoxValue.TabIndex = 18;
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Location = new System.Drawing.Point(759, 105);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(116, 23);
            this.buttonUpdate.TabIndex = 19;
            this.buttonUpdate.Text = "Update Simulation";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // FormOMSSimulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 500);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxYear);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comboBoxAction);
            this.Controls.Add(this.comboBoxTreatment);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxSectionID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tabControlOutput);
            this.Controls.Add(this.labelConnection);
            this.Controls.Add(this.buttonRunSimulation);
            this.Controls.Add(this.comboBoxSimulationDescription);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxConnectionString);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormOMSSimulation";
            this.Text = "OMS Simulation";
            this.Load += new System.EventHandler(this.FormOMSSimulation_Load);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControlOutput.ResumeLayout(false);
            this.contextMenuStripClose.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxSimulationDescription;
        private System.Windows.Forms.Button buttonRunSimulation;
        private System.Windows.Forms.Label labelConnection;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBoxSimulation1;
        private System.Windows.Forms.TabControl tabControlOutput;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripClose;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endThreadToolStripMenuItem;
        private System.Windows.Forms.Timer timerSimulation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSectionID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxTreatment;
        private System.Windows.Forms.ComboBox comboBoxAction;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxYear;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxValue;
        private System.Windows.Forms.Button buttonUpdate;
    }
}

