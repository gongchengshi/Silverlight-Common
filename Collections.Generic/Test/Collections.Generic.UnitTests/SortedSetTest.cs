///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEL.Collections.Generic;
using System.Collections.Generic;
using SEL.UnitTest;
using System.Collections;

namespace SEL.Collections.Generic.UnitTests
{
    [TestClass]
    public class SortedSetTest
    {
        public static readonly List<int> UnsortedInput = new List<int>
        {
            12, 92, 13, 1, 91, 93, 0, -1, 13, 13, 14
        };

        public static readonly SortedSet<int> Input = new SortedSet<int>(UnsortedInput);

        public static readonly List<int> SortedInput = new List<int>()
        {
            -1, 0, 1, 12, 13, 14, 91, 92, 93
        };

        [TestMethod]
        public void Constructors()
        {
            var target1 = new SortedSet<int>(10);
            var target2 = new SortedSet<int>(10, new Comparer<int>());
            var target3 = new SortedSet<int>();
            var target4 = new SortedSet<int>(new Comparer<int>());
            var target5 = new SortedSet<int>(new List<int>());
            var target6 = new SortedSet<int>(new List<int>(), new Comparer<int>());

            Assert.IsFalse(target6.IsReadOnly);
        }

        [TestMethod]
        public void Min_Max()
        {
            var target = new SortedSet<int>(new List<int> { 10, 5 });
            Assert.AreEqual(5, target.Min);
            Assert.AreEqual(10, target.Max);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Min_Throws()
        {
            var target = new SortedSet<int>(new List<int>());
            var result = target.Min;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Max_Throws()
        {
            var target = new SortedSet<int>(new List<int>());
            var result = target.Max;
        }

        [TestMethod]
        public void BasicAdd()
        {
            var sortedSet = new SortedSet<int>();
            Assert.IsTrue(sortedSet.Add(12));
            Assert.IsTrue(sortedSet.Add(92));
            Assert.IsTrue(sortedSet.Add(13));
            Assert.IsTrue(sortedSet.Add(1));
            Assert.IsTrue(sortedSet.Add(91));
            Assert.IsTrue(sortedSet.Add(93));
            Assert.IsTrue(sortedSet.Add(0));
            Assert.IsTrue(sortedSet.Add(-1));
            Assert.IsFalse(sortedSet.Add(13));
            Assert.IsFalse(sortedSet.Add(13));
            Assert.IsTrue(sortedSet.Add(14));
            Assert.IsFalse(sortedSet.Add(13));

            SELAssert.AreCollectionsEqual(SortedInput, sortedSet);
        }

        [TestMethod]
        public void ICollection_Add()
        {
            var target = new SortedSet<int>(Input);
            (target as ICollection<int>).Add(9999);
            Assert.IsTrue(target.Contains(9999));
        }

        [TestMethod]
        public void ExceptWith()
        {
            var target = new SortedSet<int>(Input);

            var expectedResult = new List<int>(SortedInput);

            target.ExceptWith(expectedResult);

            SELAssert.AreCollectionsEqual(new List<int>(), target);
        }

        [TestMethod]
        public void BasicRemove()
        {
            var sortedSet = new SortedSet<int>(Input);

            Assert.IsTrue(sortedSet.Remove(13));
            Assert.IsFalse(sortedSet.Remove(13));

            var expectedResult = new List<int>(SortedInput);
            expectedResult.Remove(13);

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);
        }

        [TestMethod]
        public void BasicUnionWith()
        {
            var sortedSet = new SortedSet<int>(Input);
            sortedSet.Remove(13);

            var expectedResult = new List<int>(SortedInput);

            sortedSet.UnionWith(expectedResult);

            SELAssert.AreCollectionsEqual(SortedInput, sortedSet);
        }

        [TestMethod]
        public void BasicRemoveAll()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.RemoveAll((s) => { return s < 14; });

            var expectedResult = new List<int> { 14, 91, 92, 93 };

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);
        }

        [TestMethod]
        public void BasicRemoveOpenOpen()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(1, 14, SortedSet.Bounds.OpenOpen);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x > 1) && (x < 14)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void BeginningRemoveOpenOpen()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(-2, 2, SortedSet.Bounds.OpenOpen);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x > -2) && (x < 2)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void EndBasicRemoveOpenOpen()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(15, 94, SortedSet.Bounds.OpenOpen);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x > 15) && (x < 94)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void BasicRemoveOpenClosed()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(1, 14, SortedSet.Bounds.OpenClosed);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x > 1) && (x <= 14)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void BeginningRemoveOpenClosed()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(-2, 2, SortedSet.Bounds.OpenClosed);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x > -2) && (x <= 2)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void EndBasicRemoveOpenClosed()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(15, 94, SortedSet.Bounds.OpenClosed);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x > 15) && (x <= 94)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void BasicRemoveClosedOpen()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(1, 14, SortedSet.Bounds.ClosedOpen);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x >= 1) && (x < 14)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void BeginningRemoveClosedOpen()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(-2, 2, SortedSet.Bounds.ClosedOpen);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x >= -2) && (x < 2)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void EndBasicRemoveClosedOpen()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(15, 94, SortedSet.Bounds.ClosedOpen);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x >= 15) && (x < 94)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void BasicRemoveAllClosedClosed()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(1, 14, SortedSet.Bounds.ClosedClosed);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x >= 1) && (x <= 14)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void BeginningRemoveAllClosedClosed()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(-2, 2, SortedSet.Bounds.ClosedClosed);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x >= -2) && (x <= 2)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void EndBasicRemoveClosedClosed()
        {
            var sortedSet = new SortedSet<int>(Input);

            sortedSet.Remove(15, 94, SortedSet.Bounds.ClosedClosed);

            var expectedResult = new List<int>(SortedInput);
            expectedResult.RemoveAll((x) => { return ((x >= 15) && (x <= 94)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void SingleItemRemoveOpenOpen()
        {
            var sortedSet = new SortedSet<int>() { 15 };

            sortedSet.Remove(15, 15, SortedSet.Bounds.OpenOpen);

            var expectedResult = new List<int>() { 15 };
            expectedResult.RemoveAll((x) => { return ((x > 15) && (x < 15)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void SingleItemRemoveOpenClosed()
        {
            var sortedSet = new SortedSet<int>() { 15 };

            sortedSet.Remove(15, 15, SortedSet.Bounds.OpenClosed);

            var expectedResult = new List<int>() { 15 };
            expectedResult.RemoveAll((x) => { return ((x > 15) && (x <= 15)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void SingleItemRemoveClosedOpen()
        {
            var sortedSet = new SortedSet<int>() { 15 };

            sortedSet.Remove(15, 15, SortedSet.Bounds.ClosedOpen);

            var expectedResult = new List<int>() { 15 };
            expectedResult.RemoveAll((x) => { return ((x >= 15) && (x < 15)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void SingleItemRemoveClosedClosed()
        {
            var sortedSet = new SortedSet<int>() { 15 };

            sortedSet.Remove(15, 15, SortedSet.Bounds.ClosedClosed);

            var expectedResult = new List<int>();
            expectedResult.RemoveAll((x) => { return ((x >= 15) && (x <= 15)); });

            SELAssert.AreCollectionsEqual(expectedResult, sortedSet);

            sortedSet.Print();
        }

        [TestMethod]
        public void CopyTo()
        {
            int[] toArray = new int[Input.Count];
            Input.CopyTo(toArray, 0);

            SELAssert.AreCollectionsEqual(Input, toArray);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throw()
        {
            var target = new SortedSet<string>();
            target.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Remove_Throw()
        {
            var target = new SortedSet<string>();
            target.Remove(null, "not null", SortedSet.Bounds.ClosedClosed);
            target.Remove("not null", null, SortedSet.Bounds.ClosedClosed);
        }

        [TestMethod]
        public void IndexOf()
        {
            var target = new SortedSet<int>(SortedInput);
            Assert.AreEqual(0, target.IndexOf(Input[0]));
        }

        [TestMethod]
        public void GetEnumerator_IEnumerable()
        {
            var target = (new SortedSet<int>() as IEnumerable).GetEnumerator();
        }

        #region NotImplementedMethods
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void IntersectWith()
        {
            Input.IntersectWith(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void IsProperSubsetOf()
        {
            Input.IsProperSubsetOf(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void IsProperSupersetOf()
        {
            Input.IsProperSupersetOf(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void IsSubsetOf()
        {
            Input.IsSubsetOf(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void IsSupersetOf()
        {
            Input.IsSupersetOf(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Overlaps()
        {
            Input.Overlaps(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SetEquals()
        {
            Input.SetEquals(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SymmetricExceptWith()
        {
            Input.SymmetricExceptWith(new int[0]);
        }
        #endregion
    }
}
