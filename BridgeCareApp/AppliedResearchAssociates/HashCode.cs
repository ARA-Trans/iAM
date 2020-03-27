using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    [Obsolete("When System.HashCode is available, remove this type.")]
    public partial struct HashCode
    {
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {

        }

        public static int Combine(params object[] values)
        {
            var result = new HashCode();

            if (values != null)
            {
                foreach (var value in values)
                {
                    result.Add(value);
                }
            }

            return result.ToHashCode();
        }

        public void Add<T>(T value, IEqualityComparer<T> equalityComparer = null)
        {
            if (!HasBeenInitialized)
            {
                Result = FNV_1A_OFFSET_BASIS;
                HasBeenInitialized = true;
            }

            if (value == null)
            {
                Result = unchecked(Result * FNV_1A_PRIME);
            }
            else
            {
                Result = unchecked((Result ^ (equalityComparer ?? EqualityComparer<T>.Default).GetHashCode(value)) * FNV_1A_PRIME);
            }
        }

        public override bool Equals(object obj) => throw new NotSupportedException();

        public override int GetHashCode() => throw new NotSupportedException();

        public int ToHashCode()
        {
            if (HasBeenCalculated)
            {
                throw new InvalidOperationException("This method should not be called more than once per instance of this type.");
            }

            if (!HasBeenInitialized)
            {
                throw new InvalidOperationException("Hash code combination requires at least one input.");
            }

            HasBeenCalculated = true;

            return Result;
        }

        private const int FNV_1A_OFFSET_BASIS = unchecked((int)2166136261);
        private const int FNV_1A_PRIME = 16777619;

        private bool HasBeenCalculated;
        private bool HasBeenInitialized;
        private int Result;
    }
}
