using System;
using Avalonia.Utilities;

namespace Avalonia
{
    public delegate bool ValidateValueCallback(object value);

    public class DependencyProperty : StyledProperty<object>
    {
        public ValidateValueCallback ValidateValueCallback { get; private set; }

        private readonly object _propertyTypeDefaultValue;

        private DependencyProperty(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback)
            : base(name, propertyType, ownerType, metadata)
        {
            _propertyTypeDefaultValue = TypeUtilities.Default(propertyType);

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
            return new DependencyProperty(
                name, propertyType, ownerType, typeMetadata ?? new PropertyMetadata(), validateValueCallback);
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

        public override bool IsValidValue(object value)
        {
            return IsValidType(value, PropertyType) &&
                (ValidateValueCallback != null ? ValidateValueCallback(value) : true);
        }

        protected override object GetDefaultBoxedValue(Type type)
        {
            return ((IStyledPropertyMetadata)GetMetadata(type)).DefaultValue ?? _propertyTypeDefaultValue;
        }
    }
}
