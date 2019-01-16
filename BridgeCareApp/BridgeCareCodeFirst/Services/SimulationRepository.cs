using BridgeCareCodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BridgeCareCodeFirst.EntityClasses;

namespace BridgeCareCodeFirst.Services
{
    public class SimulationRepository
    {
        private BridgeCareContext db = new BridgeCareContext();

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
                if (ex.Number == -2 || ex.Number == 11)
                {
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    throw new InvalidOperationException("Simulations does not exist in the database");
                }
            }
            catch (Exception)
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