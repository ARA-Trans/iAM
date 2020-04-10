using System;
using System.Text.RegularExpressions;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class NumberAdjustment : CompilableExpression
    {
        public double Adjust(double value)
        {
            if (!IsCompiled)
            {
                Compile();
                IsCompiled = true;
            }

            return Adjuster(value, Operand);
        }

        private static readonly Regex ExpressionPattern = new Regex(@"\A\s*((?:\+|-)?)(.+?)(%?)\s*\z");

        private Func<double, double, double> Adjuster;

        private double Operand;

        private static double Add(double value, double operand) => value + operand;

        private static double AddPercentage(double value, double operand) => value * (1 + operand);

        private static double Set(double value, double operand) => operand;

        private static double SetPercentage(double value, double operand) => value * operand;

        private static double Subtract(double value, double operand) => value - operand;

        private static double SubtractPercentage(double value, double operand) => value * (1 - operand);

        private void Compile()
        {
            var match = ExpressionPattern.Match(Expression);

            var operation = match.Groups[1].Value;
            Operand = double.Parse(match.Groups[2].Value);
            var operandType = match.Groups[3].Value;

            switch (operandType)
            {
            case "%":
                Operand /= 100;
                switch (operation)
                {
                case "+":
                    Adjuster = AddPercentage;
                    break;

                case "-":
                    Adjuster = SubtractPercentage;
                    break;

                case "":
                    Adjuster = SetPercentage;
                    break;

                default:
                    throw new InvalidOperationException("Invalid operation.");
                }
                break;

            case "":
                switch (operation)
                {
                case "+":
                    Adjuster = Add;
                    break;

                case "-":
                    Adjuster = Subtract;
                    break;

                case "":
                    Adjuster = Set;
                    break;

                default:
                    throw new InvalidOperationException("Invalid operation.");
                }
                break;

            default:
                throw new InvalidOperationException("Invalid operand type.");
            }
        }
    }
}
