using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadCare3;
using RoadCareGlobalOperations;
using System.IO;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Geometries;
namespace RCImageView3
{
    public partial class FormGoogleMap : BaseForm
    {
        private ICoordinateSystem wgs84;
        private ICoordinateSystem nad83;

        HtmlDocument htdoc;
        public FormGoogleMap()
        {
           // string startURL = "file:///C:\\Documents and Settings\\glarson\\My Documents\\Visual Studio 2008\\Projects\\ILeader\\googleEarth.html";
            int nFind = Application.ExecutablePath.ToString().ToLower().IndexOf("rcimageview3.exe");
            string startURL = Application.ExecutablePath.ToString().Substring(0,nFind) + "MapForm.htm";
            
            
            string wkt = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";
            CoordinateSystemFactory cFacWGS = new CoordinateSystemFactory();
            wgs84 = cFacWGS.CreateFromWkt(wkt);

            //wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Meter\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",1312333.333333333],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-77],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Foot_US\",0.30480060960121924]]";
            wkt = "PROJCS[\"NAD_1983_StatePlane_Maryland_FIPS_1900_Meter\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",399999.2],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",-77],PARAMETER[\"Standard_Parallel_1\",38.3],PARAMETER[\"Standard_Parallel_2\",39.45],PARAMETER[\"Latitude_Of_Origin\",37.66666666666666],UNIT[\"Meter\",1]]";
            CoordinateSystemFactory cFac1 = new CoordinateSystemFactory();
            nad83 = cFac1.CreateFromWkt(wkt);
            InitializeComponent();
            htdoc = webBrowser1.Document;
            webBrowser1.Navigate(startURL);

        }
        public override void NavigationTick(NavigationObject navigationObject)
        {
            htdoc = webBrowser1.Document;
            //webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
            String strLatitude = navigationObject.CurrentImage.Latitude.ToString();
            String strLongitude = navigationObject.CurrentImage.Longitude.ToString();
            //VA059(DISTRICT OF COLUMBIA CO.) NAD83 ZONE = 4501 (LAMBERT|NORTHERN ZONE)
            if (double.Parse(strLatitude) < 0 || double.Parse(strLatitude) > 90 || double.Parse(strLongitude) > 0 || double.Parse(strLongitude) < -180)
            {
                SharpMap.Geometries.Point geoTransformed = (SharpMap.Geometries.Point)TransformSingleGeometry(new SharpMap.Geometries.Point(double.Parse(strLongitude), double.Parse(strLatitude)));
                webBrowser1.Document.InvokeScript("CenterMap", new string[] { geoTransformed.Y.ToString(), geoTransformed.X.ToString() });
            }
            else
            {
                webBrowser1.Document.InvokeScript("CenterMap", new string[] { strLatitude,strLongitude });
                
            }
           

        }

        private void FormGoogleMap_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void toolStripButtonKML_Click(object sender, EventArgs e)
        {
            if (openFileDialogkml.ShowDialog() == DialogResult.OK)
            {
                StreamReader re = File.OpenText(openFileDialogkml.FileName);

                String strKML = re.ReadToEnd();
                webBrowser1.Document.InvokeScript("loadKML", new string[] { strKML });
            }
        }


        private IGeometry TransformSingleGeometry(IGeometry g)
        {
            


            CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();
            ICoordinateTransformation transDeg2M = ctFac.CreateFromCoordinateSystems(nad83, wgs84);  //Geocentric->Geographic (WGS84)
            Geometry geo = GeometryTransform.TransformGeometry((Geometry)g, transDeg2M.MathTransform);
            
            return geo;
        }

    }
}
