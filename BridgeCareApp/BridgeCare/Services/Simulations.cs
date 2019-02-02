using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.ApplicationLog;

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
    }
}