using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public sealed class ListWithoutNulls<T> : IList<T>, IReadOnlyList<T> where T : class
    {
        public ListWithoutNulls(IList<T> items = null) => Items = items ?? new List<T>();

        public int Count => Items.Count;

        public bool IsReadOnly => Items.IsReadOnly;

        public T this[int index]
        {
            get => Items[index];
            set
            {
                if (value != null)
                {
                    Items[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            if (item != null)
            {
                Items.Add(item);
            }
        }

        public void Clear() => Items.Clear();

        public bool Contains(T item) => Items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        public int IndexOf(T item) => Items.IndexOf(item);

        public void Insert(int index, T item)
        {
            if (item != null)
            {
                Items.Insert(index, item);
            }
        }

        public bool Remove(T item) => Items.Remove(item);

        public void RemoveAt(int index) => Items.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();

        private readonly IList<T> Items;
    }
}
