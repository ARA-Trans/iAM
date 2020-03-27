// WARNING: This file was automatically generated from a T4 text template at the
// following moment in time: 03/27/2020 18:00:56 -05:00. Any changes you make to
// this file will be lost when this file is regenerated from the template
// source.

using System.Collections.Generic;
using System.Linq;
using static AppliedResearchAssociates.Static;

namespace AppliedResearchAssociates
{
    partial struct HashCode
    {
        public static IEnumerable<(T1, T2)> Long<T1, T2>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, T1 default1 = default, T2 default2 = default)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            {
                bool hasCurrent1, hasCurrent2;
                while ((hasCurrent1 = enumerator1.MoveNext()) | (hasCurrent2 = enumerator2.MoveNext()))
                {
                    yield return (hasCurrent1 ? enumerator1.Current : default1, hasCurrent2 ? enumerator2.Current : default2);
                }
            }
        }

        public static IEnumerable<(T1, T2)> Short<T1, T2>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            {
                while (enumerator1.MoveNext() & enumerator2.MoveNext())
                {
                    yield return (enumerator1.Current, enumerator2.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2)> Strict<T1, T2>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            {
                while (Distinct(enumerator1.MoveNext(), enumerator2.MoveNext()).Single())
                {
                    yield return (enumerator1.Current, enumerator2.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3)> Long<T1, T2, T3>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, T1 default1 = default, T2 default2 = default, T3 default3 = default)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            {
                bool hasCurrent1, hasCurrent2, hasCurrent3;
                while ((hasCurrent1 = enumerator1.MoveNext()) | (hasCurrent2 = enumerator2.MoveNext()) | (hasCurrent3 = enumerator3.MoveNext()))
                {
                    yield return (hasCurrent1 ? enumerator1.Current : default1, hasCurrent2 ? enumerator2.Current : default2, hasCurrent3 ? enumerator3.Current : default3);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3)> Short<T1, T2, T3>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            {
                while (enumerator1.MoveNext() & enumerator2.MoveNext() & enumerator3.MoveNext())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3)> Strict<T1, T2, T3>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            {
                while (Distinct(enumerator1.MoveNext(), enumerator2.MoveNext(), enumerator3.MoveNext()).Single())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4)> Long<T1, T2, T3, T4>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, T1 default1 = default, T2 default2 = default, T3 default3 = default, T4 default4 = default)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            {
                bool hasCurrent1, hasCurrent2, hasCurrent3, hasCurrent4;
                while ((hasCurrent1 = enumerator1.MoveNext()) | (hasCurrent2 = enumerator2.MoveNext()) | (hasCurrent3 = enumerator3.MoveNext()) | (hasCurrent4 = enumerator4.MoveNext()))
                {
                    yield return (hasCurrent1 ? enumerator1.Current : default1, hasCurrent2 ? enumerator2.Current : default2, hasCurrent3 ? enumerator3.Current : default3, hasCurrent4 ? enumerator4.Current : default4);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4)> Short<T1, T2, T3, T4>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            {
                while (enumerator1.MoveNext() & enumerator2.MoveNext() & enumerator3.MoveNext() & enumerator4.MoveNext())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4)> Strict<T1, T2, T3, T4>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            {
                while (Distinct(enumerator1.MoveNext(), enumerator2.MoveNext(), enumerator3.MoveNext(), enumerator4.MoveNext()).Single())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5)> Long<T1, T2, T3, T4, T5>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, T1 default1 = default, T2 default2 = default, T3 default3 = default, T4 default4 = default, T5 default5 = default)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            {
                bool hasCurrent1, hasCurrent2, hasCurrent3, hasCurrent4, hasCurrent5;
                while ((hasCurrent1 = enumerator1.MoveNext()) | (hasCurrent2 = enumerator2.MoveNext()) | (hasCurrent3 = enumerator3.MoveNext()) | (hasCurrent4 = enumerator4.MoveNext()) | (hasCurrent5 = enumerator5.MoveNext()))
                {
                    yield return (hasCurrent1 ? enumerator1.Current : default1, hasCurrent2 ? enumerator2.Current : default2, hasCurrent3 ? enumerator3.Current : default3, hasCurrent4 ? enumerator4.Current : default4, hasCurrent5 ? enumerator5.Current : default5);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5)> Short<T1, T2, T3, T4, T5>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            {
                while (enumerator1.MoveNext() & enumerator2.MoveNext() & enumerator3.MoveNext() & enumerator4.MoveNext() & enumerator5.MoveNext())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5)> Strict<T1, T2, T3, T4, T5>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            {
                while (Distinct(enumerator1.MoveNext(), enumerator2.MoveNext(), enumerator3.MoveNext(), enumerator4.MoveNext(), enumerator5.MoveNext()).Single())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6)> Long<T1, T2, T3, T4, T5, T6>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6, T1 default1 = default, T2 default2 = default, T3 default3 = default, T4 default4 = default, T5 default5 = default, T6 default6 = default)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            {
                bool hasCurrent1, hasCurrent2, hasCurrent3, hasCurrent4, hasCurrent5, hasCurrent6;
                while ((hasCurrent1 = enumerator1.MoveNext()) | (hasCurrent2 = enumerator2.MoveNext()) | (hasCurrent3 = enumerator3.MoveNext()) | (hasCurrent4 = enumerator4.MoveNext()) | (hasCurrent5 = enumerator5.MoveNext()) | (hasCurrent6 = enumerator6.MoveNext()))
                {
                    yield return (hasCurrent1 ? enumerator1.Current : default1, hasCurrent2 ? enumerator2.Current : default2, hasCurrent3 ? enumerator3.Current : default3, hasCurrent4 ? enumerator4.Current : default4, hasCurrent5 ? enumerator5.Current : default5, hasCurrent6 ? enumerator6.Current : default6);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6)> Short<T1, T2, T3, T4, T5, T6>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            {
                while (enumerator1.MoveNext() & enumerator2.MoveNext() & enumerator3.MoveNext() & enumerator4.MoveNext() & enumerator5.MoveNext() & enumerator6.MoveNext())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current, enumerator6.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6)> Strict<T1, T2, T3, T4, T5, T6>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            {
                while (Distinct(enumerator1.MoveNext(), enumerator2.MoveNext(), enumerator3.MoveNext(), enumerator4.MoveNext(), enumerator5.MoveNext(), enumerator6.MoveNext()).Single())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current, enumerator6.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> Long<T1, T2, T3, T4, T5, T6, T7>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6, IEnumerable<T7> sequence7, T1 default1 = default, T2 default2 = default, T3 default3 = default, T4 default4 = default, T5 default5 = default, T6 default6 = default, T7 default7 = default)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            using (var enumerator7 = sequence7.GetEnumerator())
            {
                bool hasCurrent1, hasCurrent2, hasCurrent3, hasCurrent4, hasCurrent5, hasCurrent6, hasCurrent7;
                while ((hasCurrent1 = enumerator1.MoveNext()) | (hasCurrent2 = enumerator2.MoveNext()) | (hasCurrent3 = enumerator3.MoveNext()) | (hasCurrent4 = enumerator4.MoveNext()) | (hasCurrent5 = enumerator5.MoveNext()) | (hasCurrent6 = enumerator6.MoveNext()) | (hasCurrent7 = enumerator7.MoveNext()))
                {
                    yield return (hasCurrent1 ? enumerator1.Current : default1, hasCurrent2 ? enumerator2.Current : default2, hasCurrent3 ? enumerator3.Current : default3, hasCurrent4 ? enumerator4.Current : default4, hasCurrent5 ? enumerator5.Current : default5, hasCurrent6 ? enumerator6.Current : default6, hasCurrent7 ? enumerator7.Current : default7);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> Short<T1, T2, T3, T4, T5, T6, T7>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6, IEnumerable<T7> sequence7)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            using (var enumerator7 = sequence7.GetEnumerator())
            {
                while (enumerator1.MoveNext() & enumerator2.MoveNext() & enumerator3.MoveNext() & enumerator4.MoveNext() & enumerator5.MoveNext() & enumerator6.MoveNext() & enumerator7.MoveNext())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current, enumerator6.Current, enumerator7.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> Strict<T1, T2, T3, T4, T5, T6, T7>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6, IEnumerable<T7> sequence7)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            using (var enumerator7 = sequence7.GetEnumerator())
            {
                while (Distinct(enumerator1.MoveNext(), enumerator2.MoveNext(), enumerator3.MoveNext(), enumerator4.MoveNext(), enumerator5.MoveNext(), enumerator6.MoveNext(), enumerator7.MoveNext()).Single())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current, enumerator6.Current, enumerator7.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6, T7, T8)> Long<T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6, IEnumerable<T7> sequence7, IEnumerable<T8> sequence8, T1 default1 = default, T2 default2 = default, T3 default3 = default, T4 default4 = default, T5 default5 = default, T6 default6 = default, T7 default7 = default, T8 default8 = default)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            using (var enumerator7 = sequence7.GetEnumerator())
            using (var enumerator8 = sequence8.GetEnumerator())
            {
                bool hasCurrent1, hasCurrent2, hasCurrent3, hasCurrent4, hasCurrent5, hasCurrent6, hasCurrent7, hasCurrent8;
                while ((hasCurrent1 = enumerator1.MoveNext()) | (hasCurrent2 = enumerator2.MoveNext()) | (hasCurrent3 = enumerator3.MoveNext()) | (hasCurrent4 = enumerator4.MoveNext()) | (hasCurrent5 = enumerator5.MoveNext()) | (hasCurrent6 = enumerator6.MoveNext()) | (hasCurrent7 = enumerator7.MoveNext()) | (hasCurrent8 = enumerator8.MoveNext()))
                {
                    yield return (hasCurrent1 ? enumerator1.Current : default1, hasCurrent2 ? enumerator2.Current : default2, hasCurrent3 ? enumerator3.Current : default3, hasCurrent4 ? enumerator4.Current : default4, hasCurrent5 ? enumerator5.Current : default5, hasCurrent6 ? enumerator6.Current : default6, hasCurrent7 ? enumerator7.Current : default7, hasCurrent8 ? enumerator8.Current : default8);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6, T7, T8)> Short<T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6, IEnumerable<T7> sequence7, IEnumerable<T8> sequence8)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            using (var enumerator7 = sequence7.GetEnumerator())
            using (var enumerator8 = sequence8.GetEnumerator())
            {
                while (enumerator1.MoveNext() & enumerator2.MoveNext() & enumerator3.MoveNext() & enumerator4.MoveNext() & enumerator5.MoveNext() & enumerator6.MoveNext() & enumerator7.MoveNext() & enumerator8.MoveNext())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current, enumerator6.Current, enumerator7.Current, enumerator8.Current);
                }
            }
        }

        public static IEnumerable<(T1, T2, T3, T4, T5, T6, T7, T8)> Strict<T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<T1> sequence1, IEnumerable<T2> sequence2, IEnumerable<T3> sequence3, IEnumerable<T4> sequence4, IEnumerable<T5> sequence5, IEnumerable<T6> sequence6, IEnumerable<T7> sequence7, IEnumerable<T8> sequence8)
        {
            using (var enumerator1 = sequence1.GetEnumerator())
            using (var enumerator2 = sequence2.GetEnumerator())
            using (var enumerator3 = sequence3.GetEnumerator())
            using (var enumerator4 = sequence4.GetEnumerator())
            using (var enumerator5 = sequence5.GetEnumerator())
            using (var enumerator6 = sequence6.GetEnumerator())
            using (var enumerator7 = sequence7.GetEnumerator())
            using (var enumerator8 = sequence8.GetEnumerator())
            {
                while (Distinct(enumerator1.MoveNext(), enumerator2.MoveNext(), enumerator3.MoveNext(), enumerator4.MoveNext(), enumerator5.MoveNext(), enumerator6.MoveNext(), enumerator7.MoveNext(), enumerator8.MoveNext()).Single())
                {
                    yield return (enumerator1.Current, enumerator2.Current, enumerator3.Current, enumerator4.Current, enumerator5.Current, enumerator6.Current, enumerator7.Current, enumerator8.Current);
                }
            }
        }

    }
}
