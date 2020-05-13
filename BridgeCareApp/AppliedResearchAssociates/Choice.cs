// WARNING: This file was automatically generated from a T4 text template at the
// following moment in time: 05/13/2020 15:34:09 -05:00. Any changes you make to
// this file will be lost when this file is regenerated from the template
// source.

using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    public abstract class Choice<T1, T2> : IEquatable<Choice<T1, T2>>
    {
        private Choice()
        {
        }

        public static Choice<T1, T2> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2> Of(T2 value) => value != null ? new Choice2(value) : null;

        public T1 AsT1()
        {
            _ = IsT1(out var value);
            return value;
        }

        public T2 AsT2()
        {
            _ = IsT2(out var value);
            return value;
        }

        public bool IsT1() => IsT1(out _);

        public bool IsT2() => IsT2(out _);

        public virtual bool IsT1(out T1 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT2(out T2 value)
        {
            value = default;
            return false;
        }

        public abstract object Value { get; }

        public abstract void Handle(Action<T1> handler1, Action<T2> handler2);

        public abstract Choice<U1, U2> Map<U1, U2>(Func<T1, U1> mapper1, Func<T2, U2> mapper2);

        public abstract U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2);

        public abstract bool Equals(Choice<T1, T2> choice);

        public static bool operator ==(Choice<T1, T2> choice1, Choice<T1, T2> choice2) => EqualityComparer<Choice<T1, T2>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2> choice1, Choice<T1, T2> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2> choice) => choice.Reduce(_ => _, _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2>(T2 value) => Of(value);

        private sealed class Choice1 : Choice<T1, T2>
        {
            private readonly T1 _Value;

            public Choice1(T1 value) => _Value = value;

            public override bool IsT1(out T1 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));

                handler1(_Value);
            }

            public override Choice<U1, U2> Map<U1, U2>(Func<T1, U1> mapper1, Func<T2, U2> mapper2)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));

                return mapper1(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));

                return reducer1(_Value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice2 : Choice<T1, T2>
        {
            private readonly T2 _Value;

            public Choice2(T2 value) => _Value = value;

            public override bool IsT2(out T2 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));

                handler2(_Value);
            }

            public override Choice<U1, U2> Map<U1, U2>(Func<T1, U1> mapper1, Func<T2, U2> mapper2)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));

                return mapper2(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));

                return reducer2(_Value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }
    }

    public abstract class Choice<T1, T2, T3> : IEquatable<Choice<T1, T2, T3>>
    {
        private Choice()
        {
        }

        public static Choice<T1, T2, T3> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3> Of(T3 value) => value != null ? new Choice3(value) : null;

        public T1 AsT1()
        {
            _ = IsT1(out var value);
            return value;
        }

        public T2 AsT2()
        {
            _ = IsT2(out var value);
            return value;
        }

        public T3 AsT3()
        {
            _ = IsT3(out var value);
            return value;
        }

        public bool IsT1() => IsT1(out _);

        public bool IsT2() => IsT2(out _);

        public bool IsT3() => IsT3(out _);

        public virtual bool IsT1(out T1 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT2(out T2 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT3(out T3 value)
        {
            value = default;
            return false;
        }

        public abstract object Value { get; }

        public abstract void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3);

        public abstract Choice<U1, U2, U3> Map<U1, U2, U3>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3);

        public abstract U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3);

        public abstract bool Equals(Choice<T1, T2, T3> choice);

        public static bool operator ==(Choice<T1, T2, T3> choice1, Choice<T1, T2, T3> choice2) => EqualityComparer<Choice<T1, T2, T3>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3> choice1, Choice<T1, T2, T3> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3> choice) => choice.Reduce(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3>(T3 value) => Of(value);

        private sealed class Choice1 : Choice<T1, T2, T3>
        {
            private readonly T1 _Value;

            public Choice1(T1 value) => _Value = value;

            public override bool IsT1(out T1 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));

                handler1(_Value);
            }

            public override Choice<U1, U2, U3> Map<U1, U2, U3>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));

                return mapper1(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));

                return reducer1(_Value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice2 : Choice<T1, T2, T3>
        {
            private readonly T2 _Value;

            public Choice2(T2 value) => _Value = value;

            public override bool IsT2(out T2 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));

                handler2(_Value);
            }

            public override Choice<U1, U2, U3> Map<U1, U2, U3>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));

                return mapper2(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));

                return reducer2(_Value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice3 : Choice<T1, T2, T3>
        {
            private readonly T3 _Value;

            public Choice3(T3 value) => _Value = value;

            public override bool IsT3(out T3 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));

                handler3(_Value);
            }

            public override Choice<U1, U2, U3> Map<U1, U2, U3>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));

                return mapper3(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));

                return reducer3(_Value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }
    }

    public abstract class Choice<T1, T2, T3, T4> : IEquatable<Choice<T1, T2, T3, T4>>
    {
        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4> Of(T4 value) => value != null ? new Choice4(value) : null;

        public T1 AsT1()
        {
            _ = IsT1(out var value);
            return value;
        }

        public T2 AsT2()
        {
            _ = IsT2(out var value);
            return value;
        }

        public T3 AsT3()
        {
            _ = IsT3(out var value);
            return value;
        }

        public T4 AsT4()
        {
            _ = IsT4(out var value);
            return value;
        }

        public bool IsT1() => IsT1(out _);

        public bool IsT2() => IsT2(out _);

        public bool IsT3() => IsT3(out _);

        public bool IsT4() => IsT4(out _);

        public virtual bool IsT1(out T1 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT2(out T2 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT3(out T3 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT4(out T4 value)
        {
            value = default;
            return false;
        }

        public abstract object Value { get; }

        public abstract void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4);

        public abstract Choice<U1, U2, U3, U4> Map<U1, U2, U3, U4>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4);

        public abstract U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4);

        public abstract bool Equals(Choice<T1, T2, T3, T4> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4> choice1, Choice<T1, T2, T3, T4> choice2) => EqualityComparer<Choice<T1, T2, T3, T4>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4> choice1, Choice<T1, T2, T3, T4> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4> choice) => choice.Reduce(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4>(T4 value) => Of(value);

        private sealed class Choice1 : Choice<T1, T2, T3, T4>
        {
            private readonly T1 _Value;

            public Choice1(T1 value) => _Value = value;

            public override bool IsT1(out T1 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));

                handler1(_Value);
            }

            public override Choice<U1, U2, U3, U4> Map<U1, U2, U3, U4>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));

                return mapper1(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));

                return reducer1(_Value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice2 : Choice<T1, T2, T3, T4>
        {
            private readonly T2 _Value;

            public Choice2(T2 value) => _Value = value;

            public override bool IsT2(out T2 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));

                handler2(_Value);
            }

            public override Choice<U1, U2, U3, U4> Map<U1, U2, U3, U4>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));

                return mapper2(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));

                return reducer2(_Value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice3 : Choice<T1, T2, T3, T4>
        {
            private readonly T3 _Value;

            public Choice3(T3 value) => _Value = value;

            public override bool IsT3(out T3 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));

                handler3(_Value);
            }

            public override Choice<U1, U2, U3, U4> Map<U1, U2, U3, U4>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));

                return mapper3(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));

                return reducer3(_Value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice4 : Choice<T1, T2, T3, T4>
        {
            private readonly T4 _Value;

            public Choice4(T4 value) => _Value = value;

            public override bool IsT4(out T4 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));

                handler4(_Value);
            }

            public override Choice<U1, U2, U3, U4> Map<U1, U2, U3, U4>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));

                return mapper4(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));

                return reducer4(_Value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }
    }

    public abstract class Choice<T1, T2, T3, T4, T5> : IEquatable<Choice<T1, T2, T3, T4, T5>>
    {
        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4, T5> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T4 value) => value != null ? new Choice4(value) : null;

        public static Choice<T1, T2, T3, T4, T5> Of(T5 value) => value != null ? new Choice5(value) : null;

        public T1 AsT1()
        {
            _ = IsT1(out var value);
            return value;
        }

        public T2 AsT2()
        {
            _ = IsT2(out var value);
            return value;
        }

        public T3 AsT3()
        {
            _ = IsT3(out var value);
            return value;
        }

        public T4 AsT4()
        {
            _ = IsT4(out var value);
            return value;
        }

        public T5 AsT5()
        {
            _ = IsT5(out var value);
            return value;
        }

        public bool IsT1() => IsT1(out _);

        public bool IsT2() => IsT2(out _);

        public bool IsT3() => IsT3(out _);

        public bool IsT4() => IsT4(out _);

        public bool IsT5() => IsT5(out _);

        public virtual bool IsT1(out T1 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT2(out T2 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT3(out T3 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT4(out T4 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT5(out T5 value)
        {
            value = default;
            return false;
        }

        public abstract object Value { get; }

        public abstract void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5);

        public abstract Choice<U1, U2, U3, U4, U5> Map<U1, U2, U3, U4, U5>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5);

        public abstract U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5> choice1, Choice<T1, T2, T3, T4, T5> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5> choice1, Choice<T1, T2, T3, T4, T5> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5> choice) => choice.Reduce(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5>(T5 value) => Of(value);

        private sealed class Choice1 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T1 _Value;

            public Choice1(T1 value) => _Value = value;

            public override bool IsT1(out T1 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));

                handler1(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5> Map<U1, U2, U3, U4, U5>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));

                return mapper1(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));

                return reducer1(_Value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice2 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T2 _Value;

            public Choice2(T2 value) => _Value = value;

            public override bool IsT2(out T2 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));

                handler2(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5> Map<U1, U2, U3, U4, U5>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));

                return mapper2(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));

                return reducer2(_Value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice3 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T3 _Value;

            public Choice3(T3 value) => _Value = value;

            public override bool IsT3(out T3 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));

                handler3(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5> Map<U1, U2, U3, U4, U5>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));

                return mapper3(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));

                return reducer3(_Value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice4 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T4 _Value;

            public Choice4(T4 value) => _Value = value;

            public override bool IsT4(out T4 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));

                handler4(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5> Map<U1, U2, U3, U4, U5>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));

                return mapper4(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));

                return reducer4(_Value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice5 : Choice<T1, T2, T3, T4, T5>
        {
            private readonly T5 _Value;

            public Choice5(T5 value) => _Value = value;

            public override bool IsT5(out T5 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));

                handler5(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5> Map<U1, U2, U3, U4, U5>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));

                return mapper5(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));

                return reducer5(_Value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }
    }

    public abstract class Choice<T1, T2, T3, T4, T5, T6> : IEquatable<Choice<T1, T2, T3, T4, T5, T6>>
    {
        private Choice()
        {
        }

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T1 value) => value != null ? new Choice1(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T2 value) => value != null ? new Choice2(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T3 value) => value != null ? new Choice3(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T4 value) => value != null ? new Choice4(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T5 value) => value != null ? new Choice5(value) : null;

        public static Choice<T1, T2, T3, T4, T5, T6> Of(T6 value) => value != null ? new Choice6(value) : null;

        public T1 AsT1()
        {
            _ = IsT1(out var value);
            return value;
        }

        public T2 AsT2()
        {
            _ = IsT2(out var value);
            return value;
        }

        public T3 AsT3()
        {
            _ = IsT3(out var value);
            return value;
        }

        public T4 AsT4()
        {
            _ = IsT4(out var value);
            return value;
        }

        public T5 AsT5()
        {
            _ = IsT5(out var value);
            return value;
        }

        public T6 AsT6()
        {
            _ = IsT6(out var value);
            return value;
        }

        public bool IsT1() => IsT1(out _);

        public bool IsT2() => IsT2(out _);

        public bool IsT3() => IsT3(out _);

        public bool IsT4() => IsT4(out _);

        public bool IsT5() => IsT5(out _);

        public bool IsT6() => IsT6(out _);

        public virtual bool IsT1(out T1 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT2(out T2 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT3(out T3 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT4(out T4 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT5(out T5 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT6(out T6 value)
        {
            value = default;
            return false;
        }

        public abstract object Value { get; }

        public abstract void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6);

        public abstract Choice<U1, U2, U3, U4, U5, U6> Map<U1, U2, U3, U4, U5, U6>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6);

        public abstract U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5, T6> choice1, Choice<T1, T2, T3, T4, T5, T6> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5, T6>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5, T6> choice1, Choice<T1, T2, T3, T4, T5, T6> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Reduce(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T6(Choice<T1, T2, T3, T4, T5, T6> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T5 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T6 value) => Of(value);

        private sealed class Choice1 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T1 _Value;

            public Choice1(T1 value) => _Value = value;

            public override bool IsT1(out T1 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));

                handler1(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6> Map<U1, U2, U3, U4, U5, U6>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));

                return mapper1(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));

                return reducer1(_Value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice2 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T2 _Value;

            public Choice2(T2 value) => _Value = value;

            public override bool IsT2(out T2 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));

                handler2(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6> Map<U1, U2, U3, U4, U5, U6>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));

                return mapper2(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));

                return reducer2(_Value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice3 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T3 _Value;

            public Choice3(T3 value) => _Value = value;

            public override bool IsT3(out T3 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));

                handler3(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6> Map<U1, U2, U3, U4, U5, U6>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));

                return mapper3(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));

                return reducer3(_Value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice4 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T4 _Value;

            public Choice4(T4 value) => _Value = value;

            public override bool IsT4(out T4 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));

                handler4(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6> Map<U1, U2, U3, U4, U5, U6>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));

                return mapper4(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));

                return reducer4(_Value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice5 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T5 _Value;

            public Choice5(T5 value) => _Value = value;

            public override bool IsT5(out T5 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));

                handler5(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6> Map<U1, U2, U3, U4, U5, U6>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));

                return mapper5(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));

                return reducer5(_Value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice6 : Choice<T1, T2, T3, T4, T5, T6>
        {
            private readonly T6 _Value;

            public Choice6(T6 value) => _Value = value;

            public override bool IsT6(out T6 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));

                handler6(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6> Map<U1, U2, U3, U4, U5, U6>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));

                return mapper6(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));

                return reducer6(_Value);
            }

            private bool Equals(Choice6 choice) => EqualityComparer<T6>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6> choice) => choice is Choice6 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice6 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }
    }

    public abstract class Choice<T1, T2, T3, T4, T5, T6, T7> : IEquatable<Choice<T1, T2, T3, T4, T5, T6, T7>>
    {
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

        public T1 AsT1()
        {
            _ = IsT1(out var value);
            return value;
        }

        public T2 AsT2()
        {
            _ = IsT2(out var value);
            return value;
        }

        public T3 AsT3()
        {
            _ = IsT3(out var value);
            return value;
        }

        public T4 AsT4()
        {
            _ = IsT4(out var value);
            return value;
        }

        public T5 AsT5()
        {
            _ = IsT5(out var value);
            return value;
        }

        public T6 AsT6()
        {
            _ = IsT6(out var value);
            return value;
        }

        public T7 AsT7()
        {
            _ = IsT7(out var value);
            return value;
        }

        public bool IsT1() => IsT1(out _);

        public bool IsT2() => IsT2(out _);

        public bool IsT3() => IsT3(out _);

        public bool IsT4() => IsT4(out _);

        public bool IsT5() => IsT5(out _);

        public bool IsT6() => IsT6(out _);

        public bool IsT7() => IsT7(out _);

        public virtual bool IsT1(out T1 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT2(out T2 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT3(out T3 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT4(out T4 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT5(out T5 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT6(out T6 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT7(out T7 value)
        {
            value = default;
            return false;
        }

        public abstract object Value { get; }

        public abstract void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7);

        public abstract Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7);

        public abstract U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5, T6, T7> choice1, Choice<T1, T2, T3, T4, T5, T6, T7> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5, T6, T7>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5, T6, T7> choice1, Choice<T1, T2, T3, T4, T5, T6, T7> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Reduce(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T6(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T7(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T5 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T6 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T7 value) => Of(value);

        private sealed class Choice1 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T1 _Value;

            public Choice1(T1 value) => _Value = value;

            public override bool IsT1(out T1 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));

                handler1(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));

                return mapper1(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));

                return reducer1(_Value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice2 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T2 _Value;

            public Choice2(T2 value) => _Value = value;

            public override bool IsT2(out T2 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));

                handler2(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));

                return mapper2(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));

                return reducer2(_Value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice3 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T3 _Value;

            public Choice3(T3 value) => _Value = value;

            public override bool IsT3(out T3 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));

                handler3(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));

                return mapper3(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));

                return reducer3(_Value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice4 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T4 _Value;

            public Choice4(T4 value) => _Value = value;

            public override bool IsT4(out T4 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));

                handler4(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));

                return mapper4(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));

                return reducer4(_Value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice5 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T5 _Value;

            public Choice5(T5 value) => _Value = value;

            public override bool IsT5(out T5 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));

                handler5(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));

                return mapper5(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));

                return reducer5(_Value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice6 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T6 _Value;

            public Choice6(T6 value) => _Value = value;

            public override bool IsT6(out T6 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));

                handler6(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));

                return mapper6(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));

                return reducer6(_Value);
            }

            private bool Equals(Choice6 choice) => EqualityComparer<T6>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice6 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice6 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice7 : Choice<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly T7 _Value;

            public Choice7(T7 value) => _Value = value;

            public override bool IsT7(out T7 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));

                handler7(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7> Map<U1, U2, U3, U4, U5, U6, U7>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));

                return mapper7(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));

                return reducer7(_Value);
            }

            private bool Equals(Choice7 choice) => EqualityComparer<T7>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7> choice) => choice is Choice7 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice7 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }
    }

    public abstract class Choice<T1, T2, T3, T4, T5, T6, T7, T8> : IEquatable<Choice<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
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

        public T1 AsT1()
        {
            _ = IsT1(out var value);
            return value;
        }

        public T2 AsT2()
        {
            _ = IsT2(out var value);
            return value;
        }

        public T3 AsT3()
        {
            _ = IsT3(out var value);
            return value;
        }

        public T4 AsT4()
        {
            _ = IsT4(out var value);
            return value;
        }

        public T5 AsT5()
        {
            _ = IsT5(out var value);
            return value;
        }

        public T6 AsT6()
        {
            _ = IsT6(out var value);
            return value;
        }

        public T7 AsT7()
        {
            _ = IsT7(out var value);
            return value;
        }

        public T8 AsT8()
        {
            _ = IsT8(out var value);
            return value;
        }

        public bool IsT1() => IsT1(out _);

        public bool IsT2() => IsT2(out _);

        public bool IsT3() => IsT3(out _);

        public bool IsT4() => IsT4(out _);

        public bool IsT5() => IsT5(out _);

        public bool IsT6() => IsT6(out _);

        public bool IsT7() => IsT7(out _);

        public bool IsT8() => IsT8(out _);

        public virtual bool IsT1(out T1 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT2(out T2 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT3(out T3 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT4(out T4 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT5(out T5 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT6(out T6 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT7(out T7 value)
        {
            value = default;
            return false;
        }

        public virtual bool IsT8(out T8 value)
        {
            value = default;
            return false;
        }

        public abstract object Value { get; }

        public abstract void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8);

        public abstract Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8);

        public abstract U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8);

        public abstract bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice);

        public static bool operator ==(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice1, Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice2) => EqualityComparer<Choice<T1, T2, T3, T4, T5, T6, T7, T8>>.Default.Equals(choice1, choice2);

        public static bool operator !=(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice1, Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice2) => !(choice1 == choice2);

        public static explicit operator T1(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T2(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T3(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T4(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T5(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T6(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException(), _ => throw new InvalidCastException());

        public static explicit operator T7(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _, _ => throw new InvalidCastException());

        public static explicit operator T8(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice.Reduce(_ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => throw new InvalidCastException(), _ => _);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => Of(value);

        public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => Of(value);

        private sealed class Choice1 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T1 _Value;

            public Choice1(T1 value) => _Value = value;

            public override bool IsT1(out T1 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler1(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper1(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer1(_Value);
            }

            private bool Equals(Choice1 choice) => EqualityComparer<T1>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice1 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice1 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice2 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T2 _Value;

            public Choice2(T2 value) => _Value = value;

            public override bool IsT2(out T2 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler2(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper2(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer2(_Value);
            }

            private bool Equals(Choice2 choice) => EqualityComparer<T2>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice2 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice2 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice3 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T3 _Value;

            public Choice3(T3 value) => _Value = value;

            public override bool IsT3(out T3 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler3(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper3(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer3(_Value);
            }

            private bool Equals(Choice3 choice) => EqualityComparer<T3>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice3 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice3 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice4 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T4 _Value;

            public Choice4(T4 value) => _Value = value;

            public override bool IsT4(out T4 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler4(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper4(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer4(_Value);
            }

            private bool Equals(Choice4 choice) => EqualityComparer<T4>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice4 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice4 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice5 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T5 _Value;

            public Choice5(T5 value) => _Value = value;

            public override bool IsT5(out T5 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler5(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper5(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer5(_Value);
            }

            private bool Equals(Choice5 choice) => EqualityComparer<T5>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice5 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice5 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice6 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T6 _Value;

            public Choice6(T6 value) => _Value = value;

            public override bool IsT6(out T6 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler6(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper6(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer6(_Value);
            }

            private bool Equals(Choice6 choice) => EqualityComparer<T6>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice6 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice6 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice7 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T7 _Value;

            public Choice7(T7 value) => _Value = value;

            public override bool IsT7(out T7 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler7(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper7(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer7(_Value);
            }

            private bool Equals(Choice7 choice) => EqualityComparer<T7>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice7 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice7 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }

        private sealed class Choice8 : Choice<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly T8 _Value;

            public Choice8(T8 value) => _Value = value;

            public override bool IsT8(out T8 value)
            {
                value = _Value;
                return true;
            }

            public override object Value => _Value;

            public override void Handle(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
            {
                if (handler1 == null) throw new ArgumentNullException(nameof(handler1));
                if (handler2 == null) throw new ArgumentNullException(nameof(handler2));
                if (handler3 == null) throw new ArgumentNullException(nameof(handler3));
                if (handler4 == null) throw new ArgumentNullException(nameof(handler4));
                if (handler5 == null) throw new ArgumentNullException(nameof(handler5));
                if (handler6 == null) throw new ArgumentNullException(nameof(handler6));
                if (handler7 == null) throw new ArgumentNullException(nameof(handler7));
                if (handler8 == null) throw new ArgumentNullException(nameof(handler8));

                handler8(_Value);
            }

            public override Choice<U1, U2, U3, U4, U5, U6, U7, U8> Map<U1, U2, U3, U4, U5, U6, U7, U8>(Func<T1, U1> mapper1, Func<T2, U2> mapper2, Func<T3, U3> mapper3, Func<T4, U4> mapper4, Func<T5, U5> mapper5, Func<T6, U6> mapper6, Func<T7, U7> mapper7, Func<T8, U8> mapper8)
            {
                if (mapper1 == null) throw new ArgumentNullException(nameof(mapper1));
                if (mapper2 == null) throw new ArgumentNullException(nameof(mapper2));
                if (mapper3 == null) throw new ArgumentNullException(nameof(mapper3));
                if (mapper4 == null) throw new ArgumentNullException(nameof(mapper4));
                if (mapper5 == null) throw new ArgumentNullException(nameof(mapper5));
                if (mapper6 == null) throw new ArgumentNullException(nameof(mapper6));
                if (mapper7 == null) throw new ArgumentNullException(nameof(mapper7));
                if (mapper8 == null) throw new ArgumentNullException(nameof(mapper8));

                return mapper8(_Value);
            }

            public override U Reduce<U>(Func<T1, U> reducer1, Func<T2, U> reducer2, Func<T3, U> reducer3, Func<T4, U> reducer4, Func<T5, U> reducer5, Func<T6, U> reducer6, Func<T7, U> reducer7, Func<T8, U> reducer8)
            {
                if (reducer1 == null) throw new ArgumentNullException(nameof(reducer1));
                if (reducer2 == null) throw new ArgumentNullException(nameof(reducer2));
                if (reducer3 == null) throw new ArgumentNullException(nameof(reducer3));
                if (reducer4 == null) throw new ArgumentNullException(nameof(reducer4));
                if (reducer5 == null) throw new ArgumentNullException(nameof(reducer5));
                if (reducer6 == null) throw new ArgumentNullException(nameof(reducer6));
                if (reducer7 == null) throw new ArgumentNullException(nameof(reducer7));
                if (reducer8 == null) throw new ArgumentNullException(nameof(reducer8));

                return reducer8(_Value);
            }

            private bool Equals(Choice8 choice) => EqualityComparer<T8>.Default.Equals(_Value, choice._Value);

            public override bool Equals(Choice<T1, T2, T3, T4, T5, T6, T7, T8> choice) => choice is Choice8 c && Equals(c);

            public override bool Equals(object obj) => obj is Choice8 c && Equals(c);

            public override int GetHashCode() => _Value.GetHashCode();

            public override string ToString() => _Value.ToString();
        }
    }
}
