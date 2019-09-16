using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using DatabaseManager;
using System.Data.SqlClient;
using System.Data.OleDb;

using Microsoft.SqlServer.Management.Smo;
using RoadCare3;

namespace RoadCare3
{
    public partial class FormProperties : ToolWindow
    {
        //private Hashtable m_htAttributes = new Hashtable();
        //private Hashtable m_htAttributeFieldAndValue = new Hashtable();
        //private List<String> m_strListFields = new List<String>();
        
        private String m_strPropertyType;
        private AttributeProperties attrProp;
        private NetworkProperties networkProp;
		private AssetProperties assetProp;
		private CalculatedFieldsProperties calcProp;

        public FormProperties()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Attaches a Property Grid class object to the property grid tool window.
        /// </summary>
        /// <param name="o">A Custom property grid class, o is attached to the .SelectedObject property of the property grid class.</param>
        public void SetPropertyGrid(object o, bool bIsModal)
        {
            m_strPropertyType = o.GetType().Name.ToString();
			switch (m_strPropertyType)
			{
				case "AttributeProperties":
					attrProp = (AttributeProperties)o;
					attrProp.SetAttributeProperties(pgProperties);
					attrProp.UpdatePropertyGrid(pgProperties);
					break;
				case "NetworkProperties":
					networkProp = (NetworkProperties)o;
					networkProp.SetNetworkProperties(pgProperties, bIsModal);
					networkProp.UpdatePropertyGrid(pgProperties);
					break;
				case "AssetProperties":
					// We should set up a seperate class for assets as well.  We probably could do it through
					// the custom class interface, but the code is neater and easier to track/maintain if we create a new AssetProperties class.
					// CustomClass cc = (CustomClass)o;
					assetProp = (AssetProperties)o;
					assetProp.SetAssetProperties(pgProperties, bIsModal);
					break;
				case "CalculatedFieldProperties":
					calcProp = (CalculatedFieldsProperties)o;
					calcProp.CreateCalculatedFieldPropertyGrid(pgProperties);
					break;
				default:
					Global.WriteOutput("Could not access properties for the selected item. ");
					break;
			}
			return;
        }

        private void pgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (m_strPropertyType == "AttributeProperties")
            {
                // When the user changes a property value in the property grid tool window, reflect that change in the database appropriately.
				attrProp.UpdateDatabase( e.ChangedItem.Label.ToString(), e.ChangedItem.Value.ToString() );
            }
            if (m_strPropertyType == "NetworkProperties")
            {
                networkProp.UpdateDatabase(e.ChangedItem.Label.ToString(), e.ChangedItem.Value.ToString());
            }
			if (m_strPropertyType == "CalculatedFieldProperties")
			{
				calcProp.UpdateProperty(e.ChangedItem.Label.ToString(), e.ChangedItem.Value.ToString());
			}

        }

		private void FormProperties_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		protected void SecureForm()
		{
			//throw new NotImplementedException();
		}
    }
}