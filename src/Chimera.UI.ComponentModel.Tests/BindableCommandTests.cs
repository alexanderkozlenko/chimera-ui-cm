using System;
using Xunit;

namespace Chimera.UI.ComponentModel.Tests
{
    public sealed class BindableCommandTests
    {
        [Fact]
        public void CreateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand(null));
        }

        [Fact]
        public void StartTrackingPropertyWhenPropertyNameIsNull()
        {
            var bindable = new BindableCommand(x => { });

            Assert.Throws<ArgumentNullException>(() =>
                bindable.StartTrackingProperty(null));
        }

        [Fact]
        public void StartTrackingPropertyWhenBindableObjectIsUndefined()
        {
            var target = new TargetObject<int>();
            var bindable = new BindableCommand(x => { });

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StartTrackingProperty(nameof(target.Value)));
        }

        [Fact]
        public void StartTrackingPropertyWhenPropertyNameIsInvalid()
        {
            var target = new TargetObject<int>();
            var bindable = new BindableCommand(x => { });

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StartTrackingProperty("UnknownName"));
        }

        [Fact]
        public void StopTrackingPropertyWhenPropertyNameIsNull()
        {
            var bindable = new BindableCommand(x => { });

            Assert.Throws<ArgumentNullException>(() =>
                bindable.StopTrackingProperty(null));
        }

        [Fact]
        public void StopTrackingPropertyWhenBindableObjectIsUndefined()
        {
            var target = new TargetObject<int>();
            var bindable = new BindableCommand(x => { });

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StopTrackingProperty(nameof(target.Value)));
        }

        [Fact]
        public void StopTrackingPropertyWhenPropertyNameIsInvalid()
        {
            var target = new TargetObject<int>();
            var bindable = new BindableCommand(x => { });

            Assert.Throws<InvalidOperationException>(() =>
                bindable.StopTrackingProperty("UnknownName"));
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
            var target = new TargetObject<int>();
            var action = (Action<object>)(x => { });
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

        #region Test Types

        private sealed class TargetObject<T> : BindableObject
        {
            private T _value;

            public T Value
            {
                get => GetValue(ref _value);
                set => SetValue(ref _value, value);
            }
        }

        #endregion
    }
}