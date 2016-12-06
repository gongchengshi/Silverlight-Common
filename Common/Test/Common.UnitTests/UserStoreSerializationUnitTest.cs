using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.UnitTests
{
    [TestClass]
    public class UserStoreSerializationUnitTest
    {
        [TestMethod]
        public void UserStoreSerializationTest()
        {
            var target = new UserStoreSerialization<string>("test");
            target.Serialize("Hello World");

            var deserialized = target.Deserialize();

            Assert.AreEqual("Hello World", deserialized);

            var deserializedInt = UserStoreSerialization<int>.Deserialize("test", true);

            Assert.AreEqual(default(int), deserializedInt);

            UserStoreSerialization.Delete("test");

            deserialized = UserStoreSerialization<string>.Deserialize("test", true);

            Assert.AreEqual(default(string), deserialized);
        }
    }
}
