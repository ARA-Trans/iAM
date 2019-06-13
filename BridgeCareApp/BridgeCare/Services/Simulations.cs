using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Services
{
    public class Simulations : ISimulation
    {
        private readonly BridgeCareContext db;

        public Simulations(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IQueryable<SimulationModel> filterSimulation;

        public IQueryable<SimulationModel> GetAllSimulations()
        {
            var filteredColumns = from contextTable in db.SIMULATIONS
                                  select new SimulationModel
                                  {
                                      SimulationId = contextTable.SIMULATIONID,
                                      SimulationName = contextTable.SIMULATION1,
                                      NetworkId = contextTable.NETWORKID.Value,
                                      Created = contextTable.DATE_CREATED,
                                      LastRun = contextTable.DATE_LAST_RUN ?? DateTime.Now,
                                      NetworkName = contextTable.NETWORK.NETWORK_NAME
                                  };
            return filteredColumns;
        }

        public IEnumerable<SimulationModel> GetSelectedSimulation(int id)
        {
            try
            {
                filterSimulation = db.SIMULATIONS.Where(_ => _.SIMULATIONID == id)
                .Select(p => new SimulationModel
                {
                    SimulationId = p.SIMULATIONID,
                    SimulationName = p.SIMULATION1,
                    NetworkId = p.NETWORKID.Value,
                    Created = p.DATE_CREATED,
                    LastRun = p.DATE_LAST_RUN
                });
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Simulations");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Some error has occured while running query against SIMULATIONS table");
            }

            return filterSimulation;
        }

        public SIMULATION FindWithKey(int id)
        {
            return db.SIMULATIONS.Find(id);
        }

        public void UpdateName(SimulationModel model)
        {
            try
            {
                var result = db.SIMULATIONS.SingleOrDefault(b => b.SIMULATIONID == model.SimulationId);
                result.SIMULATION1 = model.SimulationName;
                db.SaveChanges();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Update Simulation Name");
            }
            return;
        }

        // Deletes simulation record and all records with FK simulationId = id in
        // other tables.
        // The traverse of the DB is setup using [Key] and [ForeignKey] attributes
        // in entity classes
        public void Delete(int id)
        {
            var sim = db.SIMULATIONS.SingleOrDefault(b => b.SIMULATIONID == id);

            if (sim == null)
            {
                return;
            }
            else
            {
                db.Entry(sim).State = EntityState.Deleted;
                db.SaveChanges();
            }

            int? networkId = sim.NETWORKID;

            var select = String.Format
                ("DROP TABLE IF EXISTS SIMULATION_{0}_{1},REPORT_{0}_{1},BENEFITCOST_{0}_{1},TARGET_{0}_{1}",
                networkId,id);

            var connection = new SqlConnection(db.Database.Connection.ConnectionString);
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(select, connection);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch (SqlException ex)
            {
                connection.Close();
                HandleException.SqlError(ex, "Error " + select);
            }
            catch (OutOfMemoryException ex)
            {
                connection.Close();
                HandleException.OutOfMemoryError(ex);
            }           
        }

        public SimulationModel CreateNewSimulation(CreateSimulationDataModel createSimulationData, BridgeCareContext db)
        {
            try { 
                var sim = new SIMULATION()
                {
                    NETWORKID = createSimulationData.NetworkId,
                    SIMULATION1 = createSimulationData.Name,
                    DATE_CREATED = DateTime.Now,
                    ANALYSIS = "Incremental Benefit/Cost",
                    BUDGET_CONSTRAINT = "As Budget Permits",
                    WEIGHTING = "none",
                    COMMITTED_START = DateTime.Now.Year,
                    COMMITTED_PERIOD = 5,
                    TREATMENTS = new List<TREATMENT>
                    {
                        new TREATMENT()
                        {
                            TREATMENT1 = "No Treatment",
                            BEFOREANY = 1,
                            BEFORESAME = 1,
                            BUDGET = null,
                            DESCRIPTION = "Default Treatment",
                            OMS_IS_EXCLUSIVE = null,
                            OMS_IS_REPEAT = null,
                            OMS_REPEAT_START = null,
                            OMS_REPEAT_INTERVAL = null,
                            CONSEQUENCES = new List<CONSEQUENCE>
                            {
                                new CONSEQUENCE
                                    {
                                    ATTRIBUTE_ = "AGE",
                                    CHANGE_ = "+1"
                                }
                            }
                        }
                    }                 
                };

                db.SIMULATIONS.Add(sim);
                db.SaveChanges();

                sim.INVESTMENTS = new INVESTMENTS()
                {
                    SIMULATIONID = sim.SIMULATIONID,
                    FIRSTYEAR = DateTime.Now.Year,
                    NUMBERYEARS = 5,
                    INFLATIONRATE = 2,
                    DISCOUNTRATE = 3,
                    BUDGETORDER = "Rehabilitation,Maintenance,Construction",
                    DESCRIPTION = "new simulation"
                };

                db.SaveChanges();

                var simulationModel = from contextTable in db.SIMULATIONS where contextTable.SIMULATIONID == sim.SIMULATIONID
                                      select new SimulationModel
                                      {
                                        SimulationId = contextTable.SIMULATIONID,
                                        SimulationName = contextTable.SIMULATION1,
                                        NetworkId = contextTable.NETWORKID.Value,
                                        Created = contextTable.DATE_CREATED,
                                        LastRun = contextTable.DATE_LAST_RUN ?? DateTime.Now,
                                        NetworkName = contextTable.NETWORK.NETWORK_NAME
                                      };
              return simulationModel.FirstOrDefault();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "SQL error: New Simulation");
            }
            return null;
        }
    }
}