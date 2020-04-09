using System;
using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public sealed class SequenceComparer : Comparer<IEnumerable>
    {
        new public static SequenceComparer Default { get; } = new SequenceComparer(Comparer<object>.Default);

        public static SequenceComparer Create(IComparer comparer) => new SequenceComparer(comparer ?? throw new ArgumentNullException(nameof(comparer)));

        public static SequenceComparer<T> Create<T>(IComparer<T> comparer) => new SequenceComparer<T>(comparer ?? throw new ArgumentNullException(nameof(comparer)));

        public override int Compare(IEnumerable x, IEnumerable y) => Static.SequenceCompare(x, y, Comparer);

        private readonly IComparer Comparer;

        private SequenceComparer(IComparer comparer) => Comparer = comparer;
    }

    public sealed class SequenceComparer<T> : Comparer<IEnumerable<T>>
    {
        new public static SequenceComparer<T> Default { get; } = new SequenceComparer<T>(Comparer<T>.Default);

        public override int Compare(IEnumerable<T> x, IEnumerable<T> y) => Static.SequenceCompare(x, y, Comparer);

        internal SequenceComparer(IComparer<T> comparer) => Comparer = comparer;

        private readonly IComparer<T> Comparer;
    }
}
