using System;
using Chimera.UI.ComponentModel.UnitTests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chimera.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class BindableObjectTests
    {
        [TestMethod]
        public void Type1GetValue()
        {
            using (var bindable = new BindableObjectType1<int>(1))
            {
                var value = bindable.InvokeGetValue();

                Assert.AreEqual(1, value);
                Assert.AreEqual(1, bindable.Value);
            }
        }

        [TestMethod]
        public void Type1SetValue()
        {
            using (var bindable = new BindableObjectType1<int>(0))
            {
                bindable.InvokeSetValue(1, null, "p");

                Assert.AreEqual(1, bindable.Value);
            }
        }

        [TestMethod]
        public void Type1SetValueWithAction()
        {
            var invoked = false;
            var action = (Action)(() => invoked = true);

            using (var bindable = new BindableObjectType1<int>(0))
            {
                bindable.InvokeSetValue(1, action, "p");

                Assert.AreEqual(1, bindable.Value);
            }

            Assert.IsTrue(invoked);
        }

        [TestMethod]
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

                Assert.AreEqual(1, value1);
                Assert.AreEqual(2, value2);
            }
        }

        [TestMethod]
        public void Type2GetValueWhenPropertyNameIsNull()
        {
            var target = new ValueObjectLevel1<int>
            {
                Value1 = 1
            };

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                    bindable.InvokeGetValue(null, default(int)));
            }
        }

        [TestMethod]
        public void Type2GetValueWhenPropertyNameIsInvalid()
        {
            var target = new ValueObjectLevel1<int>
            {
                Value1 = 1
            };

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                    bindable.InvokeGetValue("Value3", default(int)));
            }
        }

        [TestMethod]
        public void Type2GetValueWhenTargetIsNull()
        {
            var target = default(ValueObjectLevel2<int>);

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                var value1 = bindable.InvokeGetValue(nameof(target.Value1), 1);
                var value2 = bindable.InvokeGetValue(nameof(target.Value2), 2);

                Assert.AreEqual(1, value1);
                Assert.AreEqual(2, value2);
            }
        }

        [TestMethod]
        public void Type2SetValue()
        {
            var target = new ValueObjectLevel2<int>();

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                bindable.InvokeSetValue(nameof(bindable.Value.Value1), 1, null, "p1");
                bindable.InvokeSetValue(nameof(bindable.Value.Value2), 2, null, "p2");

                Assert.AreEqual(1, bindable.Value.Value1);
                Assert.AreEqual(2, bindable.Value.Value2);
            }
        }

        [TestMethod]
        public void Type2SetValueWhenPropertyNameIsNull()
        {
            var target = new ValueObjectLevel1<int>();

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                    bindable.InvokeSetValue(null, 1, null, "p1"));
            }
        }

        [TestMethod]
        public void Type2SetValueWhenPropertyNameIsInvalid()
        {
            var target = new ValueObjectLevel1<int>();

            using (var bindable = new BindableObjectType2<ValueObjectLevel1<int>>(target))
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                    bindable.InvokeSetValue("Value3", 1, null, "p3"));
            }
        }

        [TestMethod]
        public void Type2SetValueWithAction()
        {
            var target = new ValueObjectLevel2<int>();
            var invoked1 = false;
            var invoked2 = false;
            var action1 = (Action)(() => invoked1 = true);
            var action2 = (Action)(() => invoked2 = true);

            using (var bindable = new BindableObjectType2<ValueObjectLevel2<int>>(target))
            {
                bindable.InvokeSetValue(nameof(bindable.Value.Value1), 1, action1, "p1");
                bindable.InvokeSetValue(nameof(bindable.Value.Value2), 2, action2, "p2");

                Assert.AreEqual(1, bindable.Value.Value1);
                Assert.AreEqual(2, bindable.Value.Value2);
            }

            Assert.IsTrue(invoked1);
            Assert.IsTrue(invoked2);
        }

        [TestMethod]
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