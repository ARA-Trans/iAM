using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AppliedResearchAssociates
{
    public sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public static ReferenceEqualityComparer<T> Instance { get; } = new ReferenceEqualityComparer<T>();

        public bool Equals(T x, T y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);

        private ReferenceEqualityComparer()
        {
        }
    }
}
