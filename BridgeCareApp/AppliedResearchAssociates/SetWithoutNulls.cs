using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public sealed class SetWithoutNulls<T> : ISet<T>, IReadOnlyCollection<T> where T : class
    {
        public SetWithoutNulls(IEqualityComparer<T> equalityComparer = null) => Items = new HashSet<T>(equalityComparer);

        public int Count => Items.Count;

        public bool IsReadOnly => Items.IsReadOnly;

        public bool Add(T item) => item != null && Items.Add(item);

        public void Clear() => Items.Clear();

        public bool Contains(T item) => Items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public void ExceptWith(IEnumerable<T> other) => Items.ExceptWith(other);

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        public void IntersectWith(IEnumerable<T> other) => Items.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => Items.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => Items.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => Items.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => Items.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => Items.Overlaps(other);

        public bool Remove(T item) => Items.Remove(item);

        public bool SetEquals(IEnumerable<T> other) => Items.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other) => Items.SymmetricExceptWith(other);

        public void UnionWith(IEnumerable<T> other)
        {
            Items.UnionWith(other);
            _ = Remove(null);
        }

        void ICollection<T>.Add(T item)
        {
            if (item != null)
            {
                ((ICollection<T>)Items).Add(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();

        private readonly ISet<T> Items;
    }
}
