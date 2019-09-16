using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadCare3;
using WeifenLuo.WinFormsUI.Docking;

namespace RCImageView3
{
    public static class OutputWindow
    {
        private static DockPanel m_DockPanel;        
        private static FormOutputWindow m_formOutput;

        static public FormOutputWindow Output
        {
            set { m_formOutput = value; }
            get { return m_formOutput; }

        }

        static public DockPanel DockPanel
        {
            set { m_DockPanel = value; }
            get { return m_DockPanel; }

        }


        /// <summary>
        /// Writes the input string to the output window.
        /// </summary>
        /// <param name="strOutputText">Output window string.</param>
        public static void WriteOutput(String strOutputText)
        {
            String strTemp = strOutputText.ToUpper();
            if (strTemp.Contains("ERROR"))
            {
                System.Media.SystemSounds.Exclamation.Play();
            }
            if (Output != null)
            {
                Output.SetOutputText(strOutputText);
                Output.Show();
            }
        }
    }
}
