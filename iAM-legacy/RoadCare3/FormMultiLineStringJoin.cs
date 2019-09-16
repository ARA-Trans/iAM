using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using DatabaseManager;
using SharpMap.Geometries;
using System.IO;
using RoadCareGlobalOperations;
using SharpMap.Rendering.Thematics;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.Data.Providers;
using System.Collections;
using SharpMap.Forms;

namespace RoadCare3
{
	public partial class FormMultiLineStringJoin : BaseForm
	{
		public FormMultiLineStringJoin()
		{
			InitializeComponent();
		}

		private void FormMultiLineStringDefinition_Load(object sender, EventArgs e)
		{
			try
			{
				// Bulk load the multiline GEOMS into MULTILINE_GEOMETRIES_JOIN
				LoadMultiLineStrings();
				LoadMultiLineDGV();
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error processing multiline segments. " + exc.Message);
			}
		}

		private void LoadMultiLineDGV()
		{
			string query = "SELECT ND.ID_, ND.ROUTES, ND.DIRECTION, MG.LINESTRING, MG.JOIN_PRIORITY, MG.MULTILINE_GEOM_ID FROM NETWORK_DEFINITION ND, MULTILINE_GEOMETRIES_JOIN MG WHERE ND.ID_ = MG.FACILITY_ID";
			BindingSource binding = RoadCareDatabaseOperations.DBOp.CreateBoundTable(query);

			dgvMultiLineString.DataSource = binding;
			bindingNavigatorMLS.BindingSource = binding;

			dgvMultiLineString.Columns["ID_"].ReadOnly = true;
			dgvMultiLineString.Columns["ROUTES"].ReadOnly = true;
			dgvMultiLineString.Columns["DIRECTION"].ReadOnly = true;
			dgvMultiLineString.Columns["LINESTRING"].ReadOnly = true;
			dgvMultiLineString.Columns["LINESTRING"].Visible = false;
			dgvMultiLineString.Columns["MULTILINE_GEOM_ID"].Visible = false;

			List<Geometry> GeomColl = new List<Geometry>();
			Geometry lineSeg;
			foreach (DataGridViewRow lineString in dgvMultiLineString.Rows)
			{
				if (lineString.Cells["LINESTRING"].Value != null)
				{
					lineSeg = Geometry.GeomFromText(lineString.Cells["LINESTRING"].Value.ToString());
					GeomColl.Add(lineSeg);
				}
			}

			// Create vector layer
			if (GeomColl.Count > 0)
			{
				mapImageTemp.Map.Layers.Clear();
				mapImageTemp.Refresh();

				VectorLayer myLayer = new VectorLayer("MULTILINEGEOMETRIES");
				myLayer.DataSource = new GeometryProvider(GeomColl);
				mapImageTemp.Map.Layers.Insert(0, myLayer);

				mapImageTemp.Refresh();
				mapImageTemp.Map.ZoomToExtents();
			}
		}

		private void LoadMultiLineStrings()
		{
			// This query returns values that DO NOT EXIST in the MULTILINE GEOMETRY table
			// in case the user presses the Define MultiLine geometries button more than once.
			//string query = "SELECT * FROM NETWORK_DEFINITION WHERE ID_ NOT IN (SELECT DISTINCT FACILITY_ID AS ID_ FROM MULTILINE_GEOMETRIES_JOIN)";
			//DataSet doesNotExistInMultiLineGeometry = DBMgr.ExecuteQuery(query);
			string query = "SELECT * FROM NETWORK_DEFINITION";
			DataSet doesNotExistInMultiLineGeometry = DBMgr.ExecuteQuery(query);

			string clearMSGTable = "DELETE FROM MULTILINE_GEOMETRIES_JOIN";
			DBMgr.ExecuteNonQuery(clearMSGTable);

			string strOutFile = GlobalDatabaseOperations.CreateBulkLoaderPath("MULTILINE_GEOMS", "txt");
			TextWriter tw = new StreamWriter(strOutFile);

			foreach (DataRow multiGeomRow in doesNotExistInMultiLineGeometry.Tables[0].Rows)
			{
				Geometry geo = Geometry.GeomFromText(multiGeomRow["GEOMETRY"].ToString());
				if (geo is MultiLineString)
				{
					MultiLineString multiLineGeom = (MultiLineString)geo;
					foreach (LineString ls in multiLineGeom.LineStrings)
					{
						tw.WriteLine("\t" + multiGeomRow["ID_"].ToString() + "\t" + ls.ToString() + "\t");
					}
				}
			}
			tw.Close();
			DBMgr.SQLBulkLoad("MULTILINE_GEOMETRIES_JOIN", strOutFile, '\t');
		}

		private void HighlightSelectedLines(DataGridView activeDataGridView, MapImage activeMap)
		{
			List<Geometry> geomsToHighlight = new List<Geometry>();

			// Create the new "ToHighlight" vector layer and remove the old one from the map.
			VectorLayer toHighlight = new VectorLayer("HighlightedLines");
			int iRemoveIndex = activeMap.Map.Layers.FindIndex(delegate(SharpMap.Layers.ILayer layer) { return layer.LayerName == "HighlightedLines"; });
			if (iRemoveIndex > -1)
			{
				activeMap.Map.Layers.RemoveAt(iRemoveIndex);
			}
			
			// Loop through the selected values only and add them to the geom collection to highlight.
			// Then add that layer to the map.
			foreach (DataGridViewRow linestringToHighlight in activeDataGridView.SelectedRows)
			{
				if (linestringToHighlight.Cells["LINESTRING"].Value != null && linestringToHighlight.Cells["LINESTRING"].Value.ToString() != "")
				{
					Geometry geomToHighlight = Geometry.GeomFromText(linestringToHighlight.Cells["LINESTRING"].Value.ToString());
					geomToHighlight.Color = Color.Cyan;
					geomToHighlight.Width_ = 5;
					geomsToHighlight.Add(geomToHighlight);
				}
			}
			toHighlight.DataSource = new GeometryProvider(geomsToHighlight);
			activeMap.Map.Layers.Insert(0, toHighlight);
			activeMap.Refresh();
		}

		private void PopulateResultGrid(List<LineStringPriorityGroup> results)
		{
			dgvJoinedLineStrings.Rows.Clear();
			foreach (LineStringPriorityGroup lspGroup in results)
			{
				string facilityID = lspGroup.facilityID;
				string lineString = lspGroup.joinedLineString;
				dgvJoinedLineStrings.Rows.Add(facilityID, lineString);
			}
		}

		private List<Geometry> CreateJoinedLineStringLayer(List<string> newLineStrings)
		{
			List<Geometry> geomColl = new List<Geometry>();
			foreach (string geom in newLineStrings)
			{
				geomColl.Add(Geometry.GeomFromText(geom));
			}
			return geomColl;
		}

		private string JoinLineStrings(List<string> toJoin)
		{
			StringBuilder newLineString = new StringBuilder();
			newLineString.Append("LINESTRING (");

			foreach (string lineString in toJoin)
			{
				// Remove the word LINESTRING and leading space and (
				string toModify = lineString;
				toModify = toModify.Replace("LINESTRING (", "");
				
				// Now remove the last )
				toModify = toModify.Replace(")", "");

				// Now add the linestring vertices to the overall linestring being
				// created.
				newLineString.Append(toModify);
				newLineString.Append(", ");
			}
			// Remove trailing comma.
			newLineString.Remove(newLineString.Length - 2, 2);

			// Add end of the linestring )
			newLineString.Append(")");
			return newLineString.ToString();
		}

		private void tsbJoin_Click(object sender, EventArgs e)
		{
			if (dgvMultiLineString.SelectedRows.Count > 1)
			{
				List<LineStringPriority> priortiesToAdd = new List<LineStringPriority>();
				Dictionary<string, List<LineStringPriority>> binnedPriorities = new Dictionary<string, List<LineStringPriority>>();
				foreach (DataGridViewRow selectedRows in dgvMultiLineString.SelectedRows)
				{
					string facilityID = selectedRows.Cells["ID_"].Value.ToString();
					string lineString = selectedRows.Cells["LINESTRING"].Value.ToString();
					string joinPriority = selectedRows.Cells["JOIN_PRIORITY"].Value.ToString();

					try
					{
						LineStringPriority priorityToAdd = new LineStringPriority(lineString, Int32.Parse(joinPriority), facilityID);
						if (binnedPriorities.ContainsKey(facilityID))
						{
							List<LineStringPriority> temp = binnedPriorities[facilityID];
							temp.Add(priorityToAdd);
						}
						else
						{
							List<LineStringPriority> toCreate = new List<LineStringPriority>();
							toCreate.Add(priorityToAdd);
							binnedPriorities.Add(facilityID, toCreate);
						}
					}
					catch (Exception exc)
					{
						Global.WriteOutput("Could not find or create LINESTRING priorities. " + exc.Message);
					}
				}
				// bNoError is checking to make sure the user has selected at least two linestring
				// values foreach facilityID.  If they havent then the user has made an error.
				bool bNoError = true;
				foreach (string key in binnedPriorities.Keys)
				{
					List<LineStringPriority> temp = binnedPriorities[key];
					if (temp.Count < 2)
					{
						bNoError = false;
					}
				}

				if (bNoError)
				{
					List<string> newLineStrings = new List<string>();
					List<LineStringPriorityGroup> binnedResults = new List<LineStringPriorityGroup>();
					foreach (string key in binnedPriorities.Keys)
					{
						LineStringPriorityGroup priorityGroup = new LineStringPriorityGroup(binnedPriorities[key]);
						priorityGroup.SortLineStringList();

						string newLineString = JoinLineStrings(priorityGroup.sortedLineStrings);
						newLineStrings.Add(newLineString);

						priorityGroup.joinedLineString = newLineString;
						priorityGroup.facilityID = key;
						binnedResults.Add(priorityGroup);
					}



					// Now loop through the joined linestrings and add a new layer to the map.
					List<Geometry> geomColl = CreateJoinedLineStringLayer(newLineStrings);
					PopulateResultGrid(binnedResults);
					VectorLayer joinedMultiLines = new VectorLayer("JOINEDMLS");
					joinedMultiLines.DataSource = new GeometryProvider(geomColl);

					mapImageResults.Map.Layers.Clear();
					mapImageResults.Map.Layers.Insert(0, joinedMultiLines);
					mapImageResults.Refresh();
					mapImageResults.Map.ZoomToExtents();
					
				}
				else
				{
					Global.WriteOutput("Error: LINESTRING selection error.  Make sure you have selected at least two linestrings with matching facility ID's. ");
				}
			}
		}

		private void dgvJoinedLineStrings_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				HighlightSelectedLines(dgvJoinedLineStrings, mapImageResults);
			}
		}

		private void dgvJoinedLineStrings_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 17 || e.KeyValue == 38 || e.KeyValue == 40)
			{
				HighlightSelectedLines(dgvJoinedLineStrings, mapImageResults);
			}
		}

		private void dgvMultiLineString_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				HighlightSelectedLines(dgvMultiLineString, mapImageTemp);
			}
		}

		private void dgvMultiLineString_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 17 || e.KeyValue == 38 || e.KeyValue == 40)
			{
				HighlightSelectedLines(dgvMultiLineString, mapImageTemp);
			}
		}

		private void btnPan_Click(object sender, EventArgs e)
		{
			mapImageTemp.ActiveTool = SharpMap.Forms.MapImage.Tools.Pan;
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
			mapImageTemp.ActiveTool = SharpMap.Forms.MapImage.Tools.ZoomIn;
		}

		private void btnInformation_Click(object sender, EventArgs e)
		{
			mapImageTemp.ActiveTool = SharpMap.Forms.MapImage.Tools.Query;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			mapImageResults.ActiveTool = SharpMap.Forms.MapImage.Tools.Query;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			mapImageResults.ActiveTool = SharpMap.Forms.MapImage.Tools.ZoomIn;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			mapImageResults.ActiveTool = SharpMap.Forms.MapImage.Tools.Pan;
		}

		private void tsbUpdateJoinedLineStrings_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow facilityRow in dgvJoinedLineStrings.Rows)
			{
				if (facilityRow.Cells["FACILITY_ID"].Value != null && facilityRow.Cells["FACILITY_ID"].Value.ToString() != ""
					&& facilityRow.Cells["LINESTRING"].Value != null && facilityRow.Cells["LINESTRING"].Value.ToString() != "")
				{
					string facilityID = facilityRow.Cells["FACILITY_ID"].Value.ToString();
					string lineString = facilityRow.Cells["LINESTRING"].Value.ToString();
					string update = "UPDATE NETWORK_DEFINITION SET GEOMETRY = '"
						+ lineString + "' "
						+ "WHERE ID_ = " + facilityID;
					try
					{
						DBMgr.ExecuteNonQuery(update);
					}
					catch (Exception exc)
					{
						Global.WriteOutput("Error: Could not update NETWORK_DEFINITION table with new LineStrings. " + exc.Message);
					} 
				}
			}
			if (dgvMultiLineString.Rows.Count > 1)
			{
				LoadMultiLineStrings();
				LoadMultiLineDGV();
			}

		}

		//private void AddToUpdateString(string s, StringBuilder update)
		//{
		//    update.Append(s);
		//}

		//private void dgvMultiLineString_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		//{
		//    string join_priority = "";
		//    string begin_seg = "";
		//    string end_seg = "";
		//    string notes = "";
		//    string multiLineGeomID;
		//    bool bUpdate = false;
		//    StringBuilder update = new StringBuilder();
		//    update.Append("UPDATE MULTILINE_GEOMETRIES_JOIN SET ");
		//    DataGridViewRow rowToUpdate = dgvMultiLineString.Rows[e.RowIndex];

		//    if (rowToUpdate.Cells["JOIN_PRIORITY"].Value != null && rowToUpdate.Cells["JOIN_PRIORITY"].Value.ToString() != "")
		//    {
		//        join_priority = rowToUpdate.Cells["JOIN_PRIORITY"].Value.ToString();
		//        AddToUpdateString("JOIN_PRIORITY = '" + join_priority + "'", update);
		//        bUpdate = true;
		//    }
		//    if( rowToUpdate.Cells["BEGIN_SEGMENT"].Value != null && rowToUpdate.Cells["BEGIN_SEGMENT"].Value.ToString() != "")
		//    {
		//        begin_seg = rowToUpdate.Cells["BEGIN_SEGMENT"].Value.ToString();
		//        AddToUpdateString("BEGIN_SEGMENT = '" + begin_seg + "'", update);
		//        bUpdate = true;
		//    }
		//    if(rowToUpdate.Cells["END_SEGMENT"].Value != null && rowToUpdate.Cells["END_SEGMENT"].Value.ToString() != "")
		//    {
		//        end_seg = rowToUpdate.Cells["END_SEGMENT"].Value.ToString();
		//        AddToUpdateString("END_SEGMENT = '" + end_seg + "'", update);
		//        bUpdate = true;
		//    }
		//    if(rowToUpdate.Cells["NOTES"].Value != null && rowToUpdate.Cells["NOTES"].Value.ToString() != "")
		//    {
		//        notes = rowToUpdate.Cells["NOTES"].Value.ToString();
		//        AddToUpdateString("NOTES = '" + notes + "'", update);
		//        bUpdate = true;
		//    }
		//    multiLineGeomID = rowToUpdate.Cells["MULTILINE_GEOM_ID"].Value.ToString();
		//    update.Append(" WHERE MULTILINE_GEOM_ID = '" + multiLineGeomID + "'");
		//    if(bUpdate)
		//    try
		//    {
		//        DBMgr.ExecuteNonQuery(update.ToString());
		//    }
		//    catch (Exception exc)
		//    {
		//        Global.WriteOutput("Error: Could not update MULTILINE_GEOM table. " + exc.Message);
		//    }			
		//}
	}
}

public class LineStringPriority
{
	public string m_lineString;
	public int m_priority;
	public string m_facilityID;

	public LineStringPriority(string lineString, int priority, string facilityID)
	{
		m_lineString = lineString;
		m_priority = priority;
		m_facilityID = facilityID;
	}
}

public class LineStringPriorityGroup
{
	public List<LineStringPriority> m_lsPriorities = new List<LineStringPriority>();
	public List<string> sortedLineStrings;
	public string joinedLineString;
	public string facilityID;

	public LineStringPriorityGroup(List<LineStringPriority> lsPriorities)
	{
		m_lsPriorities = lsPriorities;
		sortedLineStrings = SortLineStringList();
	}

	public List<string> SortLineStringList()
	{
		List<LineStringPriority> sortedLineStringPriorities = new List<LineStringPriority>();
		List<string> sortedLineStrings = new List<string>();
		
		foreach (LineStringPriority lsp in m_lsPriorities)
		{
			m_lsPriorities.Sort(delegate(LineStringPriority lsg1, LineStringPriority lsg2) { return lsg1.m_priority.CompareTo(lsg2.m_priority); });
		}
		for (int i = 0; i < m_lsPriorities.Count; i++)
		{
			sortedLineStrings.Add(m_lsPriorities[i].m_lineString);
		}
		return sortedLineStrings;
	}
}
