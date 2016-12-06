// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEL.UnitTest;

namespace SEL.Collections.Generic.UnitTests
{
    [TestClass]
    public class ObservableSortedSetTest
    {
        [TestMethod]
        public void Constructors()
        {
            var target1 = new ObservableSortedSet<int>(10);
            var target2 = new ObservableSortedSet<int>(10, new Comparer<int>());
            var target3 = new ObservableSortedSet<int>();
            var target4 = new ObservableSortedSet<int>(new Comparer<int>());
            var target5 = new ObservableSortedSet<int>(new List<int>());
            var target6 = new ObservableSortedSet<int>(new List<int>(), new Comparer<int>());

            Assert.IsFalse(target6.IsReadOnly);
        }

        [TestMethod]
        public void BasicAdd()
        {
            var expectedResult = new List<int> {12, 92};

            var sortedSet = new ObservableSortedSet<int>();

            bool inserted = true;
            int index = 0;
            int addedItem = 0;

            sortedSet.CollectionChanged += (s, e) =>
                                               {
                                                   Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Add);

                                                   if (inserted)
                                                   {
                                                       Assert.IsNotNull(e.NewItems);
                                                       Assert.AreEqual(index, e.NewStartingIndex);
                                                   }
                                                   else
                                                   {
                                                       Assert.Fail();
                                                   }
                                               };

            inserted = true;
            index = 0;
            addedItem = 12;
            sortedSet.Add(addedItem);

            inserted = true;
            index = 1;
            addedItem = 92;
            sortedSet.Add(addedItem);

            inserted = false;
            addedItem = 12;
            sortedSet.Add(addedItem);

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);
        }

        [TestMethod]
        public void BasicRemove()
        {
            var sortedSet = new ObservableSortedSet<int>();

            int removedItem = 0;
            int removedIndex = 0;

            sortedSet.CollectionChanged += (s, e) =>
                                               {
                                                   if (e.Action == NotifyCollectionChangedAction.Add)
                                                   {
                                                       return;
                                                   }

                                                   Assert.IsNotNull(e.OldItems);
                                                   Assert.AreEqual(removedIndex, e.OldStartingIndex);
                                                   Assert.AreEqual(removedItem, e.OldItems[0]);
                                               };


            sortedSet.Add(12);
            sortedSet.Add(92);
            sortedSet.Add(13);
            sortedSet.Add(13);
            sortedSet.Add(13);
            sortedSet.Add(1);

            removedItem = 13;
            removedIndex = 2;
            Assert.IsTrue(sortedSet.Remove(removedItem));

            Assert.IsFalse(sortedSet.Remove(removedItem));

            removedItem = 92;
            removedIndex = 2;
            Assert.IsTrue(sortedSet.Remove(92));

            var expectedResult = new List<int> {1, 12};

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);
        }

        [TestMethod]
        public void BasicUnionWith()
        {
            var sortedSet = new ObservableSortedSet<int>();

            sortedSet.CollectionChanged += (s, e) =>
                                               {
                                                   Assert.IsTrue((e.Action == NotifyCollectionChangedAction.Add) ||
                                                                 (e.Action == NotifyCollectionChangedAction.Remove));
                                               };

            sortedSet.Add(13);
            sortedSet.Add(92);

            var set2 = new List<int> {12, 13, 14};

            var expectedResult = new List<int> {12, 13, 14, 92};

            sortedSet.UnionWith(set2);

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);
        }

        [TestMethod]
        public void BasicClear()
        {
            var sortedSet = new ObservableSortedSet<int>();

            sortedSet.CollectionChanged += (s, e) =>
                                               {
                                                   if (e.Action == NotifyCollectionChangedAction.Add)
                                                   {
                                                       return;
                                                   }

                                                   Assert.IsTrue(e.Action == NotifyCollectionChangedAction.Reset);
                                               };

            sortedSet.Add(13);
            sortedSet.Add(92);

            sortedSet.Clear();

            Assert.IsTrue(sortedSet.Count == 0);
        }

        [TestMethod]
        public void SortedOtherButNotThis()
        {
            var other = new List<int> {2, 4, 5, 7, 9, 10, 11};
            var set = new ObservableSortedSet<int> {1, 3, 4, 6, 7, 8, 9, 10};

            int actualNumRemoved = 0;
            int actualNumAdded = 0;

            set.CollectionChanged += (sender, args) =>
                                         {
                                             if (args.Action == NotifyCollectionChangedAction.Remove)
                                             {
                                                 actualNumRemoved += args.OldItems.Count;
                                             }
                                             else if (args.Action == NotifyCollectionChangedAction.Add)
                                             {
                                                 actualNumAdded += args.NewItems.Count;
                                             }
                                             else
                                             {
                                                 Assert.Fail("Only expecting Remove action");
                                             }
                                         };

            set.SortedOtherButNotThis(other, 3, 9);

            int expectedNumRemoved = 3;
            int expectedNumAdded = 3;
            var expectedResult = new List<int> {1, 2, 4, 5, 7, 9, 10, 11};

            Assert.AreEqual(expectedNumRemoved, actualNumRemoved);
            Assert.AreEqual(expectedNumAdded, actualNumAdded);

            SELAssert.AreCollectionsEqual(expectedResult, set);
        }

        [TestMethod]
        public void SortedOtherButNotThis_ICollection()
        {
            var target = new ObservableSortedSet<int>();
            target.SortedOtherButNotThis(new List<int>());

            target.SortedOtherButNotThis(new List<int> {5});
        }

        [TestMethod]
        public void SortedOtherButNotThis_Array()
        {
            var target = new ObservableSortedSet<int>();
            target.SortedOtherButNotThis(new int[0]);

            target.SortedOtherButNotThis(new[] {5});
        }

        [TestMethod]
        public void UnsortedOtherButNotThis_IEnumerable()
        {
            var target = new ObservableSortedSet<int>();

            target.UnsortedOtherButNotThis(SortedSetTest.UnsortedInput, 5, 10);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void SortedOtherButNotThis_ArgumentException()
        {
            var target = new ObservableSortedSet<int>();
            target.SortedOtherButNotThis(SortedSetTest.SortedInput.ToArray(), 10, 5);
        }

        [TestMethod]
        public void UnSortedSortedOtherButNotThis()
        {
            var other = new List<int> {12, 4, 9, 7, 5, 10, 11, 2};
            var set = new ObservableSortedSet<int> {1, 3, 4, 6, 7, 8, 9, 10, 13};

            int actualNumRemoved = 0;
            int actualNumAdded = 0;

            set.CollectionChanged += (sender, args) =>
                                         {
                                             if (args.Action == NotifyCollectionChangedAction.Remove)
                                             {
                                                 actualNumRemoved += args.OldItems.Count;
                                             }
                                             else if (args.Action == NotifyCollectionChangedAction.Add)
                                             {
                                                 actualNumAdded += args.NewItems.Count;
                                             }
                                             else
                                             {
                                                 Assert.Fail("Only expecting Remove action");
                                             }
                                         };

            set.SortedOtherButNotThis(other, 3, 9);

            int expectedNumRemoved = 5;
            int expectedNumAdded = 6;
            var expectedResult = new List<int> {1, 2, 4, 5, 7, 9, 10, 11, 12, 13};

            Assert.AreEqual(expectedNumRemoved, actualNumRemoved);
            Assert.AreEqual(expectedNumAdded, actualNumAdded);

            SELAssert.AreCollectionsEqual(expectedResult, set);
        }

        [TestMethod]
        public void EmptySortedOtherButNotThis()
        {
            var other = new List<int>();
            var set = new ObservableSortedSet<int> {1, 3, 4, 6, 7, 8, 9, 10, 13};

            int actualNumRemoved = 0;
            int actualNumAdded = 0;

            set.CollectionChanged += (sender, args) =>
                                         {
                                             if (args.Action == NotifyCollectionChangedAction.Remove)
                                             {
                                                 actualNumRemoved += args.OldItems.Count;
                                             }
                                             else if (args.Action == NotifyCollectionChangedAction.Add)
                                             {
                                                 actualNumAdded += args.NewItems.Count;
                                             }
                                             else
                                             {
                                                 Assert.Fail("Only expecting Remove action");
                                             }
                                         };

            set.SortedOtherButNotThis(other, 3, 9);

            var expectedResult = new List<int> {1, 10, 13};

            int expectedNumRemoved = 6;
            int expectedNumAdded = 0;

            Assert.AreEqual(expectedNumRemoved, actualNumRemoved);
            Assert.AreEqual(expectedNumAdded, actualNumAdded);

            SELAssert.AreCollectionsEqual(expectedResult, set);
        }

        [TestMethod]
        public void UnSortedOtherButNotEmptyThis()
        {
            var other = new List<int> {12, 4, 9, 7, 5, 10, 11, 2};
            var set = new ObservableSortedSet<int>();

            int actualNumRemoved = 0;
            int actualNumAdded = 0;

            set.CollectionChanged += (sender, args) =>
                                         {
                                             if (args.Action == NotifyCollectionChangedAction.Remove)
                                             {
                                                 actualNumRemoved += args.OldItems.Count;
                                             }
                                             else if (args.Action == NotifyCollectionChangedAction.Add)
                                             {
                                                 actualNumAdded += args.NewItems.Count;
                                             }
                                             else
                                             {
                                                 Assert.Fail("Only expecting Remove action");
                                             }
                                         };

            set.SortedOtherButNotThis(other, 3, 9);

            int expectedNumRemoved = 0;
            int expectedNumAdded = other.Count;
            var expectedResult = new List<int>(other.OrderBy(t => t, Comparer<int>.Default));

            Assert.AreEqual(expectedNumRemoved, actualNumRemoved);
            Assert.AreEqual(expectedNumAdded, actualNumAdded);

            SELAssert.AreCollectionsEqual(expectedResult, set);
        }

        [TestMethod]
        public void SortedOtherButNotThis_StartGreaterThanThis()
        {
            var other = new List<int> {2, 4, 5, 7, 9, 10, 11};
            var set = new ObservableSortedSet<int> {1, 3, 4, 6, 7, 8, 9, 10};

            int actualNumRemoved = 0;
            int actualNumAdded = 0;

            set.CollectionChanged += (sender, args) =>
                                         {
                                             if (args.Action == NotifyCollectionChangedAction.Remove)
                                             {
                                                 actualNumRemoved += args.OldItems.Count;
                                             }
                                             else if (args.Action == NotifyCollectionChangedAction.Add)
                                             {
                                                 actualNumAdded += args.NewItems.Count;
                                             }
                                             else
                                             {
                                                 Assert.Fail("Only expecting Remove action");
                                             }
                                         };

            set.SortedOtherButNotThis(other, 11, 12);

            int expectedNumRemoved = 0;
            int expectedNumAdded = 3;
            var expectedResult = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};

            Assert.AreEqual(expectedNumRemoved, actualNumRemoved);
            Assert.AreEqual(expectedNumAdded, actualNumAdded);

            SELAssert.AreCollectionsEqual(expectedResult, set);
        }

        [TestMethod]
        public void SortedOtherButNotThis_EndGreaterThanThis()
        {
            var other = new List<int> {2, 4, 5, 7, 9, 11};
            var set = new ObservableSortedSet<int> {1, 3, 4, 6, 7, 8, 9, 10};

            int actualNumRemoved = 0;
            int actualNumAdded = 0;

            set.CollectionChanged += (sender, args) =>
                                         {
                                             if (args.Action == NotifyCollectionChangedAction.Remove)
                                             {
                                                 actualNumRemoved += args.OldItems.Count;
                                             }
                                             else if (args.Action == NotifyCollectionChangedAction.Add)
                                             {
                                                 actualNumAdded += args.NewItems.Count;
                                             }
                                             else
                                             {
                                                 Assert.Fail("Only expecting Remove action");
                                             }
                                         };

            set.SortedOtherButNotThis(other, 10, 13);

            int expectedNumRemoved = 1;
            int expectedNumAdded = 3;
            var expectedResult = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 11};

            Assert.AreEqual(expectedNumRemoved, actualNumRemoved);
            Assert.AreEqual(expectedNumAdded, actualNumAdded);

            SELAssert.AreCollectionsEqual(expectedResult, set);
        }

        [TestMethod]
        public void RemoveAll()
        {
            var target = new ObservableSortedSet<int>(SortedSetTest.UnsortedInput);

            int value = target[target.Count / 2];

            target.RemoveAll(x => x < value);

            Assert.AreEqual(value, target[0]);
        }

        [TestMethod]
        public void ResetContents()
        {
            var target = new ObservableSortedSet<int>();
            target.SuspendCollectionChangeNotification();
            target.ResetContents(SortedSetTest.UnsortedInput);

            SELAssert.AreCollectionsEqual(SortedSetTest.SortedInput, target);
        }

        [TestMethod]
        public void Suspend_And_Resume()
        {
            var target = new ObservableSortedSet<int>();
            target.SuspendCollectionChangeNotification();

            target.Add(5);

            int numAdded = 0;

            target.CollectionChanged += (sender, args) =>
                                            {
                                                if (args.Action == NotifyCollectionChangedAction.Reset)
                                                {
                                                    ++numAdded;
                                                    //SELAssert.AreCollectionsEqual(new int[] { 5 }, args.NewItems as IEnumerable<int>);
                                                }
                                                else
                                                {
                                                    Assert.Fail("Only expecting Remove action");
                                                }
                                            };

            Assert.AreEqual(0, numAdded);

            target.ResumeCollectionChangeNotification();

            Assert.AreEqual(1, numAdded);
        }
    }
}