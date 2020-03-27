using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public static class SelectionEqualityComparer<TSource>
    {
        public static SelectionEqualityComparer<TSource, TSelection> By<TSelection>(Func<TSource, TSelection> selector) => new SelectionEqualityComparer<TSource, TSelection>(selector);
    }

    public sealed class SelectionEqualityComparer<TSource, TSelection> : EqualityComparer<TSource>
    {
        public SelectionEqualityComparer(Func<TSource, TSelection> selector) => Selector = selector ?? throw new ArgumentNullException(nameof(selector));

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

            return EqualityComparer<TSelection>.Default.Equals(Selector(x), Selector(y));
        }

        public override int GetHashCode(TSource obj) => HashCode.Combine(Selector(obj));

        private readonly Func<TSource, TSelection> Selector;
    }
}
