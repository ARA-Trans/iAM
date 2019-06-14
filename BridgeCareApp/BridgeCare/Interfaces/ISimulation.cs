﻿using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISimulation
    {
        IQueryable<SimulationModel> GetAllSimulations();

        IEnumerable<SimulationModel> GetSelectedSimulation(int id);

        void UpdateName(SimulationModel model);

        void Delete(int id);

        SimulationModel CreateNewSimulation(CreateSimulationDataModel createSimulationData, BridgeCareContext db);
    }
}