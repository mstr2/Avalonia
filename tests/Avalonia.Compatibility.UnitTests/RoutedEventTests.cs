using System;
using Avalonia.Interactivity;
using Xunit;

namespace Avalonia.Compatibility.UnitTests
{
    public class RoutedEventTests
    {
        class TestClass : Interactive
        {
            public static readonly RoutedEvent TestEvent = EventManager.RegisterRoutedEvent(
                "Test", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TestClass));

            public event RoutedEventHandler Test
            {
                add { AddHandler(TestEvent, value); }
                remove { RemoveHandler(TestEvent, value); }
            }
        }

        delegate void IllegalHandler();

        [Fact]
        public void Adding_Illegal_Handler_Type_Throws_Exception()
        {
            var testClass = new TestClass();
            Assert.Throws<ArgumentException>(
                () => testClass.AddHandler(TestClass.TestEvent, (IllegalHandler)(() => { })));
        }

        [Fact]
        public void RoutedEvent_Is_Raised_Correctly()
        {
            bool flag = false;

            var testClass = new TestClass();
            testClass.Test += (sender, e) =>
            {
                flag = true;
            };

            testClass.RaiseEvent(new RoutedEventArgs(TestClass.TestEvent));

            Assert.True(flag);
        }
    }
}
