namespace Avalonia
{
    public class DependencyObject : AvaloniaObject
    {
        protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected sealed override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property is DependencyProperty)
            {
                var args = new DependencyPropertyChangedEventArgs(
                    (DependencyObject)e.Sender, (DependencyProperty)e.Property, e.OldValue, e.NewValue);

                OnPropertyChanged(args);

                PropertyMetadata metadata = (PropertyMetadata)e.Property.GetMetadata(e.Sender.GetType());
                metadata.PropertyChangedCallback?.Invoke(this, args);
            }
        }
    }
}
