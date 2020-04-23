using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BridgeCare.Properties;
using DatabaseManager;
using log4net;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Simulation;

namespace BridgeCare.DataAccessLayer
{
    public class SimulationDAL : ISimulation
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(SimulationDAL));
        private static readonly SimulationQueue SimulationQueue = SimulationQueue.MainSimulationQueue;
        /// <summary>
        /// Fetches all simulations
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>SimulationModel list</returns>
        public List<SimulationModel> GetSimulations(BridgeCareContext db)
        {
            return db.Simulations.Include(s => s.NETWORK).Include(s => s.USERS).ToList().Select(s => new SimulationModel(s)).ToList();
        }

        /// <summary>
        /// Fetches all simulations to which the user has any level of access
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="userInformation">UserInformationModel</param>
        /// <returns>SimulationModel list</returns>
        public List<SimulationModel> GetPermittedSimulations(BridgeCareContext db, string username)
        {
            return db.Simulations.Include(s => s.NETWORK).Include(s => s.USERS).ToList()
                .Where(s => s.UserCanRead(username))
                .Select(s => new SimulationModel(s)).ToList();
        }

        /// <summary>
        /// Updates a simulation; Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="db">BridgeCareContext</param>
        private void UpdateSimulation(SimulationModel model, BridgeCareContext db)
        {
            var simulation = db.Simulations.Single(b => b.SIMULATIONID == model.simulationId);
            simulation.SIMULATION = model.simulationName;
            db.SaveChanges();
        }

        /// <summary>
        /// Updates a simulation belonging to the user; Throws a RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="db">BridgeCareContext</param>
        public void UpdatePermittedSimulation(SimulationModel model, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == model.simulationId))
                throw new RowNotInTableException($"No scenario found with id {model.simulationId}");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == model.simulationId).UserCanModify(username))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario.");
            UpdateSimulation(model, db);
        }

        /// <summary>
        /// Updates a simulation regardless of ownership; Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="db">BridgeCareContext</param>
        public void UpdateAnySimulation(SimulationModel model, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == model.simulationId))
                throw new RowNotInTableException($"No scenario found with id {model.simulationId}");
            UpdateSimulation(model, db);
        }

        /// <summary>
        /// Deletes a simulation and all records with a foreign key relation into the simulations table
        /// Simply returns if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        private void DeleteSimulation(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations.Single(b => b.SIMULATIONID == id);
            db.Entry(simulation).State = System.Data.Entity.EntityState.Deleted;
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

        /// <summary>
        /// Deletes a simulation belonging to the user and all records with a foreign key relation into the simulations table
        /// Throws RowNotInTableException if the user cannot access a scenario with the given id
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        public void DeletePermittedSimulation(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == id).UserCanModify(username))
                throw new UnauthorizedAccessException("You are not authorized to delete this scenario.");
            DeleteSimulation(id, db);
        }

        /// <summary>
        /// Deletes a simulation regardless of ownership
        /// Simply returns if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        public void DeleteAnySimulation(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id)) return;
            DeleteSimulation(id, db);
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

        public SimulationModel CloneSimulation(int simulationId, BridgeCareContext db, string username)
        {
            var simulation = db.Simulations.AsNoTracking()
                .Include(s => s.INVESTMENTS)
                .Include(s => s.PERFORMANCES)
                .Include(s => s.TREATMENTS.Select(t => t.CONSEQUENCES))
                .Include(s => s.TREATMENTS.Select(t => t.COSTS))
                .Include(s => s.TREATMENTS.Select(t => t.FEASIBILITIES))
                .Include(s => s.TREATMENTS.Select(t => t.SCHEDULEDS))
                .Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                .Include(s => s.TARGETS)
                .Include(s => s.DEFICIENTS)
                .Include(s => s.REMAINING_LIFE_LIMITS)
                .Include(s => s.SPLIT_TREATMENTS.Select(st => st.SPLIT_TREATMENT_LIMITS))
                .Include(s => s.COMMITTEDPROJECTS.Select(c => c.COMMIT_CONSEQUENCES))
                .Include(s => s.YEARLYINVESTMENTS)
                .Include(s => s.PRIORITIZEDNEEDS)
                .Include(s => s.TARGET_DEFICIENTS)
                .Include(s => s.CriteriaDrivenBudgets)
                .First(entity => entity.SIMULATIONID == simulationId);
            simulation.OWNER = username;
            db.Simulations.Add(simulation); // Primary key will automatically be changed
            db.SaveChanges();
            return new SimulationModel(simulation);
        }

        /// <summary>
        /// Creates/starts a rollup/simulation
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>string Task</returns>
        public Task<string> RunSimulation(SimulationModel model, BridgeCareContext db)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            try
            {
                if (!db.Simulations.Any(s => s.SIMULATIONID == model.simulationId))
                    throw new RowNotInTableException($"No scenario was found with id {model.simulationId}");

                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString;
                DBMgr.NativeConnectionParameters = new ConnectionParameters(connectionString, false, "MSSQL");

#if DEBUG
                var mongoConnection = Settings.Default.MongoDBDevConnectionString;
#else
                var mongoConnection = Settings.Default.MongoDBProdConnectionString;
#endif

                var simulation = db.Simulations
                    .Include(s => s.COMMITTEDPROJECTS)
                    .Single(s => s.SIMULATIONID == model.simulationId);

                if (simulation.COMMITTEDPROJECTS.Any())
                {
                    var earliestCommittedProjectStartYear = simulation.COMMITTEDPROJECTS
                        .OrderBy(cp => cp.YEARS).First().YEARS;
                    if (earliestCommittedProjectStartYear < simulation.COMMITTED_START)
                    {
                        var mongoClient = new MongoClient(mongoConnection);
                        var mongoDB = mongoClient.GetDatabase("BridgeCare");
                        var simulations = mongoDB.GetCollection<SimulationModel>("scenarios");
                        var updateStatus = Builders<SimulationModel>.Update.Set("status", "Error: Projects committed before analysis start");
                        simulations.UpdateOne(s => s.simulationId == model.simulationId, updateStatus);
                        throw new ConstraintException("Analysis error: Projects committed before analysis start");
                    }
                }

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

        public Task<string> RunPermittedSimulation(SimulationModel model, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == model.simulationId))
                throw new RowNotInTableException($"No scenario was found with id {model.simulationId}");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == model.simulationId).UserCanModify(username))
                throw new UnauthorizedAccessException("You are not authorized to run this scenario.");
            return RunSimulation(model, db);
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

            var lastRun = DateTime.Now;

            simulation.DATE_LAST_RUN = lastRun;

            db.SaveChanges();

#if DEBUG
            var mongoConnection = Settings.Default.MongoDBDevConnectionString;
#else
            var mongoConnection = Settings.Default.MongoDBProdConnectionString;
#endif
            var mongoClient = new MongoClient(mongoConnection);
            var mongoDB = mongoClient.GetDatabase("BridgeCare");
            var simulations = mongoDB.GetCollection<SimulationModel>("scenarios");
            var updateLastRunDate = Builders<SimulationModel>.Update.Set("lastRun", lastRun);
            simulations.UpdateOne(s => s.simulationId == id, updateLastRunDate);
        }

        public void SetPermittedSimulationUsers(int simulationId, List<SimulationUserModel> simulationUsers, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                throw new RowNotInTableException($"No scenario found with id {simulationId}.");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == simulationId).UserCanModify(username))
                throw new UnauthorizedAccessException($"User {username} cannot modify scenario {simulationId}.");

            var simulation = db.Simulations.Include(s => s.USERS).Single(s => s.SIMULATIONID == simulationId);

            foreach (var user in simulation.USERS.ToArray())
            {
                SimulationUserEntity.DeleteEntry(user, db);
            }

            simulation.USERS = simulationUsers.Select(user => new SimulationUserEntity(simulationId, user)).ToList();

            db.SaveChanges();
        }

        public void SetAnySimulationUsers(int simulationId, List<SimulationUserModel> simulationUsers, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                throw new RowNotInTableException($"No scenario found with id {simulationId}.");

            var simulation = db.Simulations.Include(s => s.USERS).Single(s => s.SIMULATIONID == simulationId);

            foreach (var user in simulation.USERS.ToArray())
            {
                SimulationUserEntity.DeleteEntry(user, db);
            }

            simulation.USERS = simulationUsers.Select(user => new SimulationUserEntity(simulationId, user)).ToList();

            db.SaveChanges();
        }
    }
}
