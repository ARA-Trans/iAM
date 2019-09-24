using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BridgeCare.Properties;
using DatabaseManager;

namespace BridgeCare.DataAccessLayer
{
    public class SimulationDAL : ISimulation
    {
        /// <summary>
        /// Fetches all simulations
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>SimulationModel list</returns>
        public List<SimulationModel> GetSimulations(BridgeCareContext db)
        {
            return db.Simulations.Select(s => new SimulationModel(s)).ToList();
        }

        /// <summary>
        /// Updates a simulation; Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="db">BridgeCareContext</param>
        public void UpdateSimulation(SimulationModel model, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == model.SimulationId))
                throw new RowNotInTableException($"No scenario found with id {model.SimulationId}");

            var simulation = db.Simulations.Single(b => b.SIMULATIONID == model.SimulationId);
            simulation.SIMULATION = model.SimulationName;
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes a simulation and all records with a foreign key relation into the simulations table
        /// Simply returns if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        public void DeleteSimulation(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id)) return;

            var simulation = db.Simulations.Single(b => b.SIMULATIONID == id);
            db.Entry(simulation).State = EntityState.Deleted;
            db.SaveChanges();

            var connection = new SqlConnection(db.Database.Connection.ConnectionString);
            connection.Open();

            var dropQuery = string.Format(
                "DROP TABLE IF EXISTS SIMULATION_{0}_{1}, REPORT_{0}_{1}, BENEFITCOST_{0}_{1}, TARGET_{0}_{1}",
                simulation.NETWORKID, id
            );

            var cmd = new SqlCommand(dropQuery, connection) {CommandType = CommandType.Text};
            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public SimulationModel CreateSimulation(CreateSimulationDataModel model, BridgeCareContext db)
        {
            var simulation = new SimulationEntity(model);
            db.Simulations.Add(simulation);
            // db.SaveChanges();

            /*simulation.INVESTMENTS = new InvestmentsEntity()
            {
                SIMULATIONID = simulation.SIMULATIONID,
                FIRSTYEAR = DateTime.Now.Year,
                NUMBERYEARS = 1,
                INFLATIONRATE = 0,
                DISCOUNTRATE = 0,
                BUDGETORDER = "Rehabilitation,Maintenance,Construction"
            };*/

            // db.SaveChanges();

            /*simulation.CriteriaDrivenBudgets= new List<CriteriaDrivenBudgetsEntity>
            {
                new CriteriaDrivenBudgetsEntity
                {
                    BUDGET_NAME = "Maintenance",
                    CRITERIA = "",
                    SIMULATIONID = simulation.SIMULATIONID
                },
                new CriteriaDrivenBudgetsEntity
                {
                    BUDGET_NAME = "Rehabilitation",
                    CRITERIA = "",
                    SIMULATIONID = simulation.SIMULATIONID
                },
                new CriteriaDrivenBudgetsEntity
                {
                    BUDGET_NAME = "Construction",
                    CRITERIA = "",
                    SIMULATIONID = simulation.SIMULATIONID
                }
            };*/

            db.SaveChanges();

            var defaultBudgets = new List<string> { "Rehabilitation", "Maintenance", "Construction" };
            PriorityDAL.SavePriorityFundInvestmentData(simulation.SIMULATIONID, defaultBudgets, db);

            return new SimulationModel(simulation);
        }

        /// <summary>
        /// Creates/starts a rollup/simulation
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>string Task</returns>
        public Task<string> RunSimulation(SimulationModel model)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString;
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");
#if DEBUG
                var mongoConnection = Settings.Default.MongoDBDevConnectionString;
#else
                var mongoConnection = Settings.Default.MongoDBProdConnectionString;
#endif
                var rollupSegmentation = new RollupSegmentation.RollupSegmentation(model.SimulationName, model.NetworkName,
                    model.SimulationId.ToString(), model.NetworkId.ToString(), true, mongoConnection)
                {
                    strNetwork = model.NetworkName
                };

                Thread rollupAndSimulation = new Thread(rollupSegmentation.DoRollup);

                rollupAndSimulation.Start();

                return Task.FromResult("Simulation running...");
            }
            catch (Exception ex)
            {
                DBMgr.CloseConnection();
                return Task.FromResult($"Simulation failed::{ex.Message}");
            }
        }

        /// <summary>
        /// Updates the last run date of a simulation
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        public void SetSimulationLastRunDate(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");

            var simulation = db.Simulations.Single(s => s.SIMULATIONID == id);

            simulation.DATE_LAST_RUN = DateTime.Now;

            db.SaveChanges();
        }
    }
}