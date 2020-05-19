using System;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using MathNet.Numerics;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Equation : CalculateEvaluateExpression
    {
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
                double parseValue(Capture capture)
                {
                    if (!double.TryParse(capture.Value, out var result))
                    {
                        throw ExpressionCouldNotBeCompiled();
                    }

                    return result;
                }

                double[] parseGroup(int index) => match.Groups[index].Captures.Cast<Capture>().Select(parseValue).ToArray();

                var xValues = parseGroup(1);
                var yValues = parseGroup(2);

                if (xValues.Length < 2 || yValues.Length < 2)
                {
                    throw ExpressionCouldNotBeCompiled();
                }

                var spline = Interpolate.Linear(xValues, yValues);

                Computer = (Func<double, double>)spline.Interpolate;
            }
            else
            {
                try
                {
                    Computer = Explorer.Compiler.GetCalculator(Expression);
                }
                catch (CalculateEvaluateException e)
                {
                    throw ExpressionCouldNotBeCompiled(e);
                }
            }
        }

        private static readonly Regex PiecewisePattern = new Regex($@"(?>\A\s*(?:\(\s*({PatternStrings.Number})\s*,\s*({PatternStrings.Number})\s*\)\s*)*\z)", RegexOptions.Compiled);

        private Choice<Calculator, Func<double, double>> Computer;
    }
}
