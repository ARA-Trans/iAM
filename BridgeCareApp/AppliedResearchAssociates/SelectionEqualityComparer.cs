using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public static class SelectionEqualityComparer<TSource>
    {
        public static SelectionEqualityComparer<TSource, TSelection> Create<TSelection>(Func<TSource, TSelection> selector, IEqualityComparer<TSelection> equalityComparer = null) => new SelectionEqualityComparer<TSource, TSelection>(selector, equalityComparer);
    }

    public sealed class SelectionEqualityComparer<TSource, TSelection> : EqualityComparer<TSource>
    {
        public SelectionEqualityComparer(Func<TSource, TSelection> selector, IEqualityComparer<TSelection> equalityComparer)
        {
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            EqualityComparer = equalityComparer ?? EqualityComparer<TSelection>.Default;
        }

        public override bool Equals(TSource x, TSource y)
        {
            if (!typeof(TSource).IsValueType)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }
            }

            return EqualityComparer.Equals(Selector(x), Selector(y));
        }

        public override int GetHashCode(TSource obj)
        {
            var selection = Selector(obj);

            if (!typeof(TSelection).IsValueType && selection == null)
            {
                return HashCode.Combine(selection);
            }

            return HashCode.Combine(EqualityComparer.GetHashCode(selection));
        }

        private readonly IEqualityComparer<TSelection> EqualityComparer;

        private readonly Func<TSource, TSelection> Selector;
    }
}
