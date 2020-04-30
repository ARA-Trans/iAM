using System;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class AttributeValueChange : CompilableExpression
    {
        public Action GetApplicator(Attribute attribute, CalculateEvaluateArgument argument)
        {
            switch (attribute)
            {
            case NumberAttribute _:
                var oldNumber = argument.GetNumber(attribute.Name);
                var newNumber = ChangeNumber(oldNumber);
                return () => argument.SetNumber(attribute.Name, newNumber);

            case TextAttribute _:
                var newText = Expression;
                return () => argument.SetText(attribute.Name, newText);

            default:
                throw new ArgumentException("Invalid attribute type.", nameof(attribute));
            }
        }

        protected override void Compile()
        {
            var match = NumberChangePattern.Match(Expression);

            if (!match.Success)
            {
                throw new InputException("Expression does not match the number-change pattern: (+/-) N (%)");
            }

            if (!double.TryParse(match.Groups[2].Value, out var operand))
            {
                throw new InputException("Operand is not a correctly formatted number.");
            }

            Operand = operand;
            var operation = match.Groups[1].Value;
            var operandType = match.Groups[3].Value;

            switch (operandType)
            {
            case "%":
                Operand /= 100;
                switch (operation)
                {
                case "+":
                    NumberChanger = AddPercentage;
                    break;

                case "-":
                    NumberChanger = SubtractPercentage;
                    break;

                case "":
                    NumberChanger = SetPercentage;
                    break;

                default:
                    throw new InvalidOperationException("Invalid operation.");
                }
                break;

            case "":
                switch (operation)
                {
                case "+":
                    NumberChanger = Add;
                    break;

                case "-":
                    NumberChanger = Subtract;
                    break;

                case "":
                    NumberChanger = Set;
                    break;

                default:
                    throw new InvalidOperationException("Invalid operation.");
                }
                break;

            default:
                throw new InvalidOperationException("Invalid operand type.");
            }
        }

        private static readonly Regex NumberChangePattern = new Regex(@"(?>\A\s*((?:\+|-)?)([^%]+)(%?)\s*\z)", RegexOptions.Compiled);

        private Func<double, double> NumberChanger;

        private double Operand;

        private double ChangeNumber(double value)
        {
            EnsureCompiled();
            return NumberChanger(value);
        }

        #region Number-changing operations

        private double Add(double value) => value + Operand;

        private double AddPercentage(double value) => value * (1 + Operand);

        private double Set(double value) => Operand;

        private double SetPercentage(double value) => value * Operand;

        private double Subtract(double value) => value - Operand;

        private double SubtractPercentage(double value) => value * (1 - Operand);

        #endregion Number-changing operations
    }
}
