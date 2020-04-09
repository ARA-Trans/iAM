using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public static class SelectionComparer<TSource>
    {
        public static SelectionComparer<TSource, TSelection> Create<TSelection>(Func<TSource, TSelection> selector, IComparer<TSelection> comparer = null) => new SelectionComparer<TSource, TSelection>(selector, comparer);
    }

    public sealed class SelectionComparer<TSource, TSelection> : Comparer<TSource>
    {
        public SelectionComparer(Func<TSource, TSelection> selector, IComparer<TSelection> comparer)
        {
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            Comparer = comparer ?? Comparer<TSelection>.Default;
        }

        public override int Compare(TSource x, TSource y) => Comparer.Compare(Selector(x), Selector(y));

        private readonly IComparer<TSelection> Comparer;

        private readonly Func<TSource, TSelection> Selector;
    }
}
