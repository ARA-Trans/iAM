using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public static class DeleteScenario
    {
        /// <summary>
        /// Delete simulation
        /// </summary>
        /// <param name="simulationID">SimulationID of simulation to delete</param>
        public static void DeleteSimulation(string simulationID)
        {
            if(DB.CheckIfTableExists(DB.TablePrefix + "SIMULATION_1_" + simulationID) == 1) DB.DropTable(DB.TablePrefix + "SIMULATION_1_" + simulationID);
            if (DB.CheckIfTableExists(DB.TablePrefix + "BENEFITCOST_1_" + simulationID) == 1) DB.DropTable(DB.TablePrefix + "BENEFITCOST_1_" + simulationID);
            if (DB.CheckIfTableExists(DB.TablePrefix + "REPORT_1_" + simulationID) == 1) DB.DropTable(DB.TablePrefix + "REPORT_1_" + simulationID);
            if (DB.CheckIfTableExists(DB.TablePrefix + "TARGET_1_" + simulationID) == 1) DB.DropTable(DB.TablePrefix + "TARGET_1_" + simulationID);

            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "SIMULATIONS WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }
        /// <summary>
        /// Delete existing YearlyInvestment table (this table refilled on each run).
        /// </summary>
        /// <param name="simulationID"></param>
        public static void DeleteYearlyInvestment(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "YEARLYINVESTMENT WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }



        /// <summary>
        /// Delete all targets associated with this Simualtion.  Will be rebuilt on PrepareSimulation
        /// </summary>
        /// <param name="simulationID">SimulationID of simulation to remove targets for.</param>
        public static void DeleteTargets(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "TARGETS WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }


        /// <summary>
        /// Delete all Deficients associated with this Simualtion.  Will be rebuilt on PrepareSimulation
        /// </summary>
        /// <param name="simulationID">SimulationID of simulation to remove targets for.</param>
        public static void DeleteDeficients(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "DEFICIENTS WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        /// <summary>
        /// Delete a Treatment (and cascade) for a given SimulationID and ActivityOID
        /// </summary>
        /// <param name="simulationID"></param>
        /// <param name="activityOID"></param>
        public static void DeleteTreatment(string simulationID, string activityOID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "TREATMENTS WHERE SIMULATIONID=@simulationID AND OMS_OID=@activityOID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.Parameters.Add(new SqlParameter("activityOID", activityOID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }


        /// <summary>
        /// Delete OCI Weights (ConditionCategory Weights) with the input simulationID.
        /// </summary>
        /// <param name="simulationID"></param>
        /// <param name="activityOID"></param>
        public static void DeleteConditionCategoryWeight(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "OCI_WEIGHTS WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        /// <summary>
        /// Delete previous Performance curves. Reload for current simulation.
        /// </summary>
        /// <param name="simulationID">SimulationID to delete</param>
        public static void DeletePriority(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "PRIORITY WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }





        /// <summary>
        /// Delete previous Performance curves. Reload for current simulation.
        /// </summary>
        /// <param name="simulationID">SimulationID to delete</param>
        public static void DeletePerformance(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "PERFORMANCE WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        /// <summary>
        /// Removed Activities from the committed table where the IS_OMS_REPEAT is true.
        /// </summary>
        /// <param name="simulationID">SimulationID to delete</param>
        public static void DeleteRepeatedActivities(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "COMMITTED_ WHERE OMS_IS_REPEAT='1' AND SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        /// <summary>
        /// Removed attributes from OMS_ATTRIBUTES for given simulation..
        /// </summary>
        /// <param name="simulationID">SimulationID to delete</param>
        public static void DeleteAttributes(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "OMS_ATTRIBUTES WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }


        /// <summary>
        /// Removed attributes from OMS_ATTRIBUTES for given simulation..
        /// </summary>
        /// <param name="simulationID">SimulationID to delete</param>
        public static void DeleteAssets(string simulationID)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "OMS_ASSETS WHERE SIMULATIONID=@simulationID", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }


        public static void DeleteImpact(string activityID, string attribute)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "CONSEQUENCES WHERE TREATMENTID=@treatmentID AND ATTRIBUTE_=@attribute", connection);
                    cmd.Parameters.Add(new SqlParameter("treatmentID", activityID));
                    cmd.Parameters.Add(new SqlParameter("attribute", attribute));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }


        public static void DeleteYearlyInvestment(string simulationID, int year)
        {
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "YEARLYINVESTMENT WHERE SIMULATIONID=@simulationID AND YEAR_=@year", connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.Parameters.Add(new SqlParameter("year", year));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        public static bool DeleteTarget(string targetID)
        {
            bool isSuccess = true;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + DB.TablePrefix + "TARGETS WHERE ID_=@targetID", connection);
                    cmd.Parameters.Add(new SqlParameter("targetID", targetID));
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                    isSuccess = false;
                }
            }
            return isSuccess;
        }

    }
}
