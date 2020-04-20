using System;
using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class CalculatedField
    {
        public List<Criterial<Equation>> Equations { get; }

        public string Name { get; }

        public double Calculate(CalculateEvaluateArgument argument)
        {
            throw new NotImplementedException("Need to find applicable equation(s) for calculation.");
        }
    }
}
