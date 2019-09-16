using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utility.ExceptionHandling
{
    internal partial class UIExceptionForm : Form
    {
        public UIExceptionForm(Exception ex, string msg)
        {
            InitializeComponent();
        }
    }
}
