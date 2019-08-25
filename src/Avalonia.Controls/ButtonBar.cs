// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Specialized;
using Avalonia.Controls.Utils;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Platform;

namespace Avalonia.Controls
{
    public enum ButtonKind
    {
        Help,
        Dialog,
        No,
        Yes,
        Cancel,
        OK,
        Action
    }

    public class ButtonBar : Panel, INavigableContainer
    {
        /// <summary>
        /// Defines the <see cref="Spacing"/> property.
        /// </summary>
        public static readonly StyledProperty<double> SpacingProperty =
            StackLayout.SpacingProperty.AddOwner<ButtonBar>();

        /// <summary>
        /// Defines the <see cref="Orientation"/> property.
        /// </summary>
        public static readonly StyledProperty<Orientation> OrientationProperty =
            StackLayout.OrientationProperty.AddOwner<ButtonBar>();

        /// <summary>
        /// Defines the <see cref="Order"/> property.
        /// </summary>
        public static readonly StyledProperty<ButtonKind[]> OrderProperty =
            AvaloniaProperty.Register<ButtonBar, ButtonKind[]>(nameof(Order), GetDefaultOrder());

        /// <summary>
        /// Defines the <see cref="SplitAfter"/> property.
        /// </summary>
        public static readonly StyledProperty<ButtonKind> SplitAfterProperty =
            AvaloniaProperty.Register<ButtonBar, ButtonKind>(nameof(SplitAfter), ButtonKind.Dialog);

        /// <summary>
        /// Defines the Kind attached property.
        /// </summary>
        public static readonly AttachedProperty<ButtonKind> KindProperty =
            AvaloniaProperty.RegisterAttached<ButtonBar, IControl, ButtonKind>("Kind", ButtonKind.Dialog);

        /// <summary>
        /// Initializes static members of the <see cref="ButtonBar"/> class.
        /// </summary>
        static ButtonBar()
        {
            OrderProperty.Changed.Subscribe(InvalidateMapping);
            SplitAfterProperty.Changed.Subscribe(InvalidateMapping);
            KindProperty.Changed.Subscribe(InvalidateMapping);

            AffectsMeasure<ButtonBar>(
                SpacingProperty, OrientationProperty, OrderProperty, SplitAfterProperty, KindProperty);
        }

        /// <summary>
        /// Gets or sets the size of the spacing to place between child controls.
        /// </summary>
        public double Spacing
        {
            get { return GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation in which child controls will be laid out.
        /// </summary>
        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public ButtonKind[] Order
        {
            get { return GetValue(OrderProperty); }
            set { SetValue(OrderProperty, value); }
        }

        public ButtonKind SplitAfter
        {
            get { return GetValue(SplitAfterProperty); }
            set { SetValue(SplitAfterProperty, value); }
        }

        private readonly OrderedList _orderedChildren;

        internal OrderedList OrderedChildren => _orderedChildren;

        public ButtonBar()
        {
            _orderedChildren= new OrderedList(this);
        }

        public static ButtonKind GetKind(IControl control)
        {
            return control.GetValue(KindProperty);
        }

        public static void SetKind(IControl control, ButtonKind ButtonKind)
        {
            control.SetValue(KindProperty, ButtonKind);
        }

        public static ButtonKind[] GetDefaultOrder(OperatingSystemType operatingSystem)
        {
            switch (operatingSystem)
            {
                case OperatingSystemType.OSX:
                case OperatingSystemType.iOS:
                    return new[]
                    {
                        ButtonKind.Help,
                        ButtonKind.Dialog,
                        ButtonKind.No,
                        ButtonKind.Yes,
                        ButtonKind.Cancel,
                        ButtonKind.OK,
                        ButtonKind.Action
                    };

                case OperatingSystemType.Linux:
                    return new[]
                    {
                        ButtonKind.Help,
                        ButtonKind.Dialog,
                        ButtonKind.No,
                        ButtonKind.Yes,
                        ButtonKind.Action,
                        ButtonKind.Cancel,
                        ButtonKind.OK
                    };

                default:
                    return new[]
                    {
                        ButtonKind.Help,
                        ButtonKind.Dialog,
                        ButtonKind.Yes,
                        ButtonKind.No,
                        ButtonKind.OK,
                        ButtonKind.Cancel,
                        ButtonKind.Action
                    };
            }
        }

        private static ButtonKind[] GetDefaultOrder()
        {
            var operatingSystem =
                AvaloniaLocator.Current.GetService<IRuntimePlatform>()?.GetRuntimeInfo().OperatingSystem;

            return GetDefaultOrder(operatingSystem ?? OperatingSystemType.Unknown);
        }

        private static void InvalidateMapping(AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Sender is ButtonBar)
            {
                ((ButtonBar)args.Sender)._orderedChildren.Invalidate();
            }
            else
            {
                var buttonBar = ((IControl)args.Sender).Parent as ButtonBar;
                buttonBar?._orderedChildren.Invalidate();
            }
        }

        /// <summary>
        /// Gets the next control in the specified direction.
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        /// <param name="from">The control from which movement begins.</param>
        /// <param name="wrap">Whether to wrap around when the first or last item is reached.</param>
        /// <returns>The control.</returns>
        IInputElement INavigableContainer.GetControl(NavigationDirection direction, IInputElement from, bool wrap)
        {
            var result = GetControlInDirection(direction, from as IControl);

            if (result == null && wrap)
            {
                if (Orientation == Orientation.Vertical)
                {
                    switch (direction)
                    {
                        case NavigationDirection.Up:
                        case NavigationDirection.Previous:
                        case NavigationDirection.PageUp:
                            result = GetControlInDirection(NavigationDirection.Last, null);
                            break;
                        case NavigationDirection.Down:
                        case NavigationDirection.Next:
                        case NavigationDirection.PageDown:
                            result = GetControlInDirection(NavigationDirection.First, null);
                            break;
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case NavigationDirection.Left:
                        case NavigationDirection.Previous:
                        case NavigationDirection.PageUp:
                            result = GetControlInDirection(NavigationDirection.Last, null);
                            break;
                        case NavigationDirection.Right:
                        case NavigationDirection.Next:
                        case NavigationDirection.PageDown:
                            result = GetControlInDirection(NavigationDirection.First, null);
                            break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the next control in the specified direction.
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        /// <param name="from">The control from which movement begins.</param>
        /// <returns>The control.</returns>
        protected virtual IInputElement GetControlInDirection(NavigationDirection direction, IControl from)
        {
            var horiz = Orientation == Orientation.Horizontal;
            int index = from != null ? _orderedChildren.IndexOf(from) : -1;

            switch (direction)
            {
                case NavigationDirection.First:
                    index = 0;
                    break;
                case NavigationDirection.Last:
                    index = Children.Count - 1;
                    break;
                case NavigationDirection.Next:
                    if (index != -1)
                        ++index;
                    break;
                case NavigationDirection.Previous:
                    if (index != -1)
                        --index;
                    break;
                case NavigationDirection.Left:
                    if (index != -1)
                        index = horiz ? index - 1 : -1;
                    break;
                case NavigationDirection.Right:
                    if (index != -1)
                        index = horiz ? index + 1 : -1;
                    break;
                case NavigationDirection.Up:
                    if (index != -1)
                        index = horiz ? -1 : index - 1;
                    break;
                case NavigationDirection.Down:
                    if (index != -1)
                        index = horiz ? -1 : index + 1;
                    break;
                default:
                    index = -1;
                    break;
            }

            if (index >= 0 && index < _orderedChildren.Count)
            {
                return _orderedChildren[index];
            }
            else
            {
                return null;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double parentWidth = 0;   // Our current required width due to children thus far.
            double parentHeight = 0;   // Our current required height due to children thus far.
            double accumulatedWidth = 0;   // Total width consumed by children.
            double accumulatedHeight = 0;   // Total height consumed by children.
            var orientation = Orientation;
            var spacing = Spacing;
            int visibleChildren = 0;

            for (int i = 0, count = _orderedChildren.Count; i < count; ++i)
            {
                var child = _orderedChildren[i];
                Size childConstraint;             // Contains the suggested input constraint for this child.
                Size childDesiredSize;            // Contains the return size from child measure.

                if (child == null || !child.IsVisible)
                {
                    continue;
                }

                ++visibleChildren;

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedWidth),
                                           Math.Max(0.0, constraint.Height - accumulatedHeight));

                child.Measure(childConstraint);
                childDesiredSize = child.DesiredSize;

                if (orientation == Orientation.Horizontal)
                {
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                    accumulatedWidth += childDesiredSize.Width;
                } else
                {
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                }
            }

            if (orientation == Orientation.Horizontal)
            {
                accumulatedWidth += Math.Max(0, visibleChildren - 1) * spacing;
            }
            else
            {
                accumulatedHeight += Math.Max(0, visibleChildren - 1) * spacing;
            }

            // Make sure the final accumulated size is reflected in parentSize.
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);

            return new Size(parentWidth, parentHeight);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var orientation = Orientation;
            double spacing = Spacing;
            int totalChildrenCount = _orderedChildren.Count;
            double accumulatedLeft = 0;
            double accumulatedTop = 0;
            double accumulatedRight = 0;
            double accumulatedBottom = 0;
            int splitIndex = -1;

            for (int i = 0; i < totalChildrenCount; ++i)
            {
                var child = splitIndex >= 0 ?
                    _orderedChildren.GetReversed(i - splitIndex) : _orderedChildren[i];

                if (child == null || !child.IsVisible)
                {
                    continue;
                }

                if (splitIndex < 0 && IsAfterSplit(child))
                {
                    splitIndex = i;
                    child = _orderedChildren.GetReversed(i - splitIndex);
                }

                Size childDesiredSize = child.DesiredSize;
                Rect rcChild = new Rect(
                    accumulatedLeft,
                    accumulatedTop,
                    Math.Max(0.0, arrangeSize.Width - (accumulatedLeft + accumulatedRight)),
                    Math.Max(0.0, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));

                if (i < totalChildrenCount)
                {
                    if (orientation == Orientation.Horizontal)
                    {
                        if (splitIndex >= 0)
                        {
                            accumulatedRight += childDesiredSize.Width + spacing;
                            rcChild = rcChild.WithX(Math.Max(0.0, arrangeSize.Width - accumulatedRight));
                            rcChild = rcChild.WithWidth(childDesiredSize.Width);
                        }
                        else
                        {
                            accumulatedLeft += childDesiredSize.Width + spacing;
                            rcChild = rcChild.WithWidth(childDesiredSize.Width);
                        }
                    }
                    else
                    {
                        if (splitIndex >= 0)
                        {
                            accumulatedBottom += childDesiredSize.Height + spacing;
                            rcChild = rcChild.WithY(Math.Max(0.0, arrangeSize.Height - accumulatedBottom));
                            rcChild = rcChild.WithHeight(childDesiredSize.Height);
                        }
                        else
                        {
                            accumulatedTop += childDesiredSize.Height + spacing;
                            rcChild = rcChild.WithHeight(childDesiredSize.Height);
                        }
                    }
                }

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }

        protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _orderedChildren.Invalidate();
            base.ChildrenChanged(sender, e);
        }

        private bool IsAfterSplit(IControl control)
        {
            var buttonKind = GetKind(control);
            var splitAfter = SplitAfter;

            foreach (var kind in Order)
            {
                if (kind == buttonKind)
                {
                    return false;
                }

                if (kind == splitAfter)
                {
                    return true;
                }
            }

            return true;
        }

        internal class OrderedList
        {
            private ButtonBar _buttonBar;
            private int[] _map;
            private bool _invalid = true;

            internal OrderedList(ButtonBar buttonBar)
            {
                _buttonBar = buttonBar;
                buttonBar.Children.CollectionChanged += (sender, args) => UpdateMapping();
                UpdateMapping();
            }

            public IControl this[int index]
            {
                get
                {
                    UpdateMapping();
                    return _buttonBar.Children[_map[index]];
                }
            }

            public int Count => _buttonBar.Children.Count;

            public IControl GetReversed(int index)
            {
                return _buttonBar.Children[_map[_map.Length - index - 1]];
            }

            public int IndexOf(IControl item)
            {
                UpdateMapping();
                return _map[_buttonBar.Children.IndexOf(item)];
            }

            public void Invalidate()
            {
                _invalid = true;
            }

            private void UpdateMapping()
            {
                if (!_invalid)
                {
                    return;
                }

                var children = _buttonBar.Children;
                var buttonOrder = _buttonBar.Order;
                int current = 0;

                _map = new int[children.Count];
                for (int i = 0; i < _map.Length; ++i)
                {
                    _map[i] = -1;
                }

                foreach (var kind in buttonOrder)
                {
                    for (int i = 0; i < _map.Length; ++i)
                    {
                        if (GetKind(children[i]) == kind)
                        {
                            _map[current++] = i;
                        }
                    }
                }

                _invalid = false;
                _buttonBar.InvalidateMeasure();
            }
        }
    }
}
