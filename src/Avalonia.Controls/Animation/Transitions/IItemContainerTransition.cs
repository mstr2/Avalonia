using System;
using Avalonia.Controls;

namespace Avalonia.Animation
{
    public interface IItemContainerTransition
    {
        /// <summary>
        /// Applies the transition to the specified <see cref="ItemsControl"/>.
        /// </summary>
        IDisposable Apply(ItemsControl control, IClock clock, object oldValue, object newValue);
    }
}
