using System;
using System.Collections.Generic;

namespace AASHTOWare
{
    /// <summary>
    ///     Represents the presence or absence of an object in a way that
    ///     prevents access to <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
    {
        private readonly T _Value;

        internal Option(T value)
        {
            _Value = value;
            HasValue = true;
        }

        /// <summary>
        ///     Get whether this option has a value.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        ///     Get the value if there is one, otherwise throw an exception.
        /// </summary>
        /// <returns>the value</returns>
        public T Value => HasValue ? _Value : throw new InvalidOperationException("This option does not have a value.");

        public static bool operator !=(Option<T> left, Option<T> right) => !(left == right);

        public static bool operator !=(Option<T> left, T right) => !(left == right);

        public static bool operator !=(T left, Option<T> right) => !(left == right);

        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        public static bool operator ==(Option<T> left, T right) => left.Equals(right);

        public static bool operator ==(T left, Option<T> right) => right.Equals(left);

        public override bool Equals(object obj) => obj is Option<T> option ? Equals(option) : obj is T value && Equals(value);

        public bool Equals(Option<T> other) => HasValue == other.HasValue && (!HasValue || EqualityComparer<T>.Default.Equals(_Value, other._Value));

        public bool Equals(T other) => HasValue && EqualityComparer<T>.Default.Equals(_Value, other);

        public override int GetHashCode() => HasValue ? _Value.GetHashCode() : 0;

        /// <summary>
        ///     Handle the presence or absence of a value in this option.
        /// </summary>
        /// <param name="handleValue"></param>
        /// <param name="handleNoValue"></param>
        public void Handle(Action<T> handleValue, Action handleNoValue)
        {
            if (handleValue is null)
            {
                throw new ArgumentNullException(nameof(handleValue));
            }

            if (handleNoValue is null)
            {
                throw new ArgumentNullException(nameof(handleNoValue));
            }

            if (HasValue)
            {
                handleValue(_Value);
            }
            else
            {
                handleNoValue();
            }
        }

        /// <summary>
        ///     Transform the value of this option.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="map"></param>
        /// <param name="_">
        ///     never required; only declared to disambiguate this overload
        /// </param>
        /// <returns>
        ///     a new option with the result of the transformation
        /// </returns>
        public Option<U> Map<U>(Func<T, U> map, RequireClass<U> _ = null) where U : class
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            return HasValue ? Option.Of(map(_Value)) : new Option<U>();
        }

        /// <summary>
        ///     Transform the value of this option.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="map"></param>
        /// <param name="_">
        ///     never required; only declared to disambiguate this overload
        /// </param>
        /// <returns>
        ///     a new option with the result of the transformation
        /// </returns>
        public Option<U> Map<U>(Func<T, U> map, RequireStruct<U> _ = null) where U : struct
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            return HasValue ? Option.Of(map(_Value)) : new Option<U>();
        }

        /// <summary>
        ///     Transform the value of this option.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="map"></param>
        /// <returns>
        ///     a new option with the result of the transformation
        /// </returns>
        public Option<U> Map<U>(Func<T, U?> map) where U : struct
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            return HasValue ? Option.Of(map(_Value)) : new Option<U>();
        }

        /// <summary>
        ///     Get the value if there is one, otherwise get the given default
        ///     value.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns>the value or a default value</returns>
        public T Reduce(T defaultValue = default) => HasValue ? _Value : defaultValue;

        /// <summary>
        ///     Get the value if there is one, otherwise get a default value
        ///     from the given factory function. A null factory function yields
        ///     the type's default value.
        /// </summary>
        /// <param name="getDefaultValue"></param>
        /// <returns>the value or a default value</returns>
        public T Reduce(Func<T> getDefaultValue)
        {
            if (HasValue)
            {
                return _Value;
            }
            else if (getDefaultValue is null)
            {
                return default;
            }
            else
            {
                return getDefaultValue();
            }
        }
    }

    /// <summary>
    ///     Methods for using type inference when creating
    ///     <see cref="Option{T}"/> objects.
    /// </summary>
    public static class Option
    {
        /// <summary>
        ///     Create an option object. Allows the type to be inferred.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="_">
        ///     never required; only declared to disambiguate overloads
        /// </param>
        /// <returns>an optional reference</returns>
        public static Option<T> Of<T>(T value, RequireClass<T> _ = null) where T : class => value is null ? new Option<T>() : new Option<T>(value);

        /// <summary>
        ///     Create an option object. Allows the type to be inferred.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="_">
        ///     never required; only declared to disambiguate overloads
        /// </param>
        /// <returns>an optional value</returns>
        public static Option<T> Of<T>(T value, RequireStruct<T> _ = null) where T : struct => new Option<T>(value);

        /// <summary>
        ///     Create an option object. Allows the type to be inferred.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>an optional value</returns>
        public static Option<T> Of<T>(T? value) where T : struct => value.HasValue ? new Option<T>(value.Value) : new Option<T>();
    }
}
