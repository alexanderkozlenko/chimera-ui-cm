using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class DataEventBrokerTests : UnitTest
    {
        [TestMethod]
        public void SubscribeWhenChannelNameIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Subscribe<object>(null, e => { }));
        }

        [TestMethod]
        public void SubscribeWhenEventHandlerIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Subscribe<object>("channel-name", null));
        }

        [TestMethod]
        public void UnsubscribeWhenChannelNameIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Unsubscribe<object>(null, e => { }));
        }

        [TestMethod]
        public void UnsubscribeWhenEventHandlerIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Unsubscribe<object>("channel-name", null));
        }

        [TestMethod]
        public void PublishWhenChannelNameIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Publish<object>(null, null));
        }

        [TestMethod]
        public void Publish()
        {
            var result = false;
            var broker = new DataEventBroker();
            var eventHandler = (Action<string>)(e => result = true);

            broker.Subscribe("channel-name", eventHandler);
            broker.Publish<string>("channel-name", "value");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PublishWhenChannelCountGreaterThanOne()
        {
            var result1 = false;
            var result2 = false;
            var broker = new DataEventBroker();
            var eventHandler1 = (Action<object>)(e => result1 = true);
            var eventHandler2 = (Action<object>)(e => result2 = true);

            broker.Subscribe("channel-name-1", eventHandler1);
            broker.Subscribe("channel-name-2", eventHandler2);
            broker.Publish<object>("channel-name-1", null);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void PublishWhenChannelValueTypeGreaterThanOne()
        {
            var result1 = false;
            var result2 = false;
            var broker = new DataEventBroker();
            var eventHandler1 = (Action<string>)(e => result1 = true);
            var eventHandler2 = (Action<int>)(e => result2 = true);

            broker.Subscribe("channel-name", eventHandler1);
            broker.Subscribe("channel-name", eventHandler2);
            broker.Publish<string>("channel-name", null);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void PublishWhenSubscriberUnsubscribed()
        {
            var result = false;
            var broker = new DataEventBroker();
            var eventHandler = (Action<object>)(e => result = true);

            broker.Subscribe("channel-name", eventHandler);
            broker.Unsubscribe("channel-name", eventHandler);
            broker.Publish<object>("channel-name", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PublishWithWhenSubscriberThrowException()
        {
            var broker = new DataEventBroker();
            var eventHandler = (Action<object>)(e => throw new NotSupportedException());

            broker.Subscribe("channel-name", eventHandler);

            var asserter = Assert.Throws<AggregateException>(() =>
                broker.Publish<object>("channel-name", null));

            Assert.AreEqual(1, asserter.Exception.InnerExceptions.Count);
            Assert.IsInstanceOfType(asserter.Exception.InnerExceptions[0], typeof(NotSupportedException));
        }
    }
}