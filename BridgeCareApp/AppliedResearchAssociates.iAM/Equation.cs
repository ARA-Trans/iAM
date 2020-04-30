using System;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using MathNet.Numerics;

namespace AppliedResearchAssociates.iAM
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
                double[] parseGroup(int index) => match.Groups[index].Captures.Cast<Capture>().Select(c => double.Parse(c.Value)).ToArray();

                var xValues = parseGroup(1);
                var yValues = parseGroup(2);

                var spline = Interpolate.Linear(xValues, yValues);

                Computer = (Func<double, double>)spline.Interpolate;
            }
            else
            {
                Computer = Compiler.GetCalculator(Expression);
            }
        }

        private static readonly Regex PiecewisePattern = new Regex($@"(?>\A\s*(?:\(\s*({Subpatterns.Number})\s*,\s*({Subpatterns.Number})\s*\)\s*)*\z)", RegexOptions.Compiled);

        private readonly CalculateEvaluateCompiler Compiler;

        private Choice<Calculator, Func<double, double>> Computer;

        private static class Subpatterns
        {
            public static string DecimalPart => $@"(?:\.{NaturalNumber})";

            public static string Exponent => $@"(?:[eE]{Sign}?{NaturalNumber})";

            public static string Mantissa => $@"(?:{Mantissa1}|{Mantissa2})";

            public static string Mantissa1 => $@"(?:{NaturalNumber}{DecimalPart}?)";

            public static string Mantissa2 => $@"(?:{DecimalPart})";

            public static string NaturalNumber => $@"(?:\d+)";

            public static string Number => $@"(?:{Sign}?{Mantissa}{Exponent}?)";

            public static string Sign => $@"(?:\+|-)";
        }
    }
}
