using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using System.IO;

namespace RoadCare3
{
    public partial class FormOutputWindow : ToolWindow
    {
        TextWriter m_writer;
        private int _lastProgress = 0;

        public FormOutputWindow()
        {
            InitializeComponent();
        }

        public void SetOutputText(String strSetText)
        {
            if (!strSetText.Contains("\n"))
            {
                richTextBox1.Text += strSetText + "\r\n";
            }
            else
            {
                richTextBox1.Text += strSetText;
            }

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
			richTextBox1.Refresh();
            _lastProgress = 0;
            if (m_writer != null)
            {
                try
                {
                    m_writer.Write(strSetText + "\r\n");
                }
                catch { }
            }
        
        }

        public void ReplaceOutputText(String strSetText)
        {
            if(_lastProgress > 0)
            {
                richTextBox1.ReadOnly = false;
                richTextBox1.SelectionStart = _lastProgress;
                richTextBox1.SelectionLength = richTextBox1.Text.Length - _lastProgress;
                richTextBox1.SelectedText = "";
                richTextBox1.ReadOnly = true;
            }
            else
            {
                _lastProgress = richTextBox1.Text.Length;
            }







            if (!strSetText.Contains("\n"))
            {
                richTextBox1.Text += strSetText + "\r\n";
            }
            else
            {
                richTextBox1.Text += strSetText;
            }




            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
          //  richTextBox1.Refresh();


        }




        public void ClearWindow()
        {
            richTextBox1.Clear();
        }

        private void FormOutputWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveOutputWindow(this);
        }

        private void tsmiClearOutputWindow_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox1.SelectedText, true);
        }

		private void FormOutputWindow_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		protected void SecureForm()
		{
			//The only control on this form is always readonly so there's nothing to do here...for now.
		}

        private void createLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogOutput.ShowDialog() == DialogResult.OK)
            {
                if (m_writer != null) m_writer.Close();

                m_writer = new StreamWriter(saveFileDialogOutput.FileName);
            }
        }

        private void closeLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_writer.Close();
            m_writer = null;
        }
    }
}