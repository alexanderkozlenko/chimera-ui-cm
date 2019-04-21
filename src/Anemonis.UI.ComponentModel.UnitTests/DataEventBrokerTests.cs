using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable IDE0039

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
                broker.Subscribe<object>(null, a => { }));
        }

        [TestMethod]
        public void SubscribeWhenEventHandlerIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Subscribe<object>("pipe", null));
        }

        [TestMethod]
        public void SubscribeWhenEventHandlerIsSubscribed()
        {
            var broker = new DataEventBroker();
            var eventHandler = (Action<DataEventArgs<object>>)(a => { });

            broker.Subscribe("pipe", eventHandler);
            broker.Subscribe("pipe", eventHandler);
        }

        [TestMethod]
        public void UnsubscribeWhenChannelNameIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Unsubscribe<object>(null, a => { }));
        }

        [TestMethod]
        public void UnsubscribeWhenEventHandlerIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Unsubscribe<object>("pipe", null));
        }

        [TestMethod]
        public void UnsubscribeWhenEventHandlerIsUnsubscribed()
        {
            var broker = new DataEventBroker();
            var eventHandler = (Action<DataEventArgs<object>>)(a => { });

            broker.Unsubscribe("pipe", eventHandler);
        }

        [TestMethod]
        public void PublishWhenChannelNameIsNull()
        {
            var broker = new DataEventBroker();

            Assert.Throws<ArgumentNullException>(() =>
                broker.Publish<object>(null, null));
        }

        [TestMethod]
        public void PublishWhenThereAreNoSubscribers()
        {
            var broker = new DataEventBroker();

            broker.Publish("pipe", 42);
        }

        [TestMethod]
        public void Publish()
        {
            var result = default(string);
            var broker = new DataEventBroker();
            var eventHandler = (Action<DataEventArgs<int>>)(a => result = a.ChannelName + "-" + a.Value);

            broker.Subscribe("pipe", eventHandler);
            broker.Publish("pipe", 42);

            Assert.AreEqual("pipe-42", result);
        }

        [TestMethod]
        public void PublishWhenChannelCountIsGreaterThanOne()
        {
            var result1 = false;
            var result2 = false;
            var broker = new DataEventBroker();
            var eventHandler1 = (Action<DataEventArgs<object>>)(v => result1 = true);
            var eventHandler2 = (Action<DataEventArgs<object>>)(v => result2 = true);

            broker.Subscribe("pipe-1", eventHandler1);
            broker.Subscribe("pipe-2", eventHandler2);
            broker.Publish<object>("pipe-1", null);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void PublishWhenChannelDataTypeIsGreaterThanOne()
        {
            var result1 = false;
            var result2 = false;
            var broker = new DataEventBroker();
            var eventHandler1 = (Action<DataEventArgs<string>>)(v => result1 = true);
            var eventHandler2 = (Action<DataEventArgs<int>>)(v => result2 = true);

            broker.Subscribe("pipe", eventHandler1);
            broker.Subscribe("pipe", eventHandler2);
            broker.Publish<string>("pipe", null);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void PublishWhenSubscriberIsUnsubscribed()
        {
            var result = false;
            var broker = new DataEventBroker();
            var eventHandler = (Action<DataEventArgs<object>>)(v => result = true);

            broker.Subscribe("pipe", eventHandler);
            broker.Unsubscribe("pipe", eventHandler);
            broker.Publish<object>("pipe", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PublishWhenBrokerIsDisposed()
        {
            var result = false;
            var broker = new DataEventBroker();
            var eventHandler = (Action<DataEventArgs<object>>)(v => result = true);

            broker.Subscribe("pipe", eventHandler);
            broker.Dispose();
            broker.Publish<object>("pipe", null);

            Assert.IsFalse(result);
        }
    }
}

#pragma warning restore IDE0039