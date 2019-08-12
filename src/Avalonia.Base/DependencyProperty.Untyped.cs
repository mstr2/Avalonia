using System;

namespace Avalonia
{
    public abstract partial class DependencyProperty
    {
        /// <summary>
        /// Registers an untyped <see cref="DependencyProperty"/>.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">The type of the property's value.</param>
        /// <param name="ownerType">The type of the class that is registering the property.</param>
        public static DependencyProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty Register(
            string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            return Register(name, propertyType, ownerType, metadata, null);
        }

        public static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(propertyType != null);
            Contract.Requires<ArgumentNullException>(ownerType != null);
            
            var result = new UntyedProperty(
                name,
                propertyType,
                ownerType,
                metadata ?? new PropertyMetadata(),
                validateValueCallback,
                inherits: false,
                notifying: null);

            DependencyPropertyRegistry.Instance.Register(ownerType, result);
            return result;
        }

        public DependencyProperty AddOwner(Type ownerType)
        {
            return AddOwner(ownerType, null);
        }

        public DependencyProperty AddOwner(Type ownerType, PropertyMetadata metadata)
        {
            if (IsDirect)
            {
                throw new InvalidOperationException("Cannot add an owner to a direct property.");
            }

            if (ownerType == null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (!ownerType.IsAssignableFrom(typeof(IAvaloniaObject)))
            {
                throw new ArgumentException(
                    $"The owner of a dependency property must be an {nameof(IAvaloniaObject)} instance.");
            }

            if (metadata != null)
            {
                OverrideMetadata(ownerType, metadata);
            }

            DependencyPropertyRegistry.Instance.Register(ownerType, this);
            return this;
        }

        public void OverrideMetadata(Type type, PropertyMetadata metadata, DependencyPropertyKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (ReadOnly && key.DependencyProperty != this)
            {
                throw new ArgumentException("The specified key is not authorized to access this property.");
            }

            OverrideMetadata(type, metadata);
        }
    }
}
