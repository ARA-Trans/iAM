using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RCImageView3
{
    public partial class FormMissingImages : Form
    {
        String m_strYear;
        String m_strDirection;
        public String Year
        {
            get { return m_strYear; }
        }
        public String Direction
        {
            get { return m_strDirection; }
        }

        public FormMissingImages(String strFacility, String strSection,String strDirection,String strYear)
        {
            InitializeComponent();
            
            textBoxFacility.Text = strFacility;
            textBoxSection.Text = strSection;
            List<String> listYears = RoadCareGlobalOperations.GlobalDatabaseOperations.GetImageYears(strFacility,strSection,strDirection);
            if (listYears.Count > 0)
            {
                foreach (String year in listYears)
                {
                    comboBoxYear.Items.Add(year);
                }
                if (comboBoxYear.Items.Count > 0)
                {
                    comboBoxYear.SelectedIndex = 0;
                }
                this.m_strDirection = strDirection;
            }
            else// need to get different direction
            {
                List<String> listDirection = RoadCareGlobalOperations.GlobalDatabaseOperations.GetImageDirections(strFacility, strSection, strYear);
                if (listDirection.Count > 0)
                {
                    this.m_strDirection = listDirection[0].ToString();
                }
                listYears = RoadCareGlobalOperations.GlobalDatabaseOperations.GetImageYears(strFacility, strSection, m_strDirection);
                if (listYears.Count > 0)
                {
                    foreach (String year in listYears)
                    {
                        comboBoxYear.Items.Add(year);
                    }
                    if (comboBoxYear.Items.Count > 0)
                    {
                        comboBoxYear.SelectedIndex = 0;
                    }

                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_strYear = comboBoxYear.Text;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
