namespace RoadCare3
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Reports.MDSHA;
    using Reports.MDSHA.FyTargetsSuggestedProjects;

    public partial class FormFyTargetsSuggestedProjects : Form
    {
        private readonly Report.Builder reportBuilder;

        public FormFyTargetsSuggestedProjects(Report.Builder b)
        {
            InitializeComponent();

            this.reportBuilder = b;

            if (b.AnalysisYears.Length != 0)
            {
                this.cbCurrFy.Items.AddRange(b.AnalysisYears);
                this.cbCurrFy.SelectedIndex = 0;
            }

            this.fbdPickDir.Description =
                "Select the folder in which you would like the report file" +
                " to be generated.";
        }

        public Report Report { get; private set; }

        public bool IsReportReady { get; private set; }

        private static DialogResult Show(string msg)
        {
            return MessageBox.Show(msg, "RoadCare Message");
        }

        private void cbCurrFy_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedYear = (string)this.cbCurrFy.SelectedItem;
            var priorYear = selectedYear.SubtractOne();

            var years = this.reportBuilder.AnalysisYears;
            var firstYearIdx = years.Length - 1;
            var condYear = years[firstYearIdx].SubtractOne();

            this.tbPrevFy.Text = priorYear;
            this.tbCondYr.Text = condYear;
        }

        private void btnPickDir_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.tbDstDir.Text))
            {
                this.fbdPickDir.SelectedPath = this.tbDstDir.Text;
            }

            var res = this.fbdPickDir.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.tbDstDir.Text = this.fbdPickDir.SelectedPath;
            }
        }

        private void chbTemplateFile_CheckedChanged(object sender, EventArgs e)
        {
            this.tbTemplateFile.ReadOnly = !this.chbTemplateFile.Checked;
            this.btnPickTemplate.Enabled = this.chbTemplateFile.Checked;
        }

        private void btnPickTemplate_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.tbTemplateFile.Text))
            {
                this.ofdPickTemplate.FileName = this.tbTemplateFile.Text;
            }

            var res = this.ofdPickTemplate.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.tbTemplateFile.Text = this.ofdPickTemplate.FileName;
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(this.tbDstDir.Text))
            {
                Show("Error: Destination folder does not exist");
            }
            else if (
                this.chbTemplateFile.Checked &&
                !File.Exists(this.tbTemplateFile.Text))
            {
                Show("Error: Template file does not exist");
            }
            else
            {
                var fyIdx = this.cbCurrFy.SelectedIndex;
                this.Report = this.reportBuilder.Build(fyIdx);

                this.Report.OutputDirectory =
                    new DirectoryInfo(this.tbDstDir.Text);

                if (this.chbTemplateFile.Checked)
                {
                    this.Report.TemplateFileName = this.tbTemplateFile.Text;
                }

                this.IsReportReady = true;
                this.Dispose();
            }
        }
    }
}
