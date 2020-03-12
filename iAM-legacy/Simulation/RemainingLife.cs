using RoadCareGlobalOperations;
using Simulation.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class RemainingLife : IRemainingLife
    {
        public int RemainingLifeId { get; }
        public string Attribute { get; }
        public double RemainingLifeLimit { get; }
        public Criterias Criteria { get; }

        public RemainingLife(int remainingLifeId, string attribute, double remainingLifeLimit, string criteria)
        {
            RemainingLifeId = remainingLifeId;
            Attribute = attribute;
            RemainingLifeLimit = remainingLifeLimit;

            Criteria = new Criterias("REMAINING_LIFE", "BINARY_CRITERIA", RemainingLifeId.ToString());
            byte[] assemblyCriteria = null;
            string currentCriteria = "";
            if (!string.IsNullOrWhiteSpace(criteria)) currentCriteria = criteria;
            assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate("REMAINING_LIFE", "BINARY_CRITERIA", RemainingLifeId.ToString(), assemblyCriteria);
            if (assemblyCriteria != null && assemblyCriteria.Length > 0)
            {
                Criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
                if (Criteria.Evaluate.OriginalInput != currentCriteria)
                {
                    Criteria.Evaluate = null;
                }
            }

            if (Criteria.Evaluate != null && Criteria.Evaluate.m_cr != null)
            {
                if (!File.Exists(Criteria.Evaluate.m_cr.PathToAssembly))
                {
                    Criteria.Evaluate = null;
                }
            }

            Criteria.Criteria = criteria;
            foreach (String str in Criteria.Errors)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Supersede criteria for " + criteria + " :" + str));
            }
        }
    }
}
