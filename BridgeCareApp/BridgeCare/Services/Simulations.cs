using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
                                      NetworkId = contextTable.NETWORKID.Value
                                  };
            return filteredColumns;
        }

        public IEnumerable<SimulationModel> GetSelectedSimulation(int id)
        {
            try
            {
                filterSimulation = db.SIMULATIONS.Where(_ => _.NETWORKID == id)
                .Select(p => new SimulationModel
                {
                    SimulationId = p.SIMULATIONID,
                    SimulationName = p.SIMULATION1,
                    NetworkId = p.NETWORKID.Value
                });
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Simulations");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Some error has occured while running query agains SIMULATIONS table");
            }

            return filterSimulation;
        }

        public SIMULATION FindWithKey(int id)
        {
            return db.SIMULATIONS.Find(id);
        }

        public void UpdateName(SimulationModel model, BridgeCareContext db)
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

        public int CreateNewSimulation(int networkId, string simulationName, BridgeCareContext db)
        {
            string errorString = "Unknown Error";
            try
            {
                var sim = new SIMULATION()
                {
                    NETWORKID = networkId,
                    SIMULATION1 = simulationName,
                    DATE_CREATED = DateTime.Now,
                    ANALYSIS = "Incremental Benefit/Cost",
                    BUDGET_CONSTRAINT = "As Budget Permits",
                    WEIGHTING = "none",
                    COMMITTED_START = DateTime.Now.Year,
                    COMMITTED_PERIOD = 5
                };
                errorString = "Add to simulation table";
                db.SIMULATIONS.Add(sim);

                db.SaveChanges();

                var investment = new INVESTMENTS()
                {
                    SIMULATIONID = sim.SIMULATIONID,
                    FIRSTYEAR = DateTime.Now.Year,
                    NUMBERYEARS = 5,
                    INFLATIONRATE = 2,
                    DISCOUNTRATE = 3,
                    BUDGETORDER = "Rehabilitation,Maintenance,Construction",
                    DESCRIPTION = "new simulation"
                };
                errorString = "Add to investments table";
                db.INVESTMENTs.Add(investment);
                db.SaveChanges();

                var treatments = new TREATMENT()
                {
                    TREATMENTID = 0,
                    SIMULATIONID = sim.SIMULATIONID,
                    TREATMENT1 = "No Treatment",
                    BEFOREANY = 1,
                    BEFORESAME = 1,
                    BUDGET = null,
                    DESCRIPTION = "Default Treatment",
                    OMS_IS_EXCLUSIVE = null,
                    OMS_IS_REPEAT = null,
                    OMS_REPEAT_START = null,
                    OMS_REPEAT_INTERVAL = null
                };

                errorString = "Add to treatments table"; ;
                db.Treatments.Add(treatments);

                db.SaveChanges();

                String strQuery = errorString = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE lower(ATTRIBUTE_) = 'age'";
                String strAge = db.Database.SqlQuery<string>(strQuery).SingleOrDefault();

                var consequence = new CONSEQUENCE()
                {
                    TREATMENTID = treatments.TREATMENTID,
                    ATTRIBUTE_ = strAge,
                    CHANGE_ = "+1"
                };
                errorString = "Add to consequences table";
                db.Consequences.Add(consequence);
                db.SaveChanges();

                return sim.SIMULATIONID;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "SQL error:" + errorString);
            }
            return -1;
        }
    }
}