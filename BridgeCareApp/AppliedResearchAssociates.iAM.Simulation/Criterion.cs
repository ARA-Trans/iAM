using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    //Input: any mix of numbers, strings, and/or dates
    //Output: true/false

    // FIRST, investigate usage of an up-to-date NCalc derivative, e.g.
    // https://github.com/sklose/NCalc2, which uses ANTLR and emits dynamic IL (highest
    // performance, GC-ible). Two **major** points in favor of this: (1) it supports strings,
    // dates, and bools; (2) it already uses the square-bracket parameter syntax required by CalculateEvaluate.

    // OTHERWISE, might have to use Roslyn for this. Need to be careful about the script API's
    // inability to unload the assemblies it creates. We could try to preprocess the expression
    // string by re-inserting transformations of string and date operations, such that the new
    // expression contains only numeric operations and is compatible with Jace. **Or we might
    // consider contributing to Jace, such that it supports string and date operations, plus
    // bool results.** And if contributing to Jace is infeasible (for whatever reason), we might
    // consider using ANTLR to parse expresions and emit IL.
    public class Criterion
    {
        public string Expression { get; }

        public bool Evaluate(object[] values) => throw new NotImplementedException();
    }
}
