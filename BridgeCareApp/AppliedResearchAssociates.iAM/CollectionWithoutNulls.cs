using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    internal sealed class CollectionWithoutNulls<T> : ICollection<T>, IReadOnlyCollection<T> where T : class
    {
        public int Count => ((ICollection<T>)Items).Count;

        public bool IsReadOnly => ((ICollection<T>)Items).IsReadOnly;

        public void Add(T item)
        {
            if (item != null)
            {
                ((ICollection<T>)Items).Add(item);
            }
        }

        public void Clear() => ((ICollection<T>)Items).Clear();

        public bool Contains(T item) => ((ICollection<T>)Items).Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => ((ICollection<T>)Items).CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();

        public bool Remove(T item) => ((ICollection<T>)Items).Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();

        private readonly List<T> Items = new List<T>();
    }
}
