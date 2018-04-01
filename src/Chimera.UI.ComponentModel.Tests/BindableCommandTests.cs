using System;
using Chimera.UI.ComponentModel.Tests.Objects;
using Xunit;

namespace Chimera.UI.ComponentModel.Tests
{
    public sealed class BindableCommandTests
    {
        private static void EmptyAction(object parameter)
        {
        }

        [Fact]
        public void CreateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand(null));
        }

        [Fact]
        public void StartObservingPropertiesWhenBindableObjectIsUndefined()
        {
            var target = new ObservingObject<int>();
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StartObservingProperties(nameof(target.Value)));
        }

        [Fact]
        public void StartObservingPropertiesWhenPropertyNameIsInvalid()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StartObservingProperties("*"));
        }

        [Fact]
        public void StopObservingPropertiesWhenBindableObjectIsUndefined()
        {
            var target = new ObservingObject<int>();
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StopObservingProperties(nameof(target.Value)));
        }

        [Fact]
        public void StopObservingPropertiesWhenPropertyNameIsInvalid()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StopObservingProperties("*"));
        }

        [Fact]
        public void Execute()
        {
            var invoked = false;
            var action = (Action<object>)(x => invoked = true);

            using (var bindable = new BindableCommand(action) as IBindableCommand)
            {
                Assert.True(bindable.CanExecute(null));

                bindable.Execute(null);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void ExecuteWithPredicate()
        {
            var invoked = false;
            var allowed = false;
            var action = (Action<object>)(x => invoked = true);
            var predicate = (Predicate<object>)(x => allowed);

            using (var bindable = new BindableCommand(action, predicate) as IBindableCommand)
            {
                Assert.False(bindable.CanExecute(null));

                allowed = true;

                Assert.True(bindable.CanExecute(null));

                bindable.Execute(null);
            }

            Assert.True(invoked);
        }

        [Fact]
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

                Assert.Equal(1, invocations);
            }

            Assert.Equal(1, invocations);
        }
    }
}