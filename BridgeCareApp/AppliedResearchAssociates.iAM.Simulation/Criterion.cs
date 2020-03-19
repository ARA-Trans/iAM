using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    //Input: any mix of numbers, strings, and/or dates
    //Output: true/false
    public class Criterion
    {
        //https://github.com/sklose/NCalc2
        public string Expression { get; }

        public bool Evaluate(object[] values) => throw new NotImplementedException();
    }
}
