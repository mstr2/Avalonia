using Avalonia.Data;

namespace Avalonia
{
    public interface IPropertyMetadata
    {
        BindingMode DefaultBindingMode { get; }

        void Merge(IPropertyMetadata baseMetadata, DependencyProperty property);
    }
}
