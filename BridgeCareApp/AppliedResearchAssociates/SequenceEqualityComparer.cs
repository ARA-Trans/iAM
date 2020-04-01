using System;
using System.Collections;
using System.Collections.Generic;
using static AppliedResearchAssociates.Static;

namespace AppliedResearchAssociates
{
    public sealed class SequenceEqualityComparer : EqualityComparer<IEnumerable>
    {
        new public static SequenceEqualityComparer Default { get; } = new SequenceEqualityComparer(EqualityComparer<object>.Default);

        public static SequenceEqualityComparer With(IEqualityComparer equalityComparer) => new SequenceEqualityComparer(equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer)));

        public static SequenceEqualityComparer<T> With<T>(IEqualityComparer<T> equalityComparer) => new SequenceEqualityComparer<T>(equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer)));

        public override bool Equals(IEnumerable x, IEnumerable y) => SequenceEquals(x, y, EqualityComparer);

        public override int GetHashCode(IEnumerable obj) => obj.CombineHashCodes(EqualityComparer);

        private readonly IEqualityComparer EqualityComparer;

        private SequenceEqualityComparer(IEqualityComparer equalityComparer) => EqualityComparer = equalityComparer;
    }

    public sealed class SequenceEqualityComparer<T> : EqualityComparer<IEnumerable<T>>
    {
        new public static SequenceEqualityComparer<T> Default { get; } = new SequenceEqualityComparer<T>(EqualityComparer<T>.Default);

        public override bool Equals(IEnumerable<T> x, IEnumerable<T> y) => SequenceEquals(x, y, EqualityComparer);

        public override int GetHashCode(IEnumerable<T> obj) => obj.CombineHashCodes(EqualityComparer);

        internal SequenceEqualityComparer(IEqualityComparer<T> equalityComparer) => EqualityComparer = equalityComparer;

        private readonly IEqualityComparer<T> EqualityComparer;
    }
}
