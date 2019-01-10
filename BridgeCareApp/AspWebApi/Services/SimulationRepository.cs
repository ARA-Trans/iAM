using AspWebApi.ApplicationLogs;
using AspWebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace AspWebApi.Services
{
    public class SimulationRepository
    {
        private BridgeCareEntities db = new BridgeCareEntities();

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
                    Logger.Error("The server has timed out. Please try after some time", "GetSelectedSimulation()");
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    Logger.Error("Simulations does not exist in the database", "GetSelectedSimulation()");
                    throw new InvalidOperationException("Simulations does not exist in the database");
                }
            }
            catch (Exception)
            {
                Logger.Error("Some error has occured while running query agains SIMULATIONS table", "GetSelectedSimulation()");
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