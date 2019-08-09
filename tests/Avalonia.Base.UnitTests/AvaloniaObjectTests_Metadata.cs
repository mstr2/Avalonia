// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Linq;
using System.Reactive.Linq;
using Xunit;

namespace Avalonia.Base.UnitTests
{
    public class AvaloniaObjectTests_Metadata
    {
        public AvaloniaObjectTests_Metadata()
        {
            // Ensure properties are registered.
            DependencyProperty p;
            p = Class1.FooProperty;
            p = Class2.BarProperty;
            p = AttachedOwner.AttachedProperty;
        }

        [Fact]
        public void IsSet_Returns_False_For_Unset_Property()
        {
            var target = new Class1();

            Assert.False(target.IsSet(Class1.FooProperty));
        }

        [Fact]
        public void IsSet_Returns_False_For_Set_Property()
        {
            var target = new Class1();

            target.SetValue(Class1.FooProperty, "foo");

            Assert.True(target.IsSet(Class1.FooProperty));
        }

        [Fact]
        public void IsSet_Returns_False_For_Cleared_Property()
        {
            var target = new Class1();

            target.SetValue(Class1.FooProperty, "foo");
            target.SetValue(Class1.FooProperty, DependencyProperty.UnsetValue);

            Assert.False(target.IsSet(Class1.FooProperty));
        }

        private class Class1 : AvaloniaObject
        {
            public static readonly StyledProperty<string> FooProperty =
                DependencyProperty.Register<Class1, string>("Foo");
        }

        private class Class2 : Class1
        {
            public static readonly StyledProperty<string> BarProperty =
                DependencyProperty.Register<Class2, string>("Bar");
        }

        private class AttachedOwner
        {
            public static readonly AttachedProperty<string> AttachedProperty =
                DependencyProperty.RegisterAttached<AttachedOwner, Class1, string>("Attached");
        }
    }
}
