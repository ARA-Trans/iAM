using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BridgeCare.DataAccessLayer
{
    public class SimulationDAL : ISimulation
    {
        private readonly BridgeCareContext db;

        public SimulationDAL(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IQueryable<SimulationModel> filterSimulation;

        public IQueryable<SimulationModel> GetAllSimulations()
        {
            var filteredColumns = from contextTable in db.Simulations
                                  select new SimulationModel
                                  {
                                      SimulationId = contextTable.SIMULATIONID,
                                      SimulationName = contextTable.SIMULATION,
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
                filterSimulation = db.Simulations.Where(_ => _.SIMULATIONID == id)
                .Select(p => new SimulationModel
                {
                    SimulationId = p.SIMULATIONID,
                    SimulationName = p.SIMULATION,
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

        public SimulationEntity FindWithKey(int id)
        {
            return db.Simulations.Find(id);
        }

        public void UpdateName(SimulationModel model)
        {
            try
            {
                var result = db.Simulations.SingleOrDefault(b => b.SIMULATIONID == model.SimulationId);
                result.SIMULATION = model.SimulationName;
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
        public int Delete(int id)
        {
            var sim = db.Simulations.SingleOrDefault(b => b.SIMULATIONID == id);
            var rowsAffected = -1;

            if (sim == null)
            {
                return rowsAffected;
            }
            else
            {
                db.Entry(sim).State = EntityState.Deleted;
                rowsAffected  = db.SaveChanges();
            }

            int? networkId = sim.NETWORKID;

            var select = String.Format
                ("DROP TABLE IF EXISTS SIMULATION_{0}_{1},REPORT_{0}_{1},BENEFITCOST_{0}_{1},TARGET_{0}_{1}",
                networkId, id);

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
            return rowsAffected;
        }

        public SimulationModel CreateRunnableSimulation(CreateSimulationDataModel createSimulationData, BridgeCareContext db)
        {
            try
            {
                var sim = new SimulationEntity()
                {
                    NETWORKID = createSimulationData.NetworkId,
                    SIMULATION = createSimulationData.Name,
                    DATE_CREATED = DateTime.Now,
                    ANALYSIS = "Incremental Benefit/Cost",
                    BUDGET_CONSTRAINT = "As Budget Permits",
                    WEIGHTING = "none",
                    COMMITTED_START = DateTime.Now.Year,
                    COMMITTED_PERIOD = 1,

                    YEARLYINVESTMENTS = new List<YearlyInvestmentEntity>
                    {
                        new YearlyInvestmentEntity
                        {
                            YEAR_ = DateTime.Now.Year,
                            BUDGETNAME = "Rehabilitation",
                            AMOUNT = 5000000
                        },
                        new YearlyInvestmentEntity
                        {
                            YEAR_ = DateTime.Now.Year,
                            BUDGETNAME = "Maintenance",
                            AMOUNT = 5000000
                        },
                        new YearlyInvestmentEntity
                        {
                            YEAR_ = DateTime.Now.Year,
                            BUDGETNAME = "Construction",
                            AMOUNT = 5000000
                        }
                    },
                    TREATMENTS = new List<TreatmentsEntity>
                    {
                        new TreatmentsEntity()
                        {
                            TREATMENT = "No Treatment",
                            BEFOREANY = 1,
                            BEFORESAME = 1,
                            BUDGET = "Construction,Maintenance,Rehabilitation",
                            DESCRIPTION = "Default Treatment",
                            OMS_IS_EXCLUSIVE = null,
                            OMS_IS_REPEAT = null,
                            OMS_REPEAT_START = null,
                            OMS_REPEAT_INTERVAL = null,
                            CONSEQUENCES = new List<ConsequencesEntity>
                            {
                                new ConsequencesEntity
                                    {
                                    ATTRIBUTE_ = "AGE",
                                    CHANGE_ = "+1"
                                }
                            }
                        }
                    }
                };

                db.Simulations.Add(sim);
                db.SaveChanges();

                sim.INVESTMENTS = new InvestmentsEntity()
                {
                    SIMULATIONID = sim.SIMULATIONID,
                    FIRSTYEAR = DateTime.Now.Year,
                    NUMBERYEARS = 1,
                    INFLATIONRATE = 0,
                    DISCOUNTRATE = 0,
                    BUDGETORDER = "Rehabilitation,Maintenance,Construction"
                };

                db.SaveChanges();

                sim.CriteriaDrivenBudgets= new List<CriteriaDrivenBudgetsEntity>
                {
                    new CriteriaDrivenBudgetsEntity
                    {
                        BUDGET_NAME = "Maintenance",
                        CRITERIA = "",
                        SIMULATIONID = sim.SIMULATIONID
                    },
                    new CriteriaDrivenBudgetsEntity
                    {
                        BUDGET_NAME = "Rehabilitation",
                        CRITERIA = "",
                        SIMULATIONID = sim.SIMULATIONID
                    },
                    new CriteriaDrivenBudgetsEntity
                    {
                        BUDGET_NAME = "Construction",
                        CRITERIA = "",
                        SIMULATIONID = sim.SIMULATIONID
                    }
                };

                db.SaveChanges();

                var priorities = new PriorityDAL();
                var defaultBudgets = new List<string>{"Rehabilitation", "Maintenance" ,"Construction"};
                priorities.SavePriorityFundInvestmentData(sim.SIMULATIONID, defaultBudgets, db);

                var simulationModel = from contextTable in db.Simulations
                                      where contextTable.SIMULATIONID == sim.SIMULATIONID
                                      select new SimulationModel
                                      {
                                          SimulationId = contextTable.SIMULATIONID,
                                          SimulationName = contextTable.SIMULATION,
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