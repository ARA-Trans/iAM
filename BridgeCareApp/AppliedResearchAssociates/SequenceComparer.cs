using System.Collections;
using System.Collections.Generic;
using static AppliedResearchAssociates.Static;

namespace AppliedResearchAssociates
{
    public sealed class SequenceComparer : Comparer<IEnumerable>
    {
        public static SequenceComparer Instance { get; } = new SequenceComparer();

        public override int Compare(IEnumerable x, IEnumerable y) => SequenceCompare(x, y);

        private SequenceComparer()
        {
        }
    }

    public sealed class SequenceComparer<T> : Comparer<IEnumerable<T>>
    {
        public static SequenceComparer<T> Instance { get; } = new SequenceComparer<T>();

        public override int Compare(IEnumerable<T> x, IEnumerable<T> y) => SequenceCompare(x, y);

        private SequenceComparer()
        {
        }
    }
}
