using System;
using System.Threading;

using Anemonis.UI.ComponentModel.UnitTests.TestStubs;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.UI.ComponentModel.UnitTests
{
    [TestClass]
    public sealed class BindableObjectTests : UnitTest
    {
        [TestMethod]
        public void GetByField()
        {
            var bindable = new TestBindableObject<int>(1);
            var result = bindable.BindableFieldValue;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SetByField()
        {
            var bindable = new TestBindableObject<int>(0);

            Assert.PropertyChanged(bindable, o => o.BindableFieldValue, 1);

            bindable.BindableFieldValue = 1;

            Assert.AreEqual(1, bindable.FieldValue);
        }

        [TestMethod]
        public void SetByFieldWhenValueIsTheSame()
        {
            var bindable = new TestBindableObject<int>(1);

            Assert.PropertyNotChanged(bindable, o => o.BindableFieldValue, 1);

            bindable.BindableFieldValue = 1;

            Assert.AreEqual(1, bindable.FieldValue);
        }

        [TestMethod]
        public void SetByFieldWithCallback()
        {
            var invoked = false;
            var bindable = new TestBindableObject<int>(0, () => invoked = true);

            Assert.PropertyChanged(bindable, o => o.BindableFieldValue, 1);

            bindable.BindableFieldValue = 1;

            Assert.AreEqual(1, bindable.FieldValue);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void SetByFieldWithCallbackWhenValueIsTheSame()
        {
            var result = false;
            var bindable = new TestBindableObject<int>(1, () => result = true);

            Assert.PropertyNotChanged(bindable, o => o.BindableFieldValue, 1);

            bindable.BindableFieldValue = 1;

            Assert.AreEqual(1, bindable.FieldValue);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetByPropertyWhenPropertyNameIsInvalid()
        {
            var target = new TestTargetObject<int>(0);
            var bindable = new TestBindableObject<int>(target);
            var value = default(int);

            Assert.Throws<MissingMemberException>(() =>
                value = bindable.InvalidBindablePropertyValue);
        }

        [TestMethod]
        public void GetByProperty()
        {
            var target = new TestTargetObject<int>(1);
            var bindable = new TestBindableObject<int>(target);
            var result = bindable.BindablePropertyValue;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SetByPropertyWhenPropertyNameIsInvalid()
        {
            var target = new TestTargetObject<int>(0);
            var bindable = new TestBindableObject<int>(target);

            Assert.Throws<MissingMemberException>(() =>
                bindable.InvalidBindablePropertyValue = 1);
        }

        [TestMethod]
        public void SetByPropertyWhenValueIsTheSame()
        {
            var target = new TestTargetObject<int>(1);
            var bindable = new TestBindableObject<int>(target);

            Assert.PropertyNotChanged(bindable, o => o.BindablePropertyValue, 1);

            bindable.BindablePropertyValue = 1;

            Assert.AreEqual(1, bindable.PropertyValue);
        }

        [TestMethod]
        public void SetByPropertyWithCallback()
        {
            var result = false;
            var target = new TestTargetObject<int>(0);
            var bindable = new TestBindableObject<int>(target, () => result = true);

            Assert.PropertyChanged(bindable, o => o.BindablePropertyValue, 1);

            bindable.BindablePropertyValue = 1;

            Assert.AreEqual(1, bindable.PropertyValue);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SetByPropertyWithCallbackWhenValueIsTheSame()
        {
            var result = false;
            var target = new TestTargetObject<int>(1);
            var bindable = new TestBindableObject<int>(target, () => result = true);

            Assert.PropertyNotChanged(bindable, o => o.BindablePropertyValue, 1);

            bindable.BindablePropertyValue = 1;

            Assert.AreEqual(1, bindable.PropertyValue);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SetByProperty()
        {
            var target = new TestTargetObject<int>(0);
            var bindable = new TestBindableObject<int>(target);

            Assert.PropertyChanged(bindable, o => o.BindablePropertyValue, 1);

            bindable.BindablePropertyValue = 1;

            Assert.AreEqual(1, bindable.PropertyValue);
        }

        [TestMethod]
        public void RaisePropertyChanged()
        {
            var bindable = new TestBindableObject<int>(0);
            var propertyName = nameof(TestBindableObject<int>.BindableFieldValue);

            Assert.PropertyChanged(bindable, o => o.BindableFieldValue, 1);

            bindable.FieldValue = 1;
            bindable.InvokeRaisePropertyChanged(propertyName);
        }

        [TestMethod]
        public void Dispose()
        {
            var bindable = new TestBindableObject<int>(0);

            bindable.SynchronizationContext = new();
            bindable.Dispose();

            Assert.IsNotNull(bindable.SynchronizationContext);
        }
    }
}
