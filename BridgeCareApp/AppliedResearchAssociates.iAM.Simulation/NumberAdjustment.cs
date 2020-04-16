using System;
using System.Text.RegularExpressions;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class NumberAdjustment : CompilableExpression
    {
        public double Adjust(double value)
        {
            EnsureCompiled();
            return Adjuster(value);
        }

        protected override void Compile()
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

        private static readonly Regex ExpressionPattern = new Regex(@"\A\s*((?:\+|-)?)(.+?)(%?)\s*\z");

        private Func<double, double> Adjuster;

        private double Operand;

        #region Operations

        private double Add(double value) => value + Operand;

        private double AddPercentage(double value) => value * (1 + Operand);

        private double Set(double value) => Operand;

        private double SetPercentage(double value) => value * Operand;

        private double Subtract(double value) => value - Operand;

        private double SubtractPercentage(double value) => value * (1 - Operand);

        #endregion Operations
    }
}
