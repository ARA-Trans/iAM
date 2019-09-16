using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using DataEntryChecker;
using WeifenLuo.WinFormsUI.Docking;
using DatabaseManager;
using System.Data.SqlClient;
using System.IO;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Forms;
using SharpMap.Data;
using Microsoft.SqlServer.Management.Smo;
using SharpMap.Geometries;
using System.Xml.Serialization;
using DataObjects;

namespace RoadCare3
{
    public partial class FormGISLayerManager : DockContent
    {
        private OpenFileDialog dlgOpen = new OpenFileDialog();
        private MapImage MainMapImage;
        private String m_strNetworkName;
        private List<String> listPreviousAdvancedSearchGeoID = new List<String>();
        private List<Color> m_listColor = new List<Color>();
        private FormGISView m_formGISView;		

        private Hashtable m_htAttributeYears;
        public Hashtable m_hashAttributeLevelColor = new Hashtable(); // Key is attribute, object is list of levels and LevelColors.
        private String m_strNetworkID;

        private List<Color> colorList = new List<Color>();

		// The DISPLAY VALUE of a node.
        private String m_strNode;

        private String[] strListDgvCellColors = new String[]{"Red", "Blue", "Green", "Yellow", "Black", "White"};

        public FormGISView GISView
        {
            get {return m_formGISView;}
            set {m_formGISView = value;}

        }

        public FormGISLayerManager(String strNetworkName, Hashtable htAttributeYears)
        {
            InitializeComponent();
            m_htAttributeYears = htAttributeYears;
            String strQuery = "Select NETWORKID From NETWORKS Where NETWORK_NAME = '" + strNetworkName + "'";
            DataSet ds = DBMgr.ExecuteQuery(strQuery);
            m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            m_strNetworkName = strNetworkName;

            // Load the m_listColor list
            ColorConverter d = new ColorConverter();
            foreach (Color c in d.GetStandardValues())
            {

                if (!c.Name.ToUpper().Contains("LIGHT"))
                {
                    m_listColor.Add(c);
                }
            }
        }

		public Hashtable AttributeYear
		{
			get { return m_htAttributeYears; }
			set { m_htAttributeYears = value; }
		}
		
		public void SetPointColor(Color c, List<String> listGeoID)
        {
            String strGeoID;
            for(int i = 0; i < MainMapImage.Map.Layers.Count; i++)
            {
                if (MainMapImage.Map.Layers[i].LastGeoIDs != null)
                {
                    for (int j = 0; j < MainMapImage.Map.Layers[i].LastGeoIDs.Count; j++)
                    {
                        strGeoID = MainMapImage.Map.Layers[i].LastGeoIDs[j].ToString();
                        Geometry geoLast = (Geometry)MainMapImage.Map.Layers[i].GeoIDs[strGeoID];
						if (geoLast != null)
						{
							geoLast.UseColor = false;
							//geoLast.Color = Color.Black;
						}

						// Check AdvancedSearch list to see if the selection being uncolored is in there.
						// If it is, re-color it the highlighted color.
						if (listPreviousAdvancedSearchGeoID.Exists(delegate(String str) { return str == strGeoID; }))
						{
							List<String> listTemp = new List<string>();
							for (int k = 0; k < listPreviousAdvancedSearchGeoID.Count; k++)
							{
								listTemp.Add(listPreviousAdvancedSearchGeoID[k]);
							}
							SetAdvSearchColor(Color.Cyan, listTemp);
						}
                    }
                    MainMapImage.Map.Layers[i].LastGeoIDs.Clear();
                }
            }
            MainMapImage.Map.Layers[0].LastGeoIDs = listGeoID;
            for (int i = 0; i < listGeoID.Count; i++)
            {
                Geometry geo = (Geometry)MainMapImage.Map.Layers[0].GeoIDs[listGeoID[i].ToString()];
				if (geo != null)
				{
					geo.Color = c;
				}
            }
            MainMapImage.Refresh();
        }

        public void SetLineColor(Color c, LineString lineString)
        {
            lineString.Color = Color.Cyan;
            MainMapImage.Refresh();
        }

        public void SetAdvSearchColor(Color c, List<String> listGeoID)
        {
            String strGeoID;
            if (listPreviousAdvancedSearchGeoID != null && listGeoID != null)
            {
                // De-Highlight the old geoms
                for (int i = 0; i < listPreviousAdvancedSearchGeoID.Count; i++)
                {
                    strGeoID = listPreviousAdvancedSearchGeoID[i].ToString();
                    Geometry geoLast = (Geometry)MainMapImage.Map.Layers[0].GeoIDs[strGeoID];
                    geoLast.UseColor = false;
                }

                // Clear the old list and add the new ones.
                listPreviousAdvancedSearchGeoID.Clear();
                for (int i = 0; i < listGeoID.Count; i++)
                {
                    listPreviousAdvancedSearchGeoID.Add(listGeoID[i]);
                }

                // Highlight the new geoms
                for (int i = 0; i < listGeoID.Count; i++)
                {
                    Geometry geo = (Geometry)MainMapImage.Map.Layers[0].GeoIDs[listGeoID[i].ToString()];
                    geo.Color = c;
                }
                
            }
            MainMapImage.Refresh();
        }

        public String networkID
        {
            get 
            {
                return m_strNetworkID;
            }
            set
            {
                m_strNetworkID = value;
            }
        }

        /// <summary>
        /// Gets or sets the MapImage for the GISLayerManager.
        /// </summary>
        public MapImage MapImage
        {
            get { return MainMapImage; }
            set { MainMapImage = value; }
        }

        /// <summary>
        /// The year combobox text.
        /// </summary>
        public String cbYearText
        {
            get
            {
                return cbYear.Text;
            }
        }

        /// <summary>
        /// The attribute combobox text.
        /// </summary>
        public String cbAttributeText
        {
            get
            {
                return cbAttribute.Text;
            }
        }

        private void btnMoveLayerUp_Click(object sender, EventArgs e)
        {
            // Move the layer in the layer list, and in the textbox.
            if (tvLayers.SelectedNode != null)
            {
                String strLayerName = tvLayers.SelectedNode.Text;
                int iCurr = tvLayers.SelectedNode.Index;
                if (iCurr == 0)
                {
                    return;
                }
				TreeNode tn = tvLayers.SelectedNode;
				tvLayers.SelectedNode.Remove();
				tvLayers.Nodes.Insert(iCurr - 1, tn);
				tvLayers.SelectedNode = tvLayers.Nodes[iCurr - 1];

				// Now move the layer in the Map object.  iCurr contains the position to move from, so we use it here to mirror the treeView.
                ILayer lyrToMove = MainMapImage.Map.Layers[iCurr];
				MainMapImage.Map.Layers.RemoveAt(iCurr);
				MainMapImage.Map.Layers.Insert(iCurr - 1, lyrToMove);

                tvLayers.SelectedNode.SelectedImageIndex = imageListSymbols.Images.IndexOfKey(strLayerName);
                tvLayers.SelectedNode.ImageIndex = imageListSymbols.Images.IndexOfKey(strLayerName);
				tvLayers.Nodes[iCurr - 1].Checked = true;
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            // Move the layer in the layer list, and in the textbox, and on the axMap.
            if (tvLayers.SelectedNode != null)
            {
                String strLayerName = tvLayers.SelectedNode.Name;
                
                int iCurr = tvLayers.SelectedNode.Index;
                if (iCurr == tvLayers.Nodes.Count - 1)
                {
                    return;
                }
				TreeNode tn = tvLayers.SelectedNode;
				tvLayers.SelectedNode.Remove();
				tvLayers.Nodes.Insert(iCurr + 1, tn);
				tvLayers.SelectedNode = tvLayers.Nodes[iCurr + 1];

                // Now move the layer in the Map object.  iCurr contains the position to move from, so we use it here to mirror the treeView.
                ILayer lyrToMove = MainMapImage.Map.Layers[iCurr];
                MainMapImage.Map.Layers.RemoveAt(iCurr);
                MainMapImage.Map.Layers.Insert(iCurr + 1, lyrToMove);

                tvLayers.SelectedNode.SelectedImageIndex = imageListSymbols.Images.IndexOfKey(strLayerName);
                tvLayers.SelectedNode.ImageIndex = imageListSymbols.Images.IndexOfKey(strLayerName);

				tvLayers.Nodes[iCurr + 1].Checked = true;
            }
        }

        /// <summary>
        /// Add a new layer to the axMap.
        /// </summary>
        /// <returns></returns>
        public String AddNewLayer()
        {
            #region MapObjects AddLayer DEPRECATED
            //// Declare a geodataset
            //ESRI.MapObjects2.Core.GeoDataset gds = null;
            //LayerManager layerManager = new LayerManager();
            //try
            //{
            //    // Set up dialog box to prompt user to load a dataset.
            //    this.dlgOpen.FileName = "";
            //    this.dlgOpen.Filter = "ESRI Shapefiles (*.shp)|*.shp|ArcINFO Coverages (*.adf)| aat.adf;pat.adf";
            //    this.dlgOpen.FilterIndex = 0;
            //    this.dlgOpen.CheckFileExists = true;
            //    this.dlgOpen.CheckPathExists = true;
            //    this.dlgOpen.AddExtension = false;
            //    if (this.dlgOpen.ShowDialog() == DialogResult.OK)
            //    {
            //        // Check for proper file name
            //        if (this.dlgOpen.FileName.Length > 0)
            //        {

            //            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //            // Check for .adf or .shp file
            //            if (this.dlgOpen.FileName.ToLower().EndsWith("adf"))
            //            {
            //                gds = Global.CoverageGDS(this.dlgOpen.FileName);
            //            }
            //            else if (this.dlgOpen.FileName.ToLower().EndsWith("shp"))
            //            {
            //                gds = Global.ShapeGDS(this.dlgOpen.FileName);
            //                // This will store the file name of the last shape file selected
            //                int iParseFileName = this.dlgOpen.FileName.LastIndexOf('\\');
            //                layerManager.Name = this.dlgOpen.FileName.Substring(iParseFileName + 1);
            //                m_strNode = layerManager.Name;

            //                TreeNode tn = new TreeNode(layerManager.Name);
            //                if (LayerExists())
            //                {
            //                    // Reset the variable for existing node.
            //                    m_bNodeExists = false;
            //                    Global.WriteOutput("Error: Layer already exists, or has same name as existing layer, please choose a different name for the layer.");
            //                    return "";
            //                }
            //                else
            //                {
            //                    tvLayers.Nodes.Insert(0, tn);
            //                }
            //            }
            //            if (gds != null)
            //            {
            //                layerManager.Layer.GeoDataset = gds;

            //                // Add the color to the color index
            //                ESRI.MapObjects2.Core.Symbol layerSymbol;
            //                layerSymbol = layerManager.Layer.Symbol;

            //                layerManager.Color = layerSymbol.Color;

            //                // Add label on (true or false) to the label array
            //                //alLayerLabels.Insert(0, false);
            //                layerManager.LabelsShown = false;

            //                // Check to see if the layer is a dynamically segmented layer, if it is, we need to apply the VMR to
            //                // the layer to get the correct segmentation colors on the map.
            //                if (IsDynamicallySegmentedLayer(layerManager))
            //                {
            //                    // When a dynamic segmentation occurs, store the color and attribute information in a config.
            //                    // file on a per user basis, then when loading a dynamic seg shape file, reference the config.
            //                    // file to get the color information needed.
            //                    DynamicSegmentationConfiguration dnC = new DynamicSegmentationConfiguration();
            //                    dnC = dnC.Deserialize(System.IO.Directory.GetCurrentDirectory() + "\\" + layerManager.Name + ".cfg");
            //                    layerManager.IsDynSeg = dnC.IsDynSeg;

            //                    List<LevelColors> listLevelColors = dnC.m_listLC;

            //                    ValueMapRenderer vmr = new ValueMapRenderer();

            //                    vmr.Field = "ATTNAME";
            //                    vmr.ValueCount = (short)listLevelColors.Count;
            //                    vmr.SymbolType = SymbolTypeConstants.moLineSymbol;

            //                    // Render each line in the new shapefile and draw each symbol according to its color.
            //                    short i = 0;
            //                    foreach (LevelColors lc in listLevelColors)
            //                    {

            //                        vmr.get_Symbol(i).Color = (uint)ColorTranslator.ToOle(ColorTranslator.FromHtml(lc.ColorHtml));
            //                        vmr.get_Symbol(i).Size = 3;
            //                        vmr.set_Value(i, lc.m_strLevel);
            //                        i++;
            //                    }

            //                    FormGISLayerManager formGISLayerManager;
            //                    if (FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
            //                    {
            //                        // Set the vmr to the layer being drawn.
            //                        layerManager.Layer.Renderer = vmr;

            //                        // Add the rendered layer to the map.
            //                        axMapMain.Layers.Add(layerManager.Layer);

            //                        LayerManager lm = new LayerManager();
            //                        lm.Name = layerManager.Layer.Name;
            //                        lm.Layer = layerManager.Layer;
            //                        lm.IsDynSeg = layerManager.IsDynSeg;
            //                        m_layerConfig.m_layerManagerList.Insert(0, lm);

            //                        RegenerateItems();
            //                    }
            //                    cbAttribute.Text = dnC.AttributeName;
            //                    cbYear.Text = dnC.AttributeYear;
            //                    CreateLegend();

            //                    dgvLegend.ReadOnly = true;
            //                    cbAttribute.Enabled = false;
            //                    cbYear.Enabled = false;

            //                    return layerManager.Name;
            //                }

            //                // Add the layer to the GIS map
            //                axMapMain.Layers.Add(layerManager.Layer);

            //                // Add the new layer to the top of the layer index array list 
            //                // this layer will always be drawn last.
            //                m_layerConfig.m_layerManagerList.Insert(0, layerManager);

            //                // Make the new layer checked in the treeview.
            //                m_layerConfig.m_layerManagerList[0].IsChecked = true;
            //                tvLayers.Nodes[0].Checked = true;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //catch (System.Runtime.InteropServices.ExternalException COMEx)
            //{
            //    MessageBox.Show(COMEx.ErrorCode + ": " + COMEx.Message);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    // Reset the cursor
            //    this.Cursor = System.Windows.Forms.Cursors.Default;
            //}
            //if (gds == null)
            //{
            //    return "null";
            //}
            //else
            //{
            //    return layerManager.Name;
            //}
            #endregion

            String strFilePath = "";
            String strFileName = "";

            // Set up dialog box to prompt user to load a dataset.
            this.dlgOpen.FileName = "";
            this.dlgOpen.Filter = "Georeferenced TIF(*.tif)|*.tif|ESRI Shapefiles (*.shp)|*.shp|ArcINFO Coverages (*.adf)| aat.adf;pat.adf";
            this.dlgOpen.FilterIndex = 0;
            this.dlgOpen.CheckFileExists = true;
            this.dlgOpen.CheckPathExists = true;
            this.dlgOpen.AddExtension = false;
            if (this.dlgOpen.ShowDialog() == DialogResult.OK)
            {
                strFilePath = dlgOpen.FileName;
                int iParseFileName = this.dlgOpen.FileName.LastIndexOf('\\');
                strFileName = this.dlgOpen.FileName.Substring(iParseFileName + 1);

                // TODO: Add if statement if the layer already exists.
                TreeNode tn = tvLayers.Nodes.Insert(0, strFileName, strFileName);
                tn.Tag = "SHAPEFILE";

                // Open the shapefile at the given path
                ShapeFile shapefile = new SharpMap.Data.Providers.ShapeFile(strFilePath);
                shapefile.Open();
                FeatureDataRow fdr = shapefile.GetFeature(0);
				fdr.Geometry.Color = Color.LightGray;
                // Create the shapefile layer with the name of the shapefile as the layer name.
                VectorLayer addLayer = new SharpMap.Layers.VectorLayer(strFileName);
                addLayer.Tag = "SHAPEFILE";
                addLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(strFilePath);

                shapefile.Close();

                MainMapImage.Map.Layers.Insert(0, addLayer);
                MainMapImage.Map.ZoomToExtents();
                MainMapImage.Refresh();

                // Make the new layer checked in the treeview.
                tvLayers.Nodes[0].Checked = true;
            }
            return strFileName;
        }

        private void btnAddLayer_Click(object sender, EventArgs e)
        {
            AddNewLayer();         
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            // Set the name of the layer to be removed, if it exists, remove it.
            if (tvLayers.SelectedNode != null)
            {
                m_strNode = tvLayers.SelectedNode.Name;
                int iRemoveIndex = MainMapImage.Map.Layers.FindIndex(delegate(SharpMap.Layers.ILayer layer) { return layer.LayerName == tvLayers.SelectedNode.Text; });
                try
                {
                    MainMapImage.Map.Layers.RemoveAt(iRemoveIndex);
                    MainMapImage.Refresh();

                    tvLayers.SelectedNode.Remove();
                    imageListSymbols.Images.RemoveByKey(m_strNode);
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: Could not remove selected layer. " + exc.Message);
                    return;
                }
            }
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            ShowProperties();
        }

		private void ShowProperties()
		{
			if (tvLayers.SelectedNode != null)
			{
				String strNodeTag = tvLayers.SelectedNode.Tag.ToString();
				switch (strNodeTag)
				{
					case "ATTRIBUTE":
						// Can change line thickness only
						AttributeMapProperties();
						break;
					case "ASSET":
						// Can change color, shape, and labels?
						AssetProperties();
						break;
					case "SHAPEFILE":
						// Can change color, shape, and labels.
						break;
					case "NETWORK MAP":
						// Can change line thickness and color
						NetworkMapProperties();
						break;
					default:
						break;
				}
			}
		}

		private void AssetProperties()
		{
			// Get the layer key name
			String strLayerName = tvLayers.SelectedNode.Name;

			// Pull out the layer
			VectorLayer vl = (VectorLayer)MainMapImage.Map.Layers[tvLayers.Nodes.IndexOfKey(strLayerName)];

			// Create a new layer properties.
			FormLayerProperties formLayerProperties = new FormLayerProperties(vl);
			if (formLayerProperties.ShowDialog() == DialogResult.OK)
			{
				if (!formLayerProperties.IsCustomSymbol)
				{
					if (formLayerProperties.GetNewSymbol() == null)
					{
						Color newColor = formLayerProperties.GetNewColor();
						Color oldColor = formLayerProperties.GetOldColor();

						Global.ConvertBitmapColor(oldColor, newColor, vl.Style.Symbol);
						vl.Style.Symbol.Tag = newColor;
						vl.Style.SymbolScale = formLayerProperties.GetSymbolScale();
					}
					else
					{
						Color oldColor = Color.Red;
						Color newColor = formLayerProperties.GetNewColor();
						vl.Style.Symbol = formLayerProperties.GetNewSymbol();
						vl.Style.SymbolScale = formLayerProperties.GetSymbolScale();
						Global.ConvertBitmapColor(oldColor, newColor, vl.Style.Symbol);
						vl.Style.Symbol.Tag = newColor;
					}
				}
				else
				{
					vl.Style.Symbol = formLayerProperties.GetCustomSymbol();
				}
				
				// Change the icon in imageListSymbols to match the new symbol and refresh the MapImage to reflect the new symbol.
				imageListSymbols.Images.RemoveByKey(strLayerName);
				imageListSymbols.Images.Add(strLayerName, vl.Style.Symbol);
				MainMapImage.Refresh();
			}
		}

		private void NetworkMapProperties()
		{
			// Get the layer key name
			String strLayerName = tvLayers.SelectedNode.Name;

			// Pull out the layer
			VectorLayer vl = (VectorLayer)MainMapImage.Map.Layers[tvLayers.Nodes.IndexOfKey(strLayerName)];

			// Create a new layer properties.  The two dictionaries maintain changes made to, and old colors with respect to the
			// default symbols. These lists were created as there seems to be some issue with the .Tag value for Image not being 
			// correctly implemented.
			FormLayerProperties formLayerProperties;
			formLayerProperties = new FormLayerProperties(vl);
			if (formLayerProperties.ShowDialog() == DialogResult.OK)
			{
				Color newColor = formLayerProperties.GetNewColor();

				// Get the geometries for defined networks, if they exist.
				DataSet ds;

				String strQuery = "SELECT GEOMETRY, SECTIONID FROM SECTION_" + m_strNetworkID;
				ds = DBMgr.ExecuteQuery(strQuery);

				List<Geometry> GeomColl = new List<Geometry>();
				LineString lineSeg;
				Hashtable htGeoIds = new Hashtable();

				int iCount = 0;
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					if (row[0] != null && row[0].ToString() != "")
					{
						lineSeg = (LineString)Geometry.GeomFromText(row[0].ToString());
						lineSeg.Width_ = formLayerProperties.GetLineThickness();
						lineSeg.Tag = row["SECTIONID"].ToString();
						if (newColor == Color.Transparent)
						{
							newColor = Color.Orange;
						}
						lineSeg.Color = newColor;
						GeomColl.Add(lineSeg);
						htGeoIds.Add(row["SECTIONID"].ToString(), lineSeg);
					}
					iCount++;
				}
				vl.GeoIDs = htGeoIds;
				vl.DataSource = new SharpMap.Data.Providers.GeometryProvider(GeomColl);
				//MainMapImage.Map.ZoomToExtents();
				MainMapImage.Refresh();
			}
		}

		private void AttributeMapProperties()
		{
			// Get the layer key name
			String strLayerName = tvLayers.SelectedNode.Name;

			// Pull out the layer
			VectorLayer vl = (VectorLayer)MainMapImage.Map.Layers[tvLayers.Nodes.IndexOfKey(strLayerName)];

			// Create a new layer properties.  The two dictionaries maintain changes made to, and old colors with respect to the
			// default symbols. These lists were created as there seems to be some issue with the .Tag value for Image not being 
			// correctly implemented.
			FormLayerProperties formLayerProperties;
			formLayerProperties = new FormLayerProperties(vl);
			if (formLayerProperties.ShowDialog() == DialogResult.OK)
			{
				FormGISView formGISView;
				// Get the current GIS view
                if (GISView != null)//For ImageView
                {
                    GISView.DisplayAttribute(cbAttribute.Text, cbYear.Text,
                        GetLayerColors(cbAttribute.Text, cbYear.Text, m_strNetworkID), formLayerProperties.GetLineThickness());
                }
				else if (FormManager.IsFormGISViewOpen(m_strNetworkName, out formGISView))
				{
					formGISView.DisplayAttribute(cbAttribute.Text, cbYear.Text, 
						GetLayerColors(cbAttribute.Text, cbYear.Text, m_strNetworkID), formLayerProperties.GetLineThickness());
				}
			}
		}

        private void SaveAssetColor(String ASSETS_COLORS, String strLayerName, Color c)
        {
            bool bExists = false;
            String strNewAssetColor = "";
            string[] assets = ASSETS_COLORS.Split('\t');
            for (int i = 0; i < assets.Length; i++)
            {
                string[] color = assets[i].Split(',');
                if (color[0] == strLayerName)
                {
                    strNewAssetColor = strNewAssetColor + strLayerName + "," + c.ToString() + "\t";
                    bExists = true;
                }
                else
                {
                    if (color.Length == 2)
                    {
                        strNewAssetColor = strNewAssetColor + color[0].ToString() + "," + color[1].ToString() + "\t";
                    }
                }
            }
            if (!bExists)
            {
                strNewAssetColor = strNewAssetColor + strLayerName + "," + c.ToString() + "\t";
            }

            RoadCare3.Properties.Settings.Default.ASSETS_COLORS = strNewAssetColor;
            RoadCare3.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// When the user switches between window panes, we get the LayerManager for the focused tab, and repopulate the treeview with the correct data.
        /// </summary>
		public void ClearLayerManagerTreeView()
		{
			// Clear the treeview
			if(!tvLayers.IsDisposed)
			{
				tvLayers.Nodes.Clear();
			}
		}

        private void FormGISLayerManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.GISView == null)
            {
                FormManager.RemoveFormGISLayerManager(this);
            }
        }

        private void dgvLevelDefinitions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
			if (e.ColumnIndex == 2)
			{
				String strAttribute = cbAttribute.Text;
				String strYear = cbYear.Text;
				// Button click occurred.
				ColorDialog cd = new ColorDialog();
				List<LevelColors> lcTemp = null;
				Color originalColor = dgvLegend[1, e.RowIndex].Style.BackColor;
				if (cd.ShowDialog() == DialogResult.OK)
				{
					if (Global.IsStringAttribute(strAttribute))
					{
						if (!DataCheck.IsNumber(strAttribute))
						{
							dgvLegend[1, e.RowIndex].Style.BackColor = cd.Color;

							lcTemp = (List<LevelColors>)m_hashAttributeLevelColor[strAttribute];
							if (lcTemp != null)
							{
								lcTemp[e.RowIndex].m_Color = dgvLegend[1, e.RowIndex].Style.BackColor;
							}
							else
							{
								dgvLegend[1, e.RowIndex].Style.BackColor = originalColor;
							}
						}
					}
					else
					{
						if (e.RowIndex % 2 == 0)
						{
							dgvLegend[1, e.RowIndex].Style.BackColor = cd.Color;
							dgvLegend[0, e.RowIndex].Style.BackColor = cd.Color;
							lcTemp = (List<LevelColors>)m_hashAttributeLevelColor[strAttribute];
							if (lcTemp != null)
							{
								lcTemp[(e.RowIndex / 2)].m_Color = dgvLegend[1, e.RowIndex].Style.BackColor;
							}
							else
							{
								dgvLegend[1, e.RowIndex].Style.BackColor = originalColor;
							}
						}
					}
					if (cbAttribute.Text != "" && cbYear.Text != "")
					{
                        if (GISView == null)//For Non-ImageView
                        {
                            FormGISView formGISView;
                            if (!FormManager.IsFormGISViewOpen(m_strNetworkName, out formGISView))
                            {
                                formGISView = new FormGISView(m_strNetworkName, m_htAttributeYears);
                                FormManager.AddFormGISView(formGISView);
                            }
                            formGISView.DisplayAttribute(strAttribute, strYear, lcTemp, 1);
                        }
                        else
                        {
                            this.GISView.DisplayAttribute(strAttribute, strYear, lcTemp, 1);
                        }
					}
					MainMapImage.Refresh();
					dgvLegend.ClearSelection();
				}
			}
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

		private void AddAttributeLayer()
		{
			if (cbAttribute.Text != "")
			{
				CreateLegend();
			}
			if (cbAttribute.Text != "" && cbYear.Text != "")
			{
				// Remove the previous ATTRIBUTE node from the treeview.
				for (int i = 0; i < tvLayers.Nodes.Count; i++)
				{
					String strNodeTag = (String)tvLayers.Nodes[i].Tag;
					if (strNodeTag == "ATTRIBUTE")
					{
						tvLayers.Nodes.Remove(tvLayers.Nodes[i]);
					}
				}

				String strAttribute = cbAttribute.Text;
				String strYear = cbYear.Text;

				if (this.GISView != null)
				{
					GISView.Attribute = cbAttribute.Text;
					GISView.Year = cbYear.Text;

					// Get the colors for this attribute year selection from the hashtable.
					List<LevelColors> listLevelColor = (List<LevelColors>)m_hashAttributeLevelColor[strAttribute];
					GISView.DisplayAttribute(strAttribute, strYear, listLevelColor, 1);
				}
				else
				{
					FormGISView formGISView;
					if (!FormManager.IsFormGISViewOpen(m_strNetworkName, out formGISView))
					{
						formGISView = new FormGISView(m_strNetworkName, m_htAttributeYears);
						FormManager.AddFormGISView(formGISView);
					}
					formGISView.Attribute = cbAttribute.Text;
					formGISView.Year = cbYear.Text;

					// Get the colors for this attribute year selection from the hashtable.
					List<LevelColors> listLevelColor = (List<LevelColors>)m_hashAttributeLevelColor[strAttribute];
					formGISView.DisplayAttribute(strAttribute, strYear, listLevelColor, 1);
				}
				// Now add the layer to the GISLayerManager treeview
				TreeNode tn = tvLayers.Nodes.Insert(0, strAttribute, strAttribute + "-" + strYear, "ATTRIBUTE");
				tn.ImageIndex = imageListSymbols.Images.IndexOfKey("ATTRIBUTE");
				tn.SelectedImageIndex = imageListSymbols.Images.IndexOfKey("ATTRIBUTE");
				tn.Checked = true;
				tn.Tag = "ATTRIBUTE";
			}
		}
        private void cbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
			ReloadYearComboBox();
        }

        /// <summary>
        /// Gets information based off of the attribute and the year selected in this objects attribute and year comboboxes
        /// from the database and then calls CreateDGVCells depending on the TYPE (STRING or NUMBER) of the attribute selected.
        /// </summary>
        private void CreateLegend()
        {
            if (cbAttribute.Text == "" || cbYear.Text == "")
            {
                dgvLegend.Rows.Clear();
                return;
            }
            dgvLegend.Rows.Clear();
            List<LevelColors> listLevelColors = GetLayerColors(cbAttribute.Text, cbYear.Text, m_strNetworkID);
            Color c = MakeRandomColor();
            if (Global.GetAttributeType(cbAttribute.Text) == "STRING")
            {
                int i = 0;
                foreach (LevelColors levelColor in listLevelColors)
                {
                    dgvLegend.Rows.Add();
                    dgvLegend[0, i].Value = levelColor.m_strLevel;
					dgvLegend[0, i].ReadOnly = true;
                    dgvLegend[1, i].Style.BackColor = levelColor.m_Color;
                    i++;
                }
            }
            else
            {
                int i = 0;
                foreach(LevelColors levelColor in listLevelColors)
                {
                    dgvLegend.Rows.Add();
                    dgvLegend.Rows[i * 2].ReadOnly = true;
                    dgvLegend[1, i * 2].Style.BackColor = levelColor.m_Color;
                    dgvLegend[0, i * 2].Style.BackColor = levelColor.m_Color;
                    if (i < listLevelColors.Count - 1)
                    {
                        dgvLegend.Rows.Add();
                        dgvLegend[0, i * 2 + 1].Value = levelColor.m_strLevel;
						//dgvLegend[0, i * 2 + 1].ReadOnly = true;
                    }
                    i++;
                }
            }
            return;
        }

        public void UpdateLayerConfiguration(FormGISView form)
        {
            dgvLegend.Rows.Clear();
            this.tvLayers.Nodes.Clear();

            cbYear.Text = "";
            cbAttribute.Text = "";

            m_strNetworkName = form.Tag.ToString();
            for (int i = 0; i < form.MapImage.Map.Layers.Count; i++)
            {
                String strTag = form.MapImage.Map.Layers[i].Tag.ToString();
                TreeNode tn;
                if (strTag == "ATTRIBUTE")
                {
                    cbAttribute.SelectedText = form.Attribute;
                    cbYear.SelectedText = form.Year;
                    tn = tvLayers.Nodes.Add(form.MapImage.Map.Layers[i].LayerName);// + "-" + form.Year);
                    CreateLegend();
                }
                else
                {
                    tn = tvLayers.Nodes.Add(form.MapImage.Map.Layers[i].LayerName);
                }
                tn.Name = form.MapImage.Map.Layers[i].LayerName;

                if (form.MapImage.Map.Layers[i].Enabled == true)
                {
                    tn.Checked = true;
                }
				tn.ImageIndex = imageListSymbols.Images.IndexOfKey(form.MapImage.Map.Layers[i].LayerName);
				tn.SelectedImageIndex = imageListSymbols.Images.IndexOfKey(form.MapImage.Map.Layers[i].LayerName);
				
            }
        }

        private void FormGISLayerManager_Load(object sender, EventArgs e)
        {
            // Populate the attributes listbox.
            String strQuery = "Select DISTINCT ATTRIBUTE_ FROM ATTRIBUTES_";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strQuery);
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    String strAttribute = dataRow[0].ToString();
                    if (m_htAttributeYears.Contains(strAttribute))
                    {
                        cbAttribute.Items.Add(dataRow[0].ToString());
                    }
                }
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Could not get attributes from ATTRIBUTES_ table. " + sqlE.Message);
            }
            DataReader dr = null;
            try
            {
                dr = new DataReader("SELECT ASSET FROM ASSETS");
                while (dr.Read())
                {
                    cbAssets.Items.Add(dr["ASSET"].ToString());
                }
                dr.Close();
            }
            catch(Exception exc)
            {
                Global.WriteOutput("Error: Filling asset combo box failed." + exc.Message);
            }
			
            // Now insert into the treeview.
            Size sz = new Size(6, 6);
            imageListSymbols.ImageSize = sz;
            imageListSymbols.Images.Add("ATTRIBUTE", RoadCare3.Properties.Resources.cross);
			for (int i = 0; i < 100; i++)
			{
				comboBoxNumberLevels.Items.Add(i.ToString());
			}
        }

		public String GetNameLayerSelected()
        {
            return m_strNode;
        }

        private void tvLayers_AfterCheck(object sender, TreeViewEventArgs e)
        {
            tvLayers.SelectedNode = e.Node;
            int iLayerIndex = MainMapImage.Map.Layers.FindIndex(delegate(SharpMap.Layers.ILayer layer) { return layer.LayerName == e.Node.Text; });
            if (iLayerIndex != -1)
            {
                if (e.Node.Checked == false)
                {
                    MainMapImage.Map.Layers[iLayerIndex].Enabled = false;
                }
                else
                {
                    MainMapImage.Map.Layers[iLayerIndex].Enabled = true;
                }
                MainMapImage.Refresh();
            }
        }

        private void dgvLegend_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Get the attribute being modified, then update the ATTRIBUTES table in the database.
            String strAttribute = cbAttribute.Text;

            // Determine if the cell is already blank, if it is, take no action.
            // Make sure the user is in the correct cell, being the first column for property updates.
            // Not sure if this event fires for color.  Will research that and find out.
            if (e.ColumnIndex == 0)
            {
                String strValue = dgvLegend[e.ColumnIndex, e.RowIndex].Value.ToString();
                if (Global.IsStringAttribute(strAttribute))
                {
                    // Get the new value, it must be a STRING in this case
                    if (DataCheck.IsNumber(strValue))
                    {
                        Global.WriteOutput("Value entered must be a valid STRING for attribute " + strAttribute + ".");
                        return;
                    }
                    String strUpdate = "Update ATTRIBUTES_ Set LEVEL" + e.ColumnIndex + 1 + " = '" + strValue +
                                       "' Where ATTRIBUTE_ = '" + strAttribute + "'";
                    try
                    {
                        DBMgr.ExecuteQuery(strUpdate);
                    }
                    catch (Exception ex)
                    {
                        Global.WriteOutput("Error: Update for " + strAttribute + " failed with " + ex.Message);
                    }
                    List<LevelColors> lcTemp = (List<LevelColors>)m_hashAttributeLevelColor[strAttribute];
                    lcTemp[e.RowIndex].m_strLevel = strValue;
                }
                else
                {
                    // Check for a valid numeric input
                    if (DataCheck.IsNumber(strValue))
                    {
                        // Get the level to update, formula for NUMERIC values is (n/2) + 1
                        int iLevel = e.RowIndex / 2;
                        iLevel++;

                        // Special case for numeric values, every other row should always be blank
                        if (e.RowIndex % 2 == 0)
                        {
                            dgvLegend[0, e.RowIndex].Value = "";
                            return;
                        }

                        // Update the database with the new level value.
                        String strLevel = "LEVEL" + iLevel.ToString();
                        String strUpdate = "Update ATTRIBUTES_ Set " + strLevel + " = '" + strValue
                                           + "' WHERE ATTRIBUTE_ = '" + strAttribute + "'";
                        DBMgr.ExecuteNonQuery(strUpdate);

                        // Now update the attribute-level-color hashtable
                        List<LevelColors> lcTemp = (List<LevelColors>)m_hashAttributeLevelColor[strAttribute];
                        lcTemp[iLevel - 1].m_strLevel = strValue;
                    }
                    else
                    {
                        Global.WriteOutput("Value entered must be a valid NUMERIC for attribute " + strAttribute + ".");
                    }

                }
            }
            if (e.ColumnIndex == 1)
            {
                // A color has been changed, get the color list from the layer config level color hash
                // and change the appropriate color depending on whether the value changed is NUMERIC or STRING
                String strValue = dgvLegend[e.ColumnIndex, e.RowIndex].Value.ToString();
                if (Global.IsStringAttribute(strAttribute))
                {

                }
                // Check for a valid numeric input
                if (DataCheck.IsNumber(strValue))
                {

                }
                else
                {
                    Global.WriteOutput("Value entered must be a valid NUMERIC for attribute " + strAttribute + ".");
                }

            }
        }

        private Color MakeRandomColor()
        {
            Random rnd = new Random();

            int r = rnd.Next(0, 255);
            int g = rnd.Next(0, 255);
            int b = rnd.Next(0, 255);
            Color color = Color.FromArgb(r,g,b);
            return color;
        } 

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cbAssets.Text == "")
            {
                Global.WriteOutput("Error: An asset must be selected before adding to GIS map.");
                return;
            }
			if (cbAssets.Text != "" && cbAssetProperty.Text != "")
			{
				// Note:  apparently this was never implemented.  For now, comment
				// out the offending code to allow the program to run.
				// LRR - 3/25/2010 

				//string query = "SELECT * FROM ASSET_ATTRIBUTE_LEVEL WHERE ASSET = '" + cbAssets.Text + "' AND ASSET_PROPERTY = '" + cbAssetProperty.Text + "'";
				//DataSet assetAttributeLevel = DBMgr.ExecuteQuery(query);
				ConnectionParameters cp = DBMgr.GetAssetConnectionObject(cbAssets.Text);
				//AssetAttributeLevelObject assetToFill= null;
				//if (assetAttributeLevel.Tables[0].Rows.Count == 1)
				//{
				//    assetToFill = new AssetAttributeLevelObject(assetAttributeLevel.Tables[0].Rows[0], cp);
				//}
				//else
				//{
				//    String strType = DBMgr.IsColumnTypeString(cbAssets.Text, cbAssetProperty.Text, cp);
				//    if (strType == "String" || strType == "Boolean")
				//    {
				//        assetToFill = new AssetAttributeLevelObject();
				//        assetToFill.AssetPropertyType = strType;
				//        assetToFill.Asset = cbAssets.Text;
				//        assetToFill.AssetProperty = cbAssetProperty.Text;
				//        string strSelect = "SELECT DISTINCT " + assetToFill.AssetProperty + " FROM " + assetToFill.Asset + " ORDER BY " + assetToFill.AssetProperty;
				//        DataSet ds = DBMgr.ExecuteQuery(strSelect, cp);
				//        int nIndex = 30;
				//        foreach (DataRow row in ds.Tables[0].Rows)
				//        {
				//            LevelsObject levelsObject = new LevelsObject();
				//            if (m_listColor.Count <= nIndex) nIndex = 0;
				//            levelsObject.Color = m_listColor[nIndex];
				//            levelsObject.LevelValue = row[cbAssetProperty.Text].ToString();
				//            assetToFill.Levels.Add(levelsObject);
				//            nIndex++;
				//        }
				//    }
				//    else
				//    {
				//        return;
				//    }
				//}
				
				//string assetPropertyType = assetToFill.AssetPropertyType;
				//if (assetPropertyType == "String" || assetPropertyType == "Boolean")
				//{
				//    FillStringLegend(assetToFill.Levels);
				//}
				//else
				//{
				//    FillNumberLegend(assetToFill.Levels);
				//}

				Hashtable htGeoIDs = new Hashtable();
				String strQuery = "SELECT GEO_ID, GEOMETRY, " + cbAssetProperty.Text + " FROM " + cbAssets.Text;
				List<Geometry> GeomColl = new List<Geometry>();
				DataReader dr;
				Bitmap symbol = global::RoadCare3.Properties.Resources.redDiamond;
				try
				{
					dr = new DataReader(strQuery, cp);
					while (dr.Read())
					{
						try
						{
							string geoID = dr["GEO_ID"].ToString();
							string wktGeom = dr["GEOMETRY"].ToString();
							if (String.IsNullOrEmpty(wktGeom))
							{
								string msg = String.Format("Warning: No geometry is available for GeoID {0}", geoID);
								Global.WriteOutput(msg);
							}
							else
							{
								Geometry geo = Geometry.GeomFromText(wktGeom);
								geo.Symbol = symbol;
								GeomColl.Add(geo);
								//geo.Color = assetToFill.GetColor(dr[cbAssetProperty.Text].ToString());
								htGeoIDs.Add(dr["GEO_ID"].ToString(), geo);
							}
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

				SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer(cbAssets.Text + "(" + cbAssetProperty.Text + ")");
				myLayer.Tag = "ASSET";

				
				myLayer.Style.Symbol = symbol;
				myLayer.Style.SymbolScale = 0.66f;

				myLayer.DataSource = new SharpMap.Data.Providers.GeometryProvider(GeomColl);
				myLayer.GeoIDs = htGeoIDs;

				MainMapImage.Map.Layers.Insert(0, myLayer);
				MainMapImage.Map.ZoomToExtents();
				MainMapImage.Refresh();

				imageListSymbols.Images.Add(myLayer.LayerName, symbol);

				TreeNode tn = tvLayers.Nodes.Insert(0, myLayer.LayerName, myLayer.LayerName, imageListSymbols.Images.IndexOfKey(myLayer.LayerName));
				tn.ImageIndex = imageListSymbols.Images.IndexOfKey(myLayer.LayerName);
				tn.SelectedImageIndex = imageListSymbols.Images.IndexOfKey(myLayer.LayerName);
				tn.Tag = "ASSET";
				tvLayers.Nodes[0].Checked = true;
			}
			else
			{
				// Now add the layer to the layer control list box...
				int iLayerIndex = MainMapImage.Map.Layers.FindIndex(delegate(SharpMap.Layers.ILayer layer) { return layer.LayerName == cbAssets.Text; });
				if (iLayerIndex == -1)
				{
					ConnectionParameters cp = DBMgr.GetAssetConnectionObject(cbAssets.Text);
					Hashtable htGeoIDs = new Hashtable();
					String strQuery = "SELECT GEO_ID, GEOMETRY FROM " + cbAssets.Text;
					List<Geometry> GeomColl = new List<Geometry>();
					DataReader dr;
					try
					{
						dr = new DataReader(strQuery, cp);
						while (dr.Read())
						{
							try
							{
								Geometry geo = Geometry.GeomFromText(dr["GEOMETRY"].ToString());
								GeomColl.Add(geo);
								htGeoIDs.Add(dr["GEO_ID"].ToString(), geo);
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

					SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer(cbAssets.Text);
					myLayer.Tag = "ASSET";
					Bitmap symbol = global::RoadCare3.Properties.Resources.redDiamond;

					Color rndColor = MakeRandomColor();
					colorList.Add(rndColor);
					symbol = Global.ConvertBitmapColor(Color.Red, rndColor, symbol);
					symbol.Tag = rndColor;

					myLayer.Style.Symbol = symbol;
					myLayer.Style.SymbolScale = 0.6666453f;

					myLayer.DataSource = new SharpMap.Data.Providers.GeometryProvider(GeomColl);
					myLayer.GeoIDs = htGeoIDs;

					MainMapImage.Map.Layers.Insert(0, myLayer);
					MainMapImage.Map.ZoomToExtents();
					MainMapImage.Refresh();

					imageListSymbols.Images.Add(myLayer.LayerName, symbol);

					TreeNode tn = tvLayers.Nodes.Insert(0, cbAssets.Text, cbAssets.Text, imageListSymbols.Images.IndexOfKey(myLayer.LayerName));
					tn.ImageIndex = imageListSymbols.Images.IndexOfKey(myLayer.LayerName);
					tn.SelectedImageIndex = imageListSymbols.Images.IndexOfKey(myLayer.LayerName);
					tn.Tag = "ASSET";
					tvLayers.Nodes[0].Checked = true;
				}
				tvLayers.Refresh();
			}
        }

		private void FillStringLegend(List<LevelsObject> list)
		{
			dgvLegend.Rows.Clear();
			comboBoxNumberLevels.Text = list.Count.ToString();
			foreach (LevelsObject lo in list)
			{
				int nIndex = dgvLegend.Rows.Add("", lo.LevelValue);
				dgvLegend[0, nIndex].Style.BackColor = lo.Color;
			}
		}

		private void FillNumberLegend(List<LevelsObject> list)
		{
			
		}

        public List<LevelColors> GetLayerColors(String strAttribute, String strYear, String strNetworkID)
        {
            List<LevelColors> listLevelColor;
            if (m_hashAttributeLevelColor.Contains(strAttribute))
            {
                return (List<LevelColors>)m_hashAttributeLevelColor[strAttribute];
            }
            else
            {
                listLevelColor = new List<LevelColors>();
                m_hashAttributeLevelColor.Add(strAttribute, listLevelColor);

                String strQuery = "Select TYPE_, LEVEL1, LEVEL2, LEVEL3, LEVEL4, LEVEL5 From ATTRIBUTES_ Where ATTRIBUTE_ = '" + strAttribute + "'";
                DataSet ds;
                try
                {
                    ds = DBMgr.ExecuteQuery(strQuery);

                    if (ds.Tables[0].Rows[0].ItemArray[0].ToString() == "STRING")
                    {

                        strQuery = "Select SEGMENT_TABLE From SEGMENT_CONTROL Where NETWORKID = '" + strNetworkID + "' AND ATTRIBUTE_ = '" + strAttribute + "'";
                        DataSet dsSegmentTable = DBMgr.ExecuteQuery(strQuery);

                        //strQuery = "Select DISTINCT " + strAttribute + "_" + strYear + " From " + dsSegmentTable.Tables[0].Rows[0].ItemArray[0].ToString() +
                        //           " WHERE " + strAttribute + "_" + strYear + " <> '' ORDER BY " + strAttribute + "_" + strYear;
                        strQuery = "Select DISTINCT DATA_ From " + strAttribute + " ORDER BY DATA_";
                        DataSet dsAttribute = DBMgr.ExecuteQuery(strQuery);

                        int i = 0;
                        foreach (DataRow dataRow in dsAttribute.Tables[0].Rows)
                        {
                            if (i >= m_listColor.Count)
                            {
                                i = 0;
                            }
                            LevelColors levelColor = new LevelColors();
                            levelColor.m_strLevel = dataRow[0].ToString();
                            levelColor.m_Color = m_listColor[i];
                            listLevelColor.Add(levelColor);
                            i++;
                        }

                    }
                    else // NUMBER
                    {
                        LevelColors levelColor = new LevelColors();
                        levelColor.m_strLevel = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                        levelColor.m_Color = Color.Green;
                        listLevelColor.Add(levelColor);

                        levelColor = new LevelColors();
                        levelColor.m_strLevel = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                        levelColor.m_Color = Color.Blue;
                        listLevelColor.Add(levelColor);

                        levelColor = new LevelColors();
                        levelColor.m_strLevel = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                        levelColor.m_Color = Color.Yellow;
                        listLevelColor.Add(levelColor);

                        levelColor = new LevelColors();
                        levelColor.m_strLevel = ds.Tables[0].Rows[0].ItemArray[4].ToString();
                        levelColor.m_Color = Color.Magenta;
                        listLevelColor.Add(levelColor);

                        levelColor = new LevelColors();
                        levelColor.m_strLevel = ds.Tables[0].Rows[0].ItemArray[5].ToString();
                        levelColor.m_Color = Color.Red;
                        listLevelColor.Add(levelColor);

                        levelColor = new LevelColors();
                        levelColor.m_strLevel = "";
                        levelColor.m_Color = Color.Orange;
                        listLevelColor.Add(levelColor);
                    }
                }
                catch (Exception sqlE)
                {
                    Global.WriteOutput(sqlE.Message);
                }
            }
            return listLevelColor;
        }

        internal void AddBaseMap()
        {
            if (!imageListSymbols.Images.ContainsKey("NETWORK MAP"))
            {
                imageListSymbols.Images.Add("NETWORK MAP", RoadCare3.Properties.Resources.cross);
            }
            TreeNode tn = tvLayers.Nodes.Insert(0, "NETWORK MAP", "NETWORK MAP");
            tn.Checked = true;
			tn.Tag = "NETWORK MAP";
            
            tn.ImageIndex = imageListSymbols.Images.IndexOfKey("NETWORK MAP");
            tn.SelectedImageIndex = imageListSymbols.Images.IndexOfKey("NETWORK MAP");
            tvLayers.Refresh();
        }

		public void ReloadYearComboBox()
		{
			if (cbAttribute.Text == "")
			{
				return;
			}
			else
			{
				cbYear.Text = "";
				cbYear.Items.Clear();
				cbYear.Items.Add("Most Recent");
				if (m_htAttributeYears.Contains(cbAttribute.Text))
				{
					List<String> listYears = (List<String>)m_htAttributeYears[cbAttribute.Text];
					foreach (String year in listYears)
					{
						cbYear.Items.Add(year);
					}
				}

				// Now create some levels and color boxes.
				if (cbYear.Text != "")
				{
					CreateLegend();
				}
			}
		}

		private void cbAssets_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedAsset = cbAssets.Text;
			ConnectionParameters cp = DBMgr.GetAssetConnectionObject(selectedAsset);
			List<string> assetProperties = DBMgr.GetTableColumns(selectedAsset, cp);
			foreach (string assetProperty in assetProperties)
			{
				cbAssetProperty.Items.Add(assetProperty);
			}
		}

		private void buttonAddAttribute_Click(object sender, EventArgs e)
		{
			UpdateAttributeLevels();
			AddAttributeLayer();
		}

		public void UpdateAttributeLevels()
		{
			string attributeName = cbAttribute.Text;
			string attributeYear = attributeName + "_" + cbYear.Text;
			string simulationID = "";
			if (GISView != null)
			{
				simulationID = GISView.SimulationID;
			}
			string query = "";
			DataSet ds = null;
			if (Global.GetAttributeType(attributeName) != "NUMBER")
			{
				if (simulationID != "")
				{
					query = "SELECT DISTINCT " + attributeYear + " FROM SIMULATION_" + m_strNetworkID + "_" + simulationID;
					ds = DBMgr.ExecuteQuery(query);
				}
				else
				{
					ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(attributeName);
					query = "SELECT DISTINCT DATA_ FROM " + attributeName;
					ds = DBMgr.ExecuteQuery(query, cp);
				}
				List<LevelColors> listLevelColors = (List<LevelColors>)m_hashAttributeLevelColor[attributeName];
				if (listLevelColors != null)
				{
					listLevelColors.Clear();	
				}
				else
				{
					listLevelColors = new List<LevelColors>();
					m_hashAttributeLevelColor.Add(attributeName, listLevelColors);
				}
				int i = 0;
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					string level = dr[0].ToString();
					LevelColors lc = new LevelColors();
					lc.m_Color = m_listColor[i];
					lc.m_strLevel = level;
					listLevelColors.Add(lc);
					i++;
				}
			}
		}
	}

    [Serializable]
	public class LevelColors
    {
        public String m_strLevel;

        [XmlIgnoreAttribute]
        public Color m_Color;

        // Serializes the Color to XML.
        [XmlElement("Color")]
        public string ColorHtml
        {
            get
            {
                return
                    ColorTranslator.ToHtml(m_Color);
            }
            set
            {
                m_Color = ColorTranslator.FromHtml(value);
            }
        }

        public void ConvertColors()
        {
            ColorHtml = ColorTranslator.ToHtml(m_Color);
        }
	}
}
