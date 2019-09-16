using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;
using System.IO;

namespace Reports.LC_PAVEMENTS
{
	public class PGROUPReport
	{
		string m_networkID;
		string m_simulationID;
		List<PGroupTuple> binPGroupNumber = new List<PGroupTuple>();
		List<List<PGroupTuple>> output = new List<List<PGroupTuple>>();
		List<string> listCommitedSections = new List<string>();

		public PGROUPReport(int networkID, string simulationID)
		{
			m_networkID = networkID.ToString();
			m_simulationID = simulationID;
			GetPGroupData();
		}

		private void GetPGroupData()
		{
			string query = "SELECT DISTINCT P_GROUP FROM SEGMENT_" + m_networkID + "_NS0 WHERE P_GROUP IS NOT NULL ORDER BY P_GROUP";
			DataSet distinctPGroups = DBMgr.ExecuteQuery(query);
			query = "SELECT SEG.P_GROUP, SIM.TREATMENT, SIM.YEARS, SIM.SECTIONID FROM SEGMENT_" + m_networkID + "_NS0 SEG INNER JOIN REPORT_" + m_networkID + "_" + m_simulationID + " SIM ON SEG.SECTIONID = SIM.SECTIONID WHERE SEG.P_GROUP IS NOT NULL ORDER BY SEG.P_GROUP, SIM.YEARS";
			DataSet ds = DBMgr.ExecuteQuery(query);
			query = "SELECT SEG.SECTIONID FROM COMMITTED_ COMMIT_ INNER JOIN SEGMENT_" + m_networkID + "_NS0 SEG ON SEG.SECTIONID = COMMIT_.SECTIONID WHERE COMMIT_.SIMULATIONID = '" + m_simulationID + "'";
			DataSet commitedSections = DBMgr.ExecuteQuery(query);
			foreach (DataRow dr in commitedSections.Tables[0].Rows)
			{
				listCommitedSections.Add(dr["SECTIONID"].ToString());
			}

			int i = 0;
			foreach (DataRow dr in distinctPGroups.Tables[0].Rows)
			{
				output.Add(new List<PGroupTuple>());
				bool treatmentApplied = false;
				string outerPGroup = dr["P_GROUP"].ToString();
				foreach (DataRow inner in ds.Tables[0].Rows)
				{
					bool bAddTuple = false;
					string pgroup = inner["P_GROUP"].ToString();
					string treatment = inner["TREATMENT"].ToString();
					string year = inner["YEARS"].ToString();
					string sectionID = inner["SECTIONID"].ToString();
					string isCommited = "false";
					if (outerPGroup == pgroup)
					{
						if (treatment != "No Treatment" && !treatmentApplied)
						{
							treatmentApplied = true;
							bAddTuple = true;
						}
						if (treatment == "No Treatment")
						{
							bAddTuple = true;
						}
						if (bAddTuple)
						{
							foreach (string s in listCommitedSections)
							{
								if (s == sectionID)
								{
									isCommited = "true";
								}
							}
							output[i].Add(new PGroupTuple(pgroup, treatment, year, isCommited));
						}
					}
				}
				i++;
			}
			

			// Now output the list to a file.

			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\P_GROUPS.txt";
			TextWriter tw = new StreamWriter(strOutFile);

			tw.WriteLine("PGROUP; TREATMENT; YEAR; ISCOMMITTED");
			foreach (List<PGroupTuple> listTuples in output)
			{
				foreach (PGroupTuple tuple in listTuples)
				{
					tw.WriteLine(tuple.m_pGroup + "; " + tuple.m_treatment + "; " + tuple.m_year + "; " + tuple.m_isCommited);
				}
			}
			tw.Close();
		}


	}

	internal class PGroupTuple
	{
		public string m_pGroup;
		public string m_treatment;
		public string m_year;
		public string m_isCommited = "false";

		public PGroupTuple(string pGroup, string treatment, string year, string isCommited)
		{
			m_pGroup = pGroup;
			m_treatment = treatment;
			m_year = year;
			m_isCommited = isCommited;
		}
	}
}
