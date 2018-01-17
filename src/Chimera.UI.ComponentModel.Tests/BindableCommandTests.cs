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
        public void StartTrackingPropertyWhenPropertyNameIsNull()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<ArgumentNullException>(() =>
                bindable.StartTrackingProperty(null));
        }

        [Fact]
        public void StartTrackingPropertyWhenBindableObjectIsUndefined()
        {
            var target = new TrackingObject<int>();
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StartTrackingProperty(nameof(target.Value)));
        }

        [Fact]
        public void StartTrackingPropertyWhenPropertyNameIsInvalid()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StartTrackingProperty("*"));
        }

        [Fact]
        public void StopTrackingPropertyWhenPropertyNameIsNull()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<ArgumentNullException>(() =>
                bindable.StopTrackingProperty(null));
        }

        [Fact]
        public void StopTrackingPropertyWhenBindableObjectIsUndefined()
        {
            var target = new TrackingObject<int>();
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StopTrackingProperty(nameof(target.Value)));
        }

        [Fact]
        public void StopTrackingPropertyWhenPropertyNameIsInvalid()
        {
            var bindable = new BindableCommand(EmptyAction);

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StopTrackingProperty("*"));
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
        public void CanExecuteChangedByTrackingProperty()
        {
            var target = new TrackingObject<int>();
            var action = (Action<object>)(EmptyAction);
            var invocations = 0;

            using (var bindable = new BindableCommand(action, null, target))
            {
                bindable.CanExecuteChanged += (sender, e) => invocations++;

                var chain1 = bindable.StartTrackingProperty(nameof(target.Value));

                target.Value = 1;

                var chain2 = bindable.StopTrackingProperty(nameof(target.Value));

                target.Value = 2;

                Assert.Equal(bindable, chain1);
                Assert.Equal(bindable, chain2);
                Assert.Equal(1, invocations);
            }

            Assert.Equal(1, invocations);
        }
    }
}