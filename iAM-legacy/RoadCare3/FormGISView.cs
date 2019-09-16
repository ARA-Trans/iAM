using System;
using System.Collections;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

using System.IO;
using DatabaseManager;
using System.Xml.Serialization;
using WeifenLuo.WinFormsUI.Docking;

using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Forms;
using SharpMap.Data;
using SharpMap.Data.Providers;
using OpenGISExtension;
using System.Collections.Generic;
using RoadCare3.Properties;
using RoadCareDatabaseOperations;
using DataObjects;
using RoadCareGlobalOperations;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using System.Collections.ObjectModel;


namespace RoadCare3
{
    public partial class FormGISView : BaseForm
    {
        //private LayerConfiguration m_layerConfig = new LayerConfiguration();
        private List<String> m_listGeomIDs = new List<String>(); 
        private Hashtable m_htGeometryOldColors = new Hashtable();
        private Hashtable m_htAdvancedSearchOldColors = new Hashtable();
        private Hashtable m_htAttributeYears;
        private List<String> m_listColumns = new List<String>();
        private String m_strAttribute;
        private String m_strYear;
		private GISNetworkSettings m_gisNetworkSettings;
		private String m_strSimulationID = "";

        private String m_strNetwork;
        private String m_strNetworkID;
		private Hashtable m_hashSimulationSimulationID =new Hashtable();
        List<SharpMap.Geometries.Geometry> geometries;
        SharpMap.Layers.VectorLayer m_layerImageView;
        public FormGISLayerManager m_formGISLayerManager;

        public FormGISView(String strNetworkName, Hashtable htAttributeYears)
        {
            InitializeComponent();
            //normalButtonColor = btnPan.BackColor;
            m_strNetwork = strNetworkName;
            m_htAttributeYears = htAttributeYears;
            String strQuery = "Select NETWORKID From NETWORKS Where NETWORK_NAME = '" + m_strNetwork + "'";
            DataSet ds = DBMgr.ExecuteQuery(strQuery);
            m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();

            Global.LoadAttributes();

			// Good. Create the GISNetworkSettings so the user can save GIS attributes
			m_gisNetworkSettings = new GISNetworkSettings();



        }

        public String NetworkID
        {
            get
            {
                return m_strNetworkID;
            }
        }

        public String Attribute
        {
            get { return m_strAttribute; }
            set { m_strAttribute = value; }
        }

        public String Year
        {
            get { return m_strYear; }
            set { m_strYear = value; }
        }

		public String SimulationID
		{
			get
			{
				return m_strSimulationID;
			}
			set
			{
				m_strSimulationID = value;
			}
		}

        public MapImage MapImage
        {
            get { return MainMapImage; }
        }

		protected void SecureForm()
		{
			//throw new NotImplementedException();
		}

        private void FormGISView_Load(object sender, EventArgs e)
        {
			SecureForm();
			FormLoad(Settings.Default.GIS_VIEW_IMAGE_KEY, Settings.Default.GIS_VIEW_IMAGE_KEY_SELECTED);
            MainMapImage.Map.BackColor = System.Drawing.Color.White;

            // Set the tab text to the name of the associated network
            this.TabText = "GIS-" + m_strNetwork;
			
			/* TEST CODE */
			//SharpMap.Layers.VectorLayer layCountries = new SharpMap.Layers.VectorLayer("Countries");
			//MainMapImage.Image = Image.FromFile(@"C:\Documents and Settings\cbecker\Desktop\9282_08.tif");

			/* END TEST CODE */


			
			// We need to load the simulations combo box with all simulations for a given networkID
			String query = "SELECT SIMULATION, SIMULATIONID FROM SIMULATIONS WHERE NETWORKID = '" + m_strNetworkID + "'";
			try
			{
				toolStripComboBoxSimulations.Items.Add("");
				DataSet dsSimulation = DBMgr.ExecuteQuery(query);
				foreach (DataRow dataRow in dsSimulation.Tables[0].Rows)
				{
					String strSimulationName = dataRow["SIMULATION"].ToString();
					String strSimulationID = dataRow["SIMULATIONID"].ToString();
					m_hashSimulationSimulationID.Add(strSimulationName, strSimulationID);
					toolStripComboBoxSimulations.Items.Add(strSimulationName);
				}
			}
			catch (Exception exception)
			{
				Global.WriteOutput("Error: Loading simulations." + exception.Message);

			}
			Global.Attributes.Clear();
			Global.LoadAttributes();
            if (this.ImageView)
            {

                geometries = new List<SharpMap.Geometries.Geometry>();
                m_layerImageView = new SharpMap.Layers.VectorLayer("Image View");
                m_layerImageView.Tag = "ImageView";
                m_layerImageView.DataSource = new SharpMap.Data.Providers.GeometryProvider(geometries);
                m_layerImageView.Style.Symbol = (Bitmap)imageListDefaultSymbols.Images[1];
                //m_layerImageView.Style.SymbolScale = 2;

                MainMapImage.Map.Layers.Add(m_layerImageView);

                m_formGISLayerManager = new FormGISLayerManager(m_strNetwork, m_htAttributeYears);
                m_formGISLayerManager.MapImage = this.MainMapImage;
                m_formGISLayerManager.GISView = this;
            }
            else
            {
                toolStripButtonManager.Visible = false;
            }

        }

        private void FormGISView_FormClosed(object sender, FormClosedEventArgs e)
		{
            if (this.ImageView)
            {
                m_formGISLayerManager.Close();
                return;
            }
			FormUnload();
			//SegSym.Clear();
			FormGISLayerManager formGISLayerManager;

			if (FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
			{
				List<LevelColors> colors = null;
				foreach (String attributeName in formGISLayerManager.m_hashAttributeLevelColor.Keys)
				{
					colors = (List<LevelColors>)formGISLayerManager.m_hashAttributeLevelColor[attributeName];
					m_gisNetworkSettings.AddAttributeColorList(attributeName, colors);
				}
				FormManager.AllGISSettings.AddGISNetworkSettings(m_strNetwork, m_gisNetworkSettings);
			}
			// Remove the current GIS view from the list of open GIS views
			FormManager.RemoveFormGISView(this);

			// If no more GIS views are open, then close the GIS layer manager as well.
			if (FormManager.ListGISForms.Count == 0)
			{
				// Close the associated layer manager form, if it is open.
				if (FormManager.formGISLayerManager != null)
				{
					FormManager.formGISLayerManager.Close();
				}

				// Close the asset manager tabs as well
				if (FormManager.attributeTab != null)
				{
					FormManager.attributeTab.Close();
				}
				if (FormManager.assetTab != null)
				{
					FormManager.assetTab.Close();
				}
			}
			this.Dispose();
		}

        private void FormGISView_Activated(object sender, EventArgs e)
        {
            // Change the GISLayerManger form to reflect the data in the selected tab.  This is done by setting the object variables
            // inside of FormGISLayerManager, and accessing the object from the FormManager.
            if (!this.IsDisposed)
            {
                FormGISLayerManager formGISLayerManager;
                if (FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
                {
                    formGISLayerManager.MapImage = this.MainMapImage;
                    formGISLayerManager.networkID = m_strNetworkID;
                    formGISLayerManager.ClearLayerManagerTreeView();
                    formGISLayerManager.UpdateLayerConfiguration(this);
					FormManager.formGISLayerManager.GISView = this;
                }
            }
        }

        private Color GetLevelColor(String strAttribute, String strValue, List<LevelColors> listLC, bool? bLCAscending)
        {
            if (Global.GetAttributeType(strAttribute) == "STRING")
            {
                LevelColors lcTemp = listLC.Find(delegate(LevelColors lc) { return lc.m_strLevel == strValue; });
                if (lcTemp != null)
                {
                    return lcTemp.m_Color;
                }
                else
                {
                    return System.Drawing.Color.FromArgb(0, Color.Gray);
                }
            }
            else
            {
                float fValue = 0;
				if (strValue == "")
				{
					return System.Drawing.Color.FromArgb(0, Color.Gray);
				}
                try
                {
                    fValue = float.Parse(strValue);
                }
                catch
                {
					return System.Drawing.Color.FromArgb(0, Color.Gray);
                }
                
                // Ascending attribute
                if (bLCAscending == true)
                {
                    for (int i = 0; i < listLC.Count - 1; i++)
                    {
                        if (fValue < float.Parse(listLC[i].m_strLevel))
                        {
                            return listLC[i].m_Color;
                        }
                    }
                }
                // Descending attribute
                else if (bLCAscending == false)
                {
                    for (int i = 0; i < listLC.Count - 1; i++)
                    {
                        if (fValue > float.Parse(listLC[i].m_strLevel))
                        {
                            return listLC[i].m_Color;
                        }
                    }
                }
                else
                {
                    return Color.Gray;
                }
                return listLC[listLC.Count - 1].m_Color;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //LoadBaseGeometry(Color.Black);
			LoadBaseGeometry(Color.Orange);
			MainMapImage.Map.BackColor = Color.Black;
        }

        private void LoadBaseGeometry(Color layerColor)
        {
            Hashtable htGeoIds = new Hashtable();
            // First check to see if the NETWORK_MAP layer already exists in the layers collection.
            // if it does, then we dont want to add it again.
            if (MainMapImage.Map.Layers.Find(delegate(ILayer il) { return il.LayerName == "NETWORK MAP"; }) == null)
			//if(MainMapImage.Map.Layers["NETWORK MAP"] != null)
            {
                // Get the geometries for defined networks, if they exist.
                DataSet ds;
            
                String strQuery = "SELECT GEOMETRY, SECTIONID FROM SECTION_" + m_strNetworkID;
                ds = DBMgr.ExecuteQuery(strQuery);

                List<Geometry> GeomColl = new List<Geometry>();
                //LineString lineSeg;
				Geometry lineSeg;

                int iCount = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row[0] != null && row[0].ToString() != "" )
                    {
                        //lineSeg = (LineString)Geometry.GeomFromText(row[0].ToString());
						lineSeg = Geometry.GeomFromText(row[0].ToString());
                        lineSeg.Tag = row["SECTIONID"].ToString();        
                        lineSeg.Color = layerColor;
                        GeomColl.Add(lineSeg);
                        htGeoIds.Add(row["SECTIONID"].ToString(), lineSeg);
                    }
                    iCount++;
                }

                SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("NETWORK MAP");
                myLayer.Tag = "NETWORK MAP";
                myLayer.DataSource = new SharpMap.Data.Providers.GeometryProvider(GeomColl);
                myLayer.GeoIDs = htGeoIds;

				try
				{
					MainMapImage.Map.Layers.Insert(0, myLayer);
					MainMapImage.Map.ZoomToExtents();
					MainMapImage.Refresh();

					FormGISLayerManager formGISLayerManager;
					if (FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
					{
						formGISLayerManager.AddBaseMap();
					}
				}
				catch (Exception exc)
				{
					Global.WriteOutput("Error: Could not get NETWORK MAP from NETWORK_DEFINITION table. " + exc.Message);
					//MainMapImage.Map.Layers.Remove(MainMapImage.Map.Layers["NETWORK MAP"]);
					int iRemoveIndex = MainMapImage.Map.Layers.FindIndex(delegate(SharpMap.Layers.ILayer layer) { return layer.LayerName == "NETWORK MAP"; });
                    MainMapImage.Map.Layers.RemoveAt(iRemoveIndex);
				}
            }
        }

        public void DisplayAttribute(String strAttribute, String strYear, List<LevelColors> listLC, int width)
        {
			String strFrom = DBOp.BuildFromStatement(m_strNetworkID, m_strSimulationID, true);
            String strCol;
            if(strYear == "Most Recent")
            {
                strCol = strAttribute;
            }
            else
            {
                strCol = strAttribute + "_" + strYear;
            }
			String strQuery = "SELECT " + strCol + ", SECTION_" + m_strNetworkID + ".SECTIONID, GEOMETRY, FACILITY " + strFrom;
            if (tbAdvancedSearch.Text != "")
            {
                strQuery += " WHERE " + tbAdvancedSearch.Text;
            }
            try
            {
                DataReader dr = new DataReader(strQuery);
                List<Geometry> GeomColl = new List<Geometry>();
                Hashtable htGeoIDs = new Hashtable();

                // Check the level colors list and determine if it is ascending or descending
                bool ? bLCAscending = null;
                if (Global.GetAttributeType(strAttribute) == "NUMBER")
                {
                    bLCAscending = ListLevelColorsIsAscending(listLC);
                }
                
                while (dr.Read())
                {
                    if (dr["FACILITY"].ToString() == "CR18")
                    { }
                    if (dr["GEOMETRY"].ToString() != "")
                    {
                        //LineString lineString = (LineString)SharpMap.Geometries.Geometry.GeomFromText(dr["GEOMETRY"].ToString());
						Geometry lineString = Geometry.GeomFromText(dr["GEOMETRY"].ToString());
                        lineString.Color = GetLevelColor(strAttribute, dr[0].ToString(), listLC, bLCAscending);
						lineString.Width_ = width;
                        GeomColl.Add(lineString);
                        htGeoIDs.Add(dr["SECTIONID"].ToString(), lineString);
                    }
                }
                dr.Close();
                SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer(strAttribute + "-" + strYear);
                myLayer.Tag = "ATTRIBUTE";
                myLayer.DataSource = new SharpMap.Data.Providers.GeometryProvider(GeomColl);
                myLayer.GeoIDs = htGeoIDs;

                // Remove the previous attribute layer from the GIS viewer
				ILayer vlToRemove = MainMapImage.Map.Layers.Find(delegate(ILayer vl) { return vl.Tag.ToString() == "ATTRIBUTE"; });
				//ILayer vlToRemove = MainMapImage.Map.Layers["ATTRIBUTE"];
                if (vlToRemove != null)
                {
                    MainMapImage.Map.Layers.Remove((VectorLayer)vlToRemove);
					MainMapImage.Refresh();
                }
                // Add the new one
				MainMapImage.Map.Layers.Insert(0, myLayer);
                MainMapImage.Map.ZoomToExtents();
                MainMapImage.Refresh();
            }
            catch (Exception e)
            {
                Global.WriteOutput("Error: " + e.Message);
            }

        }

        private bool? ListLevelColorsIsAscending(List<LevelColors> listLC)
        {
            bool? bListIsAscending = null;
			bool bMissingLevels = false;
            float fNext;
            float fCurr;
            for (int i = 0; i < listLC.Count - 2; i++)
            {

				// If the user didnt enter any level information for the attribute, then we need to handle
				// that case in the GIS viewer.
				if (listLC[i].m_strLevel != "")
				{
					fCurr = float.Parse(listLC[i].m_strLevel);
					fNext = float.Parse(listLC[i + 1].m_strLevel);
					if (bListIsAscending == null)
					{
						if (fNext > fCurr)
						{
							bListIsAscending = true;
						}
						else if (fNext < fCurr)
						{
							bListIsAscending = false;
						}
					}
					else
					{
						if (bListIsAscending == true && fNext < fCurr)
						{
							Global.WriteOutput("Error: From level one to level five, values must increase, decrease, or remain the same.");
							return null;
						}
						if (bListIsAscending == false && fNext > fCurr)
						{
							Global.WriteOutput("Error: From level one to level five, values must increase, decrease, or remain the same.");
							return null;
						}
					}
				}
				else
				{
					bMissingLevels = true;
				}
            }
			if (bMissingLevels)
			{
				Global.WriteOutput("Missing attribute level information. Check attribute properties.");
			}
            if (bListIsAscending == null)
            {
                return true;
            }
            else
            {
                return bListIsAscending;
            }

            
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
			MainMapImage.ActiveTool = MapImage.Tools.ZoomIn;
            //MainMapImage.Map.Zoom = MainMapImage.Map.Zoom / 2;
            //MainMapImage.Refresh();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
			MainMapImage.ActiveTool = MapImage.Tools.ZoomOut;
            //MainMapImage.Map.Zoom = MainMapImage.Map.Zoom * 2;
            //MainMapImage.Refresh();
        }

        private void btnInformation_Click(object sender, EventArgs e)
        {
            MainMapImage.ActiveTool = MapImage.Tools.Query;
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            MainMapImage.ActiveTool = SharpMap.Forms.MapImage.Tools.Pan;
        }

        private void MainMapImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (MainMapImage.ActiveTool == MapImage.Tools.Query)
            {
                QueryMap(e);
            }
            if (MainMapImage.ActiveTool == MapImage.Tools.ZoomIn)
            { }
            if (MainMapImage.ActiveTool == MapImage.Tools.ZoomOut)
            { }
        }

        private void QueryMap(MouseEventArgs e)
        {
			if( MainMapImage.Map.Layers.Count > 0 )
			{
				// We run different queries for the different type of layers being displayed.
				// there are currently three defined types.  These types are stored in the tag field of the layer.
				// ATTRIBUTE, SHAPEFILE, ASSET
				// SHAPEFILE is a special case as it can have any type of shape associated with it.
				// ATTRIBUTE is always of GEOMETRY type LINESTRING
				// ASSET is a special case and is always represented (currently) using POINT geoms.
				String strTag = MainMapImage.Map.Layers[0].Tag.ToString();
				switch( strTag )
				{
					case "ASSET":
						AssetMapQuery( e );
						break;
					case "ATTRIBUTE":
						AttributeMapQuery( e );
						break;
					case "SHAPEFILE":
						ShapefileMapQuery( e );
						break;
					case "NETWORK MAP":
						AttributeMapQuery( e );
						break;
					default:
						break;
				}
			}
            
        }

        private void ShapefileMapQuery(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void AttributeMapQuery(MouseEventArgs e)
        {
            // Get the section of the road that the user selected, and any information regarding the segmented
            // attribute.
            ApplyOldColorsToGeoms(false);
            PointF ptF = new PointF((float)e.X, (float)e.Y);
            SharpMap.Geometries.Point pt = MainMapImage.Map.ImageToWorld(ptF);
            String strQuery = "SELECT SECTIONID, FACILITY, SECTION, BEGIN_STATION, END_STATION,"
                             + " DIRECTION, AREA, UNITS, GEOMETRY FROM SECTION_" + m_strNetworkID
                             + " WHERE Envelope_MinX <"
                             + pt.X * (pt.X > 0 ? 1.001 : 0.999) + " AND " + pt.X * (pt.X > 0 ? 0.999 : 1.001) + " < Envelope_MaxX AND Envelope_MinY <"
                             + pt.Y * (pt.Y > 0 ? 1.001 : 0.999) + " AND " + pt.Y * (pt.Y > 0 ? 0.999 : 1.001) + " < Envelope_MaxY";
            try
            {
                // Get the list of geometries whose bounding boxes contain the selected point.
                DataSet ds = DBMgr.ExecuteQuery(strQuery);
                Geometry geoNearest = null;
                double dMinDistance = double.PositiveInfinity;
                double dDistance;
				int iSectionID = -1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    VectorLayer myLayer = (VectorLayer)MainMapImage.Map.Layers[0];
                    Geometry geo = (Geometry)myLayer.GeoIDs[dr["SECTIONID"].ToString()];

					// Find the smallest distance between the clicked point and the geometries that met the selection
					// criteria from the above select statement.
					if (geo != null)
					{
						dDistance = geo.Distance(pt);

						if (dDistance < dMinDistance)
						{
							dMinDistance = dDistance;
							geoNearest = geo;

							// Also, grab the section id of the clicked point
							iSectionID = Int32.Parse(dr["SECTIONID"].ToString());
						}
					}
                }
				if (geoNearest != null)
				{
					m_htGeometryOldColors.Add(geoNearest, geoNearest.Color);
					geoNearest.Width_ = 5;
					geoNearest.Color = Color.Cyan;
					MainMapImage.Refresh();

					try
					{
						AttributeTab attributeTab;
						if (FormManager.IsAttributeTabOpen(out attributeTab))
						{
							AssetTab assetTab;
							if (FormManager.IsAssetTabOpen(out assetTab))
							{
								assetTab.Hide();
							}
							if (attributeTab.IsHidden)
							{
								attributeTab.Show();
							}
							attributeTab.SetSectionID(iSectionID);
							attributeTab.PopulateComboBoxYears();
						}
					}
					catch (Exception exc)
					{
						Global.WriteOutput("Error: Problem querying SEGMENT_" + m_strNetworkID + "_NS0 table. " + exc.Message);
					}
				}
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not get ATTRIBUTE or NETWORK MAP data. " + exc.Message);
            }                  
        }

        private void AssetMapQuery(MouseEventArgs e)
        {
            PointF ptF = new PointF((float)e.X, (float)e.Y);
            SharpMap.Geometries.Point pt = MainMapImage.Map.ImageToWorld(ptF);
            BoundingBox bbox = pt.GetBoundingBox().Grow(MainMapImage.Map.PixelSize * 10);
			ConnectionParameters cp;
            String strLayerName = MainMapImage.Map.Layers[0].LayerName;
			if (strLayerName.Contains("("))
			{
				strLayerName = strLayerName.Substring(0, strLayerName.IndexOf("("));
				
			}
			cp = DBMgr.GetAssetConnectionObject(strLayerName);
            String strSelect = "SELECT * FROM ASSETS WHERE ASSET = '" + strLayerName + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            if (ds.Tables[0].Rows.Count > 0)
            {
				// The results of this query are fairly small, so we can use ADO to store the data in a table for use later.
                strSelect = "SELECT * FROM " + strLayerName + " WHERE ";
                strSelect += bbox.Left.ToString() + " < EnvelopeMinX AND EnvelopeMaxX < " + bbox.Right.ToString() + " AND EnvelopeMinY < " + bbox.Top.ToString() + " AND EnvelopeMaxY > " + bbox.Bottom.ToString();
                ds = DBMgr.ExecuteQuery(strSelect,cp);
				ds.Tables[0].TableName = strLayerName;

				AssetTab assetTab;
				if (FormManager.IsAssetTabOpen(out assetTab))
				{
					AttributeTab attributeTab;
					if (FormManager.IsAttributeTabOpen(out attributeTab))
					{
						attributeTab.Hide();
					}
					if (assetTab.IsHidden)
					{
						assetTab.Show();
					}
					assetTab.UpdateAssetData(ds.Tables[0]);
				}

				//FormAssetViewer formAssetViewer;
				//if (FormManager.IsFormAssetViewerOpen(out formAssetViewer))
				//{
				//    formAssetViewer.EnableMapQuery("tabPageASSET", -1);
				//    formAssetViewer.UpdateAssetData(ds.Tables[0]);
				//}
            }
        }

        private void btnAdvancedSearch_Click(object sender, EventArgs e)
        {
            if (MainMapImage.Map.Layers.Count > 0)
            {
                String strTag = MainMapImage.Map.Layers[0].Tag.ToString();
                if (strTag == "ASSET")
                {
                    AssetAdvancedSearch();
                }
                if (strTag == "NETWORK MAP")
                {
                    NetworkAdvancedSearch();
                }
                if (strTag == "ATTRIBUTE")
                {
                    AttributeAdvancedSearch();
                }
            }
            else
            {
                AttributeAdvancedSearch();
            }
        }

        private void AttributeAdvancedSearch()
        {
            String strQuery = tbAdvancedSearch.Text;
			FormAdvancedSearch form = new FormAdvancedSearch(m_strNetworkID, m_strSimulationID, tbAdvancedSearch.Text);
            form.Text = "Attribute/Network Map";
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbAdvancedSearch.Text = form.GetWhereClause();
                if (!String.IsNullOrEmpty(m_strYear) && !String.IsNullOrEmpty(m_strAttribute))
                {
                    FormGISLayerManager formGISLayerManager;
                    if (FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
                    {
                        DisplayAttribute(m_strAttribute, m_strYear, formGISLayerManager.GetLayerColors(m_strAttribute, m_strYear, m_strNetworkID), 1);
                    }
                }
            }
        }

        private void NetworkAdvancedSearch()
        {
            if (MainMapImage.Map.Layers.Count > 0)
            {
                ApplyOldColorsToGeoms(true);
                String strQuery = tbAdvancedSearch.Text;
                FormAdvancedSearch form = new FormAdvancedSearch(m_strNetworkID, m_strSimulationID, tbAdvancedSearch.Text);
                form.Text = "Attribute/Network Map";
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbAdvancedSearch.Text = form.GetWhereClause();
                    m_htAdvancedSearchOldColors.Clear();
                    String strSelect;
                    strSelect = "SELECT SECTION_" + m_strNetworkID + ".SECTIONID";

					String strFrom = DBOp.BuildFromStatement(m_strNetworkID, m_strSimulationID, true);
					strSelect += strFrom;

                    String strWhere = tbAdvancedSearch.Text;
                    strSelect += " WHERE " + strWhere;

                    String strSectionID;
                    Geometry geo;
                    try
                    {
                        DataReader dr = new DataReader(strSelect);
                        while (dr.Read())
                        {
                            strSectionID = dr["SECTIONID"].ToString();
                            geo = (Geometry)MainMapImage.Map.Layers[0].GeoIDs[strSectionID];
							if (geo != null)
							{
								m_htAdvancedSearchOldColors.Add(geo, geo.Color);
								geo.Color = Color.Gold;
								geo.Width_ = 3;
							}
                        }
                        dr.Close();
                    }
                    catch (Exception exc)
                    {
                        Global.WriteOutput("Error: Advanced attribute search failed. " + exc.Message);
                        return;
                    }
                    MainMapImage.Refresh();
                }
            }
        }

        private void AssetAdvancedSearch()
        {
            String strQuery = "";
            if (MainMapImage.Map.Layers.Count > 0)
            {
                strQuery = tbAdvancedSearch.Text;
                FormQueryRaw form = new FormQueryRaw(MainMapImage.Map.Layers[0].LayerName, strQuery);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    tbAdvancedSearch.Text = form.m_strQuery;

                    // Find the geoms that match the where clause of the query.  Then redraw the layer with only
                    // those geoms?  Or highlight those geoms?
                    List<Geometry> listGeoms = new List<Geometry>();
                    strQuery = "SELECT GEO_ID FROM " + MainMapImage.Map.Layers[0].LayerName + " WHERE " + form.m_strQuery;
                    DataReader dr;
                    try
                    {
                        dr = new DataReader(strQuery);
                        while (dr.Read())
                        {
                            try
                            {
                                m_listGeomIDs.Add(dr["GEO_ID"].ToString());
                            }
                            catch (Exception excInner)
                            {
                                Global.WriteOutput("Warning: Point not valid." + excInner.Message + " " + dr[0].ToString());
                            }
                        }
                        dr.Close();
                    }
                    catch (Exception exc)
                    {
                        Global.WriteOutput("Error: Failed to load asset geometries." + exc.Message);
                    }

                    // Add the new selection to the combobox and dgv
                    String strSelect = "SELECT * FROM " + MainMapImage.Map.Layers[0].LayerName + " WHERE GEO_ID = '";
                    String strWhere = "";
                    for (int i = 0; i < m_listGeomIDs.Count; i++)
                    {
                        if (i < m_listGeomIDs.Count - 1)
                        {
                            strWhere += m_listGeomIDs[i] + "' OR GEO_ID = '";
                        }
                        else
                        {
                            strWhere += m_listGeomIDs[i] + "'";
                        }
                    }
                    strSelect += strWhere;
                    try
                    {
                        DataSet ds = DBMgr.ExecuteQuery(strSelect);
                        FormGISLayerManager formGISLayerManager;
                        if (FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
                        {
                            formGISLayerManager.SetAdvSearchColor(Color.Cyan, m_listGeomIDs);
                            m_listGeomIDs.Clear();
                        }
                    }
                    catch (Exception exc)
                    {
                        Global.WriteOutput("Error: " + exc.Message);
                    }
                }
            }
        }

        public List<String> GetAdvancedSearchGeomIDList()
        {
            return m_listGeomIDs;
        }

        /// <summary>
        /// Gets the member hashtable m_htGeometryOldColors     // key: Geometry, value: Old color of geom
        /// </summary>
        private void ApplyOldColorsToGeoms(bool bAdvancedSearch)
        {
            if (bAdvancedSearch == false)
            {
                foreach (Geometry geo in m_htGeometryOldColors.Keys)
                {
                    Color c = (Color)m_htGeometryOldColors[geo];
                    geo.Color = c;
                    geo.UseCustomWidth = false;
                }
                m_htGeometryOldColors.Clear();
            }
            else
            {
                foreach (Geometry geo in m_htAdvancedSearchOldColors.Keys)
                {
                    Color c = (Color)m_htAdvancedSearchOldColors[geo];
                    geo.Color = c;
                    geo.UseCustomWidth = false;
                }
                m_htAdvancedSearchOldColors.Clear();
            }
            MainMapImage.Refresh();
        }

		private void MainMapImage_MouseDrag(SharpMap.Geometries.Point WorldPos, System.Windows.Forms.MouseEventArgs ImagePos)
		{
			
		}

		private void toolStripComboBoxSimulations_SelectedIndexChanged(object sender, EventArgs e)
		{
			String strSimulation = toolStripComboBoxSimulations.Text;
			if (strSimulation == "")
			{
				m_strSimulationID = "";
			}
			else
			{
				m_strSimulationID = m_hashSimulationSimulationID[strSimulation].ToString();
			}

			FormManager.formGISLayerManager.AttributeYear = Global.GetAttributeYear(m_strNetworkID, m_strSimulationID);

			m_htAttributeYears = FormManager.formGISLayerManager.AttributeYear;
			FormManager.formGISLayerManager.ReloadYearComboBox();
			FormManager.formGISLayerManager.GISView = this;
		}

        private void MainMapImage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (this.ImageView)
			if(true)
            {
                PointF ptF = new PointF((float)e.X, (float)e.Y);
                SharpMap.Geometries.Point pt = MainMapImage.Map.ImageToWorld(ptF);
				
#if DDOT
                TransformGeo(pt);
#endif


                String strQuery;
                if (pt.X < 0)
                {
                    strQuery = "SELECT SECTIONID, FACILITY, SECTION, BEGIN_STATION, END_STATION,"
                                     + " DIRECTION, AREA, UNITS, GEOMETRY FROM SECTION_" + m_strNetworkID
                                     + " WHERE Envelope_MinX >"
                                     + pt.X * 1.001 + " AND " + pt.X * 0.999 + " > Envelope_MaxX AND Envelope_MinY <"
                                     + pt.Y * 1.001 + " AND " + pt.Y * 0.999 + " < Envelope_MaxY";
                }
                else
                {
                    strQuery = "SELECT SECTIONID, FACILITY, SECTION, BEGIN_STATION, END_STATION,"
                                     + " DIRECTION, AREA, UNITS, GEOMETRY FROM SECTION_" + m_strNetworkID
                                     + " WHERE Envelope_MinX <"
                                     + pt.X * 1.001 + " AND " + pt.X * 0.999 + " < Envelope_MaxX AND Envelope_MinY <"
                                     + pt.Y * 1.001 + " AND " + pt.Y * 0.999 + " < Envelope_MaxY";
                }
                try
                {
                    // Get the list of geometries whose bounding boxes contain the selected point.
                    DataSet ds = DBMgr.ExecuteQuery(strQuery);
                    Geometry geoNearest = null;
                    double dMinDistance = double.PositiveInfinity;
                    double dDistance;
                    String strFacility = "";
                    String strSection = "";

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        VectorLayer myLayer = (VectorLayer)MainMapImage.Map.Layers[0];
                        Geometry geo = (Geometry)myLayer.GeoIDs[dr["SECTIONID"].ToString()];

                        // Find the smallest distance between the clicked point and the geometries that met the selection
                        // criteria from the above select statement.
                        dDistance = geo.Distance(pt);
                        if (dDistance < dMinDistance)
                        {
                            dMinDistance = dDistance;
                            geoNearest = geo;
                            strFacility = dr["FACILITY"].ToString();
                            strSection = dr["SECTION"].ToString();
                        }
                    }
                    if (!String.IsNullOrEmpty(strFacility) && !String.IsNullOrEmpty(strSection))
                    {
                        String strSelect = "SELECT SECTIONID,DIRECTION,BEGIN_STATION FROM SECTION_" + m_strNetworkID + " WHERE FACILITY='" + strFacility + "' AND SECTION='" + strSection + "'";
                        DataSet dsNavigation = DBMgr.ExecuteQuery(strSelect);
                        foreach (DataRow row in dsNavigation.Tables[0].Rows)
                        {
                            String strID = row["SECTIONID"].ToString();
                            int nSectionID = int.Parse(strID);
                            if (nSectionID < 1000000)
                            {
                                String strDirection = row["DIRECTION"].ToString();
                                String strBeginStation = row["BEGIN_STATION"].ToString();
                                m_event.issueEvent(new NavigationEvent(strFacility, strDirection, double.Parse(strBeginStation)));
                            }
                            else
                            {
                                m_event.issueEvent(new NavigationEvent(strFacility, strSection));
                            }
                        }
                    }
                }
                catch (Exception except)
                {
					Global.WriteOutput( "Error handling double click: " + except.Message );

                }
            }
        }
        public override void NavigationTick(NavigationObject navigationObject)
        {
            geometries.Clear();
            //Add two points
            SharpMap.Geometries.Point point = new SharpMap.Geometries.Point(navigationObject.CurrentImage.Longitude ,navigationObject.CurrentImage.Latitude);
            SharpMap.Geometries.Point pointTranformed =(SharpMap.Geometries.Point) TransformGeo(point);

            geometries.Add(pointTranformed);
            MainMapImage.Map.Center.X = pointTranformed.X;
            MainMapImage.Map.Center.Y = pointTranformed.Y; 
            MainMapImage.Refresh();
        }

        private void toolStripButtonManager_Click(object sender, EventArgs e)
        {
            m_formGISLayerManager.Show(this.DockingPanel,DockState.DockLeftAutoHide);
        }

		private IGeometry TransformGeo(IGeometry g)
		{
            Geometry geo = (Geometry)g;
#if DDOT
            string wkt = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";
            CoordinateSystemFactory cFacWGS = new CoordinateSystemFactory();
            ICoordinateSystem wgs84 = cFacWGS.CreateFromWkt(wkt);

            //
            // Acquire the state plane project from
            // http://spatialreference.org/
            //wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Meter\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",1312333.333333333],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-77],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Foot_US\",0.30480060960121924]]";
            //wkt = "PROJCS["NAD_1983_StatePlane_Florida_West_FIPS_0902_Feet",GEOGCS["GCS_North_American_1983",DATUM["D_North_American_1983",SPHEROID["GRS_1980",6378137,298.257222101]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Transverse_Mercator"],PARAMETER["False_Easting",656166.6666666665],PARAMETER["False_Northing",0],PARAMETER["Central_Meridian",-82],PARAMETER["Scale_Factor",0.9999411764705882],PARAMETER["Latitude_Of_Origin",24.33333333333333],UNIT["Foot_US",0.30480060960121924]]"
			wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Meter\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",399999.2],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-77],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Meter\",1]]";
            //wkt = "PROJCS[\"GRS_1980_Transverse_Mercator\",GEOGCS[\"GCS_GRS_1980\",DATUM[\"D_GRS_1980\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"Scale_Factor\",1.00005000],PARAMETER[\"False_Easting\",164041.66666667],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-96.68805556],PARAMETER[\"Latitude_Of_Origin\",40.25000000],UNIT[\"Foot_US\",0.30480060960121924]]";
            CoordinateSystemFactory cFac1 = new CoordinateSystemFactory();
            ICoordinateSystem nad83 = cFac1.CreateFromWkt(wkt);


            CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();
            ICoordinateTransformation transDeg2M = ctFac.CreateFromCoordinateSystems(nad83, wgs84);  //Geocentric->Geographic (WGS84)
            geo = GeometryTransform.TransformGeometry((Geometry)g, transDeg2M.MathTransform);
#endif
			return geo;
		}

		private void MainMapImage_Click(object sender, EventArgs e)
		{

		}
    }

    [Serializable]
    public class DynamicSegmentationConfiguration
    {
        public List<LevelColors> m_listLC;
        public String strFilePathName;

        private String strAttributeName;
        private String strAttributeYear;
        private bool bIsDynSeg = true;

        public DynamicSegmentationConfiguration()
        {

        }

        public bool IsDynSeg
        {
            get { return bIsDynSeg; }
            set { bIsDynSeg = value; }
        }

        public String AttributeName
        {
            get { return strAttributeName; }
            set { strAttributeName = value; }
        }

        public String AttributeYear
        {
            get { return strAttributeYear; }
            set { strAttributeYear = value; }
        }

        public void Serialize(string file, DynamicSegmentationConfiguration c)
        {
            XmlSerializer xs = new XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(file);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }
        public DynamicSegmentationConfiguration Deserialize(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DynamicSegmentationConfiguration));
            StreamReader reader = File.OpenText(file);
            DynamicSegmentationConfiguration c = (DynamicSegmentationConfiguration)xs.Deserialize(reader);
            reader.Close();
            return c;
        }

		
    }
}
