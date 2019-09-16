using System;
using System.Collections.Generic;
using System.Text;
using DatabaseManager;

namespace RoadCare3
{
	public class DeleteAsset
	{
		private string m_assetToDelete;

		public DeleteAsset(String assetToDelete)
		{
			m_assetToDelete = assetToDelete;
		}

		public void Delete()
		{
			// Completely remove an asset and all of its references from the RoadCare database and UI.
			ConnectionParameters cp = DBMgr.GetAssetConnectionObject(m_assetToDelete);
			List<string> transactions = new List<string>();
			if (cp.IsNative)
			{
				string dropAssetTable = "DROP TABLE " + m_assetToDelete;
				transactions.Add(dropAssetTable);
				dropAssetTable = "DROP TABLE " + m_assetToDelete + "_CHANGELOG";
				transactions.Add(dropAssetTable);
			}
			else
			{
				string dropAssetView = "DROP VIEW " + m_assetToDelete;
				transactions.Add(dropAssetView);
			}
			DBMgr.ExecuteBatchNonQuery( transactions, cp );
			string deleteAsset = "DELETE FROM ASSETS WHERE ASSET = '" + m_assetToDelete + "'";
			DBMgr.ExecuteNonQuery(deleteAsset);
		}


	}
}
