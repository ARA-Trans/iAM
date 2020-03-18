// WARNING: This file was automatically generated from a T4 text template at the
// following moment in time: 03/18/2020 16:47:49 -05:00. Any changes you make to
// this file will be lost when this file is regenerated from the template
// source.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    [KnownType(nameof(GetKnownTypes))]
    public abstract class Choice<T1, T2> : IEquatable<Choice<T1, T2>>
    {
        private static IEnumerable<Type> GetKnownTypes() => new[] { typeof(Choice1), typeof(Choice2) };

        private Choice()
        {
        }

        public static Choice<T1, T2> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2> Of(T2 value) => value != null ? new Choice2(value) : null;

        [IgnoreDataMember]
        public virtual T1 AsT1 => default;

        [IgnoreDataMember]
        public virtual T2 AsT2 => default;

        [IgnoreDataMember]
        public virtual bool IsT1 => false;

        [IgnoreDataMember]
        public virtual bool IsT2 => false;

        public abstract object Value { get; }

        public abstract void Match(Action<T1> handle1, Action<T2> handle2);

        public abstract U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2);

        public abstract bool Equals(Choice<T1, T2> choice);

        public static bool operator ==(Choice<T1, T2> choice1, Choice<T1, T2> choice2) => EqualityComparer<Choice<T1, T2>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2> choice1, Choice<T1, T2> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2> choice) => choice.Match(_ => _, _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2> choice) => choice.Match(_ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2>(T2 value) => Of(value);

        private class Choice1 : Choice<T1, T2>
        {
            private readonly T1 _value;

            public Choice1(T1 value) => _value = value;

            [IgnoreDataMember]
            public override T1 AsT1 => _value;

            [IgnoreDataMember]
            public override bool IsT1 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));

                handle1(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));

                return handle1(_value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice2 : Choice<T1, T2>
        {
            private readonly T2 _value;

            public Choice2(T2 value) => _value = value;

            [IgnoreDataMember]
            public override T2 AsT2 => _value;

            [IgnoreDataMember]
            public override bool IsT2 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));

                handle2(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));

                return handle2(_value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }
    }

    [KnownType(nameof(GetKnownTypes))]
    public abstract class Choice<T1, T2, T3> : IEquatable<Choice<T1, T2, T3>>
    {
        private static IEnumerable<Type> GetKnownTypes() => new[] { typeof(Choice1), typeof(Choice2), typeof(Choice3) };

        private Choice()
        {
        }

        public static Choice<T1, T2, T3> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3> Of(T3 value) => value != null ? new Choice3(value) : null;

        [IgnoreDataMember]
        public virtual T1 AsT1 => default;

        [IgnoreDataMember]
        public virtual T2 AsT2 => default;

        [IgnoreDataMember]
        public virtual T3 AsT3 => default;

        [IgnoreDataMember]
        public virtual bool IsT1 => false;

        [IgnoreDataMember]
        public virtual bool IsT2 => false;

        [IgnoreDataMember]
        public virtual bool IsT3 => false;

        public abstract object Value { get; }

        public abstract void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3);

        public abstract U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3);

        public abstract bool Equals(Choice<T1, T2, T3> choice);

        public static bool operator ==(Choice<T1, T2, T3> choice1, Choice<T1, T2, T3> choice2) => EqualityComparer<Choice<T1, T2, T3>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3> choice1, Choice<T1, T2, T3> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3> choice) => choice.Match(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3> choice) => choice.Match(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3>(T3 value) => Of(value);

        private class Choice1 : Choice<T1, T2, T3>
        {
            private readonly T1 _value;

            public Choice1(T1 value) => _value = value;

            [IgnoreDataMember]
            public override T1 AsT1 => _value;

            [IgnoreDataMember]
            public override bool IsT1 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));

                handle1(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));

                return handle1(_value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice2 : Choice<T1, T2, T3>
        {
            private readonly T2 _value;

            public Choice2(T2 value) => _value = value;

            [IgnoreDataMember]
            public override T2 AsT2 => _value;

            [IgnoreDataMember]
            public override bool IsT2 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));

                handle2(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));

                return handle2(_value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice3 : Choice<T1, T2, T3>
        {
            private readonly T3 _value;

            public Choice3(T3 value) => _value = value;

            [IgnoreDataMember]
            public override T3 AsT3 => _value;

            [IgnoreDataMember]
            public override bool IsT3 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));

                handle3(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));

                return handle3(_value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }
    }

    [KnownType(nameof(GetKnownTypes))]
    public abstract class Choice<T1, T2, T3, T4> : IEquatable<Choice<T1, T2, T3, T4>>
    {
        private static IEnumerable<Type> GetKnownTypes() => new[] { typeof(Choice1), typeof(Choice2), typeof(Choice3), typeof(Choice4) };

        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4> Of(T4 value) => value != null ? new Choice4(value) : null;

        [IgnoreDataMember]
        public virtual T1 AsT1 => default;

        [IgnoreDataMember]
        public virtual T2 AsT2 => default;

        [IgnoreDataMember]
        public virtual T3 AsT3 => default;

        [IgnoreDataMember]
        public virtual T4 AsT4 => default;

        [IgnoreDataMember]
        public virtual bool IsT1 => false;

        [IgnoreDataMember]
        public virtual bool IsT2 => false;

        [IgnoreDataMember]
        public virtual bool IsT3 => false;

        [IgnoreDataMember]
        public virtual bool IsT4 => false;

        public abstract object Value { get; }

        public abstract void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4);

        public abstract U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4);

        public abstract bool Equals(Choice<T1, T2, T3, T4> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4> choice1, Choice<T1, T2, T3, T4> choice2) => EqualityComparer<Choice<T1, T2, T3, T4>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4> choice1, Choice<T1, T2, T3, T4> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4> choice) => choice.Match(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4> choice) => choice.Match(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4>(T4 value) => Of(value);

        private class Choice1 : Choice<T1, T2, T3, T4>
        {
            private readonly T1 _value;

            public Choice1(T1 value) => _value = value;

            [IgnoreDataMember]
            public override T1 AsT1 => _value;

            [IgnoreDataMember]
            public override bool IsT1 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                handle1(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                return handle1(_value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice2 : Choice<T1, T2, T3, T4>
        {
            private readonly T2 _value;

            public Choice2(T2 value) => _value = value;

            [IgnoreDataMember]
            public override T2 AsT2 => _value;

            [IgnoreDataMember]
            public override bool IsT2 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                handle2(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                return handle2(_value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice3 : Choice<T1, T2, T3, T4>
        {
            private readonly T3 _value;

            public Choice3(T3 value) => _value = value;

            [IgnoreDataMember]
            public override T3 AsT3 => _value;

            [IgnoreDataMember]
            public override bool IsT3 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                handle3(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                return handle3(_value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice4 : Choice<T1, T2, T3, T4>
        {
            private readonly T4 _value;

            public Choice4(T4 value) => _value = value;

            [IgnoreDataMember]
            public override T4 AsT4 => _value;

            [IgnoreDataMember]
            public override bool IsT4 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                handle4(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));

                return handle4(_value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }
    }

    [KnownType(nameof(GetKnownTypes))]
    public abstract class Choice<T1, T2, T3, T4, T5> : IEquatable<Choice<T1, T2, T3, T4, T5>>
    {
        private static IEnumerable<Type> GetKnownTypes() => new[] { typeof(Choice1), typeof(Choice2), typeof(Choice3), typeof(Choice4), typeof(Choice5) };

        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4, T5> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T4 value) => value != null ? new Choice4(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T5 value) => value != null ? new Choice5(value) : null;

        [IgnoreDataMember]
        public virtual T1 AsT1 => default;

        [IgnoreDataMember]
        public virtual T2 AsT2 => default;

        [IgnoreDataMember]
        public virtual T3 AsT3 => default;

        [IgnoreDataMember]
        public virtual T4 AsT4 => default;

        [IgnoreDataMember]
        public virtual T5 AsT5 => default;

        [IgnoreDataMember]
        public virtual bool IsT1 => false;

        [IgnoreDataMember]
        public virtual bool IsT2 => false;

        [IgnoreDataMember]
        public virtual bool IsT3 => false;

        [IgnoreDataMember]
        public virtual bool IsT4 => false;

        [IgnoreDataMember]
        public virtual bool IsT5 => false;

        public abstract object Value { get; }

        public abstract void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5);

        public abstract U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5> choice1, Choice<T1, T2, T3, T4, T5> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5> choice1, Choice<T1, T2, T3, T4, T5> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5> choice) => choice.Match(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5> choice) => choice.Match(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T5 value) => Of(value);

        private class Choice1 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T1 _value;

            public Choice1(T1 value) => _value = value;

            [IgnoreDataMember]
            public override T1 AsT1 => _value;

            [IgnoreDataMember]
            public override bool IsT1 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                handle1(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                return handle1(_value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice2 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T2 _value;

            public Choice2(T2 value) => _value = value;

            [IgnoreDataMember]
            public override T2 AsT2 => _value;

            [IgnoreDataMember]
            public override bool IsT2 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                handle2(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                return handle2(_value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice3 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T3 _value;

            public Choice3(T3 value) => _value = value;

            [IgnoreDataMember]
            public override T3 AsT3 => _value;

            [IgnoreDataMember]
            public override bool IsT3 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                handle3(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                return handle3(_value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice4 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T4 _value;

            public Choice4(T4 value) => _value = value;

            [IgnoreDataMember]
            public override T4 AsT4 => _value;

            [IgnoreDataMember]
            public override bool IsT4 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                handle4(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                return handle4(_value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice5 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T5 _value;

            public Choice5(T5 value) => _value = value;

            [IgnoreDataMember]
            public override T5 AsT5 => _value;

            [IgnoreDataMember]
            public override bool IsT5 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                handle5(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));

                return handle5(_value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }
    }

    [KnownType(nameof(GetKnownTypes))]
    public abstract class Choice<T1, T2, T3, T4, T5, T6> : IEquatable<Choice<T1, T2, T3, T4, T5, T6>>
    {
        private static IEnumerable<Type> GetKnownTypes() => new[] { typeof(Choice1), typeof(Choice2), typeof(Choice3), typeof(Choice4), typeof(Choice5), typeof(Choice6) };

        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T4 value) => value != null ? new Choice4(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T5 value) => value != null ? new Choice5(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T6 value) => value != null ? new Choice6(value) : null;

        [IgnoreDataMember]
        public virtual T1 AsT1 => default;

        [IgnoreDataMember]
        public virtual T2 AsT2 => default;

        [IgnoreDataMember]
        public virtual T3 AsT3 => default;

        [IgnoreDataMember]
        public virtual T4 AsT4 => default;

        [IgnoreDataMember]
        public virtual T5 AsT5 => default;

        [IgnoreDataMember]
        public virtual T6 AsT6 => default;

        [IgnoreDataMember]
        public virtual bool IsT1 => false;

        [IgnoreDataMember]
        public virtual bool IsT2 => false;

        [IgnoreDataMember]
        public virtual bool IsT3 => false;

        [IgnoreDataMember]
        public virtual bool IsT4 => false;

        [IgnoreDataMember]
        public virtual bool IsT5 => false;

        [IgnoreDataMember]
        public virtual bool IsT6 => false;

        public abstract object Value { get; }

        public abstract void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6);

        public abstract U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5, T6> choice1, Choice<T1, T2, T3, T4, T5, T6> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5, T6>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5, T6> choice1, Choice<T1, T2, T3, T4, T5, T6> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Match(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Match(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T6(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T5 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T6 value) => Of(value);

        private class Choice1 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T1 _value;

            public Choice1(T1 value) => _value = value;

            [IgnoreDataMember]
            public override T1 AsT1 => _value;

            [IgnoreDataMember]
            public override bool IsT1 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                handle1(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                return handle1(_value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice2 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T2 _value;

            public Choice2(T2 value) => _value = value;

            [IgnoreDataMember]
            public override T2 AsT2 => _value;

            [IgnoreDataMember]
            public override bool IsT2 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                handle2(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                return handle2(_value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice3 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T3 _value;

            public Choice3(T3 value) => _value = value;

            [IgnoreDataMember]
            public override T3 AsT3 => _value;

            [IgnoreDataMember]
            public override bool IsT3 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                handle3(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                return handle3(_value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice4 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T4 _value;

            public Choice4(T4 value) => _value = value;

            [IgnoreDataMember]
            public override T4 AsT4 => _value;

            [IgnoreDataMember]
            public override bool IsT4 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                handle4(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                return handle4(_value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice5 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T5 _value;

            public Choice5(T5 value) => _value = value;

            [IgnoreDataMember]
            public override T5 AsT5 => _value;

            [IgnoreDataMember]
            public override bool IsT5 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                handle5(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                return handle5(_value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice6 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T6 _value;

            public Choice6(T6 value) => _value = value;

            [IgnoreDataMember]
            public override T6 AsT6 => _value;

            [IgnoreDataMember]
            public override bool IsT6 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                handle6(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));

                return handle6(_value);
            }

            private bool Equals(Choice6 choice) => EqualityComparer<T6>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice6 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice6 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }
    }

    [KnownType(nameof(GetKnownTypes))]
    public abstract class Choice<T1, T2, T3, T4, T5, T6, T7> : IEquatable<Choice<T1, T2, T3, T4, T5, T6, T7>>
    {
        private static IEnumerable<Type> GetKnownTypes() => new[] { typeof(Choice1), typeof(Choice2), typeof(Choice3), typeof(Choice4), typeof(Choice5), typeof(Choice6), typeof(Choice7) };

        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4, T5, T6, T7> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7> Of(T4 value) => value != null ? new Choice4(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7> Of(T5 value) => value != null ? new Choice5(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7> Of(T6 value) => value != null ? new Choice6(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7> Of(T7 value) => value != null ? new Choice7(value) : null;

        [IgnoreDataMember]
        public virtual T1 AsT1 => default;

        [IgnoreDataMember]
        public virtual T2 AsT2 => default;

        [IgnoreDataMember]
        public virtual T3 AsT3 => default;

        [IgnoreDataMember]
        public virtual T4 AsT4 => default;

        [IgnoreDataMember]
        public virtual T5 AsT5 => default;

        [IgnoreDataMember]
        public virtual T6 AsT6 => default;

        [IgnoreDataMember]
        public virtual T7 AsT7 => default;

        [IgnoreDataMember]
        public virtual bool IsT1 => false;

        [IgnoreDataMember]
        public virtual bool IsT2 => false;

        [IgnoreDataMember]
        public virtual bool IsT3 => false;

        [IgnoreDataMember]
        public virtual bool IsT4 => false;

        [IgnoreDataMember]
        public virtual bool IsT5 => false;

        [IgnoreDataMember]
        public virtual bool IsT6 => false;

        [IgnoreDataMember]
        public virtual bool IsT7 => false;

        public abstract object Value { get; }

        public abstract void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7);

        public abstract U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5, T6, T7> choice1, Choice<T1, T2, T3, T4, T5, T6, T7> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5, T6, T7>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5, T6, T7> choice1, Choice<T1, T2, T3, T4, T5, T6, T7> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Match(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Match(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T6(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T7(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T5 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T6 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T7 value) => Of(value);

        private class Choice1 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T1 _value;

            public Choice1(T1 value) => _value = value;

            [IgnoreDataMember]
            public override T1 AsT1 => _value;

            [IgnoreDataMember]
            public override bool IsT1 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                handle1(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                return handle1(_value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice2 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T2 _value;

            public Choice2(T2 value) => _value = value;

            [IgnoreDataMember]
            public override T2 AsT2 => _value;

            [IgnoreDataMember]
            public override bool IsT2 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                handle2(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                return handle2(_value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice3 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T3 _value;

            public Choice3(T3 value) => _value = value;

            [IgnoreDataMember]
            public override T3 AsT3 => _value;

            [IgnoreDataMember]
            public override bool IsT3 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                handle3(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                return handle3(_value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice4 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T4 _value;

            public Choice4(T4 value) => _value = value;

            [IgnoreDataMember]
            public override T4 AsT4 => _value;

            [IgnoreDataMember]
            public override bool IsT4 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                handle4(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                return handle4(_value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice5 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T5 _value;

            public Choice5(T5 value) => _value = value;

            [IgnoreDataMember]
            public override T5 AsT5 => _value;

            [IgnoreDataMember]
            public override bool IsT5 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                handle5(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                return handle5(_value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice6 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T6 _value;

            public Choice6(T6 value) => _value = value;

            [IgnoreDataMember]
            public override T6 AsT6 => _value;

            [IgnoreDataMember]
            public override bool IsT6 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                handle6(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                return handle6(_value);
            }

            private bool Equals(Choice6 choice) => EqualityComparer<T6>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice6 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice6 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice7 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T7 _value;

            public Choice7(T7 value) => _value = value;

            [IgnoreDataMember]
            public override T7 AsT7 => _value;

            [IgnoreDataMember]
            public override bool IsT7 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                handle7(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));

                return handle7(_value);
            }

            private bool Equals(Choice7 choice) => EqualityComparer<T7>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice7 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice7 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }
    }

    [KnownType(nameof(GetKnownTypes))]
    public abstract class Choice<T1, T2, T3, T4, T5, T6, T7, T8> : IEquatable<Choice<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        private static IEnumerable<Type> GetKnownTypes() => new[] { typeof(Choice1), typeof(Choice2), typeof(Choice3), typeof(Choice4), typeof(Choice5), typeof(Choice6), typeof(Choice7), typeof(Choice8) };

        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T4 value) => value != null ? new Choice4(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T5 value) => value != null ? new Choice5(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T6 value) => value != null ? new Choice6(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T7 value) => value != null ? new Choice7(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Of(T8 value) => value != null ? new Choice8(value) : null;

        [IgnoreDataMember]
        public virtual T1 AsT1 => default;

        [IgnoreDataMember]
        public virtual T2 AsT2 => default;

        [IgnoreDataMember]
        public virtual T3 AsT3 => default;

        [IgnoreDataMember]
        public virtual T4 AsT4 => default;

        [IgnoreDataMember]
        public virtual T5 AsT5 => default;

        [IgnoreDataMember]
        public virtual T6 AsT6 => default;

        [IgnoreDataMember]
        public virtual T7 AsT7 => default;

        [IgnoreDataMember]
        public virtual T8 AsT8 => default;

        [IgnoreDataMember]
        public virtual bool IsT1 => false;

        [IgnoreDataMember]
        public virtual bool IsT2 => false;

        [IgnoreDataMember]
        public virtual bool IsT3 => false;

        [IgnoreDataMember]
        public virtual bool IsT4 => false;

        [IgnoreDataMember]
        public virtual bool IsT5 => false;

        [IgnoreDataMember]
        public virtual bool IsT6 => false;

        [IgnoreDataMember]
        public virtual bool IsT7 => false;

        [IgnoreDataMember]
        public virtual bool IsT8 => false;

        public abstract object Value { get; }

        public abstract void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8);

        public abstract U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice1, Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5, T6, T7, T8>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice1, Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T6(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T7(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T8(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Match(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => Of(value);

        private class Choice1 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T1 _value;

            public Choice1(T1 value) => _value = value;

            [IgnoreDataMember]
            public override T1 AsT1 => _value;

            [IgnoreDataMember]
            public override bool IsT1 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle1(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle1(_value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice2 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T2 _value;

            public Choice2(T2 value) => _value = value;

            [IgnoreDataMember]
            public override T2 AsT2 => _value;

            [IgnoreDataMember]
            public override bool IsT2 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle2(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle2(_value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice3 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T3 _value;

            public Choice3(T3 value) => _value = value;

            [IgnoreDataMember]
            public override T3 AsT3 => _value;

            [IgnoreDataMember]
            public override bool IsT3 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle3(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle3(_value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice4 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T4 _value;

            public Choice4(T4 value) => _value = value;

            [IgnoreDataMember]
            public override T4 AsT4 => _value;

            [IgnoreDataMember]
            public override bool IsT4 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle4(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle4(_value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice5 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T5 _value;

            public Choice5(T5 value) => _value = value;

            [IgnoreDataMember]
            public override T5 AsT5 => _value;

            [IgnoreDataMember]
            public override bool IsT5 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle5(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle5(_value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice6 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T6 _value;

            public Choice6(T6 value) => _value = value;

            [IgnoreDataMember]
            public override T6 AsT6 => _value;

            [IgnoreDataMember]
            public override bool IsT6 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle6(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle6(_value);
            }

            private bool Equals(Choice6 choice) => EqualityComparer<T6>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice6 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice6 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice7 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T7 _value;

            public Choice7(T7 value) => _value = value;

            [IgnoreDataMember]
            public override T7 AsT7 => _value;

            [IgnoreDataMember]
            public override bool IsT7 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle7(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle7(_value);
            }

            private bool Equals(Choice7 choice) => EqualityComparer<T7>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice7 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice7 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }

        private class Choice8 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T8 _value;

            public Choice8(T8 value) => _value = value;

            [IgnoreDataMember]
            public override T8 AsT8 => _value;

            [IgnoreDataMember]
            public override bool IsT8 => true;

            public override object Value => _value;

            public override void Match(Action<T1> handle1, Action<T2> handle2, Action<T3> handle3, Action<T4> handle4, Action<T5> handle5, Action<T6> handle6, Action<T7> handle7, Action<T8> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                handle8(_value);
            }

            public override U Match<U>(Func<T1, U> handle1, Func<T2, U> handle2, Func<T3, U> handle3, Func<T4, U> handle4, Func<T5, U> handle5, Func<T6, U> handle6, Func<T7, U> handle7, Func<T8, U> handle8)
            {
                if (handle1 == null) throw new ArgumentNullException(nameof(handle1));
                if (handle2 == null) throw new ArgumentNullException(nameof(handle2));
                if (handle3 == null) throw new ArgumentNullException(nameof(handle3));
                if (handle4 == null) throw new ArgumentNullException(nameof(handle4));
                if (handle5 == null) throw new ArgumentNullException(nameof(handle5));
                if (handle6 == null) throw new ArgumentNullException(nameof(handle6));
                if (handle7 == null) throw new ArgumentNullException(nameof(handle7));
                if (handle8 == null) throw new ArgumentNullException(nameof(handle8));

                return handle8(_value);
            }

            private bool Equals(Choice8 choice) => EqualityComparer<T8>.Default.Equals(_value, choice._value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice8 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice8 c && Equals(c);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();
        }
    }
}
