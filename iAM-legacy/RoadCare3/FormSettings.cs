using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RoadCare3
{
	public partial class FormSettings : BaseForm
	{
		public FormSettings()
		{
			InitializeComponent();
		}

		public FormSettings( string windowName, List<PropertyGridEx.CustomProperty> properties )
		{
			InitializeComponent();
			this.Text = windowName;
			this.TabText = windowName;
			foreach( PropertyGridEx.CustomProperty property in properties )
			{
				pgeSettings.Item.Add( property );
			}
		}

		private void btnOk_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void FormSettings_FormClosed( object sender, FormClosedEventArgs e )
		{
			if( DialogResult != DialogResult.OK )
			{
				this.DialogResult = DialogResult.Cancel;
			}
		}

		public Dictionary<string, object> Settings
		{
			get
			{
				Dictionary<string, object> properties = new Dictionary<string, object>();
				foreach( PropertyGridEx.CustomProperty property in pgeSettings.Item )
				{
					properties.Add( property.Name, property.Value );
				}

				return properties;
			}
		}

		private void FormSettings_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		protected void SecureForm()
		{
			//throw new NotImplementedException();
		}


	}
}
