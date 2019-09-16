using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Data;
using SharpMap.Data.Providers;


namespace OpenGISExtension
{
    public class SharpLine
    {
        private List<SegmentedPoints> m_listSegPts;
        private String m_strBegin;
        private String m_strEnd;
        private SharpLineEnvelope m_SharpLineEnvelope;

		public double Begin_Seg
		{
			get { return double.Parse(m_strBegin); }
		}

		public double End_Seg
		{
			get { return double.Parse(m_strEnd); }
		}

        public SharpLine(List<Point> listPts, String strBegin, String strEnd)
        {
            m_strBegin = strBegin;
            m_strEnd = strEnd;
            m_listSegPts = new List<SegmentedPoints>();
            for (int i = 0; i < listPts.Count; i++)
            {
                m_listSegPts.Add(new SegmentedPoints(listPts.ElementAt(i)));
            }
            CalculateLengths();
            m_SharpLineEnvelope = new SharpLineEnvelope(m_listSegPts);
        }

        private void CalculateLengths()
        {
            double dOut;
            for (int i = 1; i < m_listSegPts.Count; i++)
            {
                double dDistance = Math.Pow(Math.Pow(m_listSegPts[i - 1].X - m_listSegPts[i].X, 2) + Math.Pow(m_listSegPts[i - 1].Y - m_listSegPts[i].Y, 2), 0.5);
                m_listSegPts[i].Length = m_listSegPts[i - 1].Length + dDistance;
            }
            double.TryParse(m_strBegin, out dOut);
            m_listSegPts[0].LengthMiles = dOut;

            double dEndMiles;
            double dBeginMiles;

            double.TryParse(m_strEnd, out dEndMiles);
            double.TryParse(m_strBegin, out dBeginMiles);

            double dLengthMiles = dEndMiles - dBeginMiles;

            for (int i = 1; i < m_listSegPts.Count; i++)
            {
                m_listSegPts[i].LengthMiles = dBeginMiles + ((m_listSegPts[i].Length / m_listSegPts[m_listSegPts.Count - 1].Length) * dLengthMiles);
            }
        }

        public SharpLineEnvelope Envelope
        {
            get { return m_SharpLineEnvelope; }
        }

        public String GetLineSegment(String strBegin, String strEnd)
        {
            double dBegin;
            double dEnd;

            double.TryParse(strEnd, out dEnd);
            double.TryParse(strBegin, out dBegin);

            Collection<Point> listPts = new Collection<Point>();

            bool bCalculateEndPt = false;

            for (int i = 0; i < m_listSegPts.Count; i++)
            {
                if (m_listSegPts[i].LengthMiles >= dBegin && m_listSegPts[i].LengthMiles <= dEnd)
                {
                    if (dEnd != m_listSegPts[i].LengthMiles)
                    {
                        bCalculateEndPt = true;
                    }
                    if (listPts.Count == 0)
                    {
                        if (m_listSegPts[i].LengthMiles != dBegin && i != 0)
                        {
                            double dDistanceMiles = m_listSegPts[i].LengthMiles - m_listSegPts[i - 1].LengthMiles;

                            double dDistanceDeltaMiles = m_listSegPts[i].LengthMiles - dBegin;

                            double dX = (m_listSegPts[i].X - m_listSegPts[i - 1].X) * (dDistanceDeltaMiles / dDistanceMiles) + m_listSegPts[i - 1].X;
                            double dY = (m_listSegPts[i].Y - m_listSegPts[i - 1].Y) * (dDistanceDeltaMiles / dDistanceMiles) + m_listSegPts[i - 1].Y;

                            Point ptToAdd = new Point(dX, dY);
                            listPts.Add(ptToAdd);
                        }

                    }

                    listPts.Add(new Point(m_listSegPts[i].X, m_listSegPts[i].Y));

                }

                if (m_listSegPts[i].LengthMiles > dEnd && bCalculateEndPt == true)
                {
                    bCalculateEndPt = false;
                    double dDistanceMiles = m_listSegPts[i].LengthMiles - m_listSegPts[i - 1].LengthMiles;

                    double dDistanceDeltaMiles = dEnd - m_listSegPts[i - 1].LengthMiles;

                    double dX = ((m_listSegPts[i].X - m_listSegPts[i - 1].X) * -(dDistanceDeltaMiles / dDistanceMiles)) + m_listSegPts[i].X;
                    double dY = ((m_listSegPts[i].Y - m_listSegPts[i - 1].Y) * -(dDistanceDeltaMiles / dDistanceMiles)) + m_listSegPts[i].Y;

                    Point ptToAdd = new Point(dX, dY);
                    listPts.Add(ptToAdd);
                }
            }
            StringBuilder strBuilder = new StringBuilder();
            strBuilder = new StringBuilder();
            strBuilder.Append("LINESTRING(");
            for (int i = 0; i < listPts.Count; i++)
            {
                Point sp = (Point)listPts[i];
                strBuilder.Append(sp.X.ToString() + " " + sp.Y.ToString());
                if (i != listPts.Count - 1)
                {
                    strBuilder.Append(",");
                }
                else
                {
                    strBuilder.Append(")");
                }
            }
            if (strBuilder.ToString() == "LINESTRING(")
            {
                return "";
            }
            else
            {
                return strBuilder.ToString();
            }
        }
    }

    public class SharpLineEnvelope
    {
        private double fMinLat = 99999999;
        private double fMaxLat = -99999999;
        private double fMinLong = 99999999;
        private double fMaxLong = -99999999;

        public SharpLineEnvelope(List<SegmentedPoints> listSegmentedPoints)
        {
            fMaxLat = listSegmentedPoints.Max((segPt) => segPt.Y);
            fMinLat = listSegmentedPoints.Min((segPt) => segPt.Y);
            fMaxLong = listSegmentedPoints.Max((segPt) => segPt.X);
            fMinLong = listSegmentedPoints.Min((segPt) => segPt.X);
        }

        public double Envelope_MinX
        {
            get { return fMinLong; }
        }

        public double Envelope_MaxX
        {
            get { return fMaxLong; }
        }

        public double Envelope_MinY
        {
            get { return fMinLat; }
        }

        public double Envelope_MaxY
        {
            get { return fMaxLat; }
        }
        
    }

    public class SegmentedPoints
    {
        double dX;
        double dY;
        double dLength;
        double dLengthMiles;

        public SegmentedPoints(SharpMap.Geometries.Point pt)
        {
            dLength = 0;
            dX = pt.X;
            dY = pt.Y;
        }

        public double X
        {
            get { return dX; }
            set { dX = value; }
        }

        public double Y
        {
            get { return dY; }
            set { dY = value; }
        }

        public double Length
        {
            get { return dLength; }
            set { dLength = value; }
        }

        public double LengthMiles
        {
            get { return dLengthMiles; }
            set { dLengthMiles = value; }
        }
    }
}
