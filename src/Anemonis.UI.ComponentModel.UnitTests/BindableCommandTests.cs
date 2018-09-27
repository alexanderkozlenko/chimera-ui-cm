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
            var result = ((IBindableCommand)command).CanExecute(null);

            Assert.IsTrue(result);
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
            var result = false;
            var command = new BindableCommand<object>(p => result = true);

            ((IBindableCommand)command).Execute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExecuteWhenPredicateReturnsFalse()
        {
            var result = false;
            var command = new BindableCommand<object>(p => result = true, p => false);

            ((IBindableCommand)command).Execute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExecuteWhenPredicateReturnsTrue()
        {
            var result = false;
            var command = new BindableCommand<object>(p => result = true, p => true);

            ((IBindableCommand)command).Execute(null);

            Assert.IsTrue(result);
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
            var result = false;
            var bindable = new TestBindableObject<int>(0);
            var command = new BindableCommand<object>(p => { });

            command.SubscribePropertyChanged(bindable);
            command.CanExecuteChanged += (sender, e) => result = true;
            bindable.BindableFieldValue = 1;

            Assert.IsTrue(result);
        }
    }
}