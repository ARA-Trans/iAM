using System;
using System.Collections;

namespace System.Collections
{
	public interface IOrderedDictionary
	{
		void Insert(Int32 index, Object key, Object value);
		void RemoveAt(Int32 index);
	}

	[Serializable]
	public class KeyedList : ICollection, IDictionary, IEnumerable, IOrderedDictionary
	{
		private Hashtable objectTable = new Hashtable ();
		private ArrayList objectList = new ArrayList ();

		public void Add (object key, object value)
		{
			objectTable.Add (key, value);
			objectList.Add (new DictionaryEntry (key, value));
		}

		public void Clear ()
		{
			objectTable.Clear ();
			objectList.Clear ();
		}

		public bool Contains (object key)
		{
			return objectTable.Contains (key);
		}

		public void CopyTo (Array array, int idx)
		{
			objectTable.CopyTo (array, idx);
		}

		public void Insert (int idx, object key, object value)
		{
			if (idx > Count)
				throw new ArgumentOutOfRangeException ("index");

			objectTable.Add (key, value);
			objectList.Insert (idx, new DictionaryEntry (key, value));
		}

		public void Remove (object key)
		{
			objectTable.Remove (key);
			objectList.RemoveAt (IndexOf (key));
		}

		public void RemoveAt (int idx)
		{
			if (idx >= Count)
				throw new ArgumentOutOfRangeException ("index");

			objectTable.Remove ( ((DictionaryEntry)objectList[idx]).Key );
			objectList.RemoveAt (idx);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator ()
		{
			return new KeyedListEnumerator (objectList);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new KeyedListEnumerator (objectList);
		}

		public int Count 
		{
			get { return objectList.Count; }
		}

		public bool IsFixedSize 
		{
			get { return false; }
		}

		public bool IsReadOnly 
		{
			get { return false; }
		}

		public bool IsSynchronized 
		{
			get { return false; }
		}

		public object this[int idx] 
		{
			get { return ((DictionaryEntry) objectList[idx]).Value; }
			set 
			{
				if (idx < 0 || idx >= Count)
					throw new ArgumentOutOfRangeException ("index");

				object key = ((DictionaryEntry) objectList[idx]).Key;
				objectList[idx] = new DictionaryEntry (key, value);
				objectTable[key] = value;
			}
		}

		public object this[object key] 
		{
			get { return objectTable[key]; }
			set 
			{
				if (objectTable.Contains (key))
				{
					objectTable[key] = value;
					objectTable[IndexOf (key)] = new DictionaryEntry (key, value);
					return;
				}
				Add (key, value);
			}
		}

		public ICollection Keys 
		{
			get 
			{ 
				ArrayList retList = new ArrayList ();
				for (int i = 0; i < objectList.Count; i++)
				{
					retList.Add ( ((DictionaryEntry)objectList[i]).Key );
				}
				return retList;
			}
		}

		public ICollection Values 
		{
			get 
			{
				ArrayList retList = new ArrayList ();
				for (int i = 0; i < objectList.Count; i++)
				{
					retList.Add ( ((DictionaryEntry)objectList[i]).Value );
				}
				return retList;
			}
		}
	
		public object SyncRoot 
		{
			get { return this; }
		}	

		private int IndexOf (object key)
		{
			for (int i = 0; i < objectList.Count; i++)
			{
				if (((DictionaryEntry) objectList[i]).Key.Equals (key))
				{
					return i;
				}
			}
			return -1;
		}
	}

	public class KeyedListEnumerator : IDictionaryEnumerator
	{
		private int index = -1;
		private ArrayList objs;

		internal KeyedListEnumerator (ArrayList list)
		{
			objs = list;
		}

		public bool MoveNext ()
		{
			index++;
			if (index >= objs.Count)
				return false;

			return true;
		}

		public void Reset ()
		{
			index = -1;
		}

		public object Current 
		{
			get 
			{
				if (index < 0 || index >= objs.Count)
					throw new InvalidOperationException ();

				return objs[index];
			}
		}

		public DictionaryEntry Entry 
		{
			get 
			{
				return (DictionaryEntry) Current;
			}
		}

		public object Key 
		{
			get 
			{
				return Entry.Key;
			}
		}

		public object Value 
		{
			get 
			{
				return Entry.Value;
			}
		}
	}
}
