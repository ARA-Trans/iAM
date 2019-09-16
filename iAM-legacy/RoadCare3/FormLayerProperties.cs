using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using SharpMap.Layers;
using SharpMap.Geometries;

namespace RoadCare3
{
    public partial class FormLayerProperties : Form
    {
        private String m_strLayerName;
		private bool m_bIsCustomSymbol = false;
		private VectorLayer m_vl;
		Bitmap m_bmpNewSymbol = null;
		private Color m_oldColor = Color.Empty;
		private float m_symbolScale = 0.6666453f;
		private int m_lineThickness = 1;

     
		public FormLayerProperties(VectorLayer vl)
		{
			InitializeComponent();
			m_strLayerName = vl.LayerName;
			m_vl = vl;
		}

		private void FormLayerProperties_Load(object sender, EventArgs e)
		{
			tbLayerName.Text = m_strLayerName;

			// Set current values for color for an asset properties window.
			// If the symbol and its tag are NOT NULL then it is a default symbol.
			if (m_vl.Style.Symbol != null && m_vl.Style.Symbol.Tag != null)
			{
				m_oldColor = Color.FromArgb(255, (Color)m_vl.Style.Symbol.Tag);
				btnColor.BackColor = (Color)m_vl.Style.Symbol.Tag;
				if (m_vl.Style.SymbolScale == 0.6666453f)
				{
					imageComboBoxLineThickness.SelectedText = "Default";
				}
				for (int i = 0; i < imageComboBoxLineThickness.Items.Count; i++)
				{
					if (imageComboBoxLineThickness.Items[i].Text == m_vl.Style.SymbolScale.ToString())
					{
						imageComboBoxLineThickness.SelectedIndex = i;
					}
				}
			}
			else if(m_vl.Style.Symbol == null)
			{
				// Access the NETWORK MAP geometries to extract width and color information from the current layer.
				SharpMap.Data.Providers.GeometryProvider geoProv;
				geoProv = (SharpMap.Data.Providers.GeometryProvider)m_vl.DataSource;
				IList<Geometry> GeomColl = geoProv.Geometries;
				LineString lineTemp = (LineString)GeomColl[0];
				btnColor.BackColor = lineTemp.Color;
				if (lineTemp.Width_.ToString() == "0")
				{
					// Set the line thickness to one if it is zero.  I know, it doesnt make sense to me either, but
					// looking at the results of the display, apparently 0 and 1 thickness are equivalent.
					imageComboBoxLineThickness.SelectedIndex = 0;
				}
				for (int i = 0; i < imageComboBoxLineThickness.Items.Count; i++)
				{
					if (imageComboBoxLineThickness.Items[i].Text == lineTemp.Width_.ToString())
					{
						imageComboBoxLineThickness.SelectedIndex = i;
					}
				}
			}
			
		}

		/// <summary>
		/// Returns true if the user has elected to use a custom symbol, and false for a default symbol selection.
		/// </summary>
		public bool IsCustomSymbol
		{
			get { return m_bIsCustomSymbol; }
			set { m_bIsCustomSymbol = value; }
		}

		public Bitmap GetNewSymbol()
		{
			return m_bmpNewSymbol;
		}

		public Color GetOldColor()
		{
			return m_oldColor;
		}

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
			try
			{
				switch (imageComboBoxLineThickness.Text)
				{
					case "1":
						m_symbolScale = 1f;
						m_lineThickness = 1;
						break;
					case "2":
						m_symbolScale = 1.5f;
						m_lineThickness = 2;
						break;
					case "3":
						m_symbolScale = 2f;
						m_lineThickness = 10;
						break;
					case "Default":
						m_symbolScale = 0.6666453f;
						m_lineThickness = 1;
						break;
					default:
						m_symbolScale = 0.6666453f;
						m_lineThickness = 1;
						break;
				}

			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Couldn't parse symbol scale text to float. " + exc.Message);
			}
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

		private void btnSymbol_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Image (*.bmp)|*.bmp|All (*.*)|*.*";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Bitmap bmp = new Bitmap(ofd.FileName);
				btnSymbol.Image = bmp;
				m_bIsCustomSymbol = true;
			}
		}

		private void imageComboBoxSymbols_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_bmpNewSymbol = (Bitmap)imageListDefaultSymbols.Images[imageComboBoxSymbols.SelectedIndex];
			m_bmpNewSymbol.Tag = Color.Red;
		}

		private void btnColor_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			if (cd.ShowDialog() == DialogResult.OK)
			{
				btnColor.BackColor = cd.Color;
			}
		}

		public Color GetNewColor()
		{
			return btnColor.BackColor;
		}

		public Bitmap GetCustomSymbol()
		{
			return (Bitmap)btnSymbol.Image;
		}

		public float GetSymbolScale()
		{
			return m_symbolScale;
		}

		public int GetLineThickness()
		{
			return m_lineThickness;
		}

		private void imageComboBoxLineThickness_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				switch (imageComboBoxLineThickness.Text)
				{
					case "1":
						m_symbolScale = 1f;
						m_lineThickness = 1;
						break;
					case "2":
						m_symbolScale = 1.5f;
						m_lineThickness = 2;
						break;
					case "3":
						m_symbolScale = 2f;
						m_lineThickness = 3;
						break;
					case "Default":
						m_symbolScale = 0.6666453f;
						m_lineThickness = 1;
						break;
					default:
						m_symbolScale = 0.6666453f;
						m_lineThickness = 1;
						break;
				}
				
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Couldn't parse symbol scale text to float. " + exc.Message);
			}
		}

		private void chkboxLabels_CheckedChanged( object sender, EventArgs e )
		{

		}

	}
}
