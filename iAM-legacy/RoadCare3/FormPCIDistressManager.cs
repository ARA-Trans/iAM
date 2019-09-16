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
	public partial class FormPCIDistressManager : BaseForm
	{
		private BindingSource binding;
		private DataAdapter pciDistressMapAdapter;
		private DataTable table;

		public FormPCIDistressManager()
		{
			InitializeComponent();
		}

		private void FormPCIDistressManager_Load(object sender, EventArgs e)
		{
			LoadDistressMapping();
		}

		private void LoadDistressMapping()
		{
			string query = "SELECT * FROM PCI_DISTRESS";

			if (pciDistressMapAdapter != null) pciDistressMapAdapter.Dispose();// Free up the resources
			if (binding != null) binding.Dispose();
			if (table != null) table.Dispose();

			pciDistressMapAdapter = new DataAdapter(query);

			// Populate a new data table and bind it to the BindingSource.
			table = new DataTable();
			table.Locale = System.Globalization.CultureInfo.InvariantCulture;
			pciDistressMapAdapter.Fill(table);
			binding = new BindingSource();
			binding.DataSource = table;
			dgvDistressMapping.DataSource = binding;
		}

		private void dgvDistressMapping_Leave(object sender, EventArgs e)
		{
			try
			{
				pciDistressMapAdapter.Update((DataTable)binding.DataSource);
			}
			catch (DBConcurrencyException dbConEx)
			{
				Global.WriteOutput( "Error: Database commit failed [" + dbConEx.Message + "].  Please validate changes." );
			}
			catch (Exception exc)
			{
				Global.WriteOutput( "Error: Database commit failed [" + exc.Message + "].  Please validate changes." );
			}
		}
	}
}
