// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Xunit;

namespace Avalonia.Base.UnitTests
{
    class TestObject : AvaloniaObject
    {
        public static readonly DependencyProperty Prop1Property = DependencyProperty.Register(
            "Prop1", typeof(int), typeof(TestObject), new PropertyMetadata(Prop1Changed));

        private static void Prop1Changed(IAvaloniaObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((TestObject)obj).ChangeCount++;
        }

        public int ChangeCount;
        
        public int Prop1
        {
            get { return (int)GetValue(Prop1Property); }
            set { SetValue(Prop1Property, value); }
        }
    }

    public class UntypedPropertyTests
    {
        [Fact]
        void PropertyChangedCallback_Is_Called()
        {
            TestObject obj = new TestObject();
            Assert.Equal(0, obj.ChangeCount);

            obj.Prop1 = 5;
            Assert.Equal(1, obj.ChangeCount);

            obj.Prop1 = 5;
            Assert.Equal(1, obj.ChangeCount);

            obj.Prop1 = 7;
            Assert.Equal(2, obj.ChangeCount);
        }
    }
}
