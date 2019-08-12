// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Reflection;
using Avalonia.Data;
using Avalonia.Utilities;

namespace Avalonia
{
    /// <summary>
    /// Base class for dependency properties.
    /// </summary>
    public abstract partial class DependencyProperty : IEquatable<DependencyProperty>
    {
        /// <summary>
        /// Represents an unset property value.
        /// </summary>
        public static readonly object UnsetValue = new UnsetValueType();

        private static int s_nextId;
        private readonly Subject<DependencyPropertyChangedEventArgs> _initialized;
        private readonly Subject<DependencyPropertyChangedEventArgs> _changed;
        private readonly IPropertyMetadata _defaultMetadata;
        private readonly Dictionary<Type, IPropertyMetadata> _metadata;
        private readonly Dictionary<Type, IPropertyMetadata> _metadataCache = new Dictionary<Type, IPropertyMetadata>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyProperty"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="valueType">The type of the property's value.</param>
        /// <param name="ownerType">The type of the class that registers the property.</param>
        /// <param name="metadata">The property metadata.</param>
        /// <param name="notifying">A <see cref="Notifying"/> callback.</param>
        protected DependencyProperty(
            string name,
            Type valueType,
            Type ownerType,
            IPropertyMetadata metadata,
            Action<IAvaloniaObject, bool> notifying = null)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(valueType != null);
            Contract.Requires<ArgumentNullException>(ownerType != null);
            Contract.Requires<ArgumentNullException>(metadata != null);

            if (name.Contains("."))
            {
                throw new ArgumentException("'name' may not contain periods.");
            }

            _initialized = new Subject<DependencyPropertyChangedEventArgs>();
            _changed = new Subject<DependencyPropertyChangedEventArgs>();
            _metadata = new Dictionary<Type, IPropertyMetadata>();

            Name = name;
            PropertyType = valueType;
            OwnerType = ownerType;
            Notifying = notifying;
            GlobalIndex = s_nextId++;

            _metadata.Add(ownerType, metadata);
            _defaultMetadata = metadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyProperty"/> class.
        /// </summary>
        /// <param name="source">The direct property to copy.</param>
        /// <param name="ownerType">The new owner type.</param>
        /// <param name="metadata">Optional overridden metadata.</param>
        protected DependencyProperty(
            DependencyProperty source,
            Type ownerType,
            IPropertyMetadata metadata)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(ownerType != null);

            _initialized = source._initialized;
            _changed = source._changed;
            _metadata = new Dictionary<Type, IPropertyMetadata>();

            Name = source.Name;
            PropertyType = source.PropertyType;
            OwnerType = ownerType;
            Notifying = source.Notifying;
            GlobalIndex = source.GlobalIndex;
            _defaultMetadata = source._defaultMetadata;

            if (metadata != null)
            {
                _metadata.Add(ownerType, metadata);
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the property's value.
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// Gets the type of the class that registered the property.
        /// </summary>
        public Type OwnerType { get; }

        /// <summary>
        /// Gets the integer ID that represents this property.
        /// </summary>
        public int GlobalIndex { get; }

        /// <summary>
        /// Gets a value indicating whether the property inherits its value.
        /// </summary>
        public virtual bool Inherits => false;

        /// <summary>
        /// Gets a value indicating whether this is an attached property.
        /// </summary>
        public virtual bool IsAttached => false;

        /// <summary>
        /// Gets a value indicating whether this is a direct property.
        /// </summary>
        public virtual bool IsDirect => false;

        /// <summary>
        /// Gets a value indicating whether this is a readonly property.
        /// </summary>
        public virtual bool ReadOnly => false;

        /// <summary>
        /// Gets an observable that is fired when this property is initialized on a
        /// new <see cref="AvaloniaObject"/> instance.
        /// </summary>
        /// <remarks>
        /// This observable is fired each time a new <see cref="AvaloniaObject"/> is constructed
        /// for all properties registered on the object's type. The default value of the property
        /// for the object is passed in the args' NewValue (OldValue will always be
        /// <see cref="UnsetValue"/>.
        /// </remarks>
        /// <value>
        /// An observable that is fired when this property is initialized on a new
        /// <see cref="AvaloniaObject"/> instance.
        /// </value>
        public IObservable<DependencyPropertyChangedEventArgs> Initialized => _initialized;

        /// <summary>
        /// Gets an observable that is fired when this property changes on any
        /// <see cref="AvaloniaObject"/> instance.
        /// </summary>
        /// <value>
        /// An observable that is fired when this property changes on any
        /// <see cref="AvaloniaObject"/> instance.
        /// </value>
        public IObservable<DependencyPropertyChangedEventArgs> Changed => _changed;

        /// <summary>
        /// Gets a method that gets called before and after the property starts being notified on an
        /// object.
        /// </summary>
        /// <remarks>
        /// When a property changes, change notifications are sent to all property subscribers;
        /// for example via the <see cref="DependencyProperty.Changed"/> observable and and the
        /// <see cref="AvaloniaObject.PropertyChanged"/> event. If this callback is set for a property,
        /// then it will be called before and after these notifications take place. The bool argument
        /// will be true before the property change notifications are sent and false afterwards. This
        /// callback is intended to support Control.IsDataContextChanging.
        /// </remarks>
        public Action<IAvaloniaObject, bool> Notifying { get; }

        /// <summary>
        /// Provides access to a property's binding via the <see cref="AvaloniaObject"/>
        /// indexer.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>A <see cref="IndexerDescriptor"/> describing the binding.</returns>
        public static IndexerDescriptor operator !(DependencyProperty property)
        {
            return new IndexerDescriptor
            {
                Priority = BindingPriority.LocalValue,
                Property = property,
            };
        }

        /// <summary>
        /// Provides access to a property's template binding via the <see cref="AvaloniaObject"/>
        /// indexer.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>A <see cref="IndexerDescriptor"/> describing the binding.</returns>
        public static IndexerDescriptor operator ~(DependencyProperty property)
        {
            return new IndexerDescriptor
            {
                Priority = BindingPriority.TemplatedParent,
                Property = property,
            };
        }

        /// <summary>
        /// Tests two <see cref="DependencyProperty"/>s for equality.
        /// </summary>
        /// <param name="a">The first property.</param>
        /// <param name="b">The second property.</param>
        /// <returns>True if the properties are equal, otherwise false.</returns>
        public static bool operator ==(DependencyProperty a, DependencyProperty b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            else if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            else
            {
                return a.Equals(b);
            }
        }

        /// <summary>
        /// Tests two <see cref="DependencyProperty"/>s for inequality.
        /// </summary>
        /// <param name="a">The first property.</param>
        /// <param name="b">The second property.</param>
        /// <returns>True if the properties are equal, otherwise false.</returns>
        public static bool operator !=(DependencyProperty a, DependencyProperty b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a binding accessor that can be passed to <see cref="AvaloniaObject"/>'s []
        /// operator to initiate a binding.
        /// </summary>
        /// <returns>A <see cref="IndexerDescriptor"/>.</returns>
        /// <remarks>
        /// The ! and ~ operators are short forms of this.
        /// </remarks>
        public IndexerDescriptor Bind()
        {
            return new IndexerDescriptor
            {
                Property = this,
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var p = obj as DependencyProperty;
            return p != null && Equals(p);
        }

        /// <inheritdoc/>
        public bool Equals(DependencyProperty other)
        {
            return other != null && GlobalIndex == other.GlobalIndex;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GlobalIndex;
        }

        /// <summary>
        /// Gets the property metadata for the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>
        /// The property metadata.
        /// </returns>
        public IPropertyMetadata GetMetadata<T>() where T : IAvaloniaObject
        {
            return GetMetadata(typeof(T));
        }

        /// <summary>
        /// Gets the property metadata for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The property metadata.
        /// </returns>
        ///
        public IPropertyMetadata GetMetadata(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            IPropertyMetadata result;
            Type currentType = type;

            if (_metadataCache.TryGetValue(type, out result))
            {
                return result;
            }

            while (currentType != null)
            {
                if (_metadata.TryGetValue(currentType, out result))
                {
                    _metadataCache[type] = result;

                    return result;
                }

                currentType = currentType.GetTypeInfo().BaseType;
            }

            _metadataCache[type] = _defaultMetadata;

            return _defaultMetadata;
        }

        /// <summary>
        /// Checks whether the <paramref name="value"/> is valid for the property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the value is valid, otherwise false.</returns>
        public bool IsValidValue(object value)
        {
            return TypeUtilities.TryConvertImplicit(PropertyType, value, out value);
        }

        /// <summary>
        /// Gets the string representation of the property.
        /// </summary>
        /// <returns>The property's string representation.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Notifies the <see cref="Initialized"/> observable.
        /// </summary>
        /// <param name="e">The observable arguments.</param>
        internal void NotifyInitialized(DependencyPropertyChangedEventArgs e)
        {
            _initialized.OnNext(e);
        }

        /// <summary>
        /// Notifies the <see cref="Changed"/> observable.
        /// </summary>
        /// <param name="e">The observable arguments.</param>
        internal virtual void NotifyChanged(DependencyPropertyChangedEventArgs e)
        {
            _changed.OnNext(e);
        }

        /// <summary>
        /// Overrides the metadata for the property on the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="metadata">The metadata.</param>
        protected void OverrideMetadata(Type type, IPropertyMetadata metadata)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            Contract.Requires<ArgumentNullException>(metadata != null);

            if (_metadata.ContainsKey(type))
            {
                throw new InvalidOperationException(
                    $"Metadata is already set for {Name} on {type}.");
            }

            var baseMetadata = GetMetadata(type);
            metadata.Merge(baseMetadata, this);
            _metadata.Add(type, metadata);
            _metadataCache.Clear();
        }

        [DebuggerHidden]
        internal static Func<IAvaloniaObject, TValue, TValue> Cast<TOwner, TValue>(Func<TOwner, TValue, TValue> f)
            where TOwner : IAvaloniaObject
        {
            if (f != null)
            {
                return (o, v) => (o is TOwner) ? f((TOwner)o, v) : v;
            }
            else
            {
                return null;
            }
        }

        
    }
    /// <summary>
    /// Class representing the <see cref="DependencyProperty.UnsetValue"/>.
    /// </summary>
    public class UnsetValueType
    {
        /// <summary>
        /// Returns the string representation of the <see cref="DependencyProperty.UnsetValue"/>.
        /// </summary>
        /// <returns>The string "(unset)".</returns>
        public override string ToString() => "(unset)";
    }
}
