using System.Collections;
using System.Collections.Generic;
using static AppliedResearchAssociates.Static;

namespace AppliedResearchAssociates
{
    public sealed class SequenceEqualityComparer : EqualityComparer<IEnumerable>
    {
        public static SequenceEqualityComparer Instance { get; } = new SequenceEqualityComparer();

        public override bool Equals(IEnumerable x, IEnumerable y) => SequenceEquals(x, y);

        public override int GetHashCode(IEnumerable obj) => obj.CombineHashCodes();

        private SequenceEqualityComparer()
        {
        }
    }

    public sealed class SequenceEqualityComparer<T> : EqualityComparer<IEnumerable<T>>
    {
        public static SequenceEqualityComparer<T> Instance { get; } = new SequenceEqualityComparer<T>();

        public override bool Equals(IEnumerable<T> x, IEnumerable<T> y) => SequenceEquals(x, y);

        public override int GetHashCode(IEnumerable<T> obj) => obj.CombineHashCodes();

        private SequenceEqualityComparer()
        {
        }
    }
}
