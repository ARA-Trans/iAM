using System;
using System.Linq;

namespace AppliedResearchAssociates.iAM
{
    internal sealed class RemainingLifeCalculator
    {
        public RemainingLifeCalculator(SectionContext context, IGrouping<NumberAttribute, double> limits)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Attribute = limits.Key;
            SetCurrentValue();

            if (Attribute.IsDecreasingWithDeterioration)
            {
                Limit = limits.Max();
                _CurrentValueIsBeyondLimit = CurrentValueIsLessThanLimit;
            }
            else
            {
                Limit = limits.Min();
                _CurrentValueIsBeyondLimit = CurrentValueIsGreaterThanLimit;
            }
        }

        public bool CurrentValueIsBeyondLimit() => _CurrentValueIsBeyondLimit();

        public double GetLimitLocationRelativeToLatestValues() => (Limit - PreviousValue) / (CurrentValue - PreviousValue);

        public void UpdateValue()
        {
            PreviousValue = CurrentValue;
            SetCurrentValue();
        }

        public sealed class Factory
        {
            public Factory(IGrouping<NumberAttribute, double> limits) => Limits = limits ?? throw new ArgumentNullException(nameof(limits));

            public RemainingLifeCalculator Create(SectionContext context) => new RemainingLifeCalculator(context, Limits);

            private readonly IGrouping<NumberAttribute, double> Limits;
        }

        private readonly Func<bool> _CurrentValueIsBeyondLimit;

        private readonly NumberAttribute Attribute;

        private readonly SectionContext Context;

        private readonly double Limit;

        private double CurrentValue;

        private double PreviousValue = double.NaN;

        private bool CurrentValueIsGreaterThanLimit() => CurrentValue > Limit;

        private bool CurrentValueIsLessThanLimit() => CurrentValue < Limit;

        private void SetCurrentValue() => CurrentValue = Context.GetNumber(Attribute.Name);
    }
}
