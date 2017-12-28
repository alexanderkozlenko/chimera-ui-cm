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
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand(null, null));
        }

        [Fact]
        public void CreateWithActionAndPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand(null, o => true));
        }

        [Fact]
        public void CreateWithObjectAndActionWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand(null, null, new TestBindableObject()));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BindableCommand(null, o => true, new TestBindableObject()));
        }

        [Fact]
        public void TrackPropertyWhenPropertyNameIsNull()
        {
            var command = new BindableCommand(o => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.StartTrackingProperty(null));
        }

        [Fact]
        public void TrackPropertyWhenBindableObjectIsUndefined()
        {
            var @object = new TestBusinessObject();
            var command = new BindableCommand(o => { });

            Assert.Throws<InvalidOperationException>(() =>
                command.StartTrackingProperty(nameof(@object.Value)));
        }

        [Fact]
        public void UntrackPropertyWhenPropertyNameIsNull()
        {
            var command = new BindableCommand(o => { });

            Assert.Throws<ArgumentNullException>(() =>
                command.StopTrackingProperty(null));
        }

        [Fact]
        public void UntrackPropertyWhenBindableObjectIsUndefined()
        {
            var @object = new TestBusinessObject();
            var command = new BindableCommand(o => { });

            Assert.Throws<InvalidOperationException>(() =>
                command.StopTrackingProperty(nameof(@object.Value)));
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

            using (var command = new BindableCommand(o => invoked = true, null))
            {
                command.SynchronizationContext = SynchronizationContext.Current;

                Assert.True((command as IBindableCommand).CanExecute(null));

                (command as IBindableCommand).Execute(null);
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

            using (var command = new BindableCommand(o => invoked = true, o => true))
            {
                command.SynchronizationContext = SynchronizationContext.Current;

                Assert.True((command as IBindableCommand).CanExecute(null));

                (command as IBindableCommand).Execute(null);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void ExecuteWithTrackingProperty()
        {
            var @object = new TestBindableObject();
            var invocations = 0;

            using (var command = new BindableCommand(o => { }, null, @object))
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