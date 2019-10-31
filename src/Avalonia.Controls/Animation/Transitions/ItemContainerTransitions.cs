using Avalonia.Collections;

namespace Avalonia.Animation
{
    /// <summary>
    /// A collection of <see cref="IItemContainerTransition"/> definitions.
    /// </summary>
    public class ItemContainerTransitions : AvaloniaList<IItemContainerTransition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transitions"/> class.
        /// </summary>
        public ItemContainerTransitions()
        {
            ResetBehavior = ResetBehavior.Remove;
        }
    }
}
