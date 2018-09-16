using System;
using Anemonis.UI.ComponentModel.UnitTests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class BindableCommandTests
    {
        private static void EmptyAction(object parameter)
        {
        }

        [TestMethod]
        public void CreateWhenActionIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new BindableCommand(null));
        }

        [TestMethod]
        public void StartObservingPropertiesWhenBindableObjectIsUndefined()
        {
            var target = new ObservingObject<int>();
            var bindable = new BindableCommand(EmptyAction);

            Assert.ThrowsException<InvalidOperationException>(() =>
                bindable.StartObservingProperties(nameof(target.Value)));
        }

        [TestMethod]
        public void StartObservingPropertiesWhenPropertyNameIsInvalid()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.ThrowsException<InvalidOperationException>(() =>
                bindable.StartObservingProperties("*"));
        }

        [TestMethod]
        public void StopObservingPropertiesWhenBindableObjectIsUndefined()
        {
            var target = new ObservingObject<int>();
            var bindable = new BindableCommand(EmptyAction);

            Assert.ThrowsException<InvalidOperationException>(() =>
                bindable.StopObservingProperties(nameof(target.Value)));
        }

        [TestMethod]
        public void StopObservingPropertiesWhenPropertyNameIsInvalid()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.ThrowsException<InvalidOperationException>(() =>
                bindable.StopObservingProperties("*"));
        }

        [TestMethod]
        public void Execute()
        {
            var invoked = false;
            var action = (Action<object>)(x => invoked = true);

            using (var bindable = new BindableCommand(action) as IBindableCommand)
            {
                Assert.IsTrue(bindable.CanExecute(null));

                bindable.Execute(null);
            }

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void ExecuteWithPredicate()
        {
            var invoked = false;
            var allowed = false;
            var action = (Action<object>)(x => invoked = true);
            var predicate = (Predicate<object>)(x => allowed);

            using (var bindable = new BindableCommand(action, predicate) as IBindableCommand)
            {
                Assert.IsFalse(bindable.CanExecute(null));

                allowed = true;

                Assert.IsTrue(bindable.CanExecute(null));

                bindable.Execute(null);
            }

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanExecuteChangedByObservingProperties()
        {
            var target = new ObservingObject<int>();
            var action = (Action<object>)(EmptyAction);
            var invocations = 0;

            using (var bindable = new BindableCommand(action, null, target))
            {
                bindable.CanExecuteChanged += (sender, e) => invocations++;

                bindable.StartObservingProperties(nameof(target.Value));

                target.Value = 1;

                bindable.StopObservingProperties(nameof(target.Value));

                target.Value = 2;

                Assert.AreEqual(1, invocations);
            }

            Assert.AreEqual(1, invocations);
        }
    }
}