using DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataAccessLayer
{
    public static class ImportOMS
    {
        /// <summary>
        /// Gets the connection string for the OMS database.
        /// </summary>
        /// <param name="connectionString">RoadCare database connection string.</param>
        /// <returns>OMS connection string</returns>
        public static String GetOMSConnectionString(string connectionString)
        {
            String omsConnectionString = null;
            if (!connectionString.Contains("Provider=OraOLEDB.Oracle;"))//OMS can never be an oracle provider
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string select = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME = 'OMS_CONNECTION_STRING'";
                        SqlCommand cmd = new SqlCommand(select, connection);
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            if (dr["OPTION_VALUE"] != DBNull.Value) omsConnectionString = Convert.ToString(dr["OPTION_VALUE"]);
                        }
                    }
                    DB.TablePrefix = "";
                    DB.ConnectionString = connectionString;
                    DB.OMSConnectionString = omsConnectionString;
                }
                catch
                {
                    omsConnectionString = null;
                }
            }
            return omsConnectionString;
        }


        /// <summary>
        /// Gets list of assets from the OMS assets to simulate on from the OMS database
        /// </summary>
        /// <returns>List of assets</returns>
        public static List<String> GetOMSAssets()
        {
            List<String> assets = new List<String>();
            String concantenatedAssets = "";
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string select = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME = 'OMS_ASSETS'";
                SqlCommand cmd = new SqlCommand(select, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["OPTION_VALUE"] != DBNull.Value) concantenatedAssets = Convert.ToString(dr["OPTION_VALUE"]);
                }
            }
            if(concantenatedAssets.Length > 0)
            {
                String[] values = concantenatedAssets.Split(';');
                assets = values.ToList();
            }
            return assets;
        }

        /// <summary>
        /// Gets list of additional OMS attributes.
        /// </summary>
        /// <returns>List of assets</returns>
        public static List<String> GetOMSAdditionalAttributes()
        {
            List<String> attributes = new List<String>();
            String additionalAttributes = "";
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string select = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME = 'OMS_ADDITIONAL_ATTRIBUTE'";
                SqlCommand cmd = new SqlCommand(select, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["OPTION_VALUE"] != DBNull.Value) additionalAttributes = Convert.ToString(dr["OPTION_VALUE"]);
                }
            }
            if (additionalAttributes.Length > 0)
            {
                String[] values = additionalAttributes.Split(';');
                attributes = values.ToList();
            }
            return attributes;
        }


        public static String MakeRawDataView(String attribute)
        {
            String view = "SELECT NULL AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, t1.FACILITY, t2.SECTION, NULL AS SAMPLE_, t3.ASSET_DATE AS DATE_, t3.DATA_ AS DATA_ ";
            view += "FROM ";
            view += "(SELECT ASSET_VALUE AS FACILITY, OID FROM OMS_ASSETS WHERE ATTRIBUTE_ = 'AssetType') AS t1 ";
            view += " INNER JOIN ";
            view += "(SELECT ASSET_VALUE + ' (' + CAST(OID AS varchar(50)) + ')' AS SECTION, OID FROM OMS_ASSETS WHERE ATTRIBUTE_ = 'ID') AS t2 ";
            view += "ON t1.OID = t2.OID ";
            view += " INNER JOIN ";
            view += " (SELECT ASSET_DATE, ASSET_VALUE AS DATA_, OID FROM OMS_ASSETS WHERE ATTRIBUTE_ = @attribute) AS t3 ";
            view += "ON t1.OID = t3.OID";
            return view;
        }



        public static void UpdateAttributesForOMS(List<AttributeStore> attributes, String server, String dataSource, bool integratedSecurity)
        {
            List<String> existingAttributes = new List<String>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string select = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_";
                SqlCommand cmd = new SqlCommand(select, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    existingAttributes.Add(dr["ATTRIBUTE_"].ToString());
                }
                connection.Close();
            }

            
            foreach(AttributeStore attribute in attributes)
            {
                if(!existingAttributes.Contains(attribute.AttributeField))
                {
                    String view = ImportOMS.MakeRawDataView(attribute.AttributeField);
                    String insertView = view.Replace("@attribute", "'" + attribute.AttributeField + "'");
                    
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        String type = "STRING";
                        String format = "";
                        float minimum = 0;
                        float maximum = 100;
                        String defaultvalue = "";
                        if (attribute.FieldType.ToUpper() == "TIME" || attribute.FieldType.ToUpper() == "DATE" || attribute.FieldType.ToUpper() == "DATETIME")
                        {
                            type = "DATETIME";
                        }
                        else if (attribute.FieldType.ToUpper() == "NUMBER" || attribute.FieldType.ToUpper() == "INTEGER" || attribute.FieldType.ToUpper() == "CURRENCY" ||attribute.FieldType.ToUpper() == "QUANTITY")
                        {
                            type = "NUMBER";
                            format = "g";
                            minimum = attribute.Minimum;
                            maximum = attribute.Maximum;
                            if (attribute.InitialValue != null)
                            {
                                defaultvalue = attribute.InitialValue;
                            }
                        }

                        connection.Open();
                        string insert = "INSERT INTO " + DB.TablePrefix + "ATTRIBUTES_ " +
                                         "(ATTRIBUTE_,NATIVE_, PROVIDER,SERVER,DATASOURCE,USERID,PASSWORD_,SQLVIEW,DEFAULT_VALUE,ASCENDING,TYPE_,FORMAT,MAXIMUM,MINIMUM_,INTEGRATED_SECURITY,NETWORK_DEFINITION_NAME) " +
                                          "VALUES(@attribute,@native,@provider,@server,@dataSource,@userId,@password,@sqlView,@defaultvalue,@ascending,@type,@format,@maximum,@minimum,@integratedSecurity,@networkConditionName)";
                        SqlCommand cmd = new SqlCommand(insert, connection);
                        cmd.Parameters.Add(new SqlParameter("attribute", attribute.AttributeField));
                        cmd.Parameters.Add(new SqlParameter("native", false));
                        cmd.Parameters.Add(new SqlParameter("provider", "MSSQL"));
                        cmd.Parameters.Add(new SqlParameter("server", server));
                        cmd.Parameters.Add(new SqlParameter("dataSource", dataSource));
                        cmd.Parameters.Add(new SqlParameter("userId", ""));
                        cmd.Parameters.Add(new SqlParameter("password", ""));
                        cmd.Parameters.Add(new SqlParameter("sqlView", insertView));
                        cmd.Parameters.Add(new SqlParameter("defaultvalue", defaultvalue));
                        cmd.Parameters.Add(new SqlParameter("ascending", true));
                        cmd.Parameters.Add(new SqlParameter("type", type));
                        cmd.Parameters.Add(new SqlParameter("format", format));
                        cmd.Parameters.Add(new SqlParameter("maximum", maximum));
                        cmd.Parameters.Add(new SqlParameter("minimum", minimum));
                        cmd.Parameters.Add(new SqlParameter("integratedSecurity", integratedSecurity));
                        cmd.Parameters.Add(new SqlParameter("networkConditionName", "Default"));
                        cmd.ExecuteNonQuery();
                    }


                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        connection.Open();
                        string createView = "CREATE VIEW " + attribute.AttributeField + " AS (" + insertView + ")";
                        SqlCommand cmd = new SqlCommand(createView, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
