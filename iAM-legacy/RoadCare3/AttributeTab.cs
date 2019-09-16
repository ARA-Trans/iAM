using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Collections;
using DatabaseManager;
using System.Web.UI.WebControls;

namespace RoadCare3
{
	public partial class AttributeTab : DockContent
	{
		//private DataTable m_MapQueryDataTable;
		//private String m_strImagePath;
		private String m_strNetworkName;
		private Hashtable m_htAttributeYears;
		//private String m_oldValue;
		private int m_iNetworkID;
		private int m_iSectionID;

		public AttributeTab(String strNetworkName, Hashtable htAttributeYears)
		{
			InitializeComponent();

			m_strNetworkName = strNetworkName;
			m_htAttributeYears = htAttributeYears;
			try
			{
				DataSet ds = DBMgr.ExecuteQuery("SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME = '" + m_strNetworkName + "'");
				m_iNetworkID = Int32.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error getting network ID for the asset viewer. " + exc.Message);
				return;
			}
		}

		public void SetSectionID(int sectionID)
		{
			m_iSectionID = sectionID;
		}

		/// <summary>
		/// Populates the years combo box on the attributes tab page with the correct years for known attributes.
		/// </summary>
		/// <param name="attributeData">Contains all the attribute data from the segment_NS0 table.</param>
		public void PopulateComboBoxYears()
		{
			List<String> years = new List<String>();
			foreach (DictionaryEntry de in m_htAttributeYears)
			{
				List<String> yearsPerAttribute = (List<String>)m_htAttributeYears[de.Key];
				foreach (String year in yearsPerAttribute)
				{
					if (!years.Contains(year))
					{
						years.Add(year);
					}
				}

			}
			years.Sort();
			years.Reverse();
			String yearSelection = comboBoxAttributeYears.Text;
			comboBoxAttributeYears.Items.Clear();
			comboBoxAttributeYears.Items.Insert(0, "Most Recent");
			comboBoxAttributeYears.Items.AddRange(years.ToArray());
			int iYearIndex = comboBoxAttributeYears.Items.IndexOf(yearSelection);
			if (iYearIndex == -1)
			{
				comboBoxAttributeYears.SelectedIndex = 0;
			}
			else
			{
				comboBoxAttributeYears.SelectedIndex = iYearIndex;
			}
		}

		private void UpdateAttributeGrid()
		{
			dgvAttributeValues.Rows.Clear();
			object[] oAttributeValues = new object[2];

            oAttributeValues[0] = "FACILITY";
            string facilityQuery = "SELECT FACILITY FROM SECTION_" + m_iNetworkID + " WHERE SECTIONID = " + m_iSectionID;
            DataSet facilityDs = DBMgr.ExecuteQuery(facilityQuery);
            oAttributeValues[1] = facilityDs.Tables[0].Rows[0][0].ToString();

            dgvAttributeValues.Rows.Add(oAttributeValues);


			if (comboBoxAttributeYears.Text != "Most Recent")
			{
				foreach (DictionaryEntry de in m_htAttributeYears)
				{
					List<String> attributeYears = (List<String>)m_htAttributeYears[de.Key];
					oAttributeValues[0] = de.Key.ToString();
					if (attributeYears.Contains(comboBoxAttributeYears.Text))
					{
                        String query = "SELECT " + de.Key + "_" + comboBoxAttributeYears.Text + " FROM SEGMENT_" + m_iNetworkID + "_NS0" +
                                " WHERE SECTIONID = " + m_iSectionID;
                        //string query = "SELECT " + de.Key + "_" + comboBoxAttributeYears.Text + " FROM SEGMENT_" + m_iNetworkID + "_NS0 SEG" +
                        //        " INNER JOIN SECTION_" + m_iNetworkID + " SEC ON SEG.SECTIONID = SEC.SECTIONID  WHERE SECTIONID = " + m_iSectionID;
						try
						{
							DataSet ds = DBMgr.ExecuteQuery(query);
							oAttributeValues[1] = ds.Tables[0].Rows[0].ItemArray[0].ToString();
							dgvAttributeValues.Rows.Add(oAttributeValues);
						}
						catch (Exception exc)
						{
							System.Diagnostics.Debug.WriteLine("Error getting attribute information for attribute " + de.Key + ". " + exc.Message);
						}
					}
					else
					{
						oAttributeValues[1] = null;
						dgvAttributeValues.Rows.Add(oAttributeValues);
					}
				}
			}
			else
			{
				// Get the most recent data for the section.
				foreach (DictionaryEntry de in m_htAttributeYears)
				{
					List<String> attributeYears = (List<String>)m_htAttributeYears[de.Key];
					oAttributeValues[0] = de.Key.ToString();
					String query = "SELECT " + de.Key + " FROM SEGMENT_" + m_iNetworkID + "_NS0" +
								   " WHERE SECTIONID = " + m_iSectionID;
					try
					{
						DataSet ds = DBMgr.ExecuteQuery(query);
						oAttributeValues[1] = ds.Tables[0].Rows[0].ItemArray[0].ToString();
						dgvAttributeValues.Rows.Add(oAttributeValues);
					}
					catch (Exception exc)
					{
						System.Diagnostics.Debug.WriteLine("Error getting attribute information for attribute " + de.Key + ". " + exc.Message);
					}
				}
			}
			dgvAttributeValues.Sort(dgvAttributeValues.Columns[0], ListSortDirection.Ascending);
		}

		private void comboBoxAttributeYears_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateAttributeGrid();
		}

		private void AttributeTab_FormClosed(object sender, FormClosedEventArgs e)
		{
			FormManager.RemoveAttributeTab(this);
		}


	}
}
