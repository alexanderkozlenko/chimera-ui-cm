using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class DataEventArgsTests : UnitTest
    {
        public void ConstructorWhenChannelNameIsNull()
        {
            var dataEventArgs = new DataEventArgs<int>(null, 42);

            Assert.IsNull(dataEventArgs.ChannelName);
        }

        [TestMethod]
        public void ChannelNameGet()
        {
            var dataEventArgs = new DataEventArgs<int>("pipe", default);

            Assert.AreEqual("pipe", dataEventArgs.ChannelName);
        }

        [TestMethod]
        public void ValueGet()
        {
            var dataEventArgs = new DataEventArgs<int>("pipe", 42);

            Assert.AreEqual(42, dataEventArgs.Value);
        }

        [TestMethod]
        public void EquatableEquals()
        {
            var dataEventArgs = new DataEventArgs<int>("pipe", 42);

            Assert.IsFalse(dataEventArgs.Equals(default(DataEventArgs<int>)));
            Assert.IsFalse(dataEventArgs.Equals(new DataEventArgs<int>(default, 42)));
            Assert.IsFalse(dataEventArgs.Equals(new DataEventArgs<int>("", 42)));
            Assert.IsFalse(dataEventArgs.Equals(new DataEventArgs<int>("pipe", default)));
            Assert.IsFalse(dataEventArgs.Equals(new DataEventArgs<int>("pipe", int.MaxValue)));
            Assert.IsTrue(dataEventArgs.Equals(new DataEventArgs<int>("pipe", 42)));
        }

        [TestMethod]
        public void ObjectEquals()
        {
            var dataEventArgs = new DataEventArgs<int>("pipe", 42);

            Assert.IsFalse(dataEventArgs.Equals(default(object)));
            Assert.IsFalse(dataEventArgs.Equals(new object()));
            Assert.IsFalse(dataEventArgs.Equals((object)default(DataEventArgs<int>)));
            Assert.IsFalse(dataEventArgs.Equals((object)new DataEventArgs<int>(default, 42)));
            Assert.IsFalse(dataEventArgs.Equals((object)new DataEventArgs<int>("", 42)));
            Assert.IsFalse(dataEventArgs.Equals((object)new DataEventArgs<int>("pipe", default)));
            Assert.IsFalse(dataEventArgs.Equals((object)new DataEventArgs<int>("pipe", int.MaxValue)));
            Assert.IsTrue(dataEventArgs.Equals((object)new DataEventArgs<int>("pipe", 42)));
        }

        [TestMethod]
        public void ObjectGetHashCode()
        {
            var hashCode = new DataEventArgs<int>("pipe", 42).GetHashCode();

            Assert.AreNotEqual(hashCode, default(DataEventArgs<int>).GetHashCode());
            Assert.AreNotEqual(hashCode, new DataEventArgs<int>(default, 42).GetHashCode());
            Assert.AreNotEqual(hashCode, new DataEventArgs<int>("", 42).GetHashCode());
            Assert.AreNotEqual(hashCode, new DataEventArgs<int>("pipe", default).GetHashCode());
            Assert.AreNotEqual(hashCode, new DataEventArgs<int>("pipe", int.MaxValue).GetHashCode());
            Assert.AreEqual(hashCode, new DataEventArgs<int>("pipe", 42).GetHashCode());
        }

        [TestMethod]
        public void OperatorEquality()
        {
            var dataEventArgs = new DataEventArgs<int>("pipe", 42);

            Assert.IsFalse(dataEventArgs == default(DataEventArgs<int>));
            Assert.IsFalse(dataEventArgs == new DataEventArgs<int>(default, 42));
            Assert.IsFalse(dataEventArgs == new DataEventArgs<int>("", 42));
            Assert.IsFalse(dataEventArgs == new DataEventArgs<int>("pipe", default));
            Assert.IsFalse(dataEventArgs == new DataEventArgs<int>("pipe", int.MaxValue));
            Assert.IsTrue(dataEventArgs == new DataEventArgs<int>("pipe", 42));
        }

        [TestMethod]
        public void OperatorInequality()
        {
            var dataEventArgs = new DataEventArgs<int>("pipe", 42);

            Assert.IsTrue(dataEventArgs != default(DataEventArgs<int>));
            Assert.IsTrue(dataEventArgs != new DataEventArgs<int>(default, 42));
            Assert.IsTrue(dataEventArgs != new DataEventArgs<int>("", 42));
            Assert.IsTrue(dataEventArgs != new DataEventArgs<int>("pipe", default));
            Assert.IsTrue(dataEventArgs != new DataEventArgs<int>("pipe", int.MaxValue));
            Assert.IsFalse(dataEventArgs != new DataEventArgs<int>("pipe", 42));
        }
    }
}
