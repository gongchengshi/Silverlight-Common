///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace SEL.Collections.Generic
{
    /// <summary>
    /// An observable collection that maintains a sorted order and has all the 
    /// properties of a set except that it maintains orderedness.
    /// 
    /// Implemented to favor fast enumeration of the underlying collection. 
    /// Using a search tree as the underlying collection can improve the 
    /// rest of the operations but it would slow down enumeration.
    /// 
    /// The constructors all have common collection constructor signatures
    /// </summary>
    public class ObservableSortedSet<T> : SortedSet<T>, IObservableCollection<T> where T : IComparable<T>
    {
        public ObservableSortedSet()
        { }

        public ObservableSortedSet(IComparer<T> comparer)
            : base(comparer)
        { }

        public ObservableSortedSet(int capacity)
            : base(capacity)
        { }

        public ObservableSortedSet(int capacity, IComparer<T> comparer)
            : base(capacity, comparer)
        { }

        public ObservableSortedSet(IEnumerable<T> collection)
            : base(collection)
        { }

        public ObservableSortedSet(IEnumerable<T> collection, IComparer<T> comparer)
            : base(collection, comparer)
        { }

        #region ISet Implementation
        public override bool Add(T item)
        {
            int index;
            if ((index = SortedSet<T>.Add(base._collection, item, base._comparer)) >= 0)
            {
                RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
                return true;
            }

            return false;
        }

        public override void Clear()
        {
            base.Clear();

            RaiseCollectionReset();
        }

        public override bool Remove(T item)
        {
            var index = _collection.IndexOf(item);

            if (index == -1)
                return false;

            RemoveAt(index);

            return true;
        }
        #endregion

        #region OtherButNotThis
        // These methods only make sense in ObservableSortedSet as opposed to its base class. The goal of these 
        // methods is to reduce the number of CollectionChange events and to maintain the structure of the 
        // set as much as possible in order to reduce flashing of the data in the UI.

        /// <summary>
        /// Assumes collection is sorted
        /// </summary>
        public virtual void SortedOtherButNotThis(ICollection<T> sortedOther)
        {
            if (sortedOther.Count == 0)
                return;

            SortedOtherButNotThis(sortedOther.ToArray());
        }

        /// <summary>
        /// Assumes collection is sorted
        /// </summary>
        public virtual void SortedOtherButNotThis(ICollection<T> sortedOther, T start, T end)
        {
            SortedOtherButNotThis(sortedOther.ToArray(), start, end);
        }

        /// <summary>
        /// Assumes array is sorted
        /// </summary>
        public virtual void SortedOtherButNotThis(T[] sortedOther)
        {
            if(sortedOther.Length == 0)
                return;

            SortedOtherButNotThis(sortedOther, sortedOther[0], sortedOther[sortedOther.Length - 1]);
        }

        /// <summary>
        /// Worst case: O((Log N) + (N Log N) + N)
        /// </summary>
        public virtual void UnsortedOtherButNotThis(IEnumerable<T> unsortedOther, T start, T end)
        {
            var sortedOther = unsortedOther.ToArray();
            Array.Sort(sortedOther, _comparer);

            SortedOtherButNotThis(sortedOther, start, end);
        }

        /// <summary>
        /// Removes everything between start and end that is in this but not other.
        /// In the end this will contain everything in other.
        /// 
        /// SortedOther should be sorted before calling this function. Calling this function
        /// with an unsorted array will have the same result but will be much slower.
        /// </summary>
        public virtual void SortedOtherButNotThis(T[] sortedOther, T start, T end)
        {
            if (start.CompareTo(end) > 0)
            {
                throw new ArgumentException("start must be less than or equal to end");
            }

            if (sortedOther.Length == 0)
            {
                // Remove everything in this from start to end
                Remove(start, end, SortedSet.Bounds.ClosedClosed);
                return;
            }

            if (this.Count == 0)
            {
                // Add everything in sortedOther to this
                UnionWith(sortedOther);
                return;
            }

            int startIndex = LeftClosed(start);

            if (startIndex >= Count)
            {
                // Add everything in sortedOther to this
                UnionWith(sortedOther);
                return;
            }

            int thisI = startIndex;
            int otherI = 0;

            while((thisI < Count) && (this[thisI].CompareTo(end) < 0))
            {
                var compareResult = this[thisI].CompareTo(sortedOther[otherI]);

                if (compareResult < 0) // this < other
                {
                    RemoveAt(thisI);
                }
                else if (compareResult > 0) // this > other
                {
                    // If start > sortedOther[0] the item may already be in the list.
                    if (Add(sortedOther[otherI]))
                    {
                        // Adding a new item will shift everything in this to the right 
                        // so thisI needs to be incremented
                        ++thisI;
                    }
                    ++otherI;
                }
                else // equal
                {
                    ++thisI;
                    ++otherI;
                }
            }

            // There may be items in other that are greater than end so add them here
            for (; otherI < sortedOther.Length; ++otherI)
            {
                Add(sortedOther[otherI]);
            }
        }
        #endregion

        #region INotifyPropertyChanged and IObservableCollection implementation and helpers
        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool _collectionChangeNotificationSuspended;

        protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (!_collectionChangeNotificationSuspended)
            {
                CollectionChanged(this, args);
            }

            RaisePropertyChanged("Count");
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        public void RaiseCollectionReset()
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void SuspendCollectionChangeNotification()
        {
            _collectionChangeNotificationSuspended = true;
        }

        public void ResumeCollectionChangeNotification()
        {
            _collectionChangeNotificationSuspended = false;
            RaiseCollectionReset();
        }

        #endregion

        /// <summary>
        /// Works just like List.RemoveAll.
        /// </summary>
        public override int RemoveAll(Predicate<T> predicate)
        {
            int numRemoved = 0;

            for (int i = 0; i < Count; ++i)
            {
                if(predicate(this[i]))
                {
                    RemoveAt(i);
                    --i;
                    ++numRemoved;
                }
            }

            return numRemoved;
        }

        /// <summary>
        /// Works just like List.RemoveRange
        /// </summary>
        public override void RemoveRange(int index, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                RemoveAt(index);
            }
        }

        public override void RemoveAt(int index)
        {
            var item = this[index];
            base.RemoveAt(index);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        /// <summary>
        /// Clears the collection and resets it with the passed in collection.
        /// </summary>
        public void ResetContents(IEnumerable<T> collection)
        {
            base.Clear();
            base.UnionWith(collection);

            RaiseCollectionReset();
        }
    }
}
