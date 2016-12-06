using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.UnitTests
{
    [TestClass]
    public class DerivedPropertyUnitTest
    {
        [TestMethod]
        public void StringPropertySerialization()
        {
            const string expectedValue = "HelloWorld";

            var target = new Property<string>(expectedValue);

            var serializedResult = DataContractXmlSerializer<Property<string>>.ToString(target);

            Assert.IsNotNull(serializedResult);

            var deserializedResult = DataContractXmlSerializer<Property<string>>.FromString(serializedResult);

            Assert.AreEqual(expectedValue, deserializedResult.Value);
        }

        private class Dummy
        {
            public int Member;

            public override bool Equals(object obj)
            {
                return Member.Equals((obj as Dummy).Member);
            }
        }

        [TestMethod]
        public void ReferenceTypeProperty()
        {
            int changedCount = 0;

            var itself = new Dummy();

            var target = new Property<Dummy>(itself);
            target.Changed += () => ++changedCount;

            target.Value = itself;
            Assert.AreEqual(0, changedCount);

            target.Value = new Dummy();
            Assert.AreEqual(0, changedCount);

            target.Value = null;
            Assert.AreEqual(1, changedCount);

            target.Value = itself;
            Assert.AreEqual(2, changedCount);

            target.Value = new Dummy { Member = 1 };
            Assert.AreEqual(3, changedCount);
        }

        [TestMethod]
        public void ValueTypeProperty()
        {
            int changedCount = 0;

            var itself = 1;

            var target = new Property<int>(itself);
            target.Changed += () => { ++changedCount; };

            target.Value = 2;
            Assert.AreEqual(1, changedCount);

            target.Value = 2;
            Assert.AreEqual(1, changedCount);

            target.Value = itself;
            Assert.AreEqual(2, changedCount);
        }

        [TestMethod]
        public void PropertyToString()
        {
            var target = new Property<int>(1);
            
            Assert.AreEqual("1", target.ToString());
        }

        [TestMethod]
        public void DerivedProperty()
        {
            int changeCount = 0;

            var targetProperty = new Property<int>();

            targetProperty.Changed += () => ++changeCount;

            var collection = new ObservableCollection<int>();
            var property = new Property<int>();

            int recomputeCount = 0;

            using (new DerivedProperty<int>(targetProperty, () => ++recomputeCount, collection, property))
            {
                Assert.AreEqual(1, targetProperty.Value);

                collection.Add(5);
                Assert.AreEqual(2, targetProperty.Value);

                property.Value = 5;

                Assert.AreEqual(3, targetProperty.Value);

                Assert.AreEqual(changeCount, recomputeCount);
            }
        }

        [TestMethod]
        public void OnPropertyChanged()
        {
            var collection = new ObservableCollection<int>();
            var property = new Property<int>();

            int recomputeCount = 0;

            new OnPropertyChanged(() => ++recomputeCount, collection, property);

            Assert.AreEqual(0, recomputeCount);

            collection.Add(5);
            Assert.AreEqual(1, recomputeCount);

            property.Value = 5;

            Assert.AreEqual(2, recomputeCount);
        }
    }
}
