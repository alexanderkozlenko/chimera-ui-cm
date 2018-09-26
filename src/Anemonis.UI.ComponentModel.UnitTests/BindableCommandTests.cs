using System;
using Anemonis.UI.ComponentModel.UnitTests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class BindableCommandTests : UnitTest
    {
        [TestMethod]
        public void ConstructorWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand<object>(default(Action<object>)));
        }

        [TestMethod]
        public void ConstructorWithPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand<object>(default(Action<object>), p => true));
        }

        [TestMethod]
        public void ConstructorWithPredicateWhenPredicateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand<object>(p => { }, default(Predicate<object>)));
        }

        [TestMethod]
        public void CanExecuteWhenPredicateIsNull()
        {
            var command = new BindableCommand<object>(p => { });

            Assert.IsTrue(((IBindableCommand)command).CanExecute(null));
        }

        [TestMethod]
        public void CanExecuteWhenPredicateReturnsFalse()
        {
            var command = new BindableCommand<object>(p => { }, p => false);
            var result = ((IBindableCommand)command).CanExecute(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanExecuteWhenPredicateReturnsTrue()
        {
            var command = new BindableCommand<object>(p => { }, p => true);
            var result = ((IBindableCommand)command).CanExecute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExecuteWhenPredicateIsNull()
        {
            var invoked = false;
            var command = new BindableCommand<object>(p => invoked = true);

            ((IBindableCommand)command).Execute(null);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ExecuteWhenPredicateReturnsFalse()
        {
            var invoked = false;
            var command = new BindableCommand<object>(p => invoked = true, p => false);

            ((IBindableCommand)command).Execute(null);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ExecuteWhenPredicateReturnsTrue()
        {
            var invoked = false;
            var command = new BindableCommand<object>(p => invoked = true, p => true);

            ((IBindableCommand)command).Execute(null);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void SubscribePropertyChangedWhenObservableIsNull()
        {
            var command = new BindableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.SubscribePropertyChanged(null));
        }

        [TestMethod]
        public void SubscribePropertyChangedWhenPropertyNamesIsNull()
        {
            var command = new BindableCommand<object>(p => { });
            var bindable = new TestBindableObject<object>(null);

            Assert.Throws<ArgumentNullException>(() =>
                command.SubscribePropertyChanged(bindable, null));
        }

        [TestMethod]
        public void UnsubscribePropertyChangedWhenObservableIsNull()
        {
            var command = new BindableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.UnsubscribePropertyChanged(null));
        }

        [TestMethod]
        public void CanExecuteChangedWhenObservingPropertyChanged()
        {
            var invoked = false;
            var bindable = new TestBindableObject<int>(0);
            var command = new BindableCommand<object>(p => { });

            command.SubscribePropertyChanged(bindable);
            command.CanExecuteChanged += (sender, e) => invoked = true;
            bindable.BindableFieldValue = 1;

            Assert.IsTrue(invoked);
        }
    }
}