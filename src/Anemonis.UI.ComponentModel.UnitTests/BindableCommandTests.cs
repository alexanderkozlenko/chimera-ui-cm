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
        public void CanExecute()
        {
            var command = new BindableCommand<object>(p => { }, p => true);

            Assert.IsTrue(((IBindableCommand)command).CanExecute(null));
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
        public void Execute()
        {
            var invoked = false;
            var command = new BindableCommand<object>(p => invoked = true, p => true);

            ((IBindableCommand)command).Execute(null);

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void AddObservingObjectWhenObservableIsNull()
        {
            var command = new BindableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.AddObservingObject(null));
        }

        [TestMethod]
        public void AddObservingObjectWhenPropertyNamesIsNull()
        {
            var command = new BindableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.AddObservingObject(new TestBindableObject<object>(null), null));
        }

        [TestMethod]
        public void RemoveObservingObjectWhenObservableIsNull()
        {
            var command = new BindableCommand<object>(p => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.RemoveObservingObject(null));
        }

        [TestMethod]
        public void ObservingPropertyChanged()
        {
            var invoked = false;
            var bindable = new TestBindableObject<int>(0);
            var command = new BindableCommand<object>(p => { });

            command.AddObservingObject(bindable);
            command.CanExecuteChanged += (sender, e) => invoked = true;
            bindable.BindableFieldValue = 1;

            Assert.IsTrue(invoked);
        }
    }
}