using System;
using Avalonia.Layout;
using Avalonia.Utilities;

namespace Avalonia
{
    public delegate bool ValidateValueCallback(object value);

    public class DependencyProperty : StyledProperty<object>
    {
        public ValidateValueCallback ValidateValueCallback { get; private set; }

        public override bool IsReadOnly => _isReadOnly;

        private readonly object _propertyTypeDefaultValue;
        private readonly bool _isReadOnly;

        private DependencyProperty(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback,
            bool isReadOnly)
            : base(name, propertyType, ownerType, metadata, metadata is FrameworkPropertyMetadata m && m.Inherits)
        {
            _propertyTypeDefaultValue = TypeUtilities.Default(propertyType);
            _isReadOnly = isReadOnly;

            ValidateValueCallback = validateValueCallback;

            object defaultValue = ((IStyledPropertyMetadata)metadata).DefaultValue;
            if (defaultValue == null)
            {
                defaultValue = TypeUtilities.Default(propertyType);
            }

            if (!IsValidType(defaultValue, propertyType))
            {
                throw new ArgumentException(string.Format(
                    "Value type mismatch for property '{0}': {1} (expected {2})",
                    name,
                    defaultValue?.GetType().FullName ?? "(null)",
                    propertyType?.FullName ?? "(null)"));
            }

            if (validateValueCallback != null && !validateValueCallback(defaultValue))
            {
                throw new ArgumentException(string.Format(
                    "Invalid default value for property '{0}': '{1}' ({2})",
                    name,
                    defaultValue,
                    defaultValue?.GetType().FullName ?? "(null)"));
            }

            Changed.Subscribe(OnPropertyChanged);
        }

        public static DependencyProperty Register(
            string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty Register(
            string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata)
        {
            return Register(name, propertyType, ownerType, typeMetadata, null);
        }

        public static DependencyProperty Register(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            var property = new DependencyProperty(
                name, propertyType, ownerType, typeMetadata ?? new PropertyMetadata(), validateValueCallback, false);

            AvaloniaPropertyRegistry.Instance.Register(ownerType, property);
            return property;
        }

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata)
        {
            return RegisterReadOnly(name, propertyType, ownerType, typeMetadata, null);
        }

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            var property = new DependencyProperty(
                name, propertyType, ownerType, typeMetadata ?? new PropertyMetadata(), validateValueCallback, true);

            AvaloniaPropertyRegistry.Instance.Register(ownerType, property);

            return new DependencyPropertyKey(property);
        }

        public static DependencyProperty RegisterAttached(
            string name, Type propertyType, Type ownerType)
        {
            return RegisterAttached(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty RegisterAttached(
            string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata)
        {
            return RegisterAttached(name, propertyType, ownerType, defaultMetadata, null);
        }

        public static DependencyProperty RegisterAttached(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            var property = new DependencyProperty(
                name, propertyType, ownerType, typeMetadata ?? new PropertyMetadata(), validateValueCallback, false);

            AvaloniaPropertyRegistry.Instance.Register(ownerType, property);
            AvaloniaPropertyRegistry.Instance.RegisterAttached(typeof(AvaloniaObject), property);
            return property;
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(
            string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata)
        {
            return RegisterAttachedReadOnly(name, propertyType, ownerType, defaultMetadata, null);
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback)
        {
            var property = new DependencyProperty(
                name, propertyType, ownerType, typeMetadata ?? new PropertyMetadata(), validateValueCallback, true);

            AvaloniaPropertyRegistry.Instance.Register(ownerType, property);
            AvaloniaPropertyRegistry.Instance.RegisterAttached(typeof(AvaloniaObject), property);
            
            return new DependencyPropertyKey(property);
        }

        public DependencyProperty AddOwner(Type ownerType)
        {
            return AddOwner(ownerType, null);
        }

        public DependencyProperty AddOwner(Type ownerType, PropertyMetadata typeMetadata)
        {
            Contract.Requires<ArgumentNullException>(ownerType != null);

            if (typeMetadata != null)
            {
                OverrideMetadata(ownerType, typeMetadata);
            }

            AvaloniaPropertyRegistry.Instance.Register(ownerType, this);
            return this;
        }

        public override void OverrideMetadata(Type type, StyledPropertyMetadata<object> metadata)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(
                    $"The property {Name} is read-only and can only be accessed using an authorization key.");
            }

            base.OverrideMetadata(type, metadata);
        }

        public override void OverrideMetadata<T>(StyledPropertyMetadata<object> metadata)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(
                    $"The property {Name} is read-only and can only be accessed using an authorization key.");
            }

            base.OverrideMetadata<T>(metadata);
        }

        public void OverrideMetadata(Type type, PropertyMetadata typeMetadata, DependencyPropertyKey key)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            Contract.Requires<ArgumentNullException>(typeMetadata != null);
            Contract.Requires<ArgumentNullException>(key != null);

            if (!IsReadOnly)
            {
                throw new InvalidOperationException($"The property {Name} does not require an authorization key.");
            }

            if (key.DependencyProperty != this)
            {
                throw new ArgumentException($"The specified key is not authorized to access the property {Name}.");
            }

            OverrideMetadata(type, typeMetadata);
        }

        private static bool IsValidType(object value, Type type)
        {
            if (value != null && !type.IsInstanceOfType(value))
            {
                return false;
            }

            if (value == null &&
                type.IsValueType &&
                (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>)))
            {
                return false;
            }

            return true;
        }

        protected internal override bool TryConvertValue(ref object value)
        {
            return IsValidType(value, PropertyType) &&
                (ValidateValueCallback != null ? ValidateValueCallback(value) : true);
        }

        protected override object GetDefaultBoxedValue(Type type)
        {
            return ((IStyledPropertyMetadata)GetMetadata(type)).DefaultValue ?? _propertyTypeDefaultValue;
        }

        private void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property is DependencyProperty)
            {
                PropertyMetadata metadata = (PropertyMetadata)e.Property.GetMetadata(e.Sender.GetType());
                metadata.PropertyChangedCallback?.Invoke(e.Sender, e);

                ILayoutable layoutable = e.Sender as ILayoutable;
                if (layoutable == null)
                {
                    return;
                }

                if (metadata is FrameworkPropertyMetadata frameworkMetadata)
                {
                    if (frameworkMetadata.AffectsMeasure)
                    {
                        layoutable.InvalidateMeasure();
                    }

                    if (frameworkMetadata.AffectsArrange)
                    {
                        layoutable.InvalidateArrange();
                    }
                }
            }
        }
    }
}
