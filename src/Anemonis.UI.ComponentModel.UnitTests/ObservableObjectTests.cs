using System;

using Anemonis.UI.ComponentModel.UnitTests.TestStubs;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class ObservableObjectTests : UnitTest
    {
        [TestMethod]
        public void SubscribeToObservableWhenObservableIsNull()
        {
            var observable = new TestObservableObject<object>();

            Assert.Throws<ArgumentNullException>(() =>
                observable.Subscribe(null));
        }

        [TestMethod]
        public void RaiseCanExecuteChanged()
        {
            var result = false;
            var observer = new TestObserverObject<EventArgs>(a => result = true);
            var observable = new TestObservableObject<object>();

            observable.Subscribe(observer);
            observable.InvokeRaisePropertyChanged("Value");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RaiseCanExecuteChangedWhenSubscriptionTokenIsDisposed()
        {
            var result = false;
            var observer = new TestObserverObject<EventArgs>(a => result = true);
            var observable = new TestObservableObject<object>();

            var subscription = observable.Subscribe(observer);

            subscription.Dispose();
            observable.InvokeRaisePropertyChanged("Value");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DisposeSubscriptionTokenMoreThanOneTime()
        {
            var observer = new TestObserverObject<EventArgs>(a => { });
            var command = new TestObservableObject<object>();

            var subscription = command.Subscribe(observer);

            subscription.Dispose();
            subscription.Dispose();
        }
    }
}
