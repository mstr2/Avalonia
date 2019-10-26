using System;

namespace Avalonia
{
    [Flags]
    public enum FrameworkPropertyMetadataOptions : int
    {
        None = 0x000,
        AffectsMeasure = 0x001,
        AffectsArrange = 0x002,
        AffectsParentMeasure = 0x004,
        AffectsParentArrange = 0x008,
        Inherits = 0x020,
        BindsTwoWayByDefault = 0x100
    }
}
