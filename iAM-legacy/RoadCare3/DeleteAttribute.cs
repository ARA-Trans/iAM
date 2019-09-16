using System;
using System.Collections.Generic;
using System.Text;
using DatabaseManager;
using System.Data;
using System.Text.RegularExpressions;
using RoadCareDatabaseOperations;
using RoadCareSecurityOperations;

namespace RoadCare3
{
    public class DeleteAttribute
    {
        private String m_strAttributeToDelete;
		private bool m_IsAttributeCalculated;

        public DeleteAttribute(String strAttributeToDelete)
        {
            m_strAttributeToDelete = strAttributeToDelete;
			m_IsAttributeCalculated = DBOp.IsAttributeCalculated(m_strAttributeToDelete);
        }

        public void Delete()
        {
            // First call the FormManager and have it close all open tabs, except NETWORK_DEFINITION, ASSETS, CONSTRUCTION_HISTORY
            // other ATTRIBUTE (RAW) tabs and Calculated fields.
            FormManager.CloseSegmentationTabs();
            FormManager.CloseAttributeTab(m_strAttributeToDelete);
            FormManager.CloseNetworkTabs();
            FormManager.CloseSimulationTabs();
			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(m_strAttributeToDelete);
            List<String> listCommandText = new List<String>();

            //dsmelser 2009.02.08
			//!!!THERE IS NO DEFAULT ESCAPE CHARACTER IN SQL, IT MUST BE EXPLICITLY DEFINED!!!
			String strDelete = "DELETE FROM ATTRIBUTES_ WHERE ATTRIBUTE_ = '" + m_strAttributeToDelete + "'";
            listCommandText.Add(strDelete);

			String strUpdate = "";
			
			string attributeSearch = m_strAttributeToDelete.Replace("_", "!_");
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					strDelete = "DELETE FROM FEASIBILITY WHERE CRITERIA LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM COSTS WHERE CRITERIA LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM CONSEQUENCES WHERE CRITERIA LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM PERFORMANCE WHERE CRITERIA LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM PERFORMANCE WHERE EQUATION LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strUpdate = "UPDATE SIMULATIONS SET JURISDICTION = NULL WHERE JURISDICTION LIKE " +
									   "'%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strUpdate);

					strDelete = "DELETE FROM DEFICIENTS WHERE CRITERIA LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM TARGETS WHERE CRITERIA LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM PRIORITY WHERE CRITERIA LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM CRITERIA_SEGMENT WHERE FAMILY_EXPRESSION LIKE '%![" + attributeSearch + "!]%' ESCAPE '!'";
					listCommandText.Add(strDelete);
					break;
				case "ORACLE":
					//we don't need to escape the brackets in ORACLE so it throws an error if we attempt it.
					strDelete = "DELETE FROM FEASIBILITY WHERE CRITERIA LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM COSTS WHERE CRITERIA LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM CONSEQUENCES WHERE CRITERIA LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM PERFORMANCE WHERE CRITERIA LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM PERFORMANCE WHERE EQUATION LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strUpdate = "UPDATE SIMULATIONS SET JURISDICTION = NULL WHERE JURISDICTION LIKE " +
									   "'%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strUpdate);

					strDelete = "DELETE FROM DEFICIENTS WHERE CRITERIA LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM TARGETS WHERE CRITERIA LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM PRIORITY WHERE CRITERIA LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);

					strDelete = "DELETE FROM CRITERIA_SEGMENT WHERE FAMILY_EXPRESSION LIKE '%[" + attributeSearch + "]%' ESCAPE '!'";
					listCommandText.Add(strDelete);
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
					//break;
			}			
			DeleteFromNetworks(listCommandText);
			try
			{
				DBMgr.ExecuteBatchNonQuery(listCommandText);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Failed to remove attribute. " + exc.Message);
				return;
			}
			if( !m_IsAttributeCalculated )
			{
				if( cp.IsNative )
				{
					strDelete = "DROP TABLE " + m_strAttributeToDelete;
					try
					{
						DBMgr.ExecuteNonQuery( strDelete );
					}
					catch( Exception exc )
					{
						Global.WriteOutput( "Error: Could not drop attribute table " + m_strAttributeToDelete + ". " + exc.Message );
					}
				}
				else
				{
					String dropView = "DROP VIEW " + m_strAttributeToDelete;
					try
					{
						DBMgr.ExecuteNonQuery( dropView, cp );
					}
					catch( Exception exc )
					{
						Global.WriteOutput( "Error: Could not drop non native view. " + exc.Message );
					}
				}
			}
			else
			{
				try
				{
					DBMgr.ExecuteNonQuery( "DELETE FROM ATTRIBUTES_CALCULATED WHERE ATTRIBUTE_ = '" + m_strAttributeToDelete + "'" );
				}
				catch( Exception ex )
				{
					Global.WriteOutput( "Error: Could not delete calculated attribute formulas: " + ex.Message );
				}
			}
			Global.SecurityOperations.RemoveAction(new RoadCareAction("ATTRIBUTE", m_strAttributeToDelete));
        }



        public void DeleteFromImageView()
        {
            // First call the FormManager and have it close all open tabs, except NETWORK_DEFINITION, ASSETS, CONSTRUCTION_HISTORY
            // other ATTRIBUTE (RAW) tabs and Calculated fields.
            ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(m_strAttributeToDelete);
            List<String> listCommandText = new List<String>();

            m_strAttributeToDelete = m_strAttributeToDelete.Replace("_", "!_");

            String strDelete = "DELETE FROM ATTRIBUTES_ WHERE ATTRIBUTE_ = '" + m_strAttributeToDelete + "'";
            listCommandText.Add(strDelete);
            DeleteFromNetworks(listCommandText);
            DBMgr.ExecuteBatchNonQuery(listCommandText);
            if (cp.IsNative)
            {
                strDelete = "DROP TABLE " + m_strAttributeToDelete;
                DBMgr.ExecuteNonQuery(strDelete);
            }
            else
            {
                String dropView = "DROP VIEW " + m_strAttributeToDelete;
                DBMgr.ExecuteNonQuery(dropView, cp);
            }
        }


        /// <summary>
        /// Adds to the delete transaction for deleting an attribute.
        /// </summary>
        /// <param name="listCommandText">The delete attribute transaction list.</param>
		private void DeleteFromNetworks( List<String> listCommandText )
		{
			String strQuery = "SELECT NETWORKID FROM NETWORKS";
			// For every network ID we have corresponding network tables.  We need to loop
			// through those tables and find where our Attribute and Attribute_Year exist
			// then remove their representative columns.
			DataSet ds = DBMgr.ExecuteQuery( strQuery );
			Regex attributeLocator = new Regex( "(^" + m_strAttributeToDelete + "$|^" + m_strAttributeToDelete + "_[1-9][0-9][0-9][0-9]$)", RegexOptions.Compiled | RegexOptions.IgnoreCase );
			foreach( DataRow dr in ds.Tables[0].Rows )
			{
				string networkID = dr[0].ToString();
				List<String> listTableColumns = DBMgr.GetTableColumns( "SEGMENT_" + dr[0].ToString() + "_NS0" );
				List<String> candidateColumns = listTableColumns.FindAll(
					delegate( String s )
					{
						return attributeLocator.IsMatch( s );
					} );

				foreach( string columnName in candidateColumns )
				{
					listCommandText.Add( "ALTER TABLE " + "SEGMENT_" + networkID + "_NS0 DROP COLUMN " + columnName );
				}
			}
		}
    }
}
