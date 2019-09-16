using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseManager;
using System.Data;

namespace SimulationDataAccess
{
    public static class SimulationData
    {
        /// <summary>
        /// Retrieves the default area equation
        /// </summary>
        /// <returns></returns>
        public static string GetAreaEquation()
        {
            //Compile AREA equation
            string strQuery = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME='AREA_CALCULATION'";
            DataSet ds = DBMgr.ExecuteQuery(strQuery);
            string area = ds.Tables[0].Rows[0]["OPTION_VALUE"].ToString();
            return area;
        }


        /// <summary>
        /// Get network specific area.
        /// </summary>
        /// <param name="networkID">NetworkID of network</param>
        /// <returns>The area specific network equation</returns>
        public static string GetNetworkSpecificArea(string networkID)
        {
            string areaEquation = "";
            string query = "SELECT NETWORK_AREA FROM NETWORKS WHERE NETWORKID='" + networkID + "'";
            DataSet ds = DBMgr.ExecuteQuery(query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["NETWORK_AREA"] != DBNull.Value)
                {
                    areaEquation = row["NETWORK_AREA"].ToString();
                }
            }
            return areaEquation;
        }




        /// <summary>
        /// Get network specific area.
        /// </summary>
        /// <param name="networkID">NetworkID of network</param>
        /// <returns>The area specific network equation</returns>
        public static string GetOMSAreaEquation(string simulationID, string prefix)
        {
            string areaEquation = "";
            string query = "SELECT SIMULATION_AREA FROM " + prefix + "SIMULATIONS WHERE SIMULATIONID='" + simulationID + "'";
            DataSet ds = DBMgr.ExecuteQuery(query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["SIMULATION_AREA"] != DBNull.Value)
                {
                    areaEquation = row["SIMULATION_AREA"].ToString();
                }
            }
            return areaEquation;
        }
    }
}
