// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Avalonia
{
    public sealed class DependencyPropertyKey
    {
        public DependencyProperty DependencyProperty { get; }

        DependencyPropertyKey(DependencyProperty property)
        {
            DependencyProperty = property;
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            if (DependencyProperty == null)
            {
                throw new InvalidOperationException();
            }

            DependencyProperty.OverrideMetadata(forType, typeMetadata, this);
        }
    }
}
