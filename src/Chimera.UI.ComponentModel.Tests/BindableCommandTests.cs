using System;
using System.Threading;
using Xunit;

namespace Chimera.UI.ComponentModel.Tests
{
    public sealed class BindableCommandTests
    {
        static BindableCommandTests()
        {
            BindableCommand.RegisterFactory(() => new TestBindableCommand());
        }

        [Fact]
        public void RegisterFactoryWhenFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.RegisterFactory(null));
        }

        [Fact]
        public void CreateWithActionWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create(null));
        }

        [Fact]
        public void CreateWithActionAndContextWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create(null, new SynchronizationContext()));
        }

        [Fact]
        public void CreateWithActionAndPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create(null, o => true));
        }

        [Fact]
        public void CreateWithActionAndPredicateAndContextWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create(null, o => true, new SynchronizationContext()));
        }

        [Fact]
        public void CreateWithObjectAndActionWhenObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create((TestBindableObject)null, o => { }));
        }

        [Fact]
        public void CreateWithObjectAndActionWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create(new TestBindableObject(), null));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateWhenObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create((TestBindableObject)null, o => { }, o => true));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create(new TestBindableObject(), null, o => true));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateAndContextWhenObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create((TestBindableObject)null, o => { }, o => true, new SynchronizationContext()));
        }

        [Fact]
        public void CreateWithObjectAndActionAndPredicateAndContextWhenActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => BindableCommand.Create(new TestBindableObject(), null, o => true, new SynchronizationContext()));
        }

        [Fact]
        public void TrackPropertyWhenPropertyNameIsNull()
        {
            var command = new TestBindableCommand();

            Assert.Throws<ArgumentNullException>(
                () => command.StartTrackingProperty(null));
        }

        [Fact]
        public void TrackPropertyWhenBindableObjectIsUndefined()
        {
            var @object = new TestBusinessObject();
            var command = BindableCommand.Create(o => { });

            Assert.Throws<InvalidOperationException>(
                () => command.StartTrackingProperty(nameof(@object.Value)));
        }

        [Fact]
        public void UntrackPropertyWhenPropertyNameIsNull()
        {
            var command = new TestBindableCommand();

            Assert.Throws<ArgumentNullException>(
                () => command.StopTrackingProperty(null));
        }

        [Fact]
        public void UntrackPropertyWhenBindableObjectIsUndefined()
        {
            var @object = new TestBusinessObject();
            var command = new TestBindableCommand();

            Assert.Throws<InvalidOperationException>(
                () => command.StopTrackingProperty(nameof(@object.Value)));
        }

        [Fact]
        public void ExecuteDisposed()
        {
            var command = (BindableCommand)BindableCommand.Create(o => { });

            command.Dispose();

            Assert.Throws<ObjectDisposedException>(
                () => command.Execute(null));
        }

        [Fact]
        public void Execute()
        {
            var invoked = false;

            using (var command = (BindableCommand)BindableCommand.Create(o => invoked = true))
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

            using (var command = (BindableCommand)BindableCommand.Create(o => invoked = true, SynchronizationContext.Current))
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

            using (var command = (BindableCommand)BindableCommand.Create(o => invoked = true, o => true))
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

            using (var command = (BindableCommand)BindableCommand.Create(o => invoked = true, o => true, SynchronizationContext.Current))
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

            using (var command = (BindableCommand)BindableCommand.Create(@object, o => { }))
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

        private sealed class TestBindableCommand : BindableCommand
        {
        }

        #endregion
    }
}