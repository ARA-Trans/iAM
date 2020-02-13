using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Properties;
using DatabaseManager;
using log4net;
using Simulation;

namespace BridgeCare.DataAccessLayer
{
    public class SimulationDAL : ISimulation
    {
        private static readonly SimulationQueue SimulationQueue = new SimulationQueue(1);

        /// <summary>
        /// Fetches all simulations
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>SimulationModel list</returns>
        public List<SimulationModel> GetSimulations(BridgeCareContext db)
        {
            return db.Simulations.Include(s => s.NETWORK).ToList().Select(s => new SimulationModel(s)).ToList();
        }

        /// <summary>
        /// Updates a simulation; Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="db">BridgeCareContext</param>
        public void UpdateSimulation(SimulationModel model, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == model.simulationId))
            {
                var log = LogManager.GetLogger(typeof(SimulationDAL));
                log.Error($"No scenario found with id {model.simulationId}");
                throw new RowNotInTableException($"No scenario found with id {model.simulationId}");
            }

            var simulation = db.Simulations.Single(b => b.SIMULATIONID == model.simulationId);
            simulation.SIMULATION = model.simulationName;
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

            using (var connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                connection.Open();
                var dropQuery = $"IF OBJECT_ID ( 'SIMULATION_{simulation.NETWORKID}_{id}_0' , 'U' )  IS NOT NULL DROP TABLE SIMULATION_{simulation.NETWORKID}_{id} " +
                                $"IF OBJECT_ID ( 'REPORT_{simulation.NETWORKID}_{id}' , 'U' )  IS NOT NULL DROP TABLE REPORT_{simulation.NETWORKID}_{id} " +
                                $"IF OBJECT_ID ( 'BENEFITCOST_{simulation.NETWORKID}_{id}' , 'U' )  IS NOT NULL DROP TABLE BENEFITCOST_{simulation.NETWORKID}_{id} " +
                                $"IF OBJECT_ID ( 'TARGET_{simulation.NETWORKID}_{id}' , 'U' )  IS NOT NULL DROP TABLE TARGET_{simulation.NETWORKID}_{id} ";
                using (var command = new SqlCommand(dropQuery, connection) { CommandType = CommandType.Text })
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public SimulationModel CreateSimulation(CreateSimulationDataModel model, BridgeCareContext db)
        {
            var simulation = new SimulationEntity(model);
            db.Simulations.Add(simulation);

            db.SaveChanges();

            simulation = db.Simulations.Include(s => s.NETWORK)
                .Single(s => s.SIMULATIONID == simulation.SIMULATIONID);

            return new SimulationModel(simulation);
        }

        /// <summary>
        /// Creates/starts a rollup/simulation
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>string Task</returns>
        public Task<string> RunSimulation(SimulationModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString;
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");

#if DEBUG
                var mongoConnection = Settings.Default.MongoDBDevConnectionString;
#else
                var mongoConnection = Settings.Default.MongoDBProdConnectionString;
#endif

                var simulationParameters = new SimulationParameters(
                    model.simulationName,
                    model.networkName,
                    model.simulationId,
                    model.networkId,
                    mongoConnection,
                    true);

                var simulationTask = SimulationQueue.Enqueue(simulationParameters);

                return Task.FromResult("Simulation running...");
            }
            catch (Exception ex)
            {
                DBMgr.CloseConnection();
                return Task.FromResult($"Simulation run failed::{ex.Message}");
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
