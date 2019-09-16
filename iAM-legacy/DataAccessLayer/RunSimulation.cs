using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using DataAccessLayer.DTOs;
using System.Data;
using Simulation;


namespace DataAccessLayer
{
    public static class RunSimulation
    {
        static Dictionary<string, Thread> _simulations = new Dictionary<string, Thread>();

        public static Dictionary<string, Thread> Simulations
        {
            get { return _simulations; }
            set { _simulations = value; }
        }


        public static void Start(string simulationID)
        {
            //Always prepare analysis.  Take snap shot of data.
            PrepareAnalysis.Simulation(simulationID);
            bool isStartNewSimulation = true;
            if (RunSimulation.Simulations.ContainsKey(simulationID))
            {
                Thread runningThread = RunSimulation.Simulations[simulationID];
                if (runningThread.IsAlive)
                {
                    isStartNewSimulation = false;
                }
                else
                {
                    RunSimulation.Simulations.Remove(simulationID);
                }
            }
            if (isStartNewSimulation)
            {
                Simulation.SimulationMessaging.ClearProgressList(simulationID);
                Simulation.Simulation simulation = new Simulation.Simulation("", "", simulationID, "1", DB.ConnectionString);
                Thread simulationThread = new Thread(new ParameterizedThreadStart(simulation.CompileSimulation));
                RunSimulation.Simulations.Add(simulationID, simulationThread);
                simulationThread.Start(false);
            }
        }


        public static string UpdateResultActivity(string sessionID,int rowIndex, string action, string treatment, int fromYear, string value)
        {

            SessionStore session = OMS.Sessions.Find(delegate(SessionStore s) { return s.SessionID == sessionID; });
            string sectionID = session.OIDs[rowIndex];
            if(session == null) return "Error: Expired sessionID=" + sessionID;
            string encoded = null;

            Simulation.SimulationMessaging.ClearProgressList(session.SimulationID);
            Simulation.Simulation simulation = new Simulation.Simulation(session.SimulationID, sectionID, action, treatment, fromYear, value,DB.ConnectionString);
            simulation.UpdateSimulation();
            encoded = SelectScenario.GetActivityResults(session.SessionID, sectionID);
            return encoded;
        }

        public static List<string> GetSimulationStatus(string simulationID)
        {
            List<string> messages = new List<string>();
            if (RunSimulation.Simulations.ContainsKey(simulationID))
            {
                Thread thread = RunSimulation.Simulations[simulationID];
                List<SimulationMessage> listSimulation = Simulation.SimulationMessaging.GetProgressList();
                lock (listSimulation)
                {
                    List<SimulationMessage> listOMS = listSimulation.FindAll(delegate(SimulationMessage sm) { return sm.SimulationID == simulationID; });
                    foreach (SimulationMessage message in listOMS)
                    {
                        messages.Add(message.Message);
                    }
                    Simulation.SimulationMessaging.ClearProgressList(simulationID);
                }
                if (!thread.IsAlive)
                {
                    messages.Add("ScenarioID=" + simulationID + " is complete and thread exited.");
                    RunSimulation.Simulations.Remove(simulationID);
                }
            }
            return messages;
        }


        public static int GetSimulationPercentComplete(string simulationID)
        {
            int percent = 0;
            if (RunSimulation.Simulations.ContainsKey(simulationID))
            {
                List<SimulationMessage> listSimulation = Simulation.SimulationMessaging.GetProgressList();
                lock (listSimulation)
                {
                    SimulationMessage message = listSimulation.FindLast(delegate(SimulationMessage sm) { return sm.SimulationID == simulationID; });
                    percent = message.Percent;
                }
            }
            return percent;
        }



        public static string CancelSimulation(string simulationID)
        {
            string message = "";
            if (RunSimulation.Simulations.ContainsKey(simulationID))
            {
                Thread thread = RunSimulation.Simulations[simulationID];
                if (!thread.IsAlive)
                {
                    message = "Thread for simulationID=" + simulationID + "has already stopped.";
                }
                else
                {
                    thread.Abort();
                    message = "Thread for simulationID=" + simulationID + "aborted.";
                }
            }
            else
            {
                message = "Thread for simulationID=" + simulationID + "was not found.";
            }
            return message;
        }

        public static string Commit(string sessionID, List<int> rowNumbers)
        {
            SessionStore session = OMS.Sessions.Find(delegate(SessionStore s) { return s.SessionID == sessionID; });
            if (session == null) return "INVALID_SESSION";

            List<CommittedStore> comitted = SelectScenario.GetCommittedForCopy(session.SimulationID);
            List<ActivityResultStore> activities = SelectScenario.GetRecommendedActivities(session.SimulationID);
            
            try
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "COMMITTED_ (SIMULATIONID,SECTIONID,YEARS,TREATMENTNAME,OMS_IS_NOT_ALLOWED) VALUES(@simulationID,@sectionID,@years,@treatmentName,0)";
                    SqlCommand cmdCommit = new SqlCommand(insert, connection);
                    cmdCommit.Parameters.Add(new SqlParameter("simulationID", SqlDbType.Int, 0));
                    cmdCommit.Parameters.Add(new SqlParameter("sectionID",SqlDbType.Int,0));
                    cmdCommit.Parameters.Add(new SqlParameter("years",SqlDbType.Int,0));
                    cmdCommit.Parameters.Add(new SqlParameter("treatmentName",SqlDbType.VarChar,50));
                    cmdCommit.Prepare();

                    //Update the report table to show that they are committed.
                    string update = "UPDATE " + DB.TablePrefix + "REPORT_1_" + session.SimulationID.ToString() + " SET RESULT_TYPE=1 WHERE SECTIONID=@sectionID AND TREATMENT<>'No Treatment' AND RESULT_TYPE=0";
                    SqlCommand cmdUpdate = new SqlCommand(update, connection);
                    cmdUpdate.Parameters.Add(new SqlParameter("sectionID", SqlDbType.Int, 0));
                    cmdUpdate.Prepare();

                    foreach (int rowNumber in rowNumbers)
                    {
                        string sectionID = session.OIDs[rowNumber];
                        List<ActivityResultStore> rowActivities = activities.FindAll(delegate(ActivityResultStore a) { return a.OID == sectionID; });
                        if(rowActivities != null)
                        {
                            foreach(ActivityResultStore activity in rowActivities)
                            {
                                
                                //Add each to the committed table.
                                cmdCommit.Parameters[0].Value = Convert.ToInt32(session.SimulationID);
                                cmdCommit.Parameters[1].Value = Convert.ToInt32(activity.OID);
                                cmdCommit.Parameters[2].Value = Convert.ToInt32(activity.Year);
                                cmdCommit.Parameters[3].Value = activity.ActivityName;
                                cmdCommit.ExecuteNonQuery();

                                //Update the report table to show that they are committed.
                                cmdUpdate.Parameters[0].Value = sectionID;
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
            }
            return "SUCCESS";
        }

        public static string Uncommit(string sessionID, List<int> rowNumbers)
        {
            SessionStore session = OMS.Sessions.Find(delegate(SessionStore s) { return s.SessionID == sessionID; });
            if (session == null) return "INVALID_SESSION";

            try
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string delete = "DELETE " + DB.TablePrefix + "COMMITTED_ WHERE SIMULATIONID=@simulationID AND SECTIONID=@sectionID";
                    SqlCommand cmdCommit = new SqlCommand(delete, connection);
                    cmdCommit.Parameters.Add(new SqlParameter("simulationID", SqlDbType.Int, 0));
                    cmdCommit.Parameters.Add(new SqlParameter("sectionID", SqlDbType.Int, 0));
                    cmdCommit.Prepare();

                    //Update the report table to show that they are committed.
                    string update = "UPDATE " + DB.TablePrefix + "REPORT_1_" + session.SimulationID.ToString() + " SET RESULT_TYPE=0 WHERE SECTIONID=@sectionID AND RESULT_TYPE=1";
                    SqlCommand cmdUpdate = new SqlCommand(update, connection);
                    cmdUpdate.Parameters.Add(new SqlParameter("sectionID", SqlDbType.Int, 0));
                    cmdUpdate.Prepare();

                    foreach (int rowNumber in rowNumbers)
                    {
                        string sectionID = session.OIDs[rowNumber];

                                //Add each to the committed table.
                        cmdCommit.Parameters[0].Value = Convert.ToInt32(session.SimulationID);
                        cmdCommit.Parameters[1].Value = Convert.ToInt32(sectionID);
                        cmdCommit.ExecuteNonQuery();

                        //Update the report table to show that they are committed.
                        cmdUpdate.Parameters[0].Value = sectionID;
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
            }
            return "SUCCESS";
        }
    }
}
