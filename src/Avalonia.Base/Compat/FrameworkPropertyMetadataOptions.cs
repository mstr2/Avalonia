// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Avalonia.Compat
{
    [Flags]
    public enum FrameworkPropertyMetadataOptions : int
    {
        None = 0x000,
        AffectsMeasure = 0x001,
        AffectsArrange = 0x002,
        Inherits = 0x020,
        BindsTwoWayByDefault = 0x100,
        SubPropertiesDoNotAffectRender = 0x800,
    }
}
