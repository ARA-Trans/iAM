using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SharpMap.Geometries;

namespace RoadCare3
{
    public class NetworkDefinitionData
    {
        private String m_strFacility;
        private String m_strSection;
        private String m_strArea;
        private String m_strGeometry;
		private String m_Routes;
		private String m_Direction;

		private double m_Begin_Station;
		private double m_End_Station;
        private double m_fEnvelopeMinX;
        private double m_fEnvelopeMaxX;
		private double m_fEnvelopeMinY;
		private double m_fEnvelopeMaxY;

		private Geometry geom;

        public NetworkDefinitionData()
        {
            // Default constructor
        }

        public String Facility { get { return m_strFacility; } set { m_strFacility = value; } }
        public String Section { get { return m_strSection; } set { m_strSection = value; } }
        public String Area { get { return m_strArea; } set { m_strArea = value; } }
        public String Geometry { get { return m_strGeometry; } set { m_strGeometry = value; } }
		public Geometry Geo
		{
			get
			{
				return geom;
			}
			set
			{
				geom = value;
			}
		}
	
        public double EnvelopeMinX { get { return m_fEnvelopeMinX; } set { m_fEnvelopeMinX = value; } }
		public double EnvelopeMaxX
		{
			get
			{
				return m_fEnvelopeMaxX;
			}
			set
			{
				m_fEnvelopeMaxX = value;
			}
		}
		public double EnvelopeMinY
		{
			get
			{
				return m_fEnvelopeMinY;
			}
			set
			{
				m_fEnvelopeMinY = value;
			}
		}
        public double EnvelopeMaxY { get { return m_fEnvelopeMaxY; } set { m_fEnvelopeMaxY = value; } }

		public String Routes { get { return m_Routes; } set { m_Routes = value; } }
		public String Direction { get { return m_Direction; } set { m_Direction = value; } }
		public double Begin_Station { get { return m_Begin_Station; } set { m_Begin_Station = value; } }
		public double End_Station { get { return m_End_Station; } set { m_End_Station = value; } }

		public void CreateEnvelope()
		{
			if (geom != null)
			{
				BoundingBox envelope = geom.GetBoundingBox();
				m_fEnvelopeMinX = envelope.Min.X;
				m_fEnvelopeMaxX = envelope.Max.X;
				m_fEnvelopeMinY = envelope.Min.Y;
				m_fEnvelopeMaxY = envelope.Max.Y;
			}
			else
			{
				Global.WriteOutput( "Error: Attempted to create envelope for null geometry." );
			}
		}
        
		//public void CreateEnvelopes(SharpMap.Geometries.GeometryType2 geoType)
		//{
		//    if(geoType == GeometryType2.LineString)
		//    {
		//        float fLat = -1;
		//        float fLong = -1;

		//        String strLat;
		//        String strLong;

		//        String strWholeGeometry = Geometry.Remove(0, 11);
                
		//        strWholeGeometry = strWholeGeometry.Replace("(", "");
		//        strWholeGeometry = strWholeGeometry.Replace(")", "");

		//        string[] listOnCommas = strWholeGeometry.Split(',');
		//        string[] listOnSpaces;

		//        listOnSpaces = listOnCommas[0].Split(' ');
		//        strLong = listOnSpaces[0].ToString();
		//        strLat = listOnSpaces[1].ToString();

		//        try
		//        {
		//            m_fEnvelopeMaxX = float.Parse(strLong);
		//            m_fEnvelopeMinX = float.Parse(strLong);
		//            m_fEnvelopeMaxY = float.Parse(strLat);
		//            m_fEnvelopeMinY = float.Parse(strLat);
		//        }
		//        catch (Exception exc)
		//        {
		//            Global.WriteOutput("Error: " + exc.Message);
		//            return;
		//        }

		//        for (int i = 1; i < listOnCommas.Length; i++)
		//        {
		//            // Split each pair into LONG then LAT
		//            listOnSpaces = listOnCommas[i].Split(' ');
                    
		//            strLong = listOnSpaces[1].ToString();
		//            strLat = listOnSpaces[2].ToString();

		//            try
		//            {
		//                fLong = float.Parse(strLong);
		//                fLat = float.Parse(strLat);
		//            }
		//            catch (Exception exc)
		//            {
		//                Global.WriteOutput("Error: " + exc.Message);
		//                return;
		//            }

		//            if (fLong > m_fEnvelopeMaxX)
		//            {
		//                m_fEnvelopeMaxX = fLong;
		//            }
		//            if (fLong < m_fEnvelopeMinX)
		//            {
		//                m_fEnvelopeMinX = fLong;
		//            }
		//            if (fLat > m_fEnvelopeMaxY)
		//            {
		//                m_fEnvelopeMaxY = fLat;
		//            }
		//            if (fLat < m_fEnvelopeMinY)
		//            {
		//                m_fEnvelopeMinY = fLat;
		//            }
		//        }
		//    }
		//}


    }
}
