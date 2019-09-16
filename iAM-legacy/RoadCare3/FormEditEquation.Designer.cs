namespace RoadCare3
{
    partial class FormEditEquation
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
            this.listBoxAttribute = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCompile = new System.Windows.Forms.TextBox();
            this.labelCompile = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dgvPerformance = new System.Windows.Forms.DataGridView();
            this.Age = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripPiecewiseEquationMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBoxFunctions = new System.Windows.Forms.ListBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.buttonPlus = new System.Windows.Forms.Button();
            this.buttonMinus = new System.Windows.Forms.Button();
            this.buttonMultiply = new System.Windows.Forms.Button();
            this.buttonDivide = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.dgvDefault = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxPiecewise = new System.Windows.Forms.CheckBox();
            this.checkBoxAsFunction = new System.Windows.Forms.CheckBox();
            this.richTextBoxEquation = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerformance)).BeginInit();
            this.contextMenuStripPiecewiseEquationMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefault)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxAttribute
            // 
            this.listBoxAttribute.FormattingEnabled = true;
            this.listBoxAttribute.Location = new System.Drawing.Point(12, 22);
            this.listBoxAttribute.Name = "listBoxAttribute";
            this.listBoxAttribute.Size = new System.Drawing.Size(240, 329);
            this.listBoxAttribute.TabIndex = 0;
            this.listBoxAttribute.SelectedIndexChanged += new System.EventHandler(this.listBoxAttribute_SelectedIndexChanged);
            this.listBoxAttribute.DoubleClick += new System.EventHandler(this.listBoxAttribute_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Attributes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 392);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Equation:";
            // 
            // textBoxCompile
            // 
            this.textBoxCompile.BackColor = System.Drawing.Color.White;
            this.textBoxCompile.Location = new System.Drawing.Point(12, 532);
            this.textBoxCompile.Multiline = true;
            this.textBoxCompile.Name = "textBoxCompile";
            this.textBoxCompile.ReadOnly = true;
            this.textBoxCompile.Size = new System.Drawing.Size(544, 90);
            this.textBoxCompile.TabIndex = 4;
            // 
            // labelCompile
            // 
            this.labelCompile.AutoSize = true;
            this.labelCompile.Location = new System.Drawing.Point(12, 518);
            this.labelCompile.Name = "labelCompile";
            this.labelCompile.Size = new System.Drawing.Size(124, 13);
            this.labelCompile.TabIndex = 5;
            this.labelCompile.Text = "Compile Output Window:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(335, 630);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCheck
            // 
            this.buttonCheck.Location = new System.Drawing.Point(245, 630);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(75, 23);
            this.buttonCheck.TabIndex = 7;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(425, 629);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // dgvPerformance
            // 
            this.dgvPerformance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerformance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Age,
            this.Value});
            this.dgvPerformance.ContextMenuStrip = this.contextMenuStripPiecewiseEquationMenu;
            this.dgvPerformance.Location = new System.Drawing.Point(563, 196);
            this.dgvPerformance.Name = "dgvPerformance";
            this.dgvPerformance.RowTemplate.Height = 24;
            this.dgvPerformance.Size = new System.Drawing.Size(242, 457);
            this.dgvPerformance.TabIndex = 9;
            // 
            // Age
            // 
            this.Age.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Age.HeaderText = "Age";
            this.Age.Name = "Age";
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // contextMenuStripPiecewiseEquationMenu
            // 
            this.contextMenuStripPiecewiseEquationMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pasteToolStripMenuItem});
            this.contextMenuStripPiecewiseEquationMenu.Name = "contextMenuStripPiecewiseEquationMenu";
            this.contextMenuStripPiecewiseEquationMenu.Size = new System.Drawing.Size(103, 26);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // listBoxFunctions
            // 
            this.listBoxFunctions.FormattingEnabled = true;
            this.listBoxFunctions.Items.AddRange(new object[] {
            "PI",
            "E",
            "Abs(x)",
            "Acos(x)",
            "Asin(x)",
            "Atan(x)",
            "Atan(x,y)",
            "Ceiling(x)",
            "Cos(x)",
            "Cosh(x)",
            "Exp(x)",
            "Floor(x)",
            "IEEERemainder(x,y)",
            "Log(x)",
            "Log10(x)",
            "Max(x,y)",
            "Min(x,y)",
            "Pow(x,y)",
            "Round(x)",
            "Sign(x)",
            "Sin(x)",
            "Sinh(x)",
            "Sqrt(x)",
            "Tan(x)",
            "Tanh(x)"});
            this.listBoxFunctions.Location = new System.Drawing.Point(260, 22);
            this.listBoxFunctions.Name = "listBoxFunctions";
            this.listBoxFunctions.Size = new System.Drawing.Size(231, 329);
            this.listBoxFunctions.TabIndex = 10;
            this.listBoxFunctions.SelectedIndexChanged += new System.EventHandler(this.listBoxFunctions_SelectedIndexChanged);
            this.listBoxFunctions.DoubleClick += new System.EventHandler(this.listBoxFunctions_DoubleClick);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDescription.Location = new System.Drawing.Point(260, 351);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.Size = new System.Drawing.Size(295, 30);
            this.textBoxDescription.TabIndex = 11;
            // 
            // buttonPlus
            // 
            this.buttonPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlus.Location = new System.Drawing.Point(506, 22);
            this.buttonPlus.Name = "buttonPlus";
            this.buttonPlus.Size = new System.Drawing.Size(39, 35);
            this.buttonPlus.TabIndex = 12;
            this.buttonPlus.Text = "+";
            this.buttonPlus.UseVisualStyleBackColor = true;
            this.buttonPlus.Click += new System.EventHandler(this.buttonPlus_Click);
            // 
            // buttonMinus
            // 
            this.buttonMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMinus.Location = new System.Drawing.Point(506, 63);
            this.buttonMinus.Name = "buttonMinus";
            this.buttonMinus.Size = new System.Drawing.Size(39, 35);
            this.buttonMinus.TabIndex = 13;
            this.buttonMinus.Text = "-";
            this.buttonMinus.UseVisualStyleBackColor = true;
            this.buttonMinus.Click += new System.EventHandler(this.buttonMinus_Click);
            // 
            // buttonMultiply
            // 
            this.buttonMultiply.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMultiply.Location = new System.Drawing.Point(506, 105);
            this.buttonMultiply.Name = "buttonMultiply";
            this.buttonMultiply.Size = new System.Drawing.Size(39, 35);
            this.buttonMultiply.TabIndex = 14;
            this.buttonMultiply.Text = "*";
            this.buttonMultiply.UseVisualStyleBackColor = true;
            this.buttonMultiply.Click += new System.EventHandler(this.buttonMultiply_Click);
            // 
            // buttonDivide
            // 
            this.buttonDivide.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDivide.Location = new System.Drawing.Point(506, 146);
            this.buttonDivide.Name = "buttonDivide";
            this.buttonDivide.Size = new System.Drawing.Size(39, 35);
            this.buttonDivide.TabIndex = 15;
            this.buttonDivide.Text = "/";
            this.buttonDivide.UseVisualStyleBackColor = true;
            this.buttonDivide.Click += new System.EventHandler(this.buttonDivide_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpen.Location = new System.Drawing.Point(506, 196);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(39, 35);
            this.buttonOpen.TabIndex = 16;
            this.buttonOpen.Text = "(";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(506, 250);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(39, 35);
            this.buttonClose.TabIndex = 17;
            this.buttonClose.Text = ")";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // dgvDefault
            // 
            this.dgvDefault.AllowUserToAddRows = false;
            this.dgvDefault.AllowUserToDeleteRows = false;
            this.dgvDefault.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDefault.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvDefault.Location = new System.Drawing.Point(563, 12);
            this.dgvDefault.Name = "dgvDefault";
            this.dgvDefault.RowHeadersVisible = false;
            this.dgvDefault.Size = new System.Drawing.Size(242, 178);
            this.dgvDefault.TabIndex = 18;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "Attribute";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // checkBoxPiecewise
            // 
            this.checkBoxPiecewise.AutoSize = true;
            this.checkBoxPiecewise.Location = new System.Drawing.Point(70, 363);
            this.checkBoxPiecewise.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxPiecewise.Name = "checkBoxPiecewise";
            this.checkBoxPiecewise.Size = new System.Drawing.Size(334, 17);
            this.checkBoxPiecewise.TabIndex = 19;
            this.checkBoxPiecewise.Text = "Check this box to enter a performance curve as piecewise points.";
            this.checkBoxPiecewise.UseVisualStyleBackColor = true;
            this.checkBoxPiecewise.CheckedChanged += new System.EventHandler(this.checkBoxPiecewise_CheckedChanged);
            // 
            // checkBoxAsFunction
            // 
            this.checkBoxAsFunction.AutoSize = true;
            this.checkBoxAsFunction.Location = new System.Drawing.Point(70, 384);
            this.checkBoxAsFunction.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxAsFunction.Name = "checkBoxAsFunction";
            this.checkBoxAsFunction.Size = new System.Drawing.Size(243, 17);
            this.checkBoxAsFunction.TabIndex = 20;
            this.checkBoxAsFunction.Text = "Check this box to enter equation as a function";
            this.checkBoxAsFunction.UseVisualStyleBackColor = true;
            this.checkBoxAsFunction.CheckedChanged += new System.EventHandler(this.checkBoxAsFunction_CheckedChanged);
            // 
            // richTextBoxEquation
            // 
            this.richTextBoxEquation.Location = new System.Drawing.Point(12, 408);
            this.richTextBoxEquation.Name = "richTextBoxEquation";
            this.richTextBoxEquation.Size = new System.Drawing.Size(543, 96);
            this.richTextBoxEquation.TabIndex = 21;
            this.richTextBoxEquation.Text = "";
            this.richTextBoxEquation.Enter += new System.EventHandler(this.richTextBoxEquation_Enter);
            this.richTextBoxEquation.Leave += new System.EventHandler(this.richTextBoxEquation_Leave);
            // 
            // FormEditEquation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 665);
            this.Controls.Add(this.richTextBoxEquation);
            this.Controls.Add(this.checkBoxAsFunction);
            this.Controls.Add(this.checkBoxPiecewise);
            this.Controls.Add(this.dgvDefault);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.buttonDivide);
            this.Controls.Add(this.buttonMultiply);
            this.Controls.Add(this.buttonMinus);
            this.Controls.Add(this.buttonPlus);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.listBoxFunctions);
            this.Controls.Add(this.dgvPerformance);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCheck);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelCompile);
            this.Controls.Add(this.textBoxCompile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxAttribute);
            this.Name = "FormEditEquation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Simulation Equations";
            this.Load += new System.EventHandler(this.FormEditEquation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerformance)).EndInit();
            this.contextMenuStripPiecewiseEquationMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefault)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxAttribute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCompile;
        private System.Windows.Forms.Label labelCompile;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridView dgvPerformance;
        private System.Windows.Forms.DataGridViewTextBoxColumn Age;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.ListBox listBoxFunctions;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonPlus;
        private System.Windows.Forms.Button buttonMinus;
        private System.Windows.Forms.Button buttonMultiply;
        private System.Windows.Forms.Button buttonDivide;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.DataGridView dgvDefault;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.CheckBox checkBoxPiecewise;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripPiecewiseEquationMenu;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxAsFunction;
        private System.Windows.Forms.RichTextBox richTextBoxEquation;
    }
}