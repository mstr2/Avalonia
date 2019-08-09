// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Linq;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Avalonia.Base.UnitTests
{
    public class AvaloniaPropertyRegistryTests
    {
        public AvaloniaPropertyRegistryTests(ITestOutputHelper s)
        {
            // Ensure properties are registered.
            DependencyProperty p;
            p = Class1.FooProperty;
            p = Class2.BarProperty;
            p = AttachedOwner.AttachedProperty;
        }

        [Fact]
        public void Registered_Properties_Count_Reflects_Newly_Added_Attached_Property()
        {
            var registry = new DependencyPropertyRegistry();
            var metadata = new StyledPropertyMetadata<int>();
            var property = new AttachedProperty<int>("test", typeof(object), metadata, true);
            registry.Register(typeof(object), property);
            registry.RegisterAttached(typeof(AvaloniaPropertyRegistryTests), property);
            property.AddOwner<Class4>();

            Assert.Equal(1, registry.Properties.Count);
        }

        [Fact]
        public void GetRegistered_Returns_Registered_Properties()
        {
            string[] names = DependencyPropertyRegistry.Instance.GetRegistered(typeof(Class1))
                .Select(x => x.Name)
                .ToArray();

            Assert.Equal(new[] { "Foo", "Baz", "Qux" }, names);
        }

        [Fact]
        public void GetRegistered_Returns_Registered_Properties_For_Base_Types()
        {
            string[] names = DependencyPropertyRegistry.Instance.GetRegistered(typeof(Class2))
                .Select(x => x.Name)
                .ToArray();

            Assert.Equal(new[] { "Bar", "Flob", "Fred", "Foo", "Baz", "Qux" }, names);
        }

        [Fact]
        public void GetRegisteredAttached_Returns_Registered_Properties()
        {
            string[] names = DependencyPropertyRegistry.Instance.GetRegisteredAttached(typeof(Class1))
                .Select(x => x.Name)
                .ToArray();

            Assert.Equal(new[] { "Attached" }, names);
        }

        [Fact]
        public void GetRegisteredAttached_Returns_Registered_Properties_For_Base_Types()
        {
            string[] names = DependencyPropertyRegistry.Instance.GetRegisteredAttached(typeof(Class2))
                .Select(x => x.Name)
                .ToArray();

            Assert.Equal(new[] { "Attached" }, names);
        }

        [Fact]
        public void FindRegistered_Finds_Property()
        {
            var result = DependencyPropertyRegistry.Instance.FindRegistered(typeof(Class1), "Foo");

            Assert.Equal(Class1.FooProperty, result);
        }

        [Fact]
        public void FindRegistered_Doesnt_Find_Nonregistered_Property()
        {
            var result = DependencyPropertyRegistry.Instance.FindRegistered(typeof(Class1), "Bar");

            Assert.Null(result);
        }

        [Fact]
        public void FindRegistered_Finds_Unqualified_Attached_Property_On_Registering_Type()
        {
            var result = DependencyPropertyRegistry.Instance.FindRegistered(typeof(AttachedOwner), "Attached");

            Assert.Same(AttachedOwner.AttachedProperty, result);
        }

        [Fact]
        public void FindRegistered_Finds_AddOwnered_Attached_Property()
        {
            var result = DependencyPropertyRegistry.Instance.FindRegistered(typeof(Class3), "Attached");

            Assert.Same(AttachedOwner.AttachedProperty, result);
        }

        [Fact]
        public void FindRegistered_Doesnt_Find_Non_AddOwnered_Attached_Property()
        {
            var result = DependencyPropertyRegistry.Instance.FindRegistered(typeof(Class2), "Attached");

            Assert.Null(result);
        }

        private class Class1 : AvaloniaObject
        {
            public static readonly StyledProperty<string> FooProperty =
                DependencyProperty.Register<Class1, string>("Foo");

            public static readonly StyledProperty<string> BazProperty =
                DependencyProperty.Register<Class1, string>("Baz");

            public static readonly StyledProperty<int> QuxProperty =
                DependencyProperty.Register<Class1, int>("Qux");
        }

        private class Class2 : Class1
        {
            public static readonly StyledProperty<string> BarProperty =
                DependencyProperty.Register<Class2, string>("Bar");

            public static readonly StyledProperty<double> FlobProperty =
                DependencyProperty.Register<Class2, double>("Flob");

            public static readonly StyledProperty<double?> FredProperty =
                DependencyProperty.Register<Class2, double?>("Fred");
        }

        private class Class3 : Class1
        {
            public static readonly AttachedProperty<string> AttachedProperty =
                AttachedOwner.AttachedProperty.AddOwner<Class3>();
        }

        private class AttachedOwner : Class1
        {
            public static readonly AttachedProperty<string> AttachedProperty =
                DependencyProperty.RegisterAttached<AttachedOwner, Class1, string>("Attached");
        }

        private class AttachedOwner2 : AttachedOwner
        {
        }

        private class Class4 : AvaloniaObject
        {
        }
    }
}
