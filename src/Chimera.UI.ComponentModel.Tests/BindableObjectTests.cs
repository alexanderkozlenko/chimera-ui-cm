using System;
using Chimera.UI.ComponentModel.Tests.Objects;
using Xunit;

namespace Chimera.UI.ComponentModel.Tests
{
    public sealed class BindableObjectTests
    {
        [Fact]
        public void Type1GetValue()
        {
            using (var bindable = new BindableObjectType1<int>(1))
            {
                var value = bindable.InvokeGetValue();

                Assert.Equal(1, value);
                Assert.Equal(1, bindable.Value);
            }
        }

        [Fact]
        public void Type1SetValue()
        {
            using (var bindable = new BindableObjectType1<int>(0))
            {
                Assert.PropertyChanged(bindable, "p", () => bindable.InvokeSetValue(1, null, "p"));
                Assert.Equal(1, bindable.Value);
            }
        }

        [Fact]
        public void Type1SetValueWithAction()
        {
            var invoked = false;
            var action = (Action)(() => invoked = true);

            using (var bindable = new BindableObjectType1<int>(0))
            {
                Assert.PropertyChanged(bindable, "p", () => bindable.InvokeSetValue(1, action, "p"));
                Assert.Equal(1, bindable.Value);
            }

            Assert.True(invoked);
        }

        [Fact]
        public void Type2GetValue()
        {
            var target = new ValueObjectLevel2<int>
            {
                Value1 = 1,
                Value2 = 2
            };

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                var value1 = bindable.InvokeGetValue(nameof(target.Value1), default(int));
                var value2 = bindable.InvokeGetValue(nameof(target.Value2), default(int));

                Assert.Equal(1, value1);
                Assert.Equal(2, value2);
            }
        }

        [Fact]
        public void Type2GetValueWhenPropertyNameIsNull()
        {
            var target = new ValueObjectLevel1<int>
            {
                Value1 = 1
            };

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.Throws<ArgumentNullException>(() =>
                    bindable.InvokeGetValue(null, default(int)));
            }
        }

        [Fact]
        public void Type2GetValueWhenPropertyNameIsInvalid()
        {
            var target = new ValueObjectLevel1<int>
            {
                Value1 = 1
            };

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.Throws<InvalidOperationException>(() =>
                    bindable.InvokeGetValue("Value3", default(int)));
            }
        }

        [Fact]
        public void Type2GetValueWhenTargetIsNull()
        {
            var target = default(ValueObjectLevel2<int>);

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                var value1 = bindable.InvokeGetValue(nameof(target.Value1), 1);
                var value2 = bindable.InvokeGetValue(nameof(target.Value2), 2);

                Assert.Equal(1, value1);
                Assert.Equal(2, value2);
            }
        }

        [Fact]
        public void Type2SetValue()
        {
            var target = new ValueObjectLevel2<int>();

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                Assert.PropertyChanged(bindable, "p1", () => bindable.InvokeSetValue(nameof(bindable.Value.Value1), 1, null, "p1"));
                Assert.PropertyChanged(bindable, "p2", () => bindable.InvokeSetValue(nameof(bindable.Value.Value2), 2, null, "p2"));
                Assert.Equal(1, bindable.Value.Value1);
                Assert.Equal(2, bindable.Value.Value2);
            }
        }

        [Fact]
        public void Type2SetValueWhenPropertyNameIsNull()
        {
            var target = new ValueObjectLevel1<int>();

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.Throws<ArgumentNullException>(() =>
                    bindable.InvokeSetValue(null, 1, null, "p1"));
            }
        }

        [Fact]
        public void Type2SetValueWhenPropertyNameIsInvalid()
        {
            var target = new ValueObjectLevel1<int>();

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.Throws<InvalidOperationException>(() =>
                    bindable.InvokeSetValue("Value3", 1, null, "p3"));
            }
        }

        [Fact]
        public void Type2SetValueWithAction()
        {
            var target = new ValueObjectLevel2<int>();
            var invoked1 = false;
            var invoked2 = false;
            var action1 = (Action)(() => invoked1 = true);
            var action2 = (Action)(() => invoked2 = true);

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                Assert.PropertyChanged(bindable, "p1", () => bindable.InvokeSetValue(nameof(bindable.Value.Value1), 1, action1, "p1"));
                Assert.PropertyChanged(bindable, "p2", () => bindable.InvokeSetValue(nameof(bindable.Value.Value2), 2, action2, "p2"));
                Assert.Equal(1, bindable.Value.Value1);
                Assert.Equal(2, bindable.Value.Value2);
            }

            Assert.True(invoked1);
            Assert.True(invoked2);
        }

        [Fact]
        public void Type2SetValueWhenTargetIsNull()
        {
            var target = default(ValueObjectLevel2<int>);

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                bindable.InvokeSetValue(nameof(target.Value1), 1, null, "p1");
                bindable.InvokeSetValue(nameof(target.Value2), 2, null, "p2");
            }
        }
    }
}