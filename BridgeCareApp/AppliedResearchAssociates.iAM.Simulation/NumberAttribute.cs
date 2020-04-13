using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class NumberAttribute : Attribute
    {
        public List<AttributeDatum<double>> Data { get; }

        public double Default { get; }

        public double Maximum { get; }

        public double Minimum { get; }
    }
}
