using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;

namespace RoadCare3
{
	public partial class FormHPMSFilter : Form
	{
		private string _whereStatement;


		public FormHPMSFilter()
		{
			InitializeComponent();
			LoadColumnListBox();
		}

		private void LoadColumnListBox()
		{
			List<string> filterValues = DBMgr.GetTableColumns("HPMS");
			foreach(string filterValue in filterValues)
			{
				listBoxField.Items.Add(filterValue);
			}
		}


		private void buttonEqual_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " = ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;
		}

		private void buttonLessEqual_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " >= ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;
		}

		private void buttonLessThan_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " > ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;
		}

		private void buttonGreaterEqual_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " <= ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;
		}

		private void buttonGreaterThan_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " < ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;

		}

		private void buttonAnd_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " AND ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;

		}

		private void buttonOR_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " OR ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;

		}

		private void buttonNotEqual_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = " <> ";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;

		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			_whereStatement = textBoxSearch.Text;
			this.DialogResult = DialogResult.OK;
			this.Visible = false;
		}

		private void buttonCheck_Click(object sender, EventArgs e)
		{
			int toDisplay = 0;
			string query = "SELECT COUNT(*) FROM HPMS WHERE " + textBoxSearch.Text;
			toDisplay = DBMgr.ExecuteScalar(query);
			labelReturn.Visible = true;
			labelReturn.Text = "Entries Returned: " + toDisplay.ToString();
		}

		public string WhereStatement
		{
			get { return _whereStatement; }
		}

		private void listBoxField_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxField.SelectedItem != null)
			{
				listBoxValue.Items.Clear();
				string query = "SELECT TOP 100 [" + listBoxField.SelectedItem.ToString() + "] FROM HPMS ORDER BY NEWID()";
				DataSet randomRows = DBMgr.ExecuteQuery(query);
				foreach (DataRow dr in randomRows.Tables[0].Rows)
				{
					listBoxValue.Items.Add(dr[listBoxField.SelectedItem.ToString()].ToString());
				}
			}
		}

		private void listBoxField_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strField = "[" + listBoxField.SelectedItem.ToString() + "] ";
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					switch (listBoxField.SelectedItem.ToString())
					{
						case "YEARS":
							strField = "year(DATE_)";
							break;
					}
					break;
				case "ORACLE":
					switch (listBoxField.SelectedItem.ToString())
					{
						case "YEARS":
							strField = "TO_CHAR(DATE_,'YYYY')";
							break;
						case "DATE_":
							strField = "TO_CHAR(DATE_,'MM/DD/YYYY')";
							break;
					}
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for listBoxField_MouseDoubleClick()");

			}
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strField);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strField.Length;
		}

		private void listBoxValue_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = "'" + listBoxValue.SelectedItem.ToString() + "'";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;
		}
	}
}
