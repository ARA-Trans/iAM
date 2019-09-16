using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.DTOs;
using System.Data.SqlClient;
using Utility.ExceptionHandling;

namespace DataAccessLayer
{
    public static class DecisionEngine
    {
        /// <summary>
        /// Retrieves a list of all Attributes available in treatments.  Extend this to include Cost and Consequence criteria (and consquence attributes)
        /// </summary>
        /// <param name="simulationID"></param>
        /// <returns></returns>
        public static List<AttributeStore> GetTreatmentAttributes(string assetName, string simulationID)
        {
            List<AttributeStore> attributes = new List<AttributeStore>();

            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))//DecisionEngine connection string
            {
                try
                {
                    connection.Open();

                    string query = "SELECT CRITERIA FROM " + DB.TablePrefix + "FEASIBILITY F INNER JOIN " + DB.TablePrefix + "TREATMENTS T ON F.TREATMENTID=T.TREATMENTID WHERE T.SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string criteria = null;
                        if (dr["CRITERIA"] != DBNull.Value)
                        {    
                            criteria = dr["CRITERIA"].ToString();
                            List<AttributeStore> criteriaAttributes = OMS.ParseAttributes(assetName,criteria); 
                            foreach(AttributeStore attribute in criteriaAttributes)
                            {
                                if (!attributes.Contains(attribute))
                                {
                                    attributes.Add(attribute);
                                }
                            }
                        }
                    }

                    dr.Close();
                    //Get COSTS
                    query = "SELECT COST_ FROM " + DB.TablePrefix + "COSTS C INNER JOIN " + DB.TablePrefix + "TREATMENTS T on C.TREATMENTID = T.TREATMENTID WHERE T.SIMULATIONID =@simulationID";
                    cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    dr = cmd.ExecuteReader();
                    while(dr.Read())
                    {
                        if(dr["COST_"] != DBNull.Value)
                        {
                           string cost = dr["COST_"].ToString();
                           List<AttributeStore> costAttributes = OMS.ParseAttributes(assetName,cost);
                           foreach (AttributeStore attribute in costAttributes)
                           {
                               if (!attributes.Contains(attribute))
                               {
                                   attributes.Add(attribute);
                               }
                           } 
                        }
                    }
               }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
            }
            return attributes;
        }


        /// <summary>
        /// Returns a list of all attributes used in Priority Critera.
        /// </summary>
        /// <param name="assetName">Asset type for which simulation is performed</param>
        /// <param name="simulationID">SimulationID of anlaysis</param>
        /// <returns></returns>
        public static List<AttributeStore> GetPriorityAttributes(string assetName, string simulationID)
        {
            List<AttributeStore> attributes = new List<AttributeStore>();
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))//DecisionEngine connection string
            {
                try
                {
                    connection.Open();

                    string query = "SELECT CRITERIA FROM " + DB.TablePrefix + "PRIORITY WHERE SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string criteria = null;
                        if (dr["CRITERIA"] != DBNull.Value)
                        {
                            criteria = dr["CRITERIA"].ToString();
                            List<AttributeStore> criteriaAttributes = OMS.ParseAttributes(assetName, criteria);
                            foreach (AttributeStore attribute in criteriaAttributes)
                            {
                                if (!attributes.Contains(attribute))
                                {
                                    attributes.Add(attribute);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
            }
            return attributes;
        }


        /// <summary>
        /// Retrieves a list of all Attributes available in Performance.  Extend if Equation based performance.
        /// </summary>
        /// <param name="simulationID"></param>
        /// <returns></returns>
        public static List<AttributeStore> GetPerformanceAttributes(string assetName, string simulationID)
        {
            List<AttributeStore> attributes = new List<AttributeStore>();

            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))//DecisionEngine connection string
            {
                try
                {
                    connection.Open();

                    string query = "SELECT CRITERIA FROM " + DB.TablePrefix + "PERFORMANCE WHERE SIMULATIONID='" + simulationID + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string criteria = null;
                        if (dr["CRITERIA"] != DBNull.Value)
                        {
                            criteria = dr["CRITERIA"].ToString();
                            List<AttributeStore> criteriaAttributes = OMS.ParseAttributes(assetName, criteria);
                            foreach (AttributeStore attribute in criteriaAttributes)
                            {
                                if (!attributes.Contains(attribute))
                                {
                                    attributes.Add(attribute);
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
            }


            AttributeStore ageAttribute = new AttributeStore();
            ageAttribute.AttributeField = "AGE";
            ageAttribute.DisplayName = "AGE";
            ageAttribute.OmsObjectUserIDHierarchy = "AGE";
            ageAttribute.FieldType = "NUMBER";
            ageAttribute.Minimum = 0;
            ageAttribute.Maximum = 100;
            ageAttribute.InitialValue = "0";
            attributes.Add(ageAttribute);


            return attributes;
        }


    }
}
