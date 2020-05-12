using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public sealed class Criterion : CompilableExpression
    {
        public CalculateEvaluateCompiler Compiler { get; set; }

        public bool? Evaluate(CalculateEvaluateArgument argument)
        {
            throw new NotImplementedException();
        }

        protected override void Compile() => throw new NotImplementedException();
    }
}
