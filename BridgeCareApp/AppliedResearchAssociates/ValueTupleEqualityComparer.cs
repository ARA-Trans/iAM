// WARNING: This file was automatically generated from a T4 text template at the
// following moment in time: 05/12/2020 11:11:07 -05:00. Any changes you make to
// this file will be lost when this file is regenerated from the template
// source.

using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public static class ValueTupleEqualityComparer
    {
        public static IEqualityComparer<(T1, T2)> Create<T1, T2>(IEqualityComparer<T1> comparer1 = null, IEqualityComparer<T2> comparer2 = null) => new _ValueTupleEqualityComparer<T1, T2>(comparer1, comparer2);

        private sealed class _ValueTupleEqualityComparer<T1, T2> : EqualityComparer<(T1, T2)>
        {
            public _ValueTupleEqualityComparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2)
            {
                Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
                Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
            }

            public override bool Equals((T1, T2) x, (T1, T2) y) => Comparer1.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2, y.Item2);

            public override int GetHashCode((T1, T2) obj) => HashCode.Combine(Comparer1.GetHashCode(obj.Item1), Comparer2.GetHashCode(obj.Item2));

            private readonly IEqualityComparer<T1> Comparer1;
            private readonly IEqualityComparer<T2> Comparer2;
        }

        public static IEqualityComparer<(T1, T2, T3)> Create<T1, T2, T3>(IEqualityComparer<T1> comparer1 = null, IEqualityComparer<T2> comparer2 = null, IEqualityComparer<T3> comparer3 = null) => new _ValueTupleEqualityComparer<T1, T2, T3>(comparer1, comparer2, comparer3);

        private sealed class _ValueTupleEqualityComparer<T1, T2, T3> : EqualityComparer<(T1, T2, T3)>
        {
            public _ValueTupleEqualityComparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2, IEqualityComparer<T3> comparer3)
            {
                Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
                Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
                Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
            }

            public override bool Equals((T1, T2, T3) x, (T1, T2, T3) y) => Comparer1.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2, y.Item2) && Comparer3.Equals(x.Item3, y.Item3);

            public override int GetHashCode((T1, T2, T3) obj) => HashCode.Combine(Comparer1.GetHashCode(obj.Item1), Comparer2.GetHashCode(obj.Item2), Comparer3.GetHashCode(obj.Item3));

            private readonly IEqualityComparer<T1> Comparer1;
            private readonly IEqualityComparer<T2> Comparer2;
            private readonly IEqualityComparer<T3> Comparer3;
        }

        public static IEqualityComparer<(T1, T2, T3, T4)> Create<T1, T2, T3, T4>(IEqualityComparer<T1> comparer1 = null, IEqualityComparer<T2> comparer2 = null, IEqualityComparer<T3> comparer3 = null, IEqualityComparer<T4> comparer4 = null) => new _ValueTupleEqualityComparer<T1, T2, T3, T4>(comparer1, comparer2, comparer3, comparer4);

        private sealed class _ValueTupleEqualityComparer<T1, T2, T3, T4> : EqualityComparer<(T1, T2, T3, T4)>
        {
            public _ValueTupleEqualityComparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2, IEqualityComparer<T3> comparer3, IEqualityComparer<T4> comparer4)
            {
                Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
                Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
                Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
                Comparer4 = comparer4 ?? EqualityComparer<T4>.Default;
            }

            public override bool Equals((T1, T2, T3, T4) x, (T1, T2, T3, T4) y) => Comparer1.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2, y.Item2) && Comparer3.Equals(x.Item3, y.Item3) && Comparer4.Equals(x.Item4, y.Item4);

            public override int GetHashCode((T1, T2, T3, T4) obj) => HashCode.Combine(Comparer1.GetHashCode(obj.Item1), Comparer2.GetHashCode(obj.Item2), Comparer3.GetHashCode(obj.Item3), Comparer4.GetHashCode(obj.Item4));

            private readonly IEqualityComparer<T1> Comparer1;
            private readonly IEqualityComparer<T2> Comparer2;
            private readonly IEqualityComparer<T3> Comparer3;
            private readonly IEqualityComparer<T4> Comparer4;
        }

        public static IEqualityComparer<(T1, T2, T3, T4, T5)> Create<T1, T2, T3, T4, T5>(IEqualityComparer<T1> comparer1 = null, IEqualityComparer<T2> comparer2 = null, IEqualityComparer<T3> comparer3 = null, IEqualityComparer<T4> comparer4 = null, IEqualityComparer<T5> comparer5 = null) => new _ValueTupleEqualityComparer<T1, T2, T3, T4, T5>(comparer1, comparer2, comparer3, comparer4, comparer5);

        private sealed class _ValueTupleEqualityComparer<T1, T2, T3, T4, T5> : EqualityComparer<(T1, T2, T3, T4, T5)>
        {
            public _ValueTupleEqualityComparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2, IEqualityComparer<T3> comparer3, IEqualityComparer<T4> comparer4, IEqualityComparer<T5> comparer5)
            {
                Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
                Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
                Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
                Comparer4 = comparer4 ?? EqualityComparer<T4>.Default;
                Comparer5 = comparer5 ?? EqualityComparer<T5>.Default;
            }

            public override bool Equals((T1, T2, T3, T4, T5) x, (T1, T2, T3, T4, T5) y) => Comparer1.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2, y.Item2) && Comparer3.Equals(x.Item3, y.Item3) && Comparer4.Equals(x.Item4, y.Item4) && Comparer5.Equals(x.Item5, y.Item5);

            public override int GetHashCode((T1, T2, T3, T4, T5) obj) => HashCode.Combine(Comparer1.GetHashCode(obj.Item1), Comparer2.GetHashCode(obj.Item2), Comparer3.GetHashCode(obj.Item3), Comparer4.GetHashCode(obj.Item4), Comparer5.GetHashCode(obj.Item5));

            private readonly IEqualityComparer<T1> Comparer1;
            private readonly IEqualityComparer<T2> Comparer2;
            private readonly IEqualityComparer<T3> Comparer3;
            private readonly IEqualityComparer<T4> Comparer4;
            private readonly IEqualityComparer<T5> Comparer5;
        }

        public static IEqualityComparer<(T1, T2, T3, T4, T5, T6)> Create<T1, T2, T3, T4, T5, T6>(IEqualityComparer<T1> comparer1 = null, IEqualityComparer<T2> comparer2 = null, IEqualityComparer<T3> comparer3 = null, IEqualityComparer<T4> comparer4 = null, IEqualityComparer<T5> comparer5 = null, IEqualityComparer<T6> comparer6 = null) => new _ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6);

        private sealed class _ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6> : EqualityComparer<(T1, T2, T3, T4, T5, T6)>
        {
            public _ValueTupleEqualityComparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2, IEqualityComparer<T3> comparer3, IEqualityComparer<T4> comparer4, IEqualityComparer<T5> comparer5, IEqualityComparer<T6> comparer6)
            {
                Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
                Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
                Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
                Comparer4 = comparer4 ?? EqualityComparer<T4>.Default;
                Comparer5 = comparer5 ?? EqualityComparer<T5>.Default;
                Comparer6 = comparer6 ?? EqualityComparer<T6>.Default;
            }

            public override bool Equals((T1, T2, T3, T4, T5, T6) x, (T1, T2, T3, T4, T5, T6) y) => Comparer1.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2, y.Item2) && Comparer3.Equals(x.Item3, y.Item3) && Comparer4.Equals(x.Item4, y.Item4) && Comparer5.Equals(x.Item5, y.Item5) && Comparer6.Equals(x.Item6, y.Item6);

            public override int GetHashCode((T1, T2, T3, T4, T5, T6) obj) => HashCode.Combine(Comparer1.GetHashCode(obj.Item1), Comparer2.GetHashCode(obj.Item2), Comparer3.GetHashCode(obj.Item3), Comparer4.GetHashCode(obj.Item4), Comparer5.GetHashCode(obj.Item5), Comparer6.GetHashCode(obj.Item6));

            private readonly IEqualityComparer<T1> Comparer1;
            private readonly IEqualityComparer<T2> Comparer2;
            private readonly IEqualityComparer<T3> Comparer3;
            private readonly IEqualityComparer<T4> Comparer4;
            private readonly IEqualityComparer<T5> Comparer5;
            private readonly IEqualityComparer<T6> Comparer6;
        }

        public static IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)> Create<T1, T2, T3, T4, T5, T6, T7>(IEqualityComparer<T1> comparer1 = null, IEqualityComparer<T2> comparer2 = null, IEqualityComparer<T3> comparer3 = null, IEqualityComparer<T4> comparer4 = null, IEqualityComparer<T5> comparer5 = null, IEqualityComparer<T6> comparer6 = null, IEqualityComparer<T7> comparer7 = null) => new _ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6, comparer7);

        private sealed class _ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7> : EqualityComparer<(T1, T2, T3, T4, T5, T6, T7)>
        {
            public _ValueTupleEqualityComparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2, IEqualityComparer<T3> comparer3, IEqualityComparer<T4> comparer4, IEqualityComparer<T5> comparer5, IEqualityComparer<T6> comparer6, IEqualityComparer<T7> comparer7)
            {
                Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
                Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
                Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
                Comparer4 = comparer4 ?? EqualityComparer<T4>.Default;
                Comparer5 = comparer5 ?? EqualityComparer<T5>.Default;
                Comparer6 = comparer6 ?? EqualityComparer<T6>.Default;
                Comparer7 = comparer7 ?? EqualityComparer<T7>.Default;
            }

            public override bool Equals((T1, T2, T3, T4, T5, T6, T7) x, (T1, T2, T3, T4, T5, T6, T7) y) => Comparer1.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2, y.Item2) && Comparer3.Equals(x.Item3, y.Item3) && Comparer4.Equals(x.Item4, y.Item4) && Comparer5.Equals(x.Item5, y.Item5) && Comparer6.Equals(x.Item6, y.Item6) && Comparer7.Equals(x.Item7, y.Item7);

            public override int GetHashCode((T1, T2, T3, T4, T5, T6, T7) obj) => HashCode.Combine(Comparer1.GetHashCode(obj.Item1), Comparer2.GetHashCode(obj.Item2), Comparer3.GetHashCode(obj.Item3), Comparer4.GetHashCode(obj.Item4), Comparer5.GetHashCode(obj.Item5), Comparer6.GetHashCode(obj.Item6), Comparer7.GetHashCode(obj.Item7));

            private readonly IEqualityComparer<T1> Comparer1;
            private readonly IEqualityComparer<T2> Comparer2;
            private readonly IEqualityComparer<T3> Comparer3;
            private readonly IEqualityComparer<T4> Comparer4;
            private readonly IEqualityComparer<T5> Comparer5;
            private readonly IEqualityComparer<T6> Comparer6;
            private readonly IEqualityComparer<T7> Comparer7;
        }

        public static IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8)> Create<T1, T2, T3, T4, T5, T6, T7, T8>(IEqualityComparer<T1> comparer1 = null, IEqualityComparer<T2> comparer2 = null, IEqualityComparer<T3> comparer3 = null, IEqualityComparer<T4> comparer4 = null, IEqualityComparer<T5> comparer5 = null, IEqualityComparer<T6> comparer6 = null, IEqualityComparer<T7> comparer7 = null, IEqualityComparer<T8> comparer8 = null) => new _ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6, comparer7, comparer8);

        private sealed class _ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8> : EqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8)>
        {
            public _ValueTupleEqualityComparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2, IEqualityComparer<T3> comparer3, IEqualityComparer<T4> comparer4, IEqualityComparer<T5> comparer5, IEqualityComparer<T6> comparer6, IEqualityComparer<T7> comparer7, IEqualityComparer<T8> comparer8)
            {
                Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
                Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
                Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
                Comparer4 = comparer4 ?? EqualityComparer<T4>.Default;
                Comparer5 = comparer5 ?? EqualityComparer<T5>.Default;
                Comparer6 = comparer6 ?? EqualityComparer<T6>.Default;
                Comparer7 = comparer7 ?? EqualityComparer<T7>.Default;
                Comparer8 = comparer8 ?? EqualityComparer<T8>.Default;
            }

            public override bool Equals((T1, T2, T3, T4, T5, T6, T7, T8) x, (T1, T2, T3, T4, T5, T6, T7, T8) y) => Comparer1.Equals(x.Item1, y.Item1) && Comparer2.Equals(x.Item2, y.Item2) && Comparer3.Equals(x.Item3, y.Item3) && Comparer4.Equals(x.Item4, y.Item4) && Comparer5.Equals(x.Item5, y.Item5) && Comparer6.Equals(x.Item6, y.Item6) && Comparer7.Equals(x.Item7, y.Item7) && Comparer8.Equals(x.Item8, y.Item8);

            public override int GetHashCode((T1, T2, T3, T4, T5, T6, T7, T8) obj) => HashCode.Combine(Comparer1.GetHashCode(obj.Item1), Comparer2.GetHashCode(obj.Item2), Comparer3.GetHashCode(obj.Item3), Comparer4.GetHashCode(obj.Item4), Comparer5.GetHashCode(obj.Item5), Comparer6.GetHashCode(obj.Item6), Comparer7.GetHashCode(obj.Item7), Comparer8.GetHashCode(obj.Item8));

            private readonly IEqualityComparer<T1> Comparer1;
            private readonly IEqualityComparer<T2> Comparer2;
            private readonly IEqualityComparer<T3> Comparer3;
            private readonly IEqualityComparer<T4> Comparer4;
            private readonly IEqualityComparer<T5> Comparer5;
            private readonly IEqualityComparer<T6> Comparer6;
            private readonly IEqualityComparer<T7> Comparer7;
            private readonly IEqualityComparer<T8> Comparer8;
        }

    }
}
