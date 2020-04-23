using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using MathNet.Numerics;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class Equation : CompilableExpression
    {
        public Equation(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

        public double Compute(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
        {
            EnsureCompiled();
            return Computer.Reduce(
                calculator => calculator(argument),
                interpolator => interpolator(argument.GetNumber(ageAttribute.Name)));
        }

        protected override void Compile()
        {
            var match = PiecewisePattern.Match(Expression);
            if (match.Success)
            {
                var xs = new List<double>();
                var ys = new List<double>();

                foreach (var group in match.Groups.Cast<Group>().Skip(1))
                {
                    var datum = group.Value.Split(',');
                    var x = double.Parse(datum[0]);
                    var y = double.Parse(datum[1]);
                    xs.Add(x);
                    ys.Add(y);
                }

                var spline = Interpolate.Linear(xs, ys);
                Computer = (Func<double, double>)spline.Interpolate;
            }
            else
            {
                Computer = Compiler.GetCalculator(Expression);
            }
        }

        private static readonly Regex PiecewisePattern = new Regex(@"\A\s*(?:\((.*?,.*?)\)\s*)*\z");

        private readonly CalculateEvaluateCompiler Compiler;

        private Choice<Calculator, Func<double, double>> Computer;
    }
}
