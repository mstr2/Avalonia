using System;
using Xunit;
using DependencyObject = Avalonia.AvaloniaObject;
using DependencyPropertyChangedEventArgs = Avalonia.AvaloniaPropertyChangedEventArgs;

namespace Avalonia.Compatibility.UnitTests
{
    public class DependencyPropertyTests
    {
        class TestClass : DependencyObject
        {
            public static readonly DependencyProperty DefaultValueTestProperty = DependencyProperty.Register(
                "DefaultValueTest", typeof(double), typeof(TestClass));

            public static readonly DependencyPropertyKey ReadOnlyTestPropertyKey = DependencyProperty.RegisterReadOnly(
                "ReadOnlyTest", typeof(double), typeof(TestClass), new PropertyMetadata());

            public static readonly DependencyProperty ReadOnlyTestProperty = ReadOnlyTestPropertyKey.DependencyProperty;

            public static readonly DependencyProperty ValidateValueTestProperty = DependencyProperty.Register(
                "ValidateValueTest", typeof(double), typeof(TestClass), new PropertyMetadata(), ValidateValueNotNegative);

            public static readonly DependencyProperty ValueChangedTestProperty = DependencyProperty.Register(
                "ValueChangedTest", typeof(int), typeof(TestClass), new PropertyMetadata(ValueChanged));

            public static readonly DependencyProperty CoerceTestProperty = DependencyProperty.Register(
                "CoerceTest", typeof(int), typeof(TestClass), new PropertyMetadata(0, null, CoerceValue));

            static bool ValidateValueNotNegative(object value)
            {
                return value is double && (double)value >= 0;
            }

            static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((TestClass)d).ValueChangedCalls++;
            }

            static object CoerceValue(DependencyObject d, object baseValue)
            {
                return Math.Max(0, Math.Min(10, (int)baseValue));
            }

            public int ValueChangedCalls;
        }
        
        [Fact]
        public void DependencyProperty_Assumes_Correct_DefaultValue_If_Not_Specified_In_Metadata()
        {
            var testClass = new TestClass();
            Assert.Equal(0.0, testClass.GetValue(TestClass.DefaultValueTestProperty));
        }

        [Fact]
        public void DependencyProperty_Cannot_Be_Registered_With_Invalid_DefaultValue()
        {
            Assert.Throws<ArgumentException>(
                () => DependencyProperty.Register(
                    "DefaultValueTest", typeof(double), typeof(TestClass), new PropertyMetadata(0)));
        }

        [Fact]
        public void DependencyProperty_Value_Cannot_Be_Set_To_Invalid_Type()
        {
            var testClass = new TestClass();
            Assert.Throws<ArgumentException>(
                () => testClass.SetValue(TestClass.DefaultValueTestProperty, 0));
        }

        [Fact]
        public void Changing_ReadOnly_DependencyProperty_Without_Key_Fails()
        {
            var testClass = new TestClass();

            Assert.Throws<InvalidOperationException>(
                () => testClass.SetValue(TestClass.ReadOnlyTestProperty, 1.0));

            Assert.Throws<InvalidOperationException>(() =>
                testClass.Bind(
                    TestClass.ReadOnlyTestProperty,
                    testClass.GetObservable(TestClass.DefaultValueTestProperty)));
        }

        [Fact]
        public void Changing_ReadOnly_DependencyProperty_With_Key_Succeeds()
        {
            var testClass = new TestClass();

            testClass.SetValue(TestClass.ReadOnlyTestPropertyKey, 1.0);
            Assert.Equal(1.0, testClass.GetValue(TestClass.ReadOnlyTestProperty));

            testClass.Bind(
                TestClass.ReadOnlyTestPropertyKey,
                testClass.GetObservable(TestClass.DefaultValueTestProperty));
            Assert.Equal(0.0, testClass.GetValue(TestClass.ReadOnlyTestProperty));
        }

        [Fact]
        public void DependencyProperty_Value_Validation_Throws_If_Invalid()
        {
            var testClass = new TestClass();
            Assert.Throws<ArgumentException>(
                () => testClass.SetValue(TestClass.ValidateValueTestProperty, -1.0));
        }

        [Fact]
        public void DependencyProperty_ValueChangedCallback_Is_Invoked()
        {
            var testClass = new TestClass();
            testClass.SetValue(TestClass.ValueChangedTestProperty, 0);
            testClass.SetValue(TestClass.ValueChangedTestProperty, 1);
            testClass.SetValue(TestClass.ValueChangedTestProperty, 2);
            testClass.SetValue(TestClass.ValueChangedTestProperty, 2);
            Assert.Equal(2, testClass.ValueChangedCalls);
        }

        [Fact]
        public void DependencyProperty_Value_Is_Coerced()
        {
            var testClass = new TestClass();

            testClass.SetValue(TestClass.CoerceTestProperty, 5);
            Assert.Equal(5, testClass.GetValue(TestClass.CoerceTestProperty));

            testClass.SetValue(TestClass.CoerceTestProperty, 11);
            Assert.Equal(10, testClass.GetValue(TestClass.CoerceTestProperty));

            testClass.SetValue(TestClass.CoerceTestProperty, -3);
            Assert.Equal(0, testClass.GetValue(TestClass.CoerceTestProperty));
        }
    }
}
