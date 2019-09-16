using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RoadCare3
{
    public partial class FormAddAssetToAttribute : Form
    {
        String m_strAttribute;
        public String Attribute
        {
            get { return m_strAttribute; }
        }
        public FormAddAssetToAttribute()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_strAttribute = textBoxAttribute.Text;
        }
    }
}
