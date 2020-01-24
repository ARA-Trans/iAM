using Simulation.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class SplitTreatment : ISplitTreatment
    {
        public string Id { get; }

        public string Description { get; }

        public Criterias Criteria { get; }

        public List<ISplitTreatmentLimit> Limits { get; }


        public SplitTreatment(string splitTreamentId, string description, string criteria,  List<ISplitTreatmentLimit> limits)
        {
            Id = splitTreamentId;
            Description = description;
            Limits = limits;
            Criteria = new Criterias("SPLIT_TREATMENT", "BINARY_CRITERIA", splitTreamentId);
            byte[] assemblyCriteria = null;
            string currentCriteria = "";
            if (!string.IsNullOrWhiteSpace(criteria)) currentCriteria = Simulation.ConvertOMSAttribute(criteria);
            assemblyCriteria = SimulationMessaging.GetSerializedCalculateEvaluate("SPLIT_TREATMENT", "BINARY_CRITERIA", splitTreamentId, assemblyCriteria);
            if (assemblyCriteria != null && assemblyCriteria.Length > 0)
            {
                Criteria.Evaluate = (CalculateEvaluate.CalculateEvaluate)RoadCareGlobalOperations.AssemblySerialize.DeSerializeObjectFromByteArray(assemblyCriteria);
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
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Split treatment criteria for " + criteria + " :" + str));
            }

        }
    }
}
