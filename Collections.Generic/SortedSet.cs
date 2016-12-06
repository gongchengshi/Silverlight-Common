///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace SEL.Collections.Generic
{
    /// <summary>
    /// A silverlight implementation of the .NET SortedSet generic class
    /// As much as possible it maintains the same interface with a few 
    /// additions.
    /// 
    /// Implemented to favor fast enumeration of the underlying collection. 
    /// Using a search tree as the underlying collection can improve the
    /// rest of the operations but it would slow down enumeration.
    /// </summary>
    public class SortedSet<T> : ISet<T>, ISortedList<T> where T : IComparable<T>
    {
        protected readonly IComparer<T> _comparer;

        protected List<T> _collection;

        public SortedSet(int capacity)
        {
            _collection = new List<T>(capacity);
            _comparer = Comparer<T>.Default;
        }

        public SortedSet(int capacity, IComparer<T> comparer)
        {
            _collection = new List<T>(capacity);
            _comparer = comparer;
        }

        #region Members Borrowed from .NET SortedSet<T>
        public SortedSet()
        {
            _collection = new List<T>();
            _comparer = Comparer<T>.Default;
        }

        public SortedSet(IComparer<T> comparer)
        {
            _collection = new List<T>();
            _comparer = comparer;
        }

        public SortedSet(IEnumerable<T> collection)
        {
            _collection = new List<T>();
            _comparer = Comparer<T>.Default;
            UnionWith(collection);
        }

        public SortedSet(IEnumerable<T> collection, IComparer<T> comparer)
        {
            _collection = new List<T>();
            _comparer = comparer;
            UnionWith(collection);            
        }

        /// <summary>
        /// returns default of T if the collection size is 0
        /// </summary>
        public T Max 
        { 
            get 
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException(
                        "Collection must contain at least one item in order to call Max.");
                }

                return _collection[Count - 1];
            } 
        }

        /// <summary>
        /// returns default of T if the collection size is 0
        /// </summary>
        public T Min 
        { 
            get 
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException(
                        "Collection must contain at least one item in order to call Min.");
                }
                
                return _collection[0]; 
            }
        }

        #endregion

        #region ISet Implementation
        public virtual bool Add(T item)
        {
            return SortedSet<T>.Add(_collection, item, _comparer) >= 0;
        }

        public virtual void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                Remove(item);
            }
        }

        public virtual void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public virtual void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public virtual void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                Add(item);
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public virtual void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool Remove(T item)
        {
            return _collection.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        protected static int Add(List<T> list, T item, IComparer<T> comparer)
        {
            if (item == null)
                throw new ArgumentException("Value can't be null");

            // check where the element should be placed
            int index = list.BinarySearch(item, comparer);
            if (index < 0)
            {
                // xor
                index = ~index;
                list.Insert(index, item);
                return index;
            }
            return -1;
        }

        // Everything from IList is implemented except for Insert() which can't be supported in a sorted collection
        #region Borrowed from IList interface
        public T this[int index]
        {
            get { return _collection[index]; }
        }

        public virtual int RemoveAll(Predicate<T> predicate)
        {
            return _collection.RemoveAll(predicate);
        }

        public virtual void RemoveRange(int index, int count)
        {
            _collection.RemoveRange(index, count);
        }

        public int IndexOf(T item)
        {
            return _collection.IndexOf(item);
        }
        #endregion

        public virtual int Remove(T min, T max, SortedSet.Bounds bounds)
        {
            if (min == null || max == null)
                throw new ArgumentException("Neither min nor max can be null");

            // Checking to see if the second to least and least significant bit of bounds is 1. 
            // So 2 and 3 mean left closed and 1 and 3 mean right closed.
            int left = (((int)bounds & 0x02) != 0) ? LeftClosed(min) : LeftOpen(min);
            int right = (((int)bounds & 0x01) != 0) ? RightClosed(max) : RightOpen(max);

            int count = (right - left) + 1;

            if (count <= 0)
            {
                return 0;
            }

            RemoveRange(left,  count);

            return count;
        }

        /// <summary>
        /// Returns the index of the value including the index it is located at
        /// </summary>
        protected int LeftClosed(T value)
        {
            int index = _collection.BinarySearch(value, _comparer);
            return (index < 0) ? ~index : index;
        }

        protected int RightClosed(T value)
        {
            int index = _collection.BinarySearch(value, _comparer);
            return (index < 0) ? ~index - 1: index;
        }

        protected int LeftOpen(T value)
        {
            int index = _collection.BinarySearch(value, _comparer);
            return (index < 0) ? ~index : index + 1;
        }

        protected int RightOpen(T value)
        {
            int index = _collection.BinarySearch(value, _comparer);
            return ((index < 0) ? ~index : index) - 1;
        }

        public virtual void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }
    }

    public static class SortedSet
    {
        public enum Bounds : byte
        {
            OpenOpen = 0,
            OpenClosed = 1,
            ClosedOpen = 2,
            ClosedClosed = 3
        }
    }
}
