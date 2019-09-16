using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpMap.Forms;
using DatabaseManager;
using RoadCareGlobalOperations;
using System.IO;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Data.Providers;

namespace RoadCare3
{
	public partial class FormMultiLineStringDefinition : BaseForm
	{
		public FormMultiLineStringDefinition()
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
				Global.WriteOutput("Error processing multiline segment definitions. " + exc.Message);
			}
		}

		private void LoadMultiLineStrings()
		{
			// This query returns values that DO NOT EXIST in the MULTILINE GEOMETRY table
			// in case the user presses the Define MultiLine geometries button more than once.
			string query = "SELECT * FROM NETWORK_DEFINITION WHERE ID_ NOT IN (SELECT DISTINCT FACILITY_ID AS ID_ FROM MULTILINE_GEOMETRIES_DEF)";
			DataSet doesNotExistInMultiLineGeometry = DBMgr.ExecuteQuery(query);

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
						tw.WriteLine("\t" + multiGeomRow["ID_"].ToString() + "\t" + ls.ToString() + "\t\t\t");
					}
				}
			}
			tw.Close();
			DBMgr.SQLBulkLoad("MULTILINE_GEOMETRIES_DEF", strOutFile, '\t');
		}

		private void LoadMultiLineDGV()
		{
			string query = "SELECT ND.ID_, ND.ROUTES, ND.DIRECTION, MG.LINESTRING, MG.BEGIN_SEGMENT, MG.END_SEGMENT, MG.NOTES, MG.MULTILINE_GEOM_ID FROM NETWORK_DEFINITION ND, MULTILINE_GEOMETRIES_DEF MG WHERE ND.ID_ = MG.FACILITY_ID";
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
				mapImageDefinition.Map.Layers.Clear();
				mapImageDefinition.Refresh();

				VectorLayer myLayer = new VectorLayer("MULTILINEGEOMETRIES");
				myLayer.DataSource = new GeometryProvider(GeomColl);
				mapImageDefinition.Map.Layers.Insert(0, myLayer);

				mapImageDefinition.Refresh();
				mapImageDefinition.Map.ZoomToExtents();
			}
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
			activeMap.Map.ZoomToExtents();
		}

		private void btnPan_Click(object sender, EventArgs e)
		{
			mapImageDefinition.ActiveTool = SharpMap.Forms.MapImage.Tools.Pan;
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
			mapImageDefinition.ActiveTool = SharpMap.Forms.MapImage.Tools.ZoomIn;
		}

		private void btnInformation_Click(object sender, EventArgs e)
		{
			mapImageDefinition.ActiveTool = SharpMap.Forms.MapImage.Tools.Query;
		}

		private void dgvMultiLineString_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				HighlightSelectedLines(dgvMultiLineString, mapImageDefinition);
			}
		}

		private void dgvMultiLineString_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 17 || e.KeyValue == 38 || e.KeyValue == 40)
			{
				HighlightSelectedLines(dgvMultiLineString, mapImageDefinition);
			}
		}

		private void AddToUpdateString(string s, StringBuilder update)
		{
			update.Append(s);
		}

		private void dgvMultiLineString_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			//string join_priority = "";
			string begin_seg = "";
			string end_seg = "";
			string notes = "";
			string multiLineGeomID;
			bool bUpdate = false;
			StringBuilder update = new StringBuilder();
			update.Append("UPDATE MULTILINE_GEOMETRIES_DEF SET ");
			DataGridViewRow rowToUpdate = dgvMultiLineString.Rows[e.RowIndex];
			if (rowToUpdate.Cells["BEGIN_SEGMENT"].Value != null && rowToUpdate.Cells["BEGIN_SEGMENT"].Value.ToString() != "")
			{
				begin_seg = rowToUpdate.Cells["BEGIN_SEGMENT"].Value.ToString();
				AddToUpdateString("BEGIN_SEGMENT = '" + begin_seg + "', ", update);
				bUpdate = true;
			}
			if (rowToUpdate.Cells["END_SEGMENT"].Value != null && rowToUpdate.Cells["END_SEGMENT"].Value.ToString() != "")
			{
				end_seg = rowToUpdate.Cells["END_SEGMENT"].Value.ToString();
				AddToUpdateString("END_SEGMENT = '" + end_seg + "', ", update);
				bUpdate = true;
			}
			if (rowToUpdate.Cells["NOTES"].Value != null && rowToUpdate.Cells["NOTES"].Value.ToString() != "")
			{
				notes = rowToUpdate.Cells["NOTES"].Value.ToString();
				AddToUpdateString("NOTES = '" + notes + "', ", update);
				bUpdate = true;
			}
            update = update.Replace(",", "", (update.Length - 3), 3);
			multiLineGeomID = rowToUpdate.Cells["MULTILINE_GEOM_ID"].Value.ToString();
			update.Append(" WHERE MULTILINE_GEOM_ID = '" + multiLineGeomID + "' ");
			if (bUpdate)
				try
				{
					DBMgr.ExecuteNonQuery(update.ToString());
				}
				catch (Exception exc)
				{
					Global.WriteOutput("Error: Could not update MULTILINE_GEOM_DEFINITION table. " + exc.Message);
				}
		}

		
	}
}
