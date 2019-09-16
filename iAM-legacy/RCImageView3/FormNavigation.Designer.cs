namespace RCImageView3
{
    partial class FormNavigation
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripComboBoxType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxFacility = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabelDirection = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxDirection = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelStationOrSection = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxStation = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripComboBoxSection = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelYear = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxYear = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSlow = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReverse = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrevious = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPlay = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFast = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelSkip = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxFrameDelay = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabelSecond = new System.Windows.Forms.ToolStripLabel();
            this.timerNavigation = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxType,
            this.toolStripLabel1,
            this.toolStripComboBoxFacility,
            this.toolStripLabelDirection,
            this.toolStripComboBoxDirection,
            this.toolStripSeparator1,
            this.toolStripLabelStationOrSection,
            this.toolStripTextBoxStation,
            this.toolStripComboBoxSection,
            this.toolStripSeparator2,
            this.toolStripLabelYear,
            this.toolStripComboBoxYear,
            this.toolStripSeparator3,
            this.toolStripButtonSlow,
            this.toolStripButtonReverse,
            this.toolStripButtonPrevious,
            this.toolStripButtonNext,
            this.toolStripButtonPlay,
            this.toolStripButtonFast,
            this.toolStripSeparator4,
            this.toolStripLabelSkip,
            this.toolStripSeparator5,
            this.toolStripLabel2,
            this.toolStripTextBoxFrameDelay,
            this.toolStripLabelSecond});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1193, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripComboBoxType
            // 
            this.toolStripComboBoxType.Name = "toolStripComboBoxType";
            this.toolStripComboBoxType.Size = new System.Drawing.Size(75, 38);
            this.toolStripComboBoxType.ToolTipText = "Reference method";
            this.toolStripComboBoxType.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxType_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 35);
            this.toolStripLabel1.Text = "Facility";
            // 
            // toolStripComboBoxFacility
            // 
            this.toolStripComboBoxFacility.DropDownWidth = 200;
            this.toolStripComboBoxFacility.Name = "toolStripComboBoxFacility";
            this.toolStripComboBoxFacility.Size = new System.Drawing.Size(150, 38);
            this.toolStripComboBoxFacility.ToolTipText = "Current Facility";
            this.toolStripComboBoxFacility.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxFacility_SelectedIndexChanged);
            this.toolStripComboBoxFacility.Click += new System.EventHandler(this.toolStripComboBoxFacility_Click);
            // 
            // toolStripLabelDirection
            // 
            this.toolStripLabelDirection.Name = "toolStripLabelDirection";
            this.toolStripLabelDirection.Size = new System.Drawing.Size(55, 35);
            this.toolStripLabelDirection.Text = "Direction";
            // 
            // toolStripComboBoxDirection
            // 
            this.toolStripComboBoxDirection.Name = "toolStripComboBoxDirection";
            this.toolStripComboBoxDirection.Size = new System.Drawing.Size(75, 38);
            this.toolStripComboBoxDirection.ToolTipText = "Direction of travel";
            this.toolStripComboBoxDirection.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxDirection_SelectedIndexChanged);
            this.toolStripComboBoxDirection.Click += new System.EventHandler(this.toolStripComboBoxDirection_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabelStationOrSection
            // 
            this.toolStripLabelStationOrSection.Name = "toolStripLabelStationOrSection";
            this.toolStripLabelStationOrSection.Size = new System.Drawing.Size(44, 35);
            this.toolStripLabelStationOrSection.Text = "Station";
            // 
            // toolStripTextBoxStation
            // 
            this.toolStripTextBoxStation.Name = "toolStripTextBoxStation";
            this.toolStripTextBoxStation.Size = new System.Drawing.Size(60, 38);
            this.toolStripTextBoxStation.ToolTipText = "Station or milepost";
            this.toolStripTextBoxStation.TextChanged += new System.EventHandler(this.toolStripTextBoxStation_TextChanged);
            // 
            // toolStripComboBoxSection
            // 
            this.toolStripComboBoxSection.Name = "toolStripComboBoxSection";
            this.toolStripComboBoxSection.Size = new System.Drawing.Size(200, 38);
            this.toolStripComboBoxSection.ToolTipText = "Section";
            this.toolStripComboBoxSection.Visible = false;
            this.toolStripComboBoxSection.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxSection_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabelYear
            // 
            this.toolStripLabelYear.Name = "toolStripLabelYear";
            this.toolStripLabelYear.Size = new System.Drawing.Size(30, 35);
            this.toolStripLabelYear.Text = "Year";
            // 
            // toolStripComboBoxYear
            // 
            this.toolStripComboBoxYear.Name = "toolStripComboBoxYear";
            this.toolStripComboBoxYear.Size = new System.Drawing.Size(75, 38);
            this.toolStripComboBoxYear.ToolTipText = "Image year";
            this.toolStripComboBoxYear.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxYear_SelectedIndexChanged);
            this.toolStripComboBoxYear.Click += new System.EventHandler(this.toolStripComboBoxYear_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButtonSlow
            // 
            this.toolStripButtonSlow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSlow.Image = global::RCImageView3.Properties.Resources.backward_icon;
            this.toolStripButtonSlow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonSlow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSlow.Name = "toolStripButtonSlow";
            this.toolStripButtonSlow.Size = new System.Drawing.Size(36, 35);
            this.toolStripButtonSlow.Text = "Slow";
            this.toolStripButtonSlow.ToolTipText = "Reduce frame skip interval (shift-left arrow)";
            this.toolStripButtonSlow.Click += new System.EventHandler(this.toolStripButtonSlow_Click);
            // 
            // toolStripButtonReverse
            // 
            this.toolStripButtonReverse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReverse.Image = global::RCImageView3.Properties.Resources.reverse_icon;
            this.toolStripButtonReverse.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonReverse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReverse.Name = "toolStripButtonReverse";
            this.toolStripButtonReverse.Size = new System.Drawing.Size(36, 35);
            this.toolStripButtonReverse.Tag = "Reverse";
            this.toolStripButtonReverse.Text = "Reverse";
            this.toolStripButtonReverse.ToolTipText = "Play in reverse to beginning of Facility (left arrow)";
            this.toolStripButtonReverse.Click += new System.EventHandler(this.toolStripButtonReverse_Click);
            // 
            // toolStripButtonPrevious
            // 
            this.toolStripButtonPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevious.Image = global::RCImageView3.Properties.Resources.minus_icon;
            this.toolStripButtonPrevious.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevious.Name = "toolStripButtonPrevious";
            this.toolStripButtonPrevious.Size = new System.Drawing.Size(36, 35);
            this.toolStripButtonPrevious.Text = "Previous";
            this.toolStripButtonPrevious.ToolTipText = "Go back to previous image (down arrow)";
            this.toolStripButtonPrevious.Click += new System.EventHandler(this.toolStripButtonPrevious_Click);
            // 
            // toolStripButtonNext
            // 
            this.toolStripButtonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNext.Image = global::RCImageView3.Properties.Resources.plus_icon;
            this.toolStripButtonNext.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNext.Name = "toolStripButtonNext";
            this.toolStripButtonNext.Size = new System.Drawing.Size(36, 35);
            this.toolStripButtonNext.Text = "Advance";
            this.toolStripButtonNext.ToolTipText = "Advance forward to next image (up arrow)";
            this.toolStripButtonNext.Click += new System.EventHandler(this.toolStripButtonNext_Click);
            // 
            // toolStripButtonPlay
            // 
            this.toolStripButtonPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPlay.Image = global::RCImageView3.Properties.Resources.play_icon;
            this.toolStripButtonPlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPlay.Name = "toolStripButtonPlay";
            this.toolStripButtonPlay.Size = new System.Drawing.Size(36, 35);
            this.toolStripButtonPlay.Tag = "Play";
            this.toolStripButtonPlay.Text = "Play";
            this.toolStripButtonPlay.ToolTipText = "Play to end of Facility (right arrow)";
            this.toolStripButtonPlay.Click += new System.EventHandler(this.toolStripButtonPlay_Click);
            // 
            // toolStripButtonFast
            // 
            this.toolStripButtonFast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFast.Image = global::RCImageView3.Properties.Resources.forward_icon;
            this.toolStripButtonFast.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonFast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFast.Name = "toolStripButtonFast";
            this.toolStripButtonFast.Size = new System.Drawing.Size(36, 35);
            this.toolStripButtonFast.Text = "Fast";
            this.toolStripButtonFast.ToolTipText = "Increase frame skip interval (shift-right arrow)";
            this.toolStripButtonFast.Click += new System.EventHandler(this.toolStripButtonFast_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabelSkip
            // 
            this.toolStripLabelSkip.Name = "toolStripLabelSkip";
            this.toolStripLabelSkip.Size = new System.Drawing.Size(73, 35);
            this.toolStripLabelSkip.Text = "Frame skip:1";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(36, 15);
            this.toolStripLabel2.Text = "Delay";
            // 
            // toolStripTextBoxFrameDelay
            // 
            this.toolStripTextBoxFrameDelay.Name = "toolStripTextBoxFrameDelay";
            this.toolStripTextBoxFrameDelay.Size = new System.Drawing.Size(50, 23);
            this.toolStripTextBoxFrameDelay.TextChanged += new System.EventHandler(this.toolStripTextBoxFrameDelay_TextChanged);
            // 
            // toolStripLabelSecond
            // 
            this.toolStripLabelSecond.Name = "toolStripLabelSecond";
            this.toolStripLabelSecond.Size = new System.Drawing.Size(50, 15);
            this.toolStripLabelSecond.Text = "seconds";
            // 
            // timerNavigation
            // 
            this.timerNavigation.Tick += new System.EventHandler(this.timerNavigation_Tick);
            // 
            // FormNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1193, 38);
            this.CloseButton = false;
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormNavigation";
            this.Text = "FormNavigation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNavigation_FormClosing);
            this.Load += new System.EventHandler(this.FormNavigation_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxFacility;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxType;
        private System.Windows.Forms.ToolStripLabel toolStripLabelStationOrSection;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxStation;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxSection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabelYear;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxYear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonReverse;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrevious;
        private System.Windows.Forms.ToolStripButton toolStripButtonNext;
        private System.Windows.Forms.ToolStripButton toolStripButtonPlay;
        private System.Windows.Forms.ToolStripButton toolStripButtonFast;
        private System.Windows.Forms.ToolStripLabel toolStripLabelSkip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxDirection;
        private System.Windows.Forms.ToolStripLabel toolStripLabelDirection;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxFrameDelay;
        private System.Windows.Forms.ToolStripLabel toolStripLabelSecond;
        private System.Windows.Forms.Timer timerNavigation;
        public System.Windows.Forms.ToolStripButton toolStripButtonSlow;
    }
}