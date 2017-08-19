using System;
using System.Threading;
using Xunit;

namespace Chimera.UI.ComponentModel.Tests
{
    public sealed class BindableCommandTests
    {
        [Fact]
        public void CreateWithActionWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(default(Action<object>), default(Predicate<object>), default(SynchronizationContext)));
        }

        [Fact]
        public void CreateWithActionAndContextWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(default(Action<object>), default(Predicate<object>), new SynchronizationContext()));
        }

        [Fact]
        public void CreateWithActionAndPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(default(Action<object>), o => true, default(SynchronizationContext)));
        }

        [Fact]
        public void CreateWithActionAndPredicateAndContextWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(default(Action<object>), o => true, new SynchronizationContext()));
        }

        [Fact]
        public void CreateWithObjectAndActionWhenObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(default(IBindableObject), o => { }));
        }

        [Fact]
        public void CreateWithObjectAndActionWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(new TestBindableObject(), null));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateWhenObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(default(IBindableObject), o => { }, o => true));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(new TestBindableObject(), null, o => true));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateAndContextWhenObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(default(IBindableObject), o => { }, o => true, new SynchronizationContext()));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateAndContextWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new BindableCommand(new TestBindableObject(), null, o => true, new SynchronizationContext()));
        }

        [Fact]
        public void TrackPropertyWhenPropertyNameIsNull()
        {
            var command = new BindableCommand(o => { });

            Assert.Throws<ArgumentNullException>(
                () => command.StartTrackingProperty(null));
        }

        [Fact]
        public void TrackPropertyWhenBindableObjectIsUndefined()
        {
            var @object = new TestBusinessObject();
            var command = new BindableCommand(o => { });

            Assert.Throws<InvalidOperationException>(
                () => command.StartTrackingProperty(nameof(@object.Value)));
        }

        [Fact]
        public void UntrackPropertyWhenPropertyNameIsNull()
        {
            var command = new BindableCommand(o => { });

            Assert.Throws<ArgumentNullException>(
                () => command.StopTrackingProperty(null));
        }

        [Fact]
        public void UntrackPropertyWhenBindableObjectIsUndefined()
        {
            var @object = new TestBusinessObject();
            var command = new BindableCommand(o => { });

            Assert.Throws<InvalidOperationException>(
                () => command.StopTrackingProperty(nameof(@object.Value)));
        }

        [Fact]
        public void Execute()
        {
            var invoked = false;

            using (var command = new BindableCommand(o => invoked = true) as IBindableCommand)
            {
                Assert.True(command.CanExecute(null));

                command.Execute(null);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void ExecuteWithCurrentContext()
        {
            var invoked = false;

            using (var command = new BindableCommand(o => invoked = true, default(Predicate<object>), SynchronizationContext.Current) as IBindableCommand)
            {
                Assert.True(command.CanExecute(null));

                command.Execute(null);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void ExecuteWithPredicate()
        {
            var invoked = false;

            using (var command = new BindableCommand(o => invoked = true, o => true) as IBindableCommand)
            {
                Assert.True(command.CanExecute(null));

                command.Execute(null);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void ExecuteWithPredicateAndCurrentContext()
        {
            var invoked = false;

            using (var command = new BindableCommand(o => invoked = true, o => true, SynchronizationContext.Current) as IBindableCommand)
            {
                Assert.True(command.CanExecute(null));

                command.Execute(null);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void ExecuteWithTrackingProperty()
        {
            var @object = new TestBindableObject();
            var invocations = 0;

            using (var command = new BindableCommand(@object, o => { }))
            {
                command.CanExecuteChanged += (sender, e) => invocations++;

                var tpr = command.StartTrackingProperty(nameof(@object.Value));

                Assert.Equal(command, tpr);

                @object.Value = 42;

                Assert.Equal(1, invocations);

                var upr = command.StopTrackingProperty(nameof(@object.Value));

                Assert.Equal(command, upr);

                @object.Value = null;
            }

            Assert.Equal(1, invocations);
        }

        #region Test Types

        private sealed class TestBusinessObject
        {
            public object Value { get; set; }
        }

        private sealed class TestBindableObject : BindableObject
        {
            private object _value;

            public object Value
            {
                get => _value;
                set => SetValue(ref _value, value);
            }
        }

        #endregion
    }
}