using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;

namespace DataObjects
{
    public class AttributeObject
    {
        String m_strAttribute;
        bool m_bNative;
        String m_strProvider;
        String m_strServer;
        String m_strDataSource;
        String m_strUserID;
        String m_strPassword;
        String m_strSQLView;
        String m_strDefault;
        double m_dDefault;
        double m_dLevel1;
        double m_dLevel2;
        double m_dLevel3;
        double m_dLevel4;
        double m_dLevel5;
        bool m_bAscending;
        String m_strType;
        String m_strFormat;
        String m_strGrouping;
        double m_dMaximum;
        double m_dMinimum;
        bool m_bCalculated;
        String m_strAsset;
        String m_strAssetProperty;

        public String Attribute
        {
            set { m_strAttribute = value; }
            get { return m_strAttribute; }
        }

        /// <summary>
        /// True if RoadCare Native attribute
        /// </summary>
        public bool Native
        {
            set { m_bNative = value; }
            get { return m_bNative; }
        }
        /// <summary>
        /// OleDB provider if non-native
        /// </summary>
        public String Provider
        {
            set { m_strProvider = value; }
            get { return m_strProvider; }
        }
        /// <summary>
        /// OleDB provider if non-native
        /// </summary>
        public String Server
        {
            set { m_strServer = value; }
            get { return m_strServer; }
        }
        /// <summary>
        /// OleDB DataSource if non-native
        /// </summary>
        public String DataSource
        {
            set { m_strDataSource = value; }
            get { return m_strDataSource; }
        }
        /// <summary>
        /// OleDB UserID if non-native
        /// </summary>
        public String UserID
        {
            set { m_strUserID = value; }
            get { return m_strUserID; }
        }
        /// <summary>
        /// OleDB password if non-native
        /// </summary>
        public String Password
        {
            set { m_strPassword = value; }
            get { return m_strPassword; }
        }
        /// <summary>
        /// SQL View for non-native
        /// </summary>
        public String SQLView
        {
            set { m_strSQLView = value; }
            get { return m_strSQLView; }
        }
        /// <summary>
        /// Default (String)
        /// </summary>
        public String DefaultString
        {
            set { m_strDefault = value; }
            get { return m_strDefault; }
        }
        /// <summary>
        /// Default (NUMBER)
        /// </summary>
        public double DefaultNumber
        {
            set { m_dDefault = value; }
            get { return m_dDefault; }
        }
        /// <summary>
        /// Default Level1 for reporting purposes
        /// </summary>
        public double Level1
        {
            set { m_dLevel1 = value; }
            get { return m_dLevel1; }
        }
        /// <summary>
        /// Default Level2 for reporting purposes
        /// </summary>
        public double Level2
        {
            set { m_dLevel2 = value; }
            get { return m_dLevel2; }
        }
        /// <summary>
        /// Default Level3 for reporting purposes
        /// </summary>
        public double Level3
        {
            set { m_dLevel3 = value; }
            get { return m_dLevel3; }
        }
        /// <summary>
        /// Default Level4 for reporting purposes
        /// </summary>
        public double Level4
        {
            set { m_dLevel4 = value; }
            get { return m_dLevel4; }
        }
        /// <summary>
        /// Default Level5 for reporting purposes
        /// </summary>
        public double Level5
        {
            set { m_dLevel5 = value; }
            get { return m_dLevel5; }
        }
        /// <summary>
        /// True if variable gets smaller with deterioration
        /// </summary>
        public bool Ascending
        {
            set { m_bAscending = value; }
            get { return m_bAscending; }
        }
        /// <summary>
        /// Attribute Type (NUMBER or STRING)
        /// </summary>
        public String Type
        {
            set { m_strType = value; }
            get { return m_strType; }
        }      
        /// <summary>
        /// Format for display purposes (NUMBER)
        /// </summary>
        public String Format
        {
            set { m_strFormat = value; }
            get { return m_strFormat; }
        }
        /// <summary>
        /// Grouping for display purposes
        /// </summary>
        public String Grouping
        {
            set { m_strGrouping = value; }
            get { return m_strGrouping; }
        }
        /// <summary>
        /// Minimum value (optional)
        /// </summary>
        public double Minimum
        {
            set { m_dMinimum = value; }
            get { return m_dMinimum; }
        }
        /// <summary>
        /// Maximum value (optional)
        /// </summary>
        public double Maximum
        {
            set { m_dMaximum = value; }
            get { return m_dMaximum; }
        }
        /// <summary>
        /// True if calculated field.
        /// </summary>
        public bool Calculated
        {
            set { m_bCalculated = value; }
            get { return m_bCalculated; }
        }

        /// <summary>
        /// Asset from which Attribute is calculated
        /// </summary>
        public String Asset
        {
            set { m_strAsset = value; }
            get { return m_strAsset; }
        }

        /// <summary>
        /// Asset property which Attribute is calculated.
        /// </summary>
        public String AssetProperty
        {
            set { m_strAssetProperty = value; }
            get { return m_strAssetProperty; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AttributeObject()
        {
        }
        /// <summary>
        /// DataRow retrived from SELECT * FROM ATTRIBUTES
        /// </summary>
        /// <param name="row"></param>
        public AttributeObject(DataRow row)
        {
            if (row["ATTRIBUTE_"] != DBNull.Value)
            {
                this.Attribute = row["ATTRIBUTE_"].ToString();
            }
            if (row["NATIVE_"] != DBNull.Value)
            {
                //Can't use bool.Parse with "1" and "0"
				//this.Native = bool.Parse(row["NATIVE_"].ToString());
				this.Native = XmlConvert.ToBoolean ( row["NATIVE_"].ToString().ToLower() );
            }
            if (row["PROVIDER"] != DBNull.Value)
            {
                this.Provider = row["PROVIDER"].ToString();
            }
            if (row["SERVER"] != DBNull.Value)
            {
                this.Server = row["SERVER"].ToString();
            }
            if (row["DATASOURCE"] != DBNull.Value)
            {
                this.DataSource = row["DATASOURCE"].ToString();
            }
            if (row["USERID"] != DBNull.Value)
            {
                this.UserID = row["USERID"].ToString();
            }
            if (row["PASSWORD_"] != DBNull.Value)
            {
                this.Password = row["PASSWORD_"].ToString();
            }
            if (row["SQLVIEW"] != DBNull.Value)
            {
                this.SQLView= row["SQLVIEW"].ToString();
            }
            if (row["TYPE_"] != DBNull.Value)
            {
                this.Type = row["TYPE_"].ToString();
            }

            if (row["DEFAULT_VALUE"] != DBNull.Value)
            {
                this.DefaultString = row["DEFAULT_VALUE"].ToString();
                if (this.Type == "NUMBER")
                {
                    this.DefaultNumber = double.Parse(this.DefaultString);
                }
            }
            if (row["LEVEL1"] != DBNull.Value)
            {
                this.Level1 = double.Parse(row["LEVEL1"].ToString());
            }
            if (row["LEVEL2"] != DBNull.Value)
            {
                this.Level2 = double.Parse(row["LEVEL2"].ToString());
            }
            if (row["LEVEL3"] != DBNull.Value)
            {
                this.Level3 = double.Parse(row["LEVEL3"].ToString());
            }
            if (row["LEVEL4"] != DBNull.Value)
            {
                this.Level4 = double.Parse(row["LEVEL4"].ToString());
            }
            if (row["LEVEL5"] != DBNull.Value)
            {
                this.Level5 = double.Parse(row["LEVEL5"].ToString());
            }
            if (row["ASCENDING"] != DBNull.Value)
            {
				//Can't use bool.Parse with "1" and "0"
				//this.Ascending = bool.Parse(row["ASCENDING"].ToString());
				this.Ascending = XmlConvert.ToBoolean( row["ASCENDING"].ToString().ToLower() );
            }
            if (row["FORMAT"] != DBNull.Value)
            {
                this.Format = row["FORMAT"].ToString();
            }
            if (row["GROUPING"] != DBNull.Value)
            {
                this.Grouping = row["GROUPING"].ToString();
            }
            if (row["MINIMUM_"] != DBNull.Value)
            {
                this.Minimum = double.Parse(row["MINIMUM_"].ToString());
            }
            if (row["MAXIMUM"] != DBNull.Value)
            {
                this.Maximum = double.Parse(row["MAXIMUM"].ToString());
            }
            if (row["CALCULATED"] != DBNull.Value)
            {
				//Can't use bool.Parse with "1" and "0"
                //this.Calculated = bool.Parse(row["CALCULATED"].ToString());
				this.Calculated = XmlConvert.ToBoolean( row["CALCULATED"].ToString().ToLower() );
            }

            if (row["ASSET"] != DBNull.Value)
            {
                this.Asset = row["ASSET"].ToString();
            }

			//if (row["ASSET_PROPERTY"] != DBNull.Value)
			//{
			//    this.AssetProperty = row["ASSET_PROPERTY"].ToString();
			//}

        }





    }
}
