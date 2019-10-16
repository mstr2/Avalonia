using System;
using Xunit;

namespace Avalonia.Compatibility.UnitTests
{
    public class DependencyPropertyTests
    {
        class TestClass : DependencyObject
        {
            public static readonly DependencyProperty DefaultValueTestProperty = DependencyProperty.Register(
                "DefaultValueTest", typeof(double), typeof(TestClass));

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
