// WARNING: This file was automatically generated from a T4 text template at the
// following moment in time: 05/11/2020 11:21:48 -05:00. Any changes you make to
// this file will be lost when this file is regenerated from the template
// source.

namespace AppliedResearchAssociates
{
    public static class ValueGetter
    {
        public static ValueGetter<T> Of<T>(T value) => new ValueGetter<T>(value);
    }

    public class ValueGetter<T>
    {
        public T Value { get; }

        public ValueGetter(T value) => Value = value;

        public T Delegate() => Value;

        public T Delegate<T1>(T1 value1) => Value;

        public T Delegate<T1, T2>(T1 value1, T2 value2) => Value;

        public T Delegate<T1, T2, T3>(T1 value1, T2 value2, T3 value3) => Value;

        public T Delegate<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) => Value;

        public T Delegate<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => Value;

        public T Delegate<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => Value;

        public T Delegate<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => Value;

        public T Delegate<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => Value;
    }
}
