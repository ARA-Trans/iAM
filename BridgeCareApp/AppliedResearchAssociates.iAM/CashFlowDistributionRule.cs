using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowDistributionRule : CompilableExpression
    {
        public decimal? CostCeiling { get; set; }

        public IReadOnlyCollection<decimal> YearlyPercentages
        {
            get
            {
                EnsureCompiled();
                return _YearlyPercentages;
            }
        }

        public override ValidationResultBag GetDirectValidationResults()
        {
            var results = base.GetDirectValidationResults();

            if (CostCeiling <= 0)
            {
                results.Add(ValidationStatus.Error, "Cost ceiling is less than or equal to zero.", this, nameof(CostCeiling));
            }

            if (YearlyPercentages.Any(percentage => percentage < 0))
            {
                results.Add(ValidationStatus.Error, "At least one yearly percentage is less than zero.", this, nameof(YearlyPercentages));
            }

            if (YearlyPercentages.Any(percentage => percentage > 100))
            {
                results.Add(ValidationStatus.Error, "At least one yearly percentage is greater than 100.", this, nameof(YearlyPercentages));
            }

            if (YearlyPercentages.Sum() != 100)
            {
                results.Add(ValidationStatus.Error, "Yearly percentages do not sum to 100.", this, nameof(YearlyPercentages));
            }

            return results;
        }

        protected override void Compile()
        {
            if (ExpressionIsBlank)
            {
                throw ExpressionCouldNotBeCompiled();
            }

            var match = YearlyPercentagesPattern.Match(Expression);
            if (!match.Success)
            {
                throw ExpressionCouldNotBeCompiled();
            }

            _YearlyPercentages.Clear();
            _YearlyPercentages.Add(ParsePercentage(match.Groups[1]));
            _YearlyPercentages.AddRange(match.Groups[2].Captures.Cast<Capture>().Select(ParsePercentage));
        }

        private static readonly Regex YearlyPercentagesPattern = new Regex($@"(?>\A\s*({PatternStrings.Number})(?:\s*/\s*({PatternStrings.Number}))*\s*\z)", RegexOptions.Compiled);

        private readonly List<decimal> _YearlyPercentages = new List<decimal>();

        private static decimal ParsePercentage(Capture capture)
        {
            if (!decimal.TryParse(capture.Value, NumberStyles.Float, null, out var percentage))
            {
                throw ExpressionCouldNotBeCompiled();
            }

            return percentage;
        }
    }
}
