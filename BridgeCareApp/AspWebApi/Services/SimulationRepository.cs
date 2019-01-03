using AspWebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace AspWebApi.Services
{
    public class SimulationRepository
    {
        private BridgeCareEntities db = new BridgeCareEntities();

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
            var filterSimulation = db.SIMULATIONS.Where(_ => _.NETWORKID == id)
                .Select(p => new SimulationModel
                {
                    SimulationId = p.SIMULATIONID,
                    SimulationName = p.SIMULATION1,
                    NetworkId = p.NETWORKID.Value
                });

            return filterSimulation;
        }

        public SIMULATION FindWithKey(int id)
        {
            return db.SIMULATIONS.Find(id);
        }
    }
}