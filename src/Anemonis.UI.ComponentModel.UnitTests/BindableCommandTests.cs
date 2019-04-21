using System;

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
                new BindableCommand<object>(null));
        }

        [TestMethod]
        public void ConstructorWithPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand<object>(null, p => true));
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
            var result = command.CanExecute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanExecuteWhenPredicateReturnsFalse()
        {
            var command = new BindableCommand<object>(p => { }, p => false);
            var result = command.CanExecute(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanExecuteWhenPredicateReturnsTrue()
        {
            var command = new BindableCommand<object>(p => { }, p => true);
            var result = command.CanExecute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExecuteWhenPredicateIsNull()
        {
            var result = false;
            var command = new BindableCommand<object>(p => result = true);

            command.Execute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExecuteWhenPredicateReturnsFalse()
        {
            var result = false;
            var command = new BindableCommand<object>(p => result = true, p => false);

            command.Execute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExecuteWhenPredicateReturnsTrue()
        {
            var result = false;
            var command = new BindableCommand<object>(p => result = true, p => true);

            command.Execute(null);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RaiseCanExecuteChanged()
        {
            var result = false;
            var command = new ObservableCommand<object>(p => { });

            command.CanExecuteChanged += (sender, e) => result = true;
            command.RaiseCanExecuteChanged();

            Assert.IsTrue(result);
        }
    }
}