using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Drawing.Text;
using DatabaseManager;
using System.IO;
using SharpMap.Geometries;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;

namespace KMLManager
{
    static public class KML
    {
        static public String CreateKML(String strDirectory, String strNetwork, String strNetworkID, String strAttribute, String strYear,StringCollection listAdditional, bool bIsNumeric, float[] fLevel, Hashtable hashValueColor, StringCollection listColors)
        {
            String strKMLFilePath;
            String strKMLTitle;
            String strKMLDescription;
            String strFields = "";
            String strID = "";
            Hashtable hashIDValue = new Hashtable();

            strKMLFilePath = strDirectory + "\\" + strAttribute + "_" + strYear + ".kml";
            strKMLFilePath = strKMLFilePath.Replace(' ', '_');
            strKMLTitle = strAttribute + " (" + strYear + ")";
            strKMLDescription = strAttribute + "(" + strYear + ")" + " - " + strNetwork;

            ArrayList listAttributeYear = new ArrayList();
            if (strYear == "Most Recent")
            {
                strFields = strAttribute;
            }
            else
            {
                strFields = strAttribute + "_" + strYear;
            }

            listAttributeYear.Add(strFields);
            //Adds additional fields for placemark description

            foreach (String attribute in listAdditional)
            {
                listAttributeYear.Add(attribute);
            }
            
            String strSelect = "SELECT SECTION_" + strNetworkID + ".SECTIONID,FACILITY,SECTION,GEOMETRY";
            foreach (String sAttribute in listAttributeYear)
            {
                strSelect += ",";
                strSelect += sAttribute;
            }

            String strFrom = " FROM SECTION_" + strNetworkID + " INNER JOIN SEGMENT_" + strNetworkID + "_NS0 ON SECTION_" + strNetworkID + ".SECTIONID=SEGMENT_" + strNetworkID + "_NS0.SECTIONID";
            strSelect += strFrom;

            SqlCommand cmdField = new System.Data.SqlClient.SqlCommand(strSelect, DBMgr.NativeConnectionParameters.SqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmdField);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            //String strLatitude = "";
            //String strLongitude = "";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            XmlTextWriter xtr = new XmlTextWriter(strKMLFilePath, System.Text.Encoding.UTF8);

            xtr.Formatting = Formatting.Indented;
            xtr.WriteStartDocument();

            xtr.WriteStartElement("kml");
            xtr.WriteAttributeString("xmlns", "http://earth.google.com/kml/2.1");


            xtr.WriteStartElement("Document");
            xtr.WriteStartElement("name");
            xtr.WriteString(strKMLTitle);
            xtr.WriteEndElement(); //End Name

            xtr.WriteStartElement("description");
            xtr.WriteString(strKMLDescription);
            xtr.WriteEndElement();// End desciption

            //Write screen overlay for legend.

            xtr.WriteStartElement("ScreenOverlay");
            xtr.WriteAttributeString("id", "Legend");
            xtr.WriteStartElement("visibility");
            xtr.WriteString("1");
            xtr.WriteEndElement();
            xtr.WriteStartElement("Icon");
            xtr.WriteString("legend.gif");
            xtr.WriteEndElement();
            xtr.WriteStartElement("name");
            xtr.WriteString("Legend");
            xtr.WriteEndElement();
            xtr.WriteStartElement("drawOrder");
            xtr.WriteString("0");
            xtr.WriteEndElement();
            xtr.WriteStartElement("overlayXY");
            xtr.WriteAttributeString("x", "0.01");
            xtr.WriteAttributeString("y", "0.99");
            xtr.WriteAttributeString("xunits", "fraction");
            xtr.WriteAttributeString("yunits", "fraction");
            xtr.WriteEndElement();
            xtr.WriteStartElement("screenXY");
            xtr.WriteAttributeString("x", "0.01");
            xtr.WriteAttributeString("y", "0.99");
            xtr.WriteAttributeString("xunits", "fraction");
            xtr.WriteAttributeString("yunits", "fraction");
            xtr.WriteEndElement();
            xtr.WriteStartElement("size");
            xtr.WriteAttributeString("x", "0");
            xtr.WriteAttributeString("y", "00");
            xtr.WriteAttributeString("xunits", "pixels");
            xtr.WriteAttributeString("yunits", "pixels");
            xtr.WriteEndElement();
            xtr.WriteEndElement();


            xtr.WriteStartElement("ScreenOverlay");
            xtr.WriteAttributeString("id", "Logo");
            xtr.WriteStartElement("visibility");
            xtr.WriteString("1");
            xtr.WriteEndElement();
            xtr.WriteStartElement("Icon");
            xtr.WriteString("logo_white_trans.png");
            xtr.WriteEndElement();
            xtr.WriteStartElement("name");
            xtr.WriteString("Logo");
            xtr.WriteEndElement();
            xtr.WriteStartElement("drawOrder");
            xtr.WriteString("1");
            xtr.WriteEndElement();
            xtr.WriteStartElement("overlayXY");
            xtr.WriteAttributeString("x", "0.01");
            xtr.WriteAttributeString("y", "0.05");
            xtr.WriteAttributeString("xunits", "fraction");
            xtr.WriteAttributeString("yunits", "fraction");
            xtr.WriteEndElement();
            xtr.WriteStartElement("screenXY");
            xtr.WriteAttributeString("x", "0.01");
            xtr.WriteAttributeString("y", "0.05");
            xtr.WriteAttributeString("xunits", "fraction");
            xtr.WriteAttributeString("yunits", "fraction");
            xtr.WriteEndElement();
            xtr.WriteStartElement("size");
            xtr.WriteAttributeString("x", "0");
            xtr.WriteAttributeString("y", "00");
            xtr.WriteAttributeString("xunits", "pixels");
            xtr.WriteAttributeString("yunits", "pixels");
            xtr.WriteEndElement();
            xtr.WriteEndElement();




            // Write the line styles.
            String strFacility;
            String strSection;
            String strObjectName;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["GEOMETRY"].ToString() == "") continue;

                strID = row["SECTIONID"].ToString();
                strFacility = row["FACILITY"].ToString();
                strSection = row["SECTION"].ToString();
                String strValue = row[strAttribute].ToString();

                //Get list of additional attributes and add to hashAdditional. 

                strObjectName = strFacility + " " + strSection;
                if (Int32.Parse(strID) > 0)
                {
                    xtr.WriteStartElement("Style");
                    xtr.WriteAttributeString("id", strObjectName);
                    xtr.WriteStartElement("LineStyle");

                    xtr.WriteStartElement("color");

                    if (bIsNumeric)
                    {
                        String strColor = GetColor(fLevel, strValue, listColors);
                        xtr.WriteString(strColor);
                    }
                    else//Lookup value in hashValueColor
                    {
                        String strColor = "00000000";
                        if (hashValueColor.Contains(strValue))
                        {
                            strColor = (String)hashValueColor[strValue];
                        }
                        xtr.WriteString(strColor.ToString());
                    }

                    xtr.WriteEndElement();

                    xtr.WriteStartElement("width");
                    xtr.WriteString("3");
                    xtr.WriteEndElement();

                    xtr.WriteEndElement();//End Line Style
                    xtr.WriteEndElement();// End Style
                }
            }


            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["GEOMETRY"].ToString() == "") continue;
                String strValue = row[strAttribute].ToString();

                StringCollection listAdditionalValues = new StringCollection();
                listAdditionalValues.Add(strValue);
                foreach (String attribute in listAdditional)
                {
                    listAdditionalValues.Add(row[attribute].ToString());
                }

                //Now write Placemark for each
                xtr.WriteStartElement("Placemark");
                strObjectName = row["FACILITY"].ToString() + " " + row["SECTION"].ToString();
                xtr.WriteStartElement("name");
                xtr.WriteString(strObjectName);
                xtr.WriteEndElement();
                strID = row["SECTIONID"].ToString();

                xtr.WriteStartElement("description");
                String strDescription = WriteDescription(strID, strAttribute, listAdditional, listAdditionalValues);
                xtr.WriteString(strDescription);

                xtr.WriteEndElement();

                xtr.WriteStartElement("styleUrl");
                xtr.WriteString("#" + strObjectName);
                xtr.WriteEndElement();

                xtr.WriteStartElement("LineString");
                xtr.WriteStartElement("altitudeMode");
                xtr.WriteString("relative");
                xtr.WriteEndElement();

                xtr.WriteStartElement("coordinates");

                String strGeometry = row["GEOMETRY"].ToString();
                strGeometry = strGeometry.Remove(0, 11);//Remove LINESTRING(
                strGeometry = strGeometry.Remove(strGeometry.Length - 1);

                string[] longitudeLatitude = strGeometry.Split(',');
                foreach (string str in longitudeLatitude)
                {
                    String strWithComma = str.Replace(" ", ",");
                    xtr.WriteString(strWithComma + ",0\n");
                }
                xtr.WriteEndElement();// end coordinate
                xtr.WriteEndElement();// end LineString
                xtr.WriteEndElement();// end placemark
            }
            xtr.WriteEndElement();// End Document
            xtr.WriteEndDocument(); //End KML
            xtr.Close();
            return strKMLFilePath;
        }


        static public String CreateAssetKML(String strAsset, String strKMLPath, String strKMLDescription,StringCollection listAssetProperty)
        {

            XmlTextWriter xtr = new XmlTextWriter(strKMLPath, System.Text.Encoding.UTF8);

            xtr.Formatting = Formatting.Indented;

            xtr.WriteStartDocument();

            xtr.WriteStartElement("kml");
            xtr.WriteAttributeString("xmlns", "http://earth.google.com/kml/2.1");


            xtr.WriteStartElement("Document");
            xtr.WriteStartElement("name");
            xtr.WriteString(strAsset);
            xtr.WriteEndElement(); //End Name

            xtr.WriteStartElement("description");
            xtr.WriteString(strKMLDescription);
            xtr.WriteEndElement();// End desciption

            xtr.WriteStartElement("ScreenOverlay");
            xtr.WriteAttributeString("id", "Logo");
            xtr.WriteStartElement("visibility");
            xtr.WriteString("1");
            xtr.WriteEndElement();//End Visiblity
            xtr.WriteStartElement("Icon");
            xtr.WriteString("logo_white_trans.png");
            xtr.WriteEndElement();//End Icon
            xtr.WriteStartElement("name");
            xtr.WriteString("Logo");
            xtr.WriteEndElement();//End Name
            xtr.WriteStartElement("drawOrder");
            xtr.WriteString("1");
            xtr.WriteEndElement();//End Draw order
            xtr.WriteStartElement("overlayXY");
            xtr.WriteAttributeString("x", "0.01");
            xtr.WriteAttributeString("y", "0.05");
            xtr.WriteAttributeString("xunits", "fraction");
            xtr.WriteAttributeString("yunits", "fraction");
            xtr.WriteEndElement(); //end overlay XY
            xtr.WriteStartElement("screenXY");
            xtr.WriteAttributeString("x", "0.01");
            xtr.WriteAttributeString("y", "0.05");
            xtr.WriteAttributeString("xunits", "fraction");
            xtr.WriteAttributeString("yunits", "fraction");
            xtr.WriteEndElement();//End screen XY
            xtr.WriteStartElement("size");
            xtr.WriteAttributeString("x", "0");
            xtr.WriteAttributeString("y", "00");
            xtr.WriteAttributeString("xunits", "pixels");
            xtr.WriteAttributeString("yunits", "pixels");
            xtr.WriteEndElement(); // End size
            xtr.WriteEndElement(); // End Secreen Overlay

			String strSelect = "SELECT ID_,LONGITUDE,LATITUDE, GEOMETRY";

            foreach (String property in listAssetProperty)
            {
                strSelect += ",";
                strSelect += property;
            }
            strSelect += " FROM " + strAsset;
            SqlCommand cmdField = new System.Data.SqlClient.SqlCommand(strSelect, DBMgr.NativeConnectionParameters.SqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmdField);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            String strObjectName = "";

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strID = row["GEO_ID"].ToString();

                //<IconStyle id="ID">
                //  <!-- inherited from ColorStyle -->
                //  <color>ffffffff</color>            <!-- kml:color -->
                //  <colorMode>normal</colorMode>      <!-- kml:colorModeEnum:normal or random -->

                //  <!-- specific to IconStyle -->
                //  <scale>1</scale>                   <!-- float -->
                //  <heading>0</heading>               <!-- float -->
                //  <Icon>
                //    <href>...</href>
                //  </Icon> 
                //  <hotSpot x="0.5"  y="0.5" 
                //    xunits="fraction" yunits="fraction"/>    <!-- kml:vec2 -->                    
                //</IconStyle>
                //Get list of additional attributes and add to hashAdditional. 

                xtr.WriteStartElement("Style");
                xtr.WriteAttributeString("id", strID);
                xtr.WriteStartElement("IconStyle");

                xtr.WriteStartElement("color");
                String strColor = "ffff0000";// Replace with table hash
                xtr.WriteString(strColor);
                xtr.WriteEndElement();//End color

                xtr.WriteStartElement("scale");
                xtr.WriteString("1");
                xtr.WriteEndElement();//End Scale

                xtr.WriteStartElement("Icon");
                xtr.WriteStartElement("href");
                xtr.WriteString("http://maps.google.com/mapfiles/kml/paddle/wht-blank.png");
                xtr.WriteEndElement();//End href
                xtr.WriteEndElement();//End Icon

                xtr.WriteStartElement("hotSpot");
                xtr.WriteAttributeString("x", "0.5");
                xtr.WriteAttributeString("y", "0.5");
                xtr.WriteAttributeString("xunits", "fraction");
                xtr.WriteAttributeString("yunits", "fraction");
                xtr.WriteEndElement(); // End hotSpot

                xtr.WriteEndElement();//End IconStyle
                xtr.WriteEndElement();// End Style
            }


            foreach (DataRow row in ds.Tables[0].Rows)
            {
                StringCollection listPropertyValues = new StringCollection();
                foreach (String property in listAssetProperty)
                {
                    listPropertyValues.Add(row[property].ToString());
                }

                //Now write Placemark for each
                xtr.WriteStartElement("Placemark");
                strObjectName = row["GEO_ID"].ToString();

                xtr.WriteStartElement("name");
                xtr.WriteString(strObjectName);
                xtr.WriteEndElement();

                xtr.WriteStartElement("description");
                xtr.WriteString(WriteAssetDescription(listAssetProperty, listPropertyValues));
                xtr.WriteEndElement();//End Description

                xtr.WriteStartElement("styleUrl");
                xtr.WriteString("#" + strObjectName);
                xtr.WriteEndElement();

                xtr.WriteStartElement("Point");
                xtr.WriteStartElement("altitudeMode");
                xtr.WriteString("clampToGround");
                xtr.WriteEndElement();//End altitude mode

                xtr.WriteStartElement("coordinates");

				//SharpMap.Geometries.Point pointOrig = new SharpMap.Geometries.Point(double.Parse(row["LONGITUDE"].ToString()), double.Parse(row["LATITUDE"].ToString()));
				//SharpMap.Geometries.Point point = TransformGeo(pointOrig) as SharpMap.Geometries.Point;
				Geometry geo = Geometry.GeomFromText(row["GEOMETRY"].ToString());
				SharpMap.Geometries.Point pointGeom = geo as SharpMap.Geometries.Point;
                //xtr.WriteString(row["LONGITUDE"].ToString() + "," + row["LATITUDE"].ToString());
				xtr.WriteString(pointGeom.X + "," + pointGeom.Y);
                xtr.WriteEndElement();// end coordinate
                xtr.WriteEndElement();// end LineString
                xtr.WriteEndElement();// end placemark
            }
            xtr.WriteEndElement();// End Document
            xtr.WriteEndDocument(); //End KML
            xtr.Close();
            return strKMLPath;

        }

		//private static string TransformGeo(double longitude, double latitude )
		//{
		//    double[] originalPoint = new double[2];
		//    originalPoint[0] = longitude;
		//    originalPoint[1] = latitude;
		//    //Geometry geo = (Geometry)g;

		//    ProjNet.CoordinateSystems.CoordinateSystemFactory coordinateSystemGenerator = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
		//    ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory transformGenerator = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();

		//    string marylandStatePlaneWellKnownText = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Meter\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",399999.2],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-77],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Meter\",1]]";

		//    ProjNet.CoordinateSystems.ICoordinateSystem wgs1984System = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
		//    ProjNet.CoordinateSystems.ICoordinateSystem marylandSystem = coordinateSystemGenerator.CreateFromWkt(marylandStatePlaneWellKnownText);
			
		//    //ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation gpsToFlorida = transformGenerator.CreateFromCoordinateSystems(wgs1984System, marylandSystem);
		//    ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation marylandToGps = transformGenerator.CreateFromCoordinateSystems(marylandSystem, wgs1984System);

		//    double[] convertedPoint = marylandToGps.MathTransform.Transform(originalPoint);

		//    //string wkt = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";
		//    //CoordinateSystemFactory cFacWGS = new CoordinateSystemFactory();
		//    //ICoordinateSystem wgs84 = cFacWGS.CreateFromWkt(wkt);
		//    //ProjNet.CoordinateSystems.ICoordinateSystem wgs84 = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
		//    //
		//    // Acquire the state plane project from
		//    // http://spatialreference.org/
		//    //wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Meter\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",1312333.333333333],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-77],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Foot_US\",0.30480060960121924]]";
		//    //wkt = "PROJCS["NAD_1983_StatePlane_Florida_West_FIPS_0902_Feet",GEOGCS["GCS_North_American_1983",DATUM["D_North_American_1983",SPHEROID["GRS_1980",6378137,298.257222101]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Transverse_Mercator"],PARAMETER["False_Easting",656166.6666666665],PARAMETER["False_Northing",0],PARAMETER["Central_Meridian",-82],PARAMETER["Scale_Factor",0.9999411764705882],PARAMETER["Latitude_Of_Origin",24.33333333333333],UNIT["Foot_US",0.30480060960121924]]"
			
			
			
		//    //wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Meter\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",399999.2],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-77],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Meter\",1]]";
            
			
			
			
		//    //wkt = "PROJCS[\"GRS_1980_Transverse_Mercator\",GEOGCS[\"GCS_GRS_1980\",DATUM[\"D_GRS_1980\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"Scale_Factor\",1.00005000],PARAMETER[\"False_Easting\",164041.66666667],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-96.68805556],PARAMETER[\"Latitude_Of_Origin\",40.25000000],UNIT[\"Foot_US\",0.30480060960121924]]";
			
		//    //wkt = "PROJCS[\"NAD83 / Maryland (ftUS)\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",39.45],PARAMETER[\"standard_parallel_2\",38.3],PARAMETER[\"latitude_of_origin\",37.66666666666666],PARAMETER[\"central_meridian\",-77],PARAMETER[\"false_easting\",1312333.333],PARAMETER[\"false_northing\",0],UNIT[\"Foot_US\",0.30480060960121924]]";
		//    //wkt = "PROJCS[\"NAD83 / Maryland\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",39.45],PARAMETER[\"standard_parallel_2\",38.3],PARAMETER[\"latitude_of_origin\",37.66666666666666],PARAMETER[\"central_meridian\",-77],PARAMETER[\"false_easting\",400000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";
		//    //wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",400000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-77.0],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Meter\",1.0]]";
		//    //wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Feet\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",1312333.333333333],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-77.0],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Foot_US\",0.30480060960121924]]";
		//    //wkt = "PROJCS[\"NAD83(NSRS2007) / Maryland\",GEOGCS[\"NAD83(NSRS2007)\",DATUM[\"D_\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",39.45],PARAMETER[\"standard_parallel_2\",38.3],PARAMETER[\"latitude_of_origin\",37.66666666666666],PARAMETER[\"central_meridian\",-77],PARAMETER[\"false_easting\",400000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";
		//    //wkt = "PROJCS[\"NAD83(NSRS2007) / Maryland (ftUS)\",GEOGCS[\"NAD83(NSRS2007)\",DATUM[\"D_\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",39.45],PARAMETER[\"standard_parallel_2\",38.3],PARAMETER[\"latitude_of_origin\",37.66666666666666],PARAMETER[\"central_meridian\",-77],PARAMETER[\"false_easting\",1312333.333],PARAMETER[\"false_northing\",0],UNIT[\"Foot_US\",0.30480060960121924]]";
		//    //wkt = "PROJCS[\"NAD83(HARN) / Maryland\",GEOGCS[\"NAD83(HARN)\",DATUM[\"D_North_American_1983_HARN\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",39.45],PARAMETER[\"standard_parallel_2\",38.3],PARAMETER[\"latitude_of_origin\",37.66666666666666],PARAMETER[\"central_meridian\",-77],PARAMETER[\"false_easting\",400000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";
		//    //wkt = "PROJCS[\"NAD27 / Maryland\",GEOGCS[\"GCS_North_American_1927\",DATUM[\"D_North_American_1927\",SPHEROID[\"Clarke_1866\",6378206.4,294.9786982138982]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",38.3],PARAMETER[\"standard_parallel_2\",39.45],PARAMETER[\"latitude_of_origin\",37.83333333333334],PARAMETER[\"central_meridian\",-77],PARAMETER[\"false_easting\",800000.0000000002],PARAMETER[\"false_northing\",0],UNIT[\"Foot_US\",0.30480060960121924]]";
		//    //wkt = "PROJCS[\"NAD83(HARN) / Maryland (ftUS)\",GEOGCS[\"NAD83(HARN)\",DATUM[\"D_North_American_1983_HARN\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",39.45],PARAMETER[\"standard_parallel_2\",38.3],PARAMETER[\"latitude_of_origin\",37.66666666666666],PARAMETER[\"central_meridian\",-77],PARAMETER[\"false_easting\",1312333.333],PARAMETER[\"false_northing\",0],UNIT[\"Foot_US\",0.30480060960121924]]";
		//    //wkt = "PROJCS[\"NAD_1983_Albers\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Albers\"],PARAMETER[\"False_Easting\",0.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"central_meridian\",-79.0],PARAMETER[\"Standard_Parallel_1\",37.0],PARAMETER[\"Standard_Parallel_2\",41.0],PARAMETER[\"latitude_of_origin\",0.0],UNIT[\"Meter\",1.0]]";
		//    //ProjNet.CoordinateSystems.CoordinateSystemFactory cFac1 = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
		//    //ProjNet.CoordinateSystems.ICoordinateSystem nad83 = cFac1.CreateFromWkt(wkt);


		//    //ProjNet.CoordinateSystems.CoordinateSystemFactory ctFac = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
		//    //ICoordinateTransformation transDeg2M = ctFac.CreateFromCoordinateSystems(nad83, wgs84);  //Geocentric->Geographic (WGS84)
		//    //geo = GeometryTransform.TransformGeometry((Geometry)g, transDeg2M.MathTransform);
		//    //return geo;

		//    string coordinate = convertedPoint[0].ToString() + "," + convertedPoint[1].ToString();

		//    return coordinate;
		//}

        private static Bitmap CreateLegend(String strAttribute, StringCollection listColor, float[] fLevel)
        {
            Bitmap bmpImage = new Bitmap(1, 1);

            String strLegend = strAttribute + "\n";
            for (int n = 0; n < fLevel.GetLength(0); n++)
            {
                strLegend += "\n";
                strLegend += fLevel[n].ToString();
                strLegend += "\n";
            }


            int iWidth = 0;
            int iHeight = 0;

            // Create the Font object for the image text drawing.
            Font MyFont = new Font("Verdana", 8,
                               System.Drawing.FontStyle.Regular,
                               System.Drawing.GraphicsUnit.Point);

            // Create a graphics object to measure the text's width and height.
            Graphics MyGraphics = Graphics.FromImage(bmpImage);

            // This is where the bitmap size is determined.
            iWidth = 60;
            int iRowHeight = (int)MyGraphics.MeasureString(strAttribute, MyFont).Height;
            iHeight = (int)MyGraphics.MeasureString(strAttribute, MyFont).Height * 12;

            // Create the bmpImage again with the correct size for the text and font.
            bmpImage = new Bitmap(bmpImage, new Size(iWidth, iHeight));

            // Add the colors to the new bitmap.
            MyGraphics = Graphics.FromImage(bmpImage);
            MyGraphics.Clear(Color.White);
            MyGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            MyGraphics.DrawString(strLegend, MyFont,
                                new SolidBrush(Color.Black), 0, 0);
            int nColor = 0;
            foreach (String strColor in listColor)
            {
                System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(strColor);
                MyGraphics.FillRectangle(new SolidBrush(c), 10, 20 + nColor * 2 * (iRowHeight - 1), 40, 8);
                nColor++;
            }


            MyGraphics.Flush();

            return (bmpImage);
        }

        private static Bitmap CreateStringLegend(String strAttribute, StringCollection list, Hashtable hashValueColor)
        {
            Bitmap bmpImage = new Bitmap(1, 1);
            Font MyFont = new Font("Verdana", 8,
                               System.Drawing.FontStyle.Regular,
                               System.Drawing.GraphicsUnit.Point);

            // Create a graphics object to measure the text's width and height.
            Graphics MyGraphics = Graphics.FromImage(bmpImage);

            int nMaxWidth = 0;
            String strLegend = strAttribute + "\n\n";
            foreach (String str in list)
            {
                int iWidth = (int)MyGraphics.MeasureString(str, MyFont).Width;
                if (iWidth > nMaxWidth) nMaxWidth = iWidth;
                strLegend += str;
                strLegend += "\n";
            }


            int iHeight = 0;

            // Create the Font object for the image text drawing.

            // This is where the bitmap size is determined.
            int iLegendWidth = 40 + nMaxWidth;
            int iRowHeight = (int)MyGraphics.MeasureString(strAttribute, MyFont).Height;
            iHeight = (int)MyGraphics.MeasureString(strAttribute, MyFont).Height * (list.Count + 2);

            // Create the bmpImage again with the correct size for the text and font.
            bmpImage = new Bitmap(bmpImage, new Size(iLegendWidth, iHeight));

            // Add the colors to the new bitmap.
            MyGraphics = Graphics.FromImage(bmpImage);
            MyGraphics.Clear(Color.White);
            MyGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            MyGraphics.DrawString(strLegend, MyFont,
                                new SolidBrush(Color.Black), 0, 0);
            int nColor = 0;
            foreach (String value in list)
            {
                String strColor = (String)hashValueColor[value];
                System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(strColor);
                MyGraphics.FillRectangle(new SolidBrush(c), nMaxWidth + 5, ((float)2.5 + nColor) * (iRowHeight - 1), 30, 8);
                nColor++;
            }

            MyGraphics.Flush();

            return (bmpImage);
        }

        private static String TransformColor(String strColor)
        {
            String strRed = strColor.Substring(1, 2);
            String strBlue = strColor.Substring(5, 2);
            String strGreen = strColor.Substring(3, 2);
            strColor = "ff" + strBlue + strGreen + strRed;
            return strColor;


        }
        private static String WriteDescription(String strID, String strAttribute, StringCollection listAdditional, StringCollection listValues)
        {
            //CDATA example
            //<![CDATA[<b>Trail Head Name</b>]]>

            String strDescription = "";
            //Add main variable to head of list.

            //Look up values for this section.

            strDescription += strAttribute;
            strDescription += "=";
            strDescription += listValues[0].ToString();
            strDescription += "<br />\n";

            for (int n = 0; n < listAdditional.Count; n++)
            {
                String strAdditional = listAdditional[n].ToString();
                String strValue = listValues[n + 1].ToString();

                strDescription += strAdditional;
                strDescription += "=";
                strDescription += strValue;
                strDescription += "<br />\n";
            }
            strDescription += "";
            return strDescription;
        }


        private static String WriteAssetDescription(StringCollection listAdditional, StringCollection listValues)
        {
            //CDATA example
            //<![CDATA[<b>Trail Head Name</b>]]>

            String strDescription = "";
            //Add main variable to head of list.

            //Look up values for this section.
            for (int n = 0; n < listAdditional.Count; n++)
            {
                String strAdditional = listAdditional[n].ToString();
                String strValue = listValues[n].ToString();

                strDescription += strAdditional;
                strDescription += "=";
                strDescription += strValue;
                strDescription += "<br />\n";
            }
            strDescription += "";
            return strDescription;
        }


        static private String GetColor(float[] fLevel, String strValue, StringCollection listColor)
        {
            if (strValue == "") return "ff000000";
            float fValue = (float)System.Convert.ToDouble(strValue);

            if (fLevel[0] > fLevel[4])//Ascending variable
            {
                if (fLevel[0] <= fValue)
                {
                    return listColor[0];
                }
                else if (fLevel[1] <= fValue && fLevel[0] > fValue)
                {
                    return listColor[1];
                }
                else if (fLevel[2] <= fValue && fLevel[1] > fValue)
                {
                    return listColor[2];
                }
                else if (fLevel[3] <= fValue && fLevel[2] > fValue)
                {
                    return listColor[3];
                }
                else if (fLevel[4] <= fValue && fLevel[3] > fValue)
                {
                    return listColor[4];
                }
                else if (fValue < fLevel[4])
                {
                    return listColor[5];
                }
                else
                {
                    return "ff000000";
                }
            }
            else
            {

                if (fLevel[0] > fValue)
                {
                    return listColor[0];
                }
                else if (fLevel[1] > fValue && fLevel[0] <= fValue)
                {
                    return listColor[1];
                }
                else if (fLevel[2] > fValue && fLevel[1] <= fValue)
                {
                    return listColor[2];
                }
                else if (fLevel[3] > fValue && fLevel[2] <= fValue)
                {
                    return listColor[3];
                }
                else if (fLevel[4] > fValue && fLevel[3] <= fValue)
                {
                    return listColor[4];
                }
                else if (fLevel[4] <= fValue)
                {
                    return listColor[5];
                }
                else
                {
                    return "ff000000";
                }
            }
        }
    }
}
