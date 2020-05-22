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

        public override ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

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
        }

        public IReadOnlyCollection<decimal> YearlyPercentages
        {
            get
            {
                EnsureCompiled();
                return _YearlyPercentages;
            }
        }

        protected override void Compile()
        {
            var match = YearlyFractionsPattern.Match(Expression);
            if (!match.Success)
            {
                throw ExpressionCouldNotBeCompiled();
            }

            var yearlyPercentages = new List<decimal> { ParsePercentage(match.Groups[1]) };
            yearlyPercentages.AddRange(match.Groups[2].Captures.Cast<Capture>().Select(ParsePercentage));

            _YearlyPercentages.Clear();
            _YearlyPercentages.AddRange(yearlyPercentages);
        }

        private static readonly Regex YearlyFractionsPattern = new Regex($@"(?>\A\s*({PatternStrings.Number})(?:\s*/\s*({PatternStrings.Number}))*\s*\z)", RegexOptions.Compiled);

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
