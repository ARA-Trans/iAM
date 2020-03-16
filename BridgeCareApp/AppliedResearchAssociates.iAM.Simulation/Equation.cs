using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    //Input: numbers
    //Output: number
    public class Equation
    {
        public string Expression { get; }

        // Should be able to use Jace for this.
        public double Calculate(double[] values) => throw new NotImplementedException();
    }
}
