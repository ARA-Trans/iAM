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
	public partial class FormHPMS : BaseForm
	{
		private BindingSource binding = new BindingSource();
		private DataAdapter dataAdapter = null;

		public FormHPMS()
		{
			InitializeComponent();
			LoadHPMSData();
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			String strQuery = tbSearch.Text;
			FormHPMSFilter form = new FormHPMSFilter();
			if (form.ShowDialog() == DialogResult.OK)
			{
				tbSearch.Text = form.WhereStatement;
				form.Close();
			}
			LoadHPMSData();
		}

		private void LoadHPMSData()
		{
			binding = new BindingSource();
			string query = "";
			if (tbSearch.Text != "")
			{
				query = "SELECT * FROM HPMS WHERE " + tbSearch.Text;
			}
			else
			{
				query = "SELECT * FROM HPMS";
			}
			try
			{
				dataAdapter = new DataAdapter(query);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: HPMS SQL query failed. " + exc.Message);
				return;
			}

			// Populate a new data table and bind it to the BindingSource.
			DataTable table = new DataTable();
			table.Locale = System.Globalization.CultureInfo.InvariantCulture;
			try
			{
				dataAdapter.Fill(table);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Could not fill data adapter. " + exc.Message);
				return;
			}
			binding.DataSource = table;
			dataGridViewHPMS.DataSource = binding;
			bindingNavigatorHPMS.BindingSource = binding;
		}

		private void FormHPMS_Load(object sender, EventArgs e)
		{
			dataGridViewHPMS.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
		}
	}
}
