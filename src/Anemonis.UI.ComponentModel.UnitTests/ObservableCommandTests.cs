using System;
using System.ComponentModel;
using Anemonis.UI.ComponentModel.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class ObservableCommandTests : UnitTest
    {
        [TestMethod]
        public void SubscribeToNotifyPropertyChangedWhenObservableIsNull()
        {
            var command = new ObservableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.Subscribe(default(INotifyPropertyChanged)));
        }

        [TestMethod]
        public void SubscribeToNotifyPropertyChangedWhenPropertyNamesIsNull()
        {
            var command = new ObservableCommand<object>(p => { });
            var bindable = new TestBindableObject<object>(null);

            Assert.Throws<ArgumentNullException>(() =>
                command.Subscribe(bindable, null));
        }

        [TestMethod]
        public void SubscribeToObservableWhenObservableIsNull()
        {
            var command = new ObservableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.Subscribe(default(IObserver<EventArgs>)));
        }

        [TestMethod]
        public void UnsubscribeFromNotifyPropertyChangedWhenObservableIsNull()
        {
            var command = new ObservableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.Unsubscribe(default(INotifyPropertyChanged)));
        }

        [TestMethod]
        public void CanExecuteChangedWhenObservingPropertyChanged()
        {
            var result = false;
            var bindable = new TestBindableObject<int>(0);
            var command = new ObservableCommand<object>(p => { });

            command.Subscribe(bindable);
            command.CanExecuteChanged += (sender, e) => result = true;
            bindable.BindableFieldValue = 1;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RaiseCanExecuteChanged()
        {
            var result = false;
            var observer = new TestObserverObject<EventArgs>(a => result = true);
            var command = new ObservableCommand<object>(p => { });

            command.Subscribe(observer);
            command.RaiseCanExecuteChanged();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RaiseCanExecuteChangedWhenSubscriptionTokenIsDisposed()
        {
            var result = false;
            var observer = new TestObserverObject<EventArgs>(a => result = true);
            var command = new ObservableCommand<object>(p => { });

            var subscription = command.Subscribe(observer);

            subscription.Dispose();
            command.RaiseCanExecuteChanged();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DisposeSubscriptionTokenMoreThanOneTime()
        {
            var observer = new TestObserverObject<EventArgs>(a => { });
            var command = new ObservableCommand<object>(p => { });

            var subscription = command.Subscribe(observer);

            subscription.Dispose();
            subscription.Dispose();
        }
    }
}