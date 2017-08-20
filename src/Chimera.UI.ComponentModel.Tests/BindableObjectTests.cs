using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Chimera.UI.ComponentModel.Tests
{
    public sealed class BindableObjectTests
    {
        [Fact]
        public void Type1SetValueGetValue()
        {
            var properties = new List<string>();

            using (var bindableObject = new TestType1BindableObject())
            {
                bindableObject.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);
                bindableObject.ValueNormal = 42;

                Assert.Single(properties);
                Assert.Equal(nameof(bindableObject.ValueNormal), properties[0]);
                Assert.Equal(42, bindableObject.ValueNormal);
                Assert.Equal(42, bindableObject.PureValue);
            }
        }

        [Fact]
        public void Type1SetValueWithSetHandler()
        {
            var properties = new List<string>();
            var invoked = false;
            var setHandler = (Action<string>)(n => invoked = true);

            using (var bindableObject = new TestType1BindableObject(setHandler))
            {
                bindableObject.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);
                bindableObject.ValueNormal = 42;

                Assert.Single(properties);
                Assert.Equal(nameof(bindableObject.ValueNormal), properties[0]);
                Assert.Equal(42, bindableObject.ValueNormal);
                Assert.Equal(42, bindableObject.PureValue);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void Type1SetValueWithActionCallback()
        {
            var properties = new List<string>();
            var invoked = false;
            var setHandler = (Action)(() => invoked = true);

            using (var bindableObject = new TestType1BindableObject(setHandler))
            {

                bindableObject.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);
                bindableObject.ValueWithActionCallback = 42;

                Assert.Single(properties);
                Assert.Equal(nameof(bindableObject.ValueWithActionCallback), properties[0]);
                Assert.Equal(42, bindableObject.ValueWithActionCallback);
                Assert.Equal(42, bindableObject.PureValue);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void Type2SetValueGetValue()
        {
            var properties = new List<string>();

            using (var bindableObject = new TestType2BindableObject(new TestBusinessObject2()))
            {
                bindableObject.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);
                bindableObject.ValueNormal = 42;

                Assert.Single(properties);
                Assert.Equal(nameof(bindableObject.ValueNormal), properties[0]);
                Assert.Equal(42, bindableObject.ValueNormal);
                Assert.Equal(42, bindableObject.PureValue);
            }
        }

        [Fact]
        public void Type2SetValueWithSetHandler()
        {
            var properties = new List<string>();
            var invoked = false;
            var setHandler = (Action<string>)(n => invoked = true);

            using (var bindableObject = new TestType2BindableObject(new TestBusinessObject2(), setHandler))
            {

                bindableObject.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);
                bindableObject.ValueNormal = 42;

                Assert.Single(properties);
                Assert.Equal(nameof(bindableObject.ValueNormal), properties[0]);
                Assert.Equal(42, bindableObject.ValueNormal);
                Assert.Equal(42, bindableObject.PureValue);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void Type2SetValueWithActionCallback()
        {
            var properties = new List<string>();
            var invoked = false;
            var setHandler = (Action)(() => invoked = true);

            using (var bindableObject = new TestType2BindableObject(new TestBusinessObject2(), setHandler))
            {

                bindableObject.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);
                bindableObject.ValueWithActionCallback = 42;

                Assert.Single(properties);
                Assert.Equal(nameof(bindableObject.ValueWithActionCallback), properties[0]);
                Assert.Equal(42, bindableObject.ValueWithActionCallback);
                Assert.Equal(42, bindableObject.PureValue);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void Type2SetValueWhenPropertyNameIsInvalid()
        {
            using (var bindableObject = new TestType2BindableObject(new TestBusinessObject2()))
            {
                Assert.Throws<InvalidOperationException>(
                    () => bindableObject.ValueWithInvalidPropertyName = 42);
            }
        }

        #region Test Types

        private sealed class TestType1BindableObject : BindableObject
        {
            private readonly Action<string> _handler;

            private readonly Action _callbackAction;
            private readonly Func<Task> _callbackFunc;

            private int _value;

            public TestType1BindableObject()
            {
            }

            public TestType1BindableObject(Action<string> handler)
            {
                _handler = handler;
            }

            public TestType1BindableObject(Action callback)
            {
                _callbackAction = callback;
            }

            public TestType1BindableObject(Func<Task> callback)
            {
                _callbackFunc = callback;
            }

            protected override void OnPropertyChanged(string propertyName)
            {
                base.OnPropertyChanged(propertyName);

                _handler?.Invoke(propertyName);
            }

            public int PureValue => _value;

            public int ValueNormal
            {
                get => GetValue(ref _value);
                set => SetValue(ref _value, value);
            }

            public int ValueWithActionCallback
            {
                get => GetValue(ref _value);
                set => SetValue(ref _value, value, _callbackAction);
            }
        }

        private abstract class TestBusinessObject1
        {
            public int Value { get; set; }
        }

        private sealed class TestBusinessObject2 : TestBusinessObject1
        {
        }

        private sealed class TestType2BindableObject : BindableObject
        {
            private readonly TestBusinessObject2 _object;

            private readonly Action<string> _handler;

            private readonly Action _callbackAction;
            private readonly Func<Task> _callbackFunc;

            public TestType2BindableObject(TestBusinessObject2 @object)
            {
                _object = @object;
            }

            public TestType2BindableObject(TestBusinessObject2 @object, Action<string> handler)
            {
                _object = @object;
                _handler = handler;
            }

            public TestType2BindableObject(TestBusinessObject2 @object, Action callback)
            {
                _object = @object;
                _callbackAction = callback;
            }

            public TestType2BindableObject(TestBusinessObject2 @object, Func<Task> callback)
            {
                _object = @object;
                _callbackFunc = callback;
            }

            protected override void OnPropertyChanged(string propertyName)
            {
                base.OnPropertyChanged(propertyName);

                _handler?.Invoke(propertyName);
            }

            public int PureValue => _object.Value;

            public int ValueNormal
            {
                get => GetValue(_object, nameof(_object.Value), default(int));
                set => SetValue(_object, nameof(_object.Value), value);
            }

            public int ValueWithActionCallback
            {
                get => GetValue(_object, nameof(_object.Value), default(int));
                set => SetValue(_object, nameof(_object.Value), value, _callbackAction);
            }

            public int ValueWithInvalidPropertyName
            {
                get => GetValue(_object, "property_0", default(int));
                set => SetValue(_object, "property_0", value);
            }
        }

        #endregion
    }
}