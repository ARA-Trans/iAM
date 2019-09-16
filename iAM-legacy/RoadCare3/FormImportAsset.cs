using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpMap.Data;
using SharpMap.Data.Providers;
using System.Collections;
using Microsoft.SqlServer.Management.Smo;
using DatabaseManager;
using System.IO;
using SharpMap.Geometries;

namespace RoadCare3
{
    public partial class FormImportAsset : Form
    {
        private String m_connectionString;
        private String m_strShapeFilePath;
        private Hashtable m_htAssetFields = new Hashtable();  // Key is name of field, object is datatype.
        private Hashtable m_htShapefileFields = new Hashtable();
        private String m_strAssetTableName;

        public FormImportAsset()
        {
            InitializeComponent();
        }

        private void FormImportAsset_Load(object sender, EventArgs e)
        {
            lbAssetFields.Items.Add("FACILITY");
            lbAssetFields.Items.Add("SECTION");
			lbAssetFields.Items.Add("BEGIN_STATION");
			lbAssetFields.Items.Add("END_STATION");
            lbAssetFields.Items.Add("DIRECTION");
            lbAssetFields.Items.Add("MILEPOST");
			lbAssetFields.Items.Add("LATITUDE");
			lbAssetFields.Items.Add("LONGITUDE");
            lbAssetFields.Items.Add("GEOMETRY");
			lbAssetFields.Items.Add("EnvelopeMaxX");
			lbAssetFields.Items.Add("EnvelopeMaxY");
			lbAssetFields.Items.Add("EnvelopeMinX");
			lbAssetFields.Items.Add("EnvelopeMinY");
			lbAssetFields.Items.Add("LAST_MODIFIED");

			m_htAssetFields.Add("FACILITY", DataType.VarChar(-1));
			m_htAssetFields.Add("SECTION", DataType.VarChar(-1));
			m_htAssetFields.Add("BEGIN_STATION", DataType.Float);
			m_htAssetFields.Add("END_STATION", DataType.Float);
			m_htAssetFields.Add("DIRECTION", DataType.VarChar(-1));
            m_htAssetFields.Add("MILEPOST", DataType.Float);
            m_htAssetFields.Add("LAST_MODIFIED", DataType.DateTime);
			m_htAssetFields.Add("LONGITUDE", DataType.Float);
			m_htAssetFields.Add("LATITUDE", DataType.Float);
			m_htAssetFields.Add("EnvelopeMaxX", DataType.Float);
			m_htAssetFields.Add("EnvelopeMinX", DataType.Float);
			m_htAssetFields.Add("EnvelopeMaxY", DataType.Float);
			m_htAssetFields.Add("EnvelopeMinY", DataType.Float);
			m_htAssetFields.Add("GEOMETRY", DataType.VarChar(-1));
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = "ESRI Shapefiles (*.shp)|*.shp";
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                //lbAssetFields.Items.Clear();
                lbFieldNames.Items.Clear();
                String strFilePath = dlgOpen.FileName;
                int iParseFileName = dlgOpen.FileName.LastIndexOf('\\');
                String strFileName = dlgOpen.FileName.Substring(iParseFileName + 1);
                m_strShapeFilePath = strFilePath;
                strFilePath = strFilePath.Substring(0, iParseFileName + 1);
                tbShapeFilePath.Text = m_strShapeFilePath;

                // Set the DBF database connection string
                m_connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + ";Extended Properties=dBASE IV;User ID=Admin;Password=;";

                // Open the shapefile at the given path
                ShapeFile shapefile = null;
                try
                {
                    shapefile = new SharpMap.Data.Providers.ShapeFile(m_strShapeFilePath);
                    shapefile.Open();
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: Couldn't open shapefile." + exc.Message);
                    return;
                }
                FeatureDataRow fdr = shapefile.GetFeature(0);
                lbFieldNames.Items.Add("GEOMETRY");
                m_htShapefileFields.Clear();
                for (int i = 0; i < fdr.Table.Columns.Count; i++)
                {
                    lbFieldNames.Items.Add(fdr.Table.Columns[i].ColumnName.ToUpper());
                    m_htShapefileFields.Add(fdr.Table.Columns[i].ColumnName.ToUpper(), fdr.Table.Columns[i].DataType.ToString());
                }
                shapefile.Close();
            }
        }

        private void btnAddNewAttribute_Click(object sender, EventArgs e)
        {
            if (cbDataType.Text != "" && tbNewField.Text != "")
            {
                for (int i = 0; i < lbAssetFields.Items.Count; i++)
                {
                    if (lbAssetFields.Items[i].ToString().ToUpper().Trim() == tbNewField.Text.ToUpper().Trim())
                    {
                        return;
                    }
                }
                m_htAssetFields.Add(tbNewField.Text, Global.ConvertStringToDataType(cbDataType.Text));
                lbAssetFields.Items.Add(tbNewField.Text);
            }
            else
            {
                Global.WriteOutput("Error: Please complete both the data type field and new asset attribute field.");
            }
            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbFieldNames.SelectedItems.Count; i++)
            {
                String strField = lbFieldNames.SelectedItems[i].ToString();
                if(CheckMatchingAssetField(strField))continue;
                lbAssetFields.Items.Add(strField);
                m_htAssetFields.Add(strField,Global.ConvertStringToDataType(m_htShapefileFields[strField].ToString()));
            }
            
        }

        private bool CheckMatchingAssetField(String strValue)
        {
            for (int j = 0; j < lbAssetFields.Items.Count; j++)
            {
                if (lbAssetFields.Items[j].ToString().ToUpper().Trim() == strValue.ToUpper().Trim())
                {
                    return true;
                }
            }
            return false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbAssetFields.SelectedItems.Count; i++)
            {
                if (lbAssetFields.SelectedItems[i].ToString() != "EnvelopeMaxX" &&
					lbAssetFields.SelectedItems[i].ToString() != "EnvelopeMinX" &&
					lbAssetFields.SelectedItems[i].ToString() != "EnvelopeMaxY" &&
					lbAssetFields.SelectedItems[i].ToString() != "EnvelopeMinY" &&
					lbAssetFields.SelectedItems[i].ToString() != "LATITUDE" &&
					lbAssetFields.SelectedItems[i].ToString() != "LONGITUDE" &&
					lbAssetFields.SelectedItems[i].ToString() != "BEGIN_STATION" &&
					lbAssetFields.SelectedItems[i].ToString() != "END_STATION" &&
                    lbAssetFields.SelectedItems[i].ToString() != "FACILITY" &&
                    lbAssetFields.SelectedItems[i].ToString() != "DIRECTION" &&
                    lbAssetFields.SelectedItems[i].ToString() != "SECTION" &&
                    lbAssetFields.SelectedItems[i].ToString() != "MILEPOST" &&
                    lbAssetFields.SelectedItems[i].ToString() != "LAST_MODIFIED" &&
                    lbAssetFields.SelectedItems[i].ToString() != "GEOMETRY")
                {
                    m_htAssetFields.Remove(lbAssetFields.SelectedItems[i].ToString());
                    lbAssetFields.Items.Remove(lbAssetFields.SelectedItems[i]);
                }
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            //Make a list of new indexes.
            List<int> listIndex = new List<int>();
            List<String> listUp = new List<String>();


            foreach (String str in lbAssetFields.SelectedItems)
            {
                int nIndex = lbAssetFields.FindStringExact(str);
                if (!listIndex.Contains(nIndex - 1) && (nIndex - 1) >= 0)
                {
                    listIndex.Add(nIndex - 1);
                }
                else
                {
                    listIndex.Add(nIndex);
                }
                listUp.Add(str);
            }

            // Delete selected items        
            while (lbAssetFields.SelectedItems.Count > 0)
            {
                lbAssetFields.Items.Remove(lbAssetFields.SelectedItems[0]);
            }

            //Re-add moved up one
            int n = 0;
            foreach (String str in listUp)
            {
                int nIndex = listIndex[n];
                lbAssetFields.Items.Insert(nIndex, str);
                n++;
            }
            //Reselect items
            foreach (int nIndex in listIndex)
            {
                lbAssetFields.SetSelected(nIndex, true);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            //Make a list of new indexes.
            List<int> listIndex = new List<int>();
            List<String> listUp = new List<String>();

            foreach (String str in lbAssetFields.SelectedItems)
            {
                int nIndex = lbAssetFields.FindStringExact(str);
                if (!listIndex.Contains(nIndex + 1) && (nIndex + 1) < lbAssetFields.Items.Count)
                {
                    listIndex.Add(nIndex + 1);
                }
                else
                {
                    listIndex.Add(nIndex);
                }
                listUp.Add(str);
            }

            // Delete selected items        
            while (lbAssetFields.SelectedItems.Count > 0)
            {
                lbAssetFields.Items.Remove(lbAssetFields.SelectedItems[0]);
            }

            //Re-add moved up one
            int n = 0;
            foreach (String str in listUp)
            {
                int nIndex = listIndex[n];
                if (nIndex >= lbAssetFields.Items.Count) nIndex = lbAssetFields.Items.Count;
                lbAssetFields.Items.Insert(nIndex, str);
                n++;
            }
            //Reselect items
            foreach (int nIndex in listIndex)
            {
                lbAssetFields.SetSelected(nIndex, true);
            }
        }

		private void btnOk_Click( object sender, EventArgs e )
		{
			if( tbAssetName.Text.ToString().Trim() == "" )
			{
				return;
			}
			try
			{
				List<TableParameters> listCol = new List<TableParameters>();
				listCol.Add( new TableParameters( "GEO_ID", DataType.Int, false, true, true ) );
				for( int i = 0; i < lbAssetFields.Items.Count; i++ )
				{
					String strAssetField = lbAssetFields.Items[i].ToString();
					DataType dataType = ( DataType )m_htAssetFields[strAssetField];
					listCol.Add( new TableParameters( strAssetField, dataType, true ) );
				}
				DBMgr.CreateTable( tbAssetName.Text, listCol );
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: " + exc.Message );
				return;
			}

			// Create the <asset name>_CHANGELOG table
			List<TableParameters> listAssetChangeLog = new List<TableParameters>();
			listAssetChangeLog.Add( new TableParameters( "ID", DataType.Int, false, true, true ) );
			listAssetChangeLog.Add( new TableParameters( "ATTRIBUTE_ID", DataType.Int, false, false, false ) );
			listAssetChangeLog.Add( new TableParameters( "FIELD", DataType.VarChar( 50 ), false, false, false ) );
			listAssetChangeLog.Add( new TableParameters( "VALUE", DataType.VarChar( 50 ), true, false, false ) );
			listAssetChangeLog.Add( new TableParameters( "USER_ID", DataType.VarChar( 200 ), true, false, false ) );
			listAssetChangeLog.Add( new TableParameters( "WORKACTIVITY_ID", DataType.VarChar( -1 ), true, false, false ) );
			listAssetChangeLog.Add( new TableParameters( "DATE_MODIFIED", DataType.DateTime, false, false, false ) );
			try
			{
				DBMgr.CreateTable( tbAssetName.Text + "_CHANGELOG", listAssetChangeLog );
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: Couldn't create " + tbAssetName.Text + "_CHANGELOG table. " + exc.Message );
			}

			String strInsert = "INSERT INTO ASSETS (ASSET, DATE_CREATED) VALUES ('" + tbAssetName.Text + "', '" + DateTime.Now.ToString() + "')";
			try
			{
				//NSERT INTO table_name (column1, column2,...)VALUES (value1, value2,....)
				DBMgr.ExecuteNonQuery( strInsert );
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: Adding new asset to ASSETS table. " + exc.Message );
			}

			// This is set so SolutionExplorer can add the node with the correct name.
			m_strAssetTableName = tbAssetName.Text;

			// Create the temp shapefile
			Global.CreateTempShapeFile( tbShapeFilePath.Text );

			// Now run the select statement the user has in the select textbox.
			TextWriter tw = null;
			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);


			String strOutFile = strMyDocumentsFolder + "\\ShapeFileImport.txt";
			try
			{
				tw = new StreamWriter( strOutFile );
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: " + exc.Message );
				return;
			}


			String strSelect = tbSelect.Text;
			DataReader dr = null;
			try
			{
				dr = new DataReader( strSelect );
				List<string> listColumns = new List<string>();
				for( int i = 0; i < dr.FieldCount; i++ )
				{
					listColumns.Add( dr.GetName( i ) );
				}
				while( dr.Read() )
				{
					//String strGeometry = dr["GEOMETRY"].ToString();
					//string[] aGeometry = strGeometry.Split('(', ' ', ')');
					//String strLongitude = "";
					//String strLatitude = "";
					//if (aGeometry[0].ToString() == "POINT")
					//{
					//    strLongitude = aGeometry[2].ToString();
					//    strLatitude = aGeometry[3].ToString();
					//}
					Geometry shapefileGeom = Geometry.GeomFromText( dr["GEOMETRY"].ToString() );
					String strOut = "\t";
					for( int i = 0; i < lbAssetFields.Items.Count; i++ )
					{
						//if (lbAssetFields.Items[i].ToString() == "LATITUDE")
						//{
						//    strOut += strLatitude;

						//}
						//else if (lbAssetFields.Items[i].ToString() == "LONGITUDE")
						//{
						//    strOut += strLongitude;
						//}
						string uppered = lbAssetFields.Items[i].ToString().ToUpper();
						if( uppered == "ENVELOPEMAXX" )
						{
							strOut += shapefileGeom.GetBoundingBox().Max.X;
						}
						else if( uppered == "ENVELOPEMINX" )
						{
							strOut += shapefileGeom.GetBoundingBox().Min.X;
						}
						else if( uppered == "ENVELOPEMAXY" )
						{
							strOut += shapefileGeom.GetBoundingBox().Max.Y;
						}
						else if( uppered == "ENVELOPEMINY" )
						{
							strOut += shapefileGeom.GetBoundingBox().Min.Y;
						}
						else if( listColumns.Contains( lbAssetFields.Items[i].ToString() ) )
						{
							strOut += dr[lbAssetFields.Items[i].ToString()].ToString();
						}

						if( i != lbAssetFields.Items.Count - 1 )
						{
							strOut += '\t';
						}
					}
					tw.WriteLine( strOut );
				}
				dr.Close();
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: " + exc.Message );
			}
			tw.Close();

			try
			{
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad( m_strAssetTableName, strOutFile, '\t' );
						break;
					case "ORACLE":
						throw new NotImplementedException( "TODO: figure out columns for btnOk_Click()" );
					//DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, m_strAssetTableName, strOutFile, 
					//break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
					//break;
				}
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: " + exc.Message );
			}
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

        public String AssetName
        {
            get { return m_strAssetTableName; }
            //set { m_strAssetTableName = value; }
        }

        private void btnSQL_Click(object sender, EventArgs e)
        {
            String strQuery = "SELECT ";
            for (int i = 0; i < lbAssetFields.Items.Count; i++)
            {
                strQuery += lbAssetFields.Items[i].ToString();
                if (i != lbAssetFields.Items.Count - 1)
                {
                    strQuery += ",";
                }
            }
            strQuery += " FROM TEMP_SHAPEFILE";
            tbSelect.Text = strQuery;
        }
    }
}
